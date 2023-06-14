using Microsoft.AspNetCore.Identity;

namespace GraduationProject.Services.SecurityServices
{
    public class SecurityHelper
    {
        public static string parseError(IEnumerable<IdentityError> Errors) 
        {
            var errors = string.Empty;

            foreach (var error in Errors)
                errors += $"{error.Description},";

            return  errors ;
        }
    }
}
