using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    [DebuggerDisplay("DayDate-{DayDate} SumAll-{SumAll}, SumOfEat-{SumOfEat}")]
    public class DayData {
        public DayData() {

        }
        public DayData(DateTime _dt) {
            DayDate = _dt;
        }

        DateTime _dayDate;
        int _sumOfEat;
        int _sumAll;

        public DateTime DayDate {
            get { return _dayDate; }
            set { _dayDate = value; }
        }
        public int SumOfEat {
            get { return _sumOfEat; }
            set { _sumOfEat = value; }
        }
        public int SumAll {
            get { return _sumAll; }
            set { _sumAll = value; }
        }
    }

    public class DayOrderData {
        public DayOrderData(DateTime _dt) {
            DayDate = _dt;
        }
        DateTime _dayDate;
        int _value;

        public int Value {
            get { return _value; }
            set { _value = value; }
        }
        public DateTime DayDate {
            get { return _dayDate; }
            set { _dayDate = value; }
        }

    }
}
