using GraduationProject.Controllers.FilterParameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraduationProject.Repository.Extensions
{
    public class GenericFieldBasedJsonConverter<T> : JsonConverter<T> {
        private readonly HashSet<string> _fields;
        public override bool CanRead => false;
        
        public GenericFieldBasedJsonConverter(HashSet<string> allFields, EntityFieldsFilter fieldsFilters) {
            if (string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields)) {
                if (string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                    _fields = allFields;
                }
                
                else {
                    HashSet<string> excludeFields = fieldsFilters.FieldsToExclude.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToHashSet();
                    _fields = allFields.Where(field => !excludeFields.Contains(field)).ToHashSet();
                }
            }
            
            else {
                string fields = string.Join(',', allFields);
                HashSet<string> includeFields = fieldsFilters.OnlySelectFields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToHashSet();
                if (!string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                    _fields = includeFields.Where(field => !fieldsFilters.FieldsToExclude.Contains(field)).ToHashSet();
                }
                
                else {
                    _fields = includeFields;
                }
            }
        }

        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer) {
            // var jsonObject = JObject.FromObject(value, serializer);
            var jsonObject = JObject.FromObject(value,
                JsonSerializer.CreateDefault(new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
            }));

            // Remove any properties that are not included in the list of fields
            var propertiesToRemove = jsonObject.Properties()
                .Where(p => !_fields.Contains(p.Name))
                .ToList();
            
            propertiesToRemove.ForEach(p => p.Remove());
            // Console.WriteLine($"Properties: " + string.Join(", ", jsonObject.Properties().Select(p => p.Name)));
            
            jsonObject.WriteTo(writer);
        }

        public override T ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }

}
