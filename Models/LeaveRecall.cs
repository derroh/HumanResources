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
    
    public partial class LeaveRecall
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LeaveRecall()
        {
            this.Attachments = new HashSet<Attachment>();
        }
    
        public string DocumentNo { get; set; }
        public string LeaveDocumentNo { get; set; }
        public int LeaveDaysRecalled { get; set; }
        public string EmployeeNo { get; set; }
        public string ApprovalStatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual Leaf Leaf { get; set; }
    }
}
