using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;

namespace WeighingSolution.Events
{
    public class ChangetokenEvent : PubSubEvent<ChangetokenEventArgs>
    {
    }
    public class AfterChangeTokenEvent:PubSubEvent<ChangetokenEventArgs>
    {
    }
    public class ChangetokenEventArgs
   {
       public TransactionTokenLog objTranTokenLog { get; set; }
   }
}
