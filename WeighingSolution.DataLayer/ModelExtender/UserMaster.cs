using System.Collections.Generic;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class UserMaster : NotifyDataErrorInfoBase
    {
        private string _ConfirmPassword;
        public string ConfirmPassword
        {
            get { return _ConfirmPassword; }
            set { _ConfirmPassword = value; OnPropertyChanged(); ValidatePropertyInternal("ConfirmPassword", value); }
        }
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "UserName":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (UserID == 0)
                        {
                            if (ctx.UserMasters.Any(x => x.UserName.ToLower() == UserName.ToLower()))
                            {
                                yield return "User name already exist, can not add duplicate user name.";
                            }
                        }
                        else
                        {
                            if (ctx.UserMasters.Any(x => x.UserName.ToLower() == UserName.ToLower() && x.UserID != UserID))
                            {
                                yield return "User name already exist, can not add duplicate user name.";
                            }
                        }
                    }
                    break;

                case "ConfirmPassword":
                    if (Password != ConfirmPassword)
                    {
                        yield return "Confirm Password does not match";
                    }
                    break;
            }
        }
    }
}
