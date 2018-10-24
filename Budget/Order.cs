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
    
    public partial class Order
    {
        public int Id { get; set; }
        public System.DateTime DateOrder { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
        public int ParentTag { get; set; }
        public string Tags { get; set; }
        public bool Ignore { get; set; }
        public Nullable<int> OldParentTag { get; set; }
        public bool IsJourney { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
    
        public virtual PaymentType PaymentType { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
