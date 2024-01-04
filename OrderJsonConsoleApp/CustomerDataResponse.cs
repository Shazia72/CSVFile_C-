using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderJsonConsoleApp
{
    public class CustomerDataResponse
    {
        public List<VM> CustomerData { get; set; }
    }
    public class VM
    {
        public double OrderNumber { get; set; }
        public int CustomerId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
