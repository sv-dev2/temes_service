//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DevinWindowService.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class TriggerType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TriggerType()
        {
            this.ScheduleMessages = new HashSet<ScheduleMessage>();
        }
    
        public int TriggerType_ID { get; set; }
        public string TriggerType_Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScheduleMessage> ScheduleMessages { get; set; }
    }
}