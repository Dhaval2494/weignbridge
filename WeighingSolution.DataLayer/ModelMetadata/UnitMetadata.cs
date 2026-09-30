using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Unit Metadata class
    /// </summary>
    public
      partial class UnitMetadata
    {
        /// <summary>
        /// Unit ID
        /// </summary>        
        [DisplayName("Unit ID")]
        [Required(ErrorMessage = "Unit ID is required")]
        public int
          UnitID
        {
            get;
            set;
        }

        /// <summary>
        /// Unit Name
        /// </summary>        
        [DisplayName("Unit Name")]
        [Required(ErrorMessage = "Unit Name is required")]
        [MaxLength(50, ErrorMessage = "Unit Name cannot be longer than 50 characters")]
        public string
          UnitName
        {
            get;
            set;
        }

    }
}
