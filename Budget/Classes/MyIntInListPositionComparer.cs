using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {

        class MyIntInListPositionComparer<T> : IComparer<int> {
            public MyIntInListPositionComparer(List<int> _innerList) {
                innerList = _innerList;
            }

            List<int> innerList;
            public int Compare(int x, int y) {
                var index1 = innerList.IndexOf(x);
                var index2 = innerList.IndexOf(y);
                var res = Comparer<int>.Default.Compare(index1, index2);

                //   Debug.Print(string.Format("{0} {1}  - {2} ", index1, index2, res));
                return res;
            }
        }
    
}
