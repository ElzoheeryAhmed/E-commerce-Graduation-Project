using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraduationProject.Repository
{
    public class GenericFieldBasedJsonConverter<T> : JsonConverter<T> {
        private readonly HashSet<string> _fields;
        public override bool CanRead => false;
        
        public GenericFieldBasedJsonConverter(HashSet<string> allFields, string? onlyIncludeFields=null, string? fieldsToExclude=null) {
            if (string.IsNullOrWhiteSpace(onlyIncludeFields)) {
                if (string.IsNullOrWhiteSpace(fieldsToExclude)) {
                    _fields = allFields;
                }
                
                else {
                    HashSet<string> excludeFields = fieldsToExclude.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToHashSet();
                    _fields = allFields.Where(field => !excludeFields.Contains(field)).ToHashSet();
                }
            }
            
            else {
                string fields = string.Join(',', allFields);
                _fields = onlyIncludeFields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToHashSet();
            }
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer) {
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

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }

}