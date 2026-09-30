using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// SMSSetup Metadata class
    /// </summary>
    public
      partial class SMSSetupMetadata
    {
        /// <summary>
        /// SMS Setup ID
        /// </summary>        
        [DisplayName("SMS Setup ID")]
        [Required(ErrorMessage = "SMS Setup ID is required")]
        public int SMSSetupID { get; set; }

        /// <summary>
        /// Schedule Name
        /// </summary>        
        [DisplayName("Schedule Name")]
        [Required(ErrorMessage = "Schedule Name is required")]
        [MaxLength(50, ErrorMessage = "Schedule Name cannot be longer than 50 characters")]
        public string ScheduleName { get; set; }

        /// <summary>
        /// Schedule Description
        /// </summary>        
        [DisplayName("Schedule Description")]
        public string ScheduleDescription { get; set; }

        /// <summary>
        /// Is Enable
        /// </summary>        
        [DisplayName("Is Enable")]
        [Required(ErrorMessage = "Is Enable is required")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// SMS Type
        /// </summary>        
        [DisplayName("SMS Type")]
        [Required(ErrorMessage = "SMS Type is required")]
        [MaxLength(50, ErrorMessage = "SMS Type cannot be longer than 50 characters")]
        public string SMSType { get; set; }

        /// <summary>
        /// SMS To
        /// </summary>        
        [DisplayName("SMS To")]
        //[Required(ErrorMessage = "SMS To is required")]
        public string SMSTo { get; set; }

        /// <summary>
        /// SMS Body
        /// </summary>        
        [DisplayName("SMS Body")]
        public string SMSBody { get; set; }

        /// <summary>
        /// Column Selection
        /// </summary>        
        [DisplayName("Column Selection")]
        public string ColumnSelection { get; set; }

        /// <summary>
        /// Schedule Type
        /// </summary>        
        [DisplayName("Schedule Type")]
        [MaxLength(50, ErrorMessage = "Schedule Type cannot be longer than 50 characters")]
        public string ScheduleType { get; set; }

        /// <summary>
        /// Schedule Time
        /// </summary>        
        [DisplayName("Schedule Time")]
        [MaxLength(200, ErrorMessage = "Schedule Time cannot be longer than 200 characters")]
        public string ScheduleTime { get; set; }

        /// <summary>
        /// Next Run Time
        /// </summary>        
        [DisplayName("Next Run Time")]
        public Nullable<System.DateTime> NextRunTime { get; set; }

        /// <summary>
        /// Filter Detail
        /// </summary>        
        [DisplayName("Filter Detail")]
        public string FilterDetail { get; set; }

        /// <summary>
        /// Where Clause
        /// </summary>        
        [DisplayName("Where Clause")]
        public string WhereClause { get; set; }

    }
}
