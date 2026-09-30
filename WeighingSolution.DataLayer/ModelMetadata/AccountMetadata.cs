using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Account Metadata class
    /// </summary>
    public
      partial class AccountMetadata
    {
        /// <summary>
        /// Account ID
        /// </summary>        
        [DisplayName("Account ID")]
        [Required(ErrorMessage = "Account ID is required")]
        public int
          AccountID
        {
            get;
            set;
        }

        /// <summary>
        /// Account Code
        /// </summary>        
        [DisplayName("Account Code")]
        [MaxLength(50, ErrorMessage = "Account Code cannot be longer than 50 characters")]
        public string
          AccountCode
        {
            get;
            set;
        }

        /// <summary>
        /// Company Name
        /// </summary>        
        [DisplayName("Company Name")]
        [Required(ErrorMessage = "Company Name is required")]
        [MaxLength(250, ErrorMessage = "Company Name cannot be longer than 250 characters")]
        public string
          CompanyName
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
        /// Contact No
        /// </summary>        
        [DisplayName("Contact No")]
        [MaxLength(20, ErrorMessage = "Contact No cannot be longer than 20 characters")]
        public string
          ContactNo
        {
            get;
            set;
        }

        /// <summary>
        /// Email
        /// </summary>        
        [DisplayName("Email")]
        [EmailAddress]
        [MaxLength(250, ErrorMessage = "Email cannot be longer than 250 characters")]
        public string
          Email
        {
            get;
            set;
        }

        /// <summary>
        /// Is Account
        /// </summary>        
        [DisplayName("Is Account")]
        public Nullable<bool>
          IsAccount
        {
            get;
            set;
        }

        /// <summary>
        /// Is Transporter
        /// </summary>        
        [DisplayName("Is Transporter")]
        public Nullable<bool>
          IsTransporter
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
        /// Address
        /// </summary>        
        [DisplayName("Address")]
        [MaxLength(500, ErrorMessage = "Address cannot be longer than 500 characters")]
        public string
          Address
        {
            get;
            set;
        }

        /// <summary>
        /// City
        /// </summary>        
        [DisplayName("City")]
        [MaxLength(100, ErrorMessage = "City cannot be longer than 100 characters")]
        public string
          City
        {
            get;
            set;
        }

        /// <summary>
        /// Pincode
        /// </summary>        
        [DisplayName("Pincode")]
        [MaxLength(20, ErrorMessage = "Pincode cannot be longer than 20 characters")]
        public string
          Pincode
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
