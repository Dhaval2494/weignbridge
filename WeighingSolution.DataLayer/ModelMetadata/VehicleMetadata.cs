using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Vehicle Metadata class
    /// </summary>
    public
      partial class VehicleMetadata
    {
        /// <summary>
        /// Vehicle ID
        /// </summary>        
        [DisplayName("Vehicle ID")]
        [Required(ErrorMessage = "Vehicle ID is required")]
        public int
          VehicleID
        {
            get;
            set;
        }

        /// <summary>
        /// Vehicle Number
        /// </summary>        
        [DisplayName("Vehicle Number")]
        [Required(ErrorMessage = "Vehicle Number is required")]
        [MaxLength(50, ErrorMessage = "Vehicle Number cannot be longer than 50 characters")]
        public string
          VehicleNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Vehicle Type
        /// </summary>        
        [DisplayName("Vehicle Type")]
        [MaxLength(50, ErrorMessage = "Vehicle Type cannot be longer than 50 characters")]
        public string
          VehicleType
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
        /// Account ID
        /// </summary>        
        [DisplayName("Account ID")]
        public Nullable<int>
          AccountID
        {
            get;
            set;
        }

    }
}
