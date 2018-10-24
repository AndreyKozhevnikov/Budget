using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    public interface IWebEntity {
        int LocalId { get; set; }
        string _id { get; }
    }
}
