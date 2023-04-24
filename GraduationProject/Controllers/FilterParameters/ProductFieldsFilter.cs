using System.ComponentModel;

namespace GraduationProject.Controllers.FilterParameters
{
    public class EntityFieldsFilter
    {
        /// <summary>
        /// A comma-separated list of fields to be returned in the API response. By default, all fields are returned. Cannot be used along with fieldsToExclude.
        /// </summary>
        public virtual string? OnlySelectFields { get; set; } = null;
        
        /// <summary>
		/// A comma-separated list of fields to be excluded from the API response. By default, all fields are returned. Cannot be used along with onlyIncludeFields.
        /// </summary>
        public virtual string? FieldsToExclude { get; set; } = null;
    }
    
    public class ProductFieldsFilter : EntityFieldsFilter
    {
        [DefaultValue("Features,Description,HighResImageURLs")]
        public override string? FieldsToExclude { get; set; } = null;
    }
}
