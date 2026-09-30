using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// CustomValue Metadata class
    /// </summary>
    public
      partial class CustomValueMetadata
    {
        /// <summary>
        /// Custom Value ID
        /// </summary>        
        [DisplayName("Custom Value ID")]
        [Required(ErrorMessage = "Custom Value ID is required")]
        public int
          CustomValueID
        {
            get;
            set;
        }

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
        /// Custom Value1
        /// </summary>        
        [DisplayName("Custom Value1")]
        public string
          CustomValue1
        {
            get;
            set;
        }

        /// <summary>
        /// Receipt Ticket ID
        /// </summary>        
        [DisplayName("Receipt Ticket ID")]
        public Nullable<System.Guid>
          ReceiptTicketID
        {
            get;
            set;
        }

    }
}
