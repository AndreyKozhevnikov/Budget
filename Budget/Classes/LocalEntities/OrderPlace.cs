﻿using Budget.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget {
    public partial class OrderPlace : ILocalEntity {
        public string ComplexValue { get; set; }
        public void GetPropertiesFromWebEntity(IWebEntity wEntity) {
            PlaceName = wEntity.Name;
        }
    }
}
