using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// PluginManagement Metadata class
    /// </summary>
    public
      partial class PluginManagementMetadata
    {
        /// <summary>
        /// Plugin ID
        /// </summary>        
        [DisplayName("Plugin ID")]
        [Required(ErrorMessage = "Plugin ID is required")]
        public int
          PluginID
        {
            get;
            set;
        }

        /// <summary>
        /// Plugin Name
        /// </summary>        
        [DisplayName("Plugin Name")]
        [Required(ErrorMessage = "Plugin Name is required")]
        [MaxLength(50, ErrorMessage = "Plugin Name cannot be longer than 50 characters")]
        public string
          PluginName
        {
            get;
            set;
        }

        /// <summary>
        /// Is Enable
        /// </summary>        
        [DisplayName("Is Enable")]
        [Required(ErrorMessage = "Is Enable is required")]
        public bool
          IsEnable
        {
            get;
            set;
        }

    }
}
