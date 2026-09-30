using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// ApplicationSetting Metadata class
    /// </summary>
    public
      partial class ApplicationSettingMetadata
    {
        /// <summary>
        /// Setting Id
        /// </summary>        
        [DisplayName("Setting Id")]
        [Required(ErrorMessage = "Setting Id is required")]
        public int
          SettingId
        {
            get;
            set;
        }

        /// <summary>
        /// Module Name
        /// </summary>        
        [DisplayName("Module Name")]
        [Required(ErrorMessage = "Module Name is required")]
        [MaxLength(100, ErrorMessage = "Module Name cannot be longer than 100 characters")]
        public string
          ModuleName
        {
            get;
            set;
        }

        /// <summary>
        /// Setting Name
        /// </summary>        
        [DisplayName("Setting Name")]
        [Required(ErrorMessage = "Setting Name is required")]
        [MaxLength(100, ErrorMessage = "Setting Name cannot be longer than 100 characters")]
        public string
          SettingName
        {
            get;
            set;
        }

        /// <summary>
        /// Setting Type
        /// </summary>        
        [DisplayName("Setting Type")]
        [Required(ErrorMessage = "Setting Type is required")]
        [MaxLength(50, ErrorMessage = "Setting Type cannot be longer than 50 characters")]
        public string
          SettingType
        {
            get;
            set;
        }

        /// <summary>
        /// Setting Value
        /// </summary>        
        [DisplayName("Setting Value")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Setting Value is required")]
        public string
          SettingValue
        {
            get;
            set;
        }

    }
}
