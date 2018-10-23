using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    [DebuggerDisplay("TagName-{TagName} Id-{Id}")]
    public class MyTag2 {
        //test
        public Tag parentTagEntity;

        public MyTag2() { }

        public string TagName {
            get {
                return parentTagEntity.TagName;
            }
            set {
                parentTagEntity.TagName = value;
            }
        }

        public int Id {
            get {
                return parentTagEntity.Id;
            }
            set {
                parentTagEntity.Id = value;
            }
        }

        public void Save() {
            OrderViewModel.generalEntity.SaveChanges();
        }
    }

    public partial class Tag: IHaveId {
        public string ComplexValue { get; set; }
    }
}
