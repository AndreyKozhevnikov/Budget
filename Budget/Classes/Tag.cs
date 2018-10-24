using Budget.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    public partial class Tag: ILocalEntity {
        public string ComplexValue { get; set; }
        public void GetPropertiesFromWebEntity(IWebEntity wTag) {
            TagName = ((WebTag)wTag).Name;
        }
    }
}
