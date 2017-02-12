using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
  public  class GroupData {
 
      public string ParentTagName { get; set; }
      public int ParentTagId { get; set; }
      public int Value { get; set; }

      public int Count { get; set; }
      public int Average {
          get {
              if (Count>0)
              return Value / Count;
              return 0;
          }
      }
      public int MaxValue { get; set; }
      public int MinValue { get; set; }
    }
}
