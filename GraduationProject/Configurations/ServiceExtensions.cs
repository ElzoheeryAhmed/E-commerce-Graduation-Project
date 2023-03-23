using GraduationProject.Data;
using GraduationProject.Models;
using Microsoft.AspNetCore.Identity;

namespace GraduationProject.Configurations
{
	public static class ServiceExtensions
	{
		public static void ConfigureIdentity(this IServiceCollection services)
		{
			var builder = services.AddIdentityCore<User>(Queryable => Queryable.User.RequireUniqueEmail = true);

			builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
			builder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

			// Or write the next line in the Program.cs file:
			// builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
		}
	}
}
