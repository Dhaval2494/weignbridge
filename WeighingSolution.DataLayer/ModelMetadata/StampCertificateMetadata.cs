using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// StampCertificate Metadata class
    /// </summary>
    public partial class StampCertificateMetadata
    {
        /// <summary>
        /// Stamp Id
        /// </summary>        
        [DisplayName("Stamp Id")]
        [Required(ErrorMessage = "Stamp Id is required")]
        public int
          StampId
        {
            get;
            set;
        }

        /// <summary>
        /// Stamp Name
        /// </summary>        
        [DisplayName("Stamp Name")]
        [Required(ErrorMessage = "Stamp Name is required")]
        [MaxLength(100, ErrorMessage = "Stamp Name cannot be longer than 100 characters")]
        public string
          StampName
        {
            get;
            set;
        }

        /// <summary>
        /// Issue Date
        /// </summary>        
        [DisplayName("Issue Date")]
        [Required(ErrorMessage = "Issue Date is required")]
        public System.DateTime
          IssueDate
        {
            get;
            set;
        }

        /// <summary>
        /// Expiry Date
        /// </summary>        
        [DisplayName("Expiry Date")]
        [Required(ErrorMessage = "Expiry Date is required")]
        public System.DateTime
          ExpiryDate
        {
            get;
            set;
        }

        /// <summary>
        /// Issued Auth
        /// </summary>        
        [DisplayName("Issued Auth")]
        [MaxLength(100, ErrorMessage = "Issued Auth cannot be longer than 100 characters")]
        public string
          IssuedAuth
        {
            get;
            set;
        }

        /// <summary>
        /// Created By
        /// </summary>        
        [DisplayName("Created By")]
        public Nullable<int>
          CreatedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Stamp Pdf
        /// </summary>        
        [DisplayName("Stamp Pdf")]
        public byte[]
          StampPdf
        {
            get;
            set;
        }

    }
}
