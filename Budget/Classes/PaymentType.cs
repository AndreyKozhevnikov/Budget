using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget
{
    public partial class PaymentType : ILocalEntity {
        public void GetPropertiesFromWebEntity(IWebEntity wEntity) {
            var wPType = (WebPaymentType)wEntity;
            this.Name = wPType.Name;
            this.IsYandex = wPType.IsYandex;
            this.CurrentCount = wPType.CurrentCount;
        }
    }
}
