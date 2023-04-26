using GraduationProject.Configurations;
using GraduationProject.Data;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Console.WriteLine("Setting up the backend server...");

// To get any configuration strings from the appsettings.json file.
IConfiguration _config = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
builder.Services.AddSwaggerGen();

// Configuring Entity Framework Core.
builder.Services.AddDbContext<AppDbContext>(options => {
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(3600));
});

// Configuring Identity.
builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();
//Configure some configuration

// Configuring Authentication.
builder.Services.AddAuthentication(); //options =>
//{
//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})

//// Adding Jwt Bearer
//.AddJwtBearer(options =>
// {
//	 options.SaveToken = true;
//	 options.RequireHttpsMetadata = false;
//	 options.TokenValidationParameters = new TokenValidationParameters()
//	 {
//		 ValidateIssuer = true,
//		 ValidateAudience = true,
//		 ValidAudience = configuration["JWT:ValidAudience"],
//		 ValidIssuer = configuration["JWT:ValidIssuer"],
//		 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
//	 };
// });

var app = builder.Build();

Log.Information("The backend server has started.");
Console.WriteLine("The backend server has started.");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.Information("The backend server has stopped.");
