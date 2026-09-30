using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// TransactionDetail Metadata class
    /// </summary>
    public
      partial class TransactionDetailMetadata
    {
        /// <summary>
        /// Transaction Detail ID
        /// </summary>        
        [DisplayName("Transaction Detail ID")]
        [Required(ErrorMessage = "Transaction Detail ID is required")]
        public int
          TransactionDetailID
        {
            get;
            set;
        }

        /// <summary>
        /// Weight Bridge ID
        /// </summary>        
        [DisplayName("Weight Bridge ID")]
        [Required(ErrorMessage = "Weight Bridge ID is required")]
        public int
          WeightBridgeID
        {
            get;
            set;
        }

        /// <summary>
        /// Sequence No
        /// </summary>        
        [DisplayName("Sequence No")]
        [Required(ErrorMessage = "Sequence No is required")]
        public byte
          SequenceNo
        {
            get;
            set;
        }

        /// <summary>
        /// Product ID
        /// </summary>        
        [DisplayName("Product ID")]
        public Nullable<int>
          ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// Gross Weight
        /// </summary>        
        [DisplayName("Gross Weight")]
        public Nullable<decimal>
          GrossWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Tare Weight
        /// </summary>        
        [DisplayName("Tare Weight")]
        public Nullable<decimal>
          TareWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gross Time
        /// </summary>        
        [DisplayName("Gross Time")]
        public Nullable<System.DateTime>
          GrossTime
        {
            get;
            set;
        }

        /// <summary>
        /// Tare Time
        /// </summary>        
        [DisplayName("Tare Time")]
        public Nullable<System.DateTime>
          TareTime
        {
            get;
            set;
        }

        /// <summary>
        /// WeighmentType
        /// </summary>        
        [DisplayName("Weighment Type")]
        [MaxLength(32, ErrorMessage = "Weighment Type cannot be longer than 32 characters")]
        public string
          WeighmentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gate ID
        /// </summary>        
        [DisplayName("Gate ID")]
        public Nullable<int>
          GateID
        {
            get;
            set;
        }

        /// <summary>
        /// User ID
        /// </summary>        
        [DisplayName("User ID")]
        public Nullable<int>
          UserID
        {
            get;
            set;
        }

        /// <summary>
        /// Capture Weight
        /// </summary>        
        [DisplayName("Capture Weight")]
        public Nullable<decimal>
          CaptureWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Capture Time
        /// </summary>        
        [DisplayName("Capture Time")]
        public Nullable<System.DateTime>
          CaptureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Net Weight
        /// </summary>        
        [DisplayName("Net Weight")]
        public Nullable<decimal>
          NetWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Weight Unit
        /// </summary>        
        [DisplayName("Weight Unit")]
        [MaxLength(20, ErrorMessage = "Weight Unit cannot be longer than 20 characters")]
        public string
          WeightUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Is Tare Manual
        /// </summary>        
        [DisplayName("Is Tare Manual")]
        [Required(ErrorMessage = "Is Tare Manual is required")]
        public bool
          IsTareManual
        {
            get;
            set;
        }

        /// <summary>
        /// Is Gross Manual
        /// </summary>        
        [DisplayName("Is Gross Manual")]
        [Required(ErrorMessage = "Is Gross Manual is required")]
        public bool
          IsGrossManual
        {
            get;
            set;
        }

        /// <summary>
        /// Is Captured Manual
        /// </summary>        
        [DisplayName("Is Captured Manual")]
        [Required(ErrorMessage = "Is Captured Manual is required")]
        public bool
          IsCapturedManual
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

    }
}
