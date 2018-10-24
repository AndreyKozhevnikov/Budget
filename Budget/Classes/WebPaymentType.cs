using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget
{
    public class WebPaymentType:IWebEntity {
        public string _id { get; set; }
        public string Name { get; set; }
        public int CurrentCount { get; set; }
        public bool IsYandex { get; set; }
        public int LocalId { get; set; }
        public int __v { get; set; }
    }
}
