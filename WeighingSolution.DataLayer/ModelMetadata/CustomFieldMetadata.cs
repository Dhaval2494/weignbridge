using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// CustomField Metadata class
    /// </summary>
    public
      partial class CustomFieldMetadata
    {
        /// <summary>
        /// Custom Field ID
        /// </summary>        
        [DisplayName("Custom Field ID")]
        [Required(ErrorMessage = "Custom Field ID is required")]
        public int
          CustomFieldID
        {
            get;
            set;
        }

        /// <summary>
        /// Field Name
        /// </summary>        
        [DisplayName("Field Name")]
        [MaxLength(50, ErrorMessage = "Field Name cannot be longer than 50 characters")]
        public string
          FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// Display Name
        /// </summary>        
        [DisplayName("Display Name")]
        [MaxLength(50, ErrorMessage = "Display Name cannot be longer than 50 characters")]
        public string
          DisplayName
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

        /// <summary>
        /// Max Length
        /// </summary>        
        [DisplayName("Max Length")]
        public Nullable<int>
          MaxLength
        {
            get;
            set;
        }

        /// <summary>
        /// Is Visible
        /// </summary>        
        [DisplayName("Is Visible")]
        [Required(ErrorMessage = "Is Visible is required")]
        public bool
          IsVisible
        {
            get;
            set;
        }

    }
}
