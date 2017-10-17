using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    public class GroupData {

        public string ParentTagName { get; set; }
        public int ParentTagId { get; set; }
        public int Value { get; set; }

        public int Count { get; set; }
        public int Average {
            get {
                if(Count > 0)
                    return Value / Count;
                return 0;
            }
        }
        List<Order> orders;
        public List<Order> Orders {
            get {
                return orders;
            }
            set {
                orders = value;
                if(orders.Count > 0) {
                    Value = orders.Sum(x => x.Value);
                    MaxValue = orders.Max(x => x.Value);
                    MinValue = orders.Min(x => x.Value);
                    StartDate = orders.Select(z => z.DateOrder).Min();
                    FinishDate = orders.Select(z => z.DateOrder).Max();
                }
                Count = orders.Count;
                Value = orders.Sum(x => x.Value);
            }
        }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int AverageInMonth { get; set; }
        public DateTime StartDate { get; set; }
        DateTime finishDate;
        public DateTime FinishDate {
            get {
                return finishDate;
            }
            set {
                finishDate = value;
                CalcAverageInMonth();
            }
        }
        int countOfMonths;
        public void CalcAverageInMonth() {
            countOfMonths = (int)Math.Ceiling((FinishDate.Subtract(StartDate).Days / (365.25 / 12)));
            if(countOfMonths > 0)
                AverageInMonth = (Value / countOfMonths);
            else
                AverageInMonth = Value;
        }
        public string ToExportString() {
            var st = string.Format("{0},  {1},  {2},  {3} ", ParentTagName, AverageInMonth, Value, countOfMonths);
            return st;
        }
    }


}
