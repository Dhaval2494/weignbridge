using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// TransactionData Metadata class
    /// </summary>
    public
      partial class TransactionDataMetadata
    {
        /// <summary>
        /// Ticket ID
        /// </summary>        
        [DisplayName("Ticket ID")]
        [Required(ErrorMessage = "Ticket ID is required")]
        public int
          TicketID
        {
            get;
            set;
        }

        /// <summary>
        /// Driver ID
        /// </summary>        
        [DisplayName("Driver ID")]
        public Nullable<int>
          DriverID
        {
            get;
            set;
        }

        /// <summary>
        /// Vehicle ID
        /// </summary>        
        [DisplayName("Vehicle ID")]
        [Required(ErrorMessage = "Vehicle information is required")]
        public Nullable<int>
          VehicleID
        {
            get;
            set;
        }

        // <summary>
        // RFToken ID
        // </summary>        
        //[DisplayName("RFToken ID")]
        //public Nullable<int> 
        //  RFTokenID { get; 
        //  set; }

        /// <summary>
        /// Status
        /// </summary>        
        [DisplayName("Status")]
        [MaxLength(50, ErrorMessage = "Status cannot be longer than 50 characters")]
        public string
          Status
        {
            get;
            set;
        }

        /// <summary>
        /// Transaction Mode
        /// </summary>        
        [DisplayName("Transaction Mode")]
        [Required(ErrorMessage = "Transaction Mode is required")]
        [MaxLength(10, ErrorMessage = "Transaction Mode cannot be longer than 10 characters")]
        public string
          TransactionMode
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
        /// Transporter ID
        /// </summary>        
        [DisplayName("Transporter ID")]
        public Nullable<int>
          TransporterID
        {
            get;
            set;
        }

        /// <summary>
        /// Receipt Ticket ID
        /// </summary>        
        [DisplayName("Receipt Ticket ID")]
        [Required(ErrorMessage = "Receipt Ticket ID is required")]
        public System.Guid
          ReceiptTicketID
        {
            get;
            set;
        }

        /// <summary>
        /// Charges
        /// </summary>        
        [DisplayName("Charges")]
        public Nullable<decimal>
          Charges
        {
            get;
            set;
        }

        /// <summary>
        /// CreatedBy
        /// </summary>        
        [DisplayName("CreatedBy")]
        public Nullable<int>
            CreatedBy
        {
            get;
            set;
        }

        /// <summary>
        /// CreationTime
        /// </summary>        
        [DisplayName("CreationTime")]
        public DateTime
            CreationTime
        {
            get;
            set;
        }
    }
}
