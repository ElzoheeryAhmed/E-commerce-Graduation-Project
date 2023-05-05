using System.Reflection;
using GraduationProject.Configurations;
using GraduationProject.Data;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using GraduationProject.Controllers.Helpers;
using GraduationProject.Services.SecurityServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

Console.WriteLine("Setting up the backend server...");

// To get any configuration strings from the appsettings.json file.
IConfiguration _config = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.Build();

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

// Adding Serilog for logging.
Log.Logger = new LoggerConfiguration()
	.WriteTo.File(
		path: _config.GetValue<string>("LogFilePath"),
		outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:1j}{NewLine}{Exception}",
		rollingInterval: RollingInterval.Day,
		restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning
	).CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddControllers()
	.AddNewtonsoftJson(op => {
		op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
		op.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
	});

// This line registers AutoMapper in the DI container and tells it to scan all assemblies loaded into the application domain for mapping configurations. This allows AutoMapper to automatically discover and register any mapping profiles that are defined in the application or its referenced assemblies.
// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MapperInitializer));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options => {
	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Configuring Entity Framework Core.
builder.Services.AddDbContext<AppDbContext>(options => {
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(3600));
});

// Configuring Identity.
builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>();


//map JWT home Settings
builder.Services.Configure<JWT>(_config.GetSection("JWT"));

//register AuthService class in service container
builder.Services.AddScoped<IAuthService, AuthService>();

//add default configuration for authentication schema rather than explict define for each end point
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = _config["JWT:Issuer"],
                        ValidAudience = _config["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]))
                    };
                });

var app = builder.Build();

Log.Information("The backend server has started.");
Console.WriteLine("The backend server has started at: https://localhost:7185/swagger/index.html");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.Information("The backend server has stopped.");
