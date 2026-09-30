using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// WeightBridge Metadata class
    /// </summary>
    public
      partial class WeightBridgeMetadata
    {
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
        /// Scale Name
        /// </summary>        
        [DisplayName("Scale Name")]
        [Required(ErrorMessage = "Scale Name is required")]
        [MaxLength(250, ErrorMessage = "Scale Name cannot be longer than 250 characters")]
        public string
          ScaleName
        {
            get;
            set;
        }

        /// <summary>
        /// Max Capacity
        /// </summary>        
        [DisplayName("Max Capacity")]
        [MaxLength(50, ErrorMessage = "Max Capacity cannot be longer than 50 characters")]
        public string
          MaxCapacity
        {
            get;
            set;
        }

        /// <summary>
        /// Increment Size
        /// </summary>        
        [DisplayName("Increment Size")]
        [MaxLength(50, ErrorMessage = "Increment Size cannot be longer than 50 characters")]
        public string
          IncrementSize
        {
            get;
            set;
        }

        /// <summary>
        /// Unit ID
        /// </summary>        
        [DisplayName("Unit ID")]
        [Required(ErrorMessage = "Unit is required")]
        public Nullable<int>
          UnitID
        {
            get;
            set;
        }

        /// <summary>
        /// Is Active
        /// </summary>        
        [DisplayName("Is Active")]
        [Required(ErrorMessage = "Is Active is required")]
        public bool
          IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Connection Type
        /// </summary>        
        [DisplayName("Connection Type")]
        [MaxLength(20, ErrorMessage = "Connection Type cannot be longer than 20 characters")]
        public string
          ConnectionType
        {
            get;
            set;
        }

        /// <summary>
        /// IPAddress
        /// </summary>        
        [DisplayName("IPAddress")]
        [MaxLength(40, ErrorMessage = "IPAddress cannot be longer than 40 characters")]
        public string
          IPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// IPPort
        /// </summary>        
        [DisplayName("IPPort")]
        [MaxLength(10, ErrorMessage = "IPPort cannot be longer than 10 characters")]
        public string
          IPPort
        {
            get;
            set;
        }

        /// <summary>
        /// COMPort
        /// </summary>        
        [DisplayName("COMPort")]
        [MaxLength(10, ErrorMessage = "COMPort cannot be longer than 10 characters")]
        public string
          COMPort
        {
            get;
            set;
        }

        /// <summary>
        /// Baud Rate
        /// </summary>        
        [DisplayName("Baud Rate")]
        [MaxLength(20, ErrorMessage = "Baud Rate cannot be longer than 20 characters")]
        public string
          BaudRate
        {
            get;
            set;
        }

        /// <summary>
        /// Data Bits
        /// </summary>        
        [DisplayName("Data Bits")]
        public Nullable<byte>
          DataBits
        {
            get;
            set;
        }

        /// <summary>
        /// Stop Bits
        /// </summary>        
        [DisplayName("Stop Bits")]
        [MaxLength(50, ErrorMessage = "Stop Bits cannot be longer than 50 characters")]
        public string
          StopBits
        {
            get;
            set;
        }

        /// <summary>
        /// Parity
        /// </summary>        
        [DisplayName("Parity")]
        [MaxLength(10, ErrorMessage = "Parity cannot be longer than 10 characters")]
        public string
          Parity
        {
            get;
            set;
        }

        /// <summary>
        /// Total String Length
        /// </summary>        
        [DisplayName("Total String Length")]
        [Required(ErrorMessage = "Total String Length is required")]
        [Range(1, 999, ErrorMessage = "Total string length can be 1 to 999")]
        public Nullable<short>
          TotalStringLength
        {
            get;
            set;
        }

        /// <summary>
        /// Start String Char
        /// </summary>        
        [DisplayName("Start String Char")]
        [MaxLength(50, ErrorMessage = "Start String Char cannot be longer than 50 characters")]
        public string
          StartStringChar
        {
            get;
            set;
        }

        /// <summary>
        /// String Char Length
        /// </summary>        
        [DisplayName("String Char Length")]
        [MaxLength(50, ErrorMessage = "String Char Length cannot be longer than 50 characters")]
        public string
          StringCharLength
        {
            get;
            set;
        }

        /// <summary>
        /// Format
        /// </summary>        
        [DisplayName("Format")]
        [Required(ErrorMessage = "Format is required")]
        [MaxLength(50, ErrorMessage = "Format cannot be longer than 50 characters")]
        public string
          Format
        {
            get;
            set;
        }

        /// <summary>
        /// String Starting Value
        /// </summary>        
        [DisplayName("String Starting Value")]
        [MaxLength(50, ErrorMessage = "String Starting Value cannot be longer than 50 characters")]
        public string
          StringStartingValue
        {
            get;
            set;
        }

        /// <summary>
        /// Is Decimal In String
        /// </summary>        
        [DisplayName("Is Decimal In String")]
        public Nullable<bool>
          IsDecimalInString
        {
            get;
            set;
        }

        /// <summary>
        /// Decimal Point Location
        /// </summary>        
        [DisplayName("Decimal Point Location")]
        public Nullable<short>
          DecimalPointLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Is Reverse String
        /// </summary>        
        [DisplayName("Is Reverse String")]
        public Nullable<bool>
          IsReverseString
        {
            get;
            set;
        }

        /// <summary>
        /// WEIGHTSTARTFROM
        /// </summary>        
        [DisplayName("Weight Start From")]
        [Required(ErrorMessage = "Weight Start From is required")]
        [MaxLength(50, ErrorMessage = "Weight Start From cannot be longer than 50 characters")]
        public string
          WEIGHTSTARTFROM
        {
            get;
            set;
        }

        /// <summary>
        /// WEIGHTLENGTH
        /// </summary>        
        [DisplayName("Weight length")]
        [Required(ErrorMessage = "Weight length is required")]
        [MaxLength(50, ErrorMessage = "Weight length cannot be longer than 50 characters")]
        public string
          WEIGHTLENGTH
        {
            get;
            set;
        }

        /// <summary>
        /// Hand Shake
        /// </summary>        
        [DisplayName("Hand Shake")]
        [MaxLength(50, ErrorMessage = "Hand Shake cannot be longer than 50 characters")]
        public string
          HandShake
        {
            get;
            set;
        }

    }
}
