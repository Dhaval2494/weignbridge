using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Driver Metadata class
    /// </summary>
    public
      partial class DriverMetadata
    {
        /// <summary>
        /// Driver ID
        /// </summary>        
        [DisplayName("Driver ID")]
        [Required(ErrorMessage = "Driver ID is required")]
        public int
          DriverID
        {
            get;
            set;
        }

        /// <summary>
        /// First Name
        /// </summary>        
        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(250, ErrorMessage = "First Name cannot be longer than 250 characters")]
        public string
          FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Last Name
        /// </summary>        
        [DisplayName("Last Name")]
        [MaxLength(250, ErrorMessage = "Last Name cannot be longer than 250 characters")]
        public string
          LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Photo
        /// </summary>        
        [DisplayName("Photo")]
        [MaxLength(300, ErrorMessage = "Photo cannot be longer than 300 characters")]
        public string
          Photo
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

        /// <summary>
        /// Account ID
        /// </summary>        
        [DisplayName("Account ID")]
        public Nullable<int>
          AccountID
        {
            get;
            set;
        }

        /// <summary>
        /// IDProof Scan
        /// </summary>        
        [DisplayName("IDProof Scan")]
        [MaxLength(300, ErrorMessage = "IDProof Scan cannot be longer than 300 characters")]
        public string
          IDProofScan
        {
            get;
            set;
        }

        /// <summary>
        /// IDProof Type
        /// </summary>        
        [DisplayName("IDProof Type")]
        [MaxLength(50, ErrorMessage = "IDProof Type cannot be longer than 50 characters")]
        public string
          IDProofType
        {
            get;
            set;
        }

        /// <summary>
        /// IDProof No
        /// </summary>        
        [DisplayName("IDProof No")]
        [MaxLength(50, ErrorMessage = "IDProof No cannot be longer than 50 characters")]
        public string
          IDProofNo
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
