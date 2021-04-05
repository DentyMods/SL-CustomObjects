using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLCustomObjects.Args
{
    public class PickupInteractEvent : EventArgs
    {
        public string SchematicName;
        public Pickup Pickup;
        public string EventName;
    }
}
