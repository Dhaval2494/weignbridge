using System;

namespace WeighingSolution.DataLayer
{
    public partial class Driver : NotifyDataErrorInfoBase
    {
        public string DriverName
        {
            get
            {
                return FirstName + (String.IsNullOrEmpty(LastName) ? "" : " " + LastName);
            }
        }
    }
}
