using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// UserMaster Metadata class
    /// </summary>
    public
      partial class UserMasterMetadata
    {
        /// <summary>
        /// User ID
        /// </summary>        
        [DisplayName("User ID")]
        [Required(ErrorMessage = "User ID is required")]
        public int
          UserID
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
        [MaxLength(250, ErrorMessage = "Email cannot be longer than 250 characters")]
        [EmailAddress]
        public string
          Email
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
        /// User Name
        /// </summary>        
        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name is required")]
        [MaxLength(50, ErrorMessage = "User Name cannot be longer than 50 characters")]
        [RegularExpression("^(?=[A-Za-z])[A-Za-z0-9]{1,16}$", ErrorMessage = "User Name is not valid, it does not allow special charecter and must start with alphabet and must between 1 to 16 charecters.")]
        public string
          UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password
        /// </summary>        
        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(50, ErrorMessage = "Password cannot be longer than 50 characters")]
        public string
          Password
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

        /// <summary>
        /// Account ID
        /// </summary>        
        [DisplayName("Template ID")]
        public Nullable<int>
          TemplateID
        {
            get;
            set;
        }

    }
}
