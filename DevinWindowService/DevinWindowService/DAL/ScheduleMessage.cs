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
    
    public partial class ScheduleMessage
    {
        public int ScheduleMessage_ID { get; set; }
        public string ScheduleMessage_Message { get; set; }
        public Nullable<int> ScheduleMessage_TriggerTypeID { get; set; }
        public Nullable<int> ScheduleMessage_TriggerEventID { get; set; }
        public Nullable<int> ScheduleMessage_TriggerID { get; set; }
        public Nullable<System.DateTime> ScheduleMessage_StartTime { get; set; }
        public Nullable<System.DateTime> ScheduleMessage_EndTime { get; set; }
        public string ScheduleMessage_FrequencyOfDelivery { get; set; }
        public string ScheduleMessage_If1 { get; set; }
        public string ScheduleMessage_If2 { get; set; }
        public string ScheduleMessage_OtherNotes { get; set; }
        public string ScheduleMessage_TimesSent { get; set; }
        public Nullable<int> ScheduleMessage_MMDomainID { get; set; }
        public Nullable<int> ScheduleMessage_MMSubDomainID { get; set; }
        public string ScheduleMessage_Time { get; set; }
        public Nullable<int> Display_MessageId { get; set; }
        public Nullable<int> Appointment_Schedule { get; set; }
    
        public virtual TriggerType TriggerType { get; set; }
    }
}
