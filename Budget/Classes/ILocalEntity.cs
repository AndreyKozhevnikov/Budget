using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    interface ILocalEntity {
        int Id { get; }
        void GetPropertiesFromWebEntity(IWebEntity wEntity);
    }
}
