using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget.Classes {
    public class UniqueDateKeeper {
        public UniqueDateKeeper(IEnumerable<DateTime> list) {
            UniqueDateCollection = new Dictionary<DateTime, int>();
            var lst = list.Distinct();
            CreateIndexes(lst);
        }
        public Dictionary<DateTime, int> UniqueDateCollection { get; set; }
        public void AddDate(DateTime dt) {
            if(UniqueDateCollection.ContainsKey(dt)) {
                return;
            }
            bool isLastDate = UniqueDateCollection.Where(x => x.Key > dt).Count() == 0;
            if(isLastDate) {
                int lastValue = UniqueDateCollection.Last().Value;
                int newValue = (++lastValue + 3) % 3;
                UniqueDateCollection.Add(dt, newValue);
            } else {
                UniqueDateCollection.Add(dt, -1);
                CreateIndexes(null);
            }

        }

        void CreateIndexes(IEnumerable<DateTime> list) {
            if(list == null){
                list =new List<DateTime>( UniqueDateCollection.Keys.OrderBy(x => x));
            }
            UniqueDateCollection.Clear();
            int index = 0;
            foreach(var dt in list) {
                UniqueDateCollection.Add(dt, (index++ + 3) % 3);
            }
        }
    }


    [TestFixture]
    public class UniqueDateKeeperTest {
        [Test]
        public void Constructor() {
            //arrange
            var lst = new List<DateTime>();
            lst.Add(new DateTime(2018, 3, 20));
            lst.Add(new DateTime(2018, 3, 21));
            lst.Add(new DateTime(2018, 3, 22));
            lst.Add(new DateTime(2018, 3, 20));
            lst.Add(new DateTime(2018, 3, 28));

            //act
            UniqueDateKeeper keeper = new UniqueDateKeeper(lst);
            //assert
            Assert.AreEqual(0, keeper.UniqueDateCollection[new DateTime(2018, 3, 20)]);
            Assert.AreEqual(1, keeper.UniqueDateCollection[new DateTime(2018, 3, 21)]);
            Assert.AreEqual(2, keeper.UniqueDateCollection[new DateTime(2018, 3, 22)]);
            Assert.AreEqual(0, keeper.UniqueDateCollection[new DateTime(2018, 3, 28)]);

        }
        [Test]
        public void AddNewDateInTheEnd() {
            //arrange
            var lst = new List<DateTime>();
            lst.Add(new DateTime(2018, 3, 20));
            lst.Add(new DateTime(2018, 3, 21));
            lst.Add(new DateTime(2018, 3, 22));
            //act
            UniqueDateKeeper keeper = new UniqueDateKeeper(lst);
            keeper.AddDate(new DateTime(2018, 3, 29));
            //assert
            Assert.AreEqual(0, keeper.UniqueDateCollection[new DateTime(2018, 3, 29)]);
        }

        [Test]
        public void AddNewDateInTheMiddle() {
            //arrange
            var lst = new List<DateTime>();
            lst.Add(new DateTime(2018, 3, 20));
            lst.Add(new DateTime(2018, 3, 21));
            lst.Add(new DateTime(2018, 3, 22));
            lst.Add(new DateTime(2018, 3, 28));
            //act
            UniqueDateKeeper keeper = new UniqueDateKeeper(lst);
            int firstValueof28 = keeper.UniqueDateCollection[new DateTime(2018, 3, 28)];
            keeper.AddDate(new DateTime(2018, 3, 27));
            //assert
            Assert.AreEqual(0, firstValueof28);
            Assert.AreEqual(0, keeper.UniqueDateCollection[new DateTime(2018, 3, 27)]);
            Assert.AreEqual(1, keeper.UniqueDateCollection[new DateTime(2018, 3, 28)]);


        }
    }


}