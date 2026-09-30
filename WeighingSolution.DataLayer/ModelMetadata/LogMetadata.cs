using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Log Metadata class
    /// </summary>
    public
      partial class LogMetadata
    {
        /// <summary>
        /// Id
        /// </summary>        
        [DisplayName("Id")]
        [Required(ErrorMessage = "Id is required")]
        public int
          Id
        {
            get;
            set;
        }

        /// <summary>
        /// Date
        /// </summary>        
        [DisplayName("Date")]
        [Required(ErrorMessage = "Date is required")]
        public System.DateTime
          Date
        {
            get;
            set;
        }

        /// <summary>
        /// Thread
        /// </summary>        
        [DisplayName("Thread")]
        [Required(ErrorMessage = "Thread is required")]
        [MaxLength(255, ErrorMessage = "Thread cannot be longer than 255 characters")]
        public string
          Thread
        {
            get;
            set;
        }

        /// <summary>
        /// Level
        /// </summary>        
        [DisplayName("Level")]
        [Required(ErrorMessage = "Level is required")]
        [MaxLength(50, ErrorMessage = "Level cannot be longer than 50 characters")]
        public string
          Level
        {
            get;
            set;
        }

        /// <summary>
        /// Logger
        /// </summary>        
        [DisplayName("Logger")]
        [Required(ErrorMessage = "Logger is required")]
        [MaxLength(255, ErrorMessage = "Logger cannot be longer than 255 characters")]
        public string
          Logger
        {
            get;
            set;
        }

        /// <summary>
        /// Message
        /// </summary>        
        [DisplayName("Message")]
        [Required(ErrorMessage = "Message is required")]
        [MaxLength(4000, ErrorMessage = "Message cannot be longer than 4000 characters")]
        public string
          Message
        {
            get;
            set;
        }

        /// <summary>
        /// Exception
        /// </summary>        
        [DisplayName("Exception")]
        [MaxLength(8000, ErrorMessage = "Exception cannot be longer than 8000 characters")]
        public string
          Exception
        {
            get;
            set;
        }

    }
}
