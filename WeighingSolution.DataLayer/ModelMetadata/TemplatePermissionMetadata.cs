using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// TemplatePermission Metadata class
    /// </summary>
    public
      partial class TemplatePermissionMetadata
    {
        /// <summary>
        /// Template Permission ID
        /// </summary>        
        [DisplayName("Template Permission ID")]
        [Required(ErrorMessage = "Template Permission ID is required")]
        public int
          TemplatePermissionID
        {
            get;
            set;
        }

        /// <summary>
        /// Feature ID
        /// </summary>        
        [DisplayName("Feature ID")]
        [Required(ErrorMessage = "Feature ID is required")]
        public int
          FeatureID
        {
            get;
            set;
        }

        /// <summary>
        /// Template ID
        /// </summary>        
        [DisplayName("Template ID")]
        [Required(ErrorMessage = "Template ID is required")]
        public int
          TemplateID
        {
            get;
            set;
        }

        /// <summary>
        /// Permission Set
        /// </summary>        
        [DisplayName("Permission Set")]
        [MaxLength(10, ErrorMessage = "Permission Set cannot be longer than 10 characters")]
        public string
          PermissionSet
        {
            get;
            set;
        }

    }
}
