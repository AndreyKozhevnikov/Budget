//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Budget
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderObject
    {
        public OrderObject()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public string ObjectName { get; set; }
        public int Id { get; set; }
    
        public virtual ICollection<Order> Orders { get; set; }
    }
}
