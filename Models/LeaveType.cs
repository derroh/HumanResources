//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HumanResources.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LeaveType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<int> TotalAbsence { get; set; }
        public string UnitOfMeasure { get; set; }
        public string AnnualLeaveDaysType { get; set; }
    }
}
