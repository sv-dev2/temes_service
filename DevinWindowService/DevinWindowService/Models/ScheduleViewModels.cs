using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevinWindowService.Models
{
    public class ScheduleViewModels
    {
        public int ScheduleMessageID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Time { get; set; }

        public string Message { get; set; }

        public int TriggerTypeId { get; set; }

        public string TriggerTypeName { get; set; }
        public virtual TriggerType TriggerType { get; set; }

        public int TriggerEventId { get; set; }

        public string TriggerEventName { get; set; }
        public string TriggerEventDay { get; set; }
        public virtual TriggerEvent TriggerEvent { get; set; }

        public int TriggerId { get; set; }
        public virtual Trigger Trigger { get; set; }
        public string TriggerName { get; set; }

        public string FrequencyOfDelivery { get; set; }
        public string If1 { get; set; }
        public string If2 { get; set; }
        public string OtherNotes { get; set; }

        public string TimeSent { get; set; }

        public int MMDomainId { get; set; }
        public virtual MMDomain MMDomain { get; set; }

        public string MMDomainName { get; set; }

        public int MMSubDomainId { get; set; }
        public virtual MMSubDomain MMSubDomain { get; set; }
        public string MMSubDomainName { get; set; }

        // Get the user List
     

    }

    public class TriggerType
    {
        public int TriggerTypeId { get; set; }
        public string TriggerTypeName { get; set; }
    }

    public class TriggerEvent
    {
        public int TriggerEventId { get; set; }
        public string TriggerEventName { get; set; }
    }

    public class Trigger
    {
        public int TriggerId { get; set; }
        public string TriggerName { get; set; }
    }

    public class MMDomain
    {
        public int MMDomainId { get; set; }
        public string MMDomainName { get; set; }
    }

    public class MMSubDomain
    {
        public int MMSubDomainId { get; set; }
        public string MMSubDomainName { get; set; }
    }
}
