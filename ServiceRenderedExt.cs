using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP01_Prasolova_Clinic
{
    public partial class ServiceRendered
    {
        public decimal Total
        {
            get { return Quantity * Price; }
        }
    }
}
