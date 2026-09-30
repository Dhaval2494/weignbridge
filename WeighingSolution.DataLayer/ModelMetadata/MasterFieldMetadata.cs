using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// MasterField Metadata class
    /// </summary>
    public
      partial class MasterFieldMetadata
    {
        /// <summary>
        /// Master Field ID
        /// </summary>        
        [DisplayName("Master Field ID")]
        [Required(ErrorMessage = "Master Field ID is required")]
        public int
          MasterFieldID
        {
            get;
            set;
        }

        /// <summary>
        /// Field Name
        /// </summary>        
        [DisplayName("Field Name")]
        [Required(ErrorMessage = "Field Name is required")]
        [MaxLength(50, ErrorMessage = "Field Name cannot be longer than 50 characters")]
        public string
          FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// Isvisible
        /// </summary>        
        [DisplayName("Isvisible")]
        [Required(ErrorMessage = "Isvisible is required")]
        public bool
          Isvisible
        {
            get;
            set;
        }

        /// <summary>
        /// Is Required
        /// </summary>        
        [DisplayName("Is Required")]
        [Required(ErrorMessage = "Is Required is required")]
        public bool
          IsRequired
        {
            get;
            set;
        }

    }
}
