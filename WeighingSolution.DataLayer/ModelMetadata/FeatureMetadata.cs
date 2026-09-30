using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace WeighingSolution.DataLayer
{
    /// <summary>
    /// Feature Metadata class
    /// </summary>
    public
      partial class FeatureMetadata
    {
        /// <summary>
        /// Feature ID
        /// </summary>        
        [DisplayName("Feature ID")]
        [Required(ErrorMessage = "Feature ID is required")]
        public int
          FeatureID
        {
            get;
            set;
        }

        /// <summary>
        /// Feature Name
        /// </summary>        
        [DisplayName("Feature Name")]
        [Required(ErrorMessage = "Feature Name is required")]
        [MaxLength(100, ErrorMessage = "Feature Name cannot be longer than 100 characters")]
        public string
          FeatureName
        {
            get;
            set;
        }

        /// <summary>
        /// Action
        /// </summary>        
        [DisplayName("Action")]
        [Required(ErrorMessage = "Action is required")]
        [MaxLength(10, ErrorMessage = "Action cannot be longer than 10 characters")]
        public string
          Action
        {
            get;
            set;
        }

        /// <summary>
        /// Sort Order
        /// </summary>        
        [DisplayName("Sort Order")]
        [Required(ErrorMessage = "Sort Order is required")]
        public decimal
          SortOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Parent Feature ID
        /// </summary>        
        [DisplayName("Parent Feature ID")]
        public Nullable<int>
          ParentFeatureID
        {
            get;
            set;
        }

        /// <summary>
        /// Is Master
        /// </summary>        
        [DisplayName("Is Master")]
        [Required(ErrorMessage = "Is Master is required")]
        public bool
          IsMaster
        {
            get;
            set;
        }

    }
}
