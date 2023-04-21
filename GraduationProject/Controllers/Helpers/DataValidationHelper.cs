using System.Collections;

namespace GraduationProject.Controllers.Helpers
{
    public class DataValidationHelper {
        public static bool HasNonNullOrDefaultProperties(object obj) {
            if (obj == null) return false;
            
            var properties = obj.GetType().GetProperties();
            foreach (var property in properties) {
                var value = property.GetValue(obj);
                
                // Check if the property is null (in case of object) or default value (in case of value type)
                if (value is string strValue && !string.IsNullOrWhiteSpace(strValue)) {
                    return true;
                }
                
                // if (value != default) {
                //     return true;
                // }
                
                else if (value is int intValue && intValue != default) {
                    Console.WriteLine("Int value is not default");
                    return true;
                }
                
                else if (value is double dobuleValue && dobuleValue != default) {
                    Console.WriteLine("Double value is not default");
                    return true;
                }
                
                else if (value is decimal decimalValue && decimalValue != default) {
                    Console.WriteLine("Decimal value is not default");
                    return true;
                }
                
                else if (value is ICollection collection && collection.Count != 0) {
                    Console.WriteLine("Collection value is not default");
                    return true;
                }
            }
            
            return false;
        }
    }
}