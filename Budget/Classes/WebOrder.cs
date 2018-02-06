using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Classes {


    public class WebOrder {
        public string _id { get; set; }
        public DateTime DateOrder { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
        public WebTag ParentTag { get; set; }
        public bool IsJourney { get; set; }
        public string Tags { get; set; }
        public int __v { get; set; }
    }
}
