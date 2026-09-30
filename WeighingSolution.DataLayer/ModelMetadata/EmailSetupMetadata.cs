using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// EmailSetup Metadata class
    /// </summary>
    public
      partial class EmailSetupMetadata
    {
        /// <summary>
        /// Email Setup ID
        /// </summary>        
        [DisplayName("Email Setup ID")]
        [Required(ErrorMessage = "Email Setup ID is required")]
        public int
          EmailSetupID
        {
            get;
            set;
        }

        /// <summary>
        /// Schedule Name
        /// </summary>        
        [DisplayName("Schedule Name")]
        [Required(ErrorMessage = "Schedule Name is required")]
        [MaxLength(50, ErrorMessage = "Schedule Name cannot be longer than 50 characters")]
        public string
          ScheduleName
        {
            get;
            set;
        }

        /// <summary>
        /// Schedule Description
        /// </summary>        
        [DisplayName("Schedule Description")]
        public string
          ScheduleDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Is Enable
        /// </summary>        
        [DisplayName("Is Enable")]
        [Required(ErrorMessage = "Is Enable is required")]
        public bool
          IsEnable
        {
            get;
            set;
        }

        /// <summary>
        /// Email Type
        /// </summary>        
        [DisplayName("Email Type")]
        [Required(ErrorMessage = "Email Type is required")]
        [MaxLength(50, ErrorMessage = "Email Type cannot be longer than 50 characters")]
        public string
          EmailType
        {
            get;
            set;
        }

        /// <summary>
        /// Email To
        /// </summary>        
        [DisplayName("Email To")]
        [RegularExpression(@"(([a-zA-Z_\-0-9_\.\-]+@)([a-zA-Z\-0-9]+\.)([a-zA-Z\-0-9]+\.)([a-zA-Z\-0-9]+)[;])+", ErrorMessage = "Email To is not valid. Seperate multiple emails by ; without any space.")]
        [Required(ErrorMessage = "Email To is required")]
        public string
          EmailTo
        {
            get;
            set;
        }

        /// <summary>
        /// Email Subject
        /// </summary>        
        [DisplayName("Email Subject")]
        [Required(ErrorMessage = "Email Subject is required")]
        public string
          EmailSubject
        {
            get;
            set;
        }

        /// <summary>
        /// Email Body
        /// </summary>        
        [DisplayName("Email Body")]
        public string
          EmailBody
        {
            get;
            set;
        }

        /// <summary>
        /// Column Selection
        /// </summary>        
        [DisplayName("Column Selection")]
        public string
          ColumnSelection
        {
            get;
            set;
        }

        /// <summary>
        /// Schedule Type
        /// </summary>        
        [DisplayName("Schedule Type")]
        [MaxLength(50, ErrorMessage = "Schedule Type cannot be longer than 50 characters")]
        public string
          ScheduleType
        {
            get;
            set;
        }

        /// <summary>
        /// Schedule Time
        /// </summary>        
        [DisplayName("Schedule Time")]
        [MaxLength(200, ErrorMessage = "Schedule Time cannot be longer than 200 characters")]
        public string
          ScheduleTime
        {
            get;
            set;
        }

        /// <summary>
        /// Next Run Time
        /// </summary>        
        [DisplayName("Next Run Time")]
        public Nullable<System.DateTime>
          NextRunTime
        {
            get;
            set;
        }

        /// <summary>
        /// Filter Detail
        /// </summary>        
        [DisplayName("Filter Detail")]
        public string
          FilterDetail
        {
            get;
            set;
        }

        /// <summary>
        /// Where Clause
        /// </summary>        
        [DisplayName("Where Clause")]
        public string
          WhereClause
        {
            get;
            set;
        }

    }
}
