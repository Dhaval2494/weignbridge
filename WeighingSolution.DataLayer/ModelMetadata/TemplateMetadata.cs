using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Template Metadata class
    /// </summary>
    public
      partial class TemplateMetadata
    {
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
        /// Template Name
        /// </summary>        
        [DisplayName("Template Name")]
        [Required(ErrorMessage = "Template Name is required")]
        [MaxLength(50, ErrorMessage = "Template Name cannot be longer than 50 characters")]
        public string
          TemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// Active
        /// </summary>        
        [DisplayName("Active")]
        [Required(ErrorMessage = "Active is required")]
        public bool
          Active
        {
            get;
            set;
        }

    }
}
