using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class Template : NotifyDataErrorInfoBase
    {
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "TemplateName":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (!String.IsNullOrEmpty(TemplateName))
                        {
                            if (TemplateID == 0)
                            {
                                if (ctx.Templates.Any(x => x.TemplateName.ToLower().Trim() == TemplateName.ToLower().Trim()))
                                {
                                    yield return "User role name already exist, can not add duplicate user role name.";
                                }
                            }
                            else
                            {
                                if (ctx.Templates.Any(x => x.TemplateName.ToLower().Trim() == TemplateName.ToLower().Trim() && x.TemplateID != TemplateID))
                                {
                                    yield return "User role name already exist, can not add duplicate user role name.";
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
