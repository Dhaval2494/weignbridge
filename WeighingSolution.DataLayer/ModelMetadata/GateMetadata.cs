using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Gate Metadata class
    /// </summary>
    public
      partial class GateMetadata
    {
        /// <summary>
        /// Gate ID
        /// </summary>        
        [DisplayName("Gate ID")]
        [Required(ErrorMessage = "Gate ID is required")]
        public int
          GateID
        {
            get;
            set;
        }

        /// <summary>
        /// Gate Name
        /// </summary>        
        [DisplayName("Gate Name")]
        [Required(ErrorMessage = "Gate Name is required")]
        [MaxLength(100, ErrorMessage = "Gate Name cannot be longer than 100 characters")]
        public string
          GateName
        {
            get;
            set;
        }

        /// <summary>
        /// Gate Type
        /// </summary>        
        [DisplayName("Gate Type")]
        [Required(ErrorMessage = "Gate Type is required")]
        [MaxLength(10, ErrorMessage = "Gate Type cannot be longer than 10 characters")]
        public string
          GateType
        {
            get;
            set;
        }

        /// <summary>
        /// Notes
        /// </summary>        
        [DisplayName("Notes")]
        public string
          Notes
        {
            get;
            set;
        }

    }
}
