using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Product Metadata class
    /// </summary>
    public
      partial class ProductMetadata
    {
        /// <summary>
        /// Product ID
        /// </summary>        
        [DisplayName("Product ID")]
        [Required(ErrorMessage = "Product ID is required")]
        public int
          ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// Product Name
        /// </summary>        
        [DisplayName("Product Name")]
        [Required(ErrorMessage = "Product Name is required")]
        [MaxLength(250, ErrorMessage = "Product Name cannot be longer than 250 characters")]
        public string
          ProductName
        {
            get;
            set;
        }

        /// <summary>
        /// Product Code
        /// </summary>        
        [DisplayName("Product Code")]
        [MaxLength(50, ErrorMessage = "Product Code cannot be longer than 50 characters")]
        public string
          ProductCode
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
