using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DevinWindowService.DAL;
using DevinWindowService.Models;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio;
using System.Globalization;

namespace DevinWindowService
{
    public partial class Scheduler : ServiceBase
    {
        private Timer timer1 = null;
        private Timer timer2 = null;
        const string accountSid = "ACb61b5dc2ad53c6336667449e67302c48";
        const string authToken = "b1dfd0ebd09b0bd2d840f07618b33a27";
        public Scheduler()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = 1000 * 60 * 60;  // Time interval for 1 hour
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;

            timer2 = new Timer();
            // this.timer2.Interval = 1000 * 60 * 60 * 24;  // Time Interval for 1 day
            this.timer2.Interval = 1000 * 60 * 60;  // Time Interval for 1 hour
            this.timer2.Elapsed += new System.Timers.ElapsedEventHandler(this.timer2_Tick);
            timer2.Enabled = true;
            Library.WriteErrorLog("Devin Window service started");
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            Library.WriteErrorLog("Devin Window service stopped");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            //creating and opening connection
            //con = new SqlConnection("server=192.168.5.253\\MSSQL2014; database=DevinTwilio;uid=sa;password=data@123");
            // Send the appointment messages automatically
            Library.WriteErrorLog("Timer 1 has been started.");
            SendAppointmentMessage();
        }
        private void timer2_Tick(object sender, ElapsedEventArgs e)
        {
            // Send the schedule messages automatically
            Library.WriteErrorLog("Timer 2 has been started.");
            SendScheduledMessage();

        }
        public void SendAppointmentMessage()
        {
            try
            {
                Library.WriteErrorLog("Inside Send Appointment Messages");
                DevinTwilioEntities _context = new DevinTwilioEntities();
                TwilioClient.Init(accountSid, authToken);
                var list = (from appoint in _context.Appointments
                            join user in _context.Users on appoint.Appointment_PatientID equals user.User_ID
                            join msg in _context.ScheduleMessages on appoint.Appointment_Trigger_EventId equals msg.ScheduleMessage_TriggerEventID
                            join trgEventt in _context.TriggerEvents on appoint.Appointment_Trigger_EventId equals trgEventt.TriggerEvent_ID
                            // join trigg in _context.Triggers on msg.ScheduleMessage_TriggerID equals trigg.Trigger_ID
                            // join triggtype in _context.TriggerTypes on msg.ScheduleMessage_TriggerTypeID equals triggtype.TriggerType_ID
                            select new MessageFrequencyModel
                            {
                                MessageId = msg.ScheduleMessage_ID,
                                Message = msg.ScheduleMessage_Message,
                                UserId = user.User_ID,
                                UserName = user.User_Name,
                                MobileNum = user.MobileNumber,
                                // Trigger = trigg.Trigger_Trigger,
                              //  TriggerId = (int)msg.ScheduleMessage_TriggerID,
                                TriggerEventId = trgEventt.TriggerEvent_ID,
                                // TriggerType = triggtype.TriggerType_Type,
                                AppointmentDate = appoint.Appointment_StartTime,
                                MobileNumber = user.MobileNumber,
                                // AppoinmentType = trgEventTypes.TriggerEvent_Event,
                                TriggerEventDay = trgEventt.TriggerEvent_Day,
                                Time = msg.ScheduleMessage_Time,
                                AppointmentId = appoint.Appointment_ID,
                                StatusId = user.StatusId,
                                Day = msg.Appointment_Schedule,
                                Appointment_Trigger_EventId = appoint.Appointment_Trigger_EventId ?? 0
                            }).Distinct().ToList();
                var distnctMessageList = new List<MessageFrequencyModel>();
                foreach (var item in list)
                {
                    if (distnctMessageList.FirstOrDefault(c => c.AppointmentId == item.AppointmentId) == null)
                    {
                        distnctMessageList.Add(item);
                    }
                }
                foreach (var item in distnctMessageList)
                {
                    if (item.Day != null)
                    {
                        if (item.StatusId == 1 || item.StatusId == 3) // for active or pause
                        {
                            var to = new PhoneNumber(item.MobileNumber.ToString());
                            Library.WriteErrorLog("Appointment to " + to);
                            var from = new PhoneNumber("+16467591379");
                            Library.WriteErrorLog("Appointment from " + from);

                            if ((item.AppointmentDate != null) && (!string.IsNullOrEmpty(item.TriggerEventDay)))
                            {
                                DateTime currentAppointmentDate = Convert.ToDateTime(item.AppointmentDate);
                                Library.WriteErrorLog("currentAppointmentDate " + currentAppointmentDate);
                                DateTime currentDate = DateTime.Now;
                                Library.WriteErrorLog("currentDate " + currentDate);
                                //  int dayDiff = Convert.ToInt32((currentAppointmentDate - currentDate).TotalDays);

                                var finalAppoinmentDate = item.AppointmentDate.Value.AddDays(item.Day.Value);

                                //    var calculatedTime = currentDate.ToString("hh:mm:ss tt"); // 7:00 AM // 12 hour clock
                                var calculatedTime = currentDate.ToString("htt");
                                var msgToBeSend = item.Message;
                                var timeAfterRemovingSpace = "";

                                if (!string.IsNullOrEmpty(item.Time))
                                {
                                    timeAfterRemovingSpace = item.Time.Trim().Replace(" ", "");
                                    timeAfterRemovingSpace = timeAfterRemovingSpace.Replace(":00", "");
                                    timeAfterRemovingSpace = timeAfterRemovingSpace.Replace(":30", "");
                                    timeAfterRemovingSpace = timeAfterRemovingSpace.TrimStart('0');
                                }

                                var setTime = item.Time == null ? "9am" : timeAfterRemovingSpace;

                                Library.WriteErrorLog("appoinment date after adding number:" + finalAppoinmentDate);
                                Library.WriteErrorLog("calculatedTime:" + calculatedTime);
                                Library.WriteErrorLog("setTime:" + setTime);
                                try
                                {
                                    if (currentDate.Date == finalAppoinmentDate.Date)
                                    {
                                        if (calculatedTime.ToLower() == setTime.ToLower())
                                        {
                                            Library.WriteErrorLog("Inside Condition");
                                            // Appointcondition wll be for 1 day and 7  day before

                                            var eventMessages = new List<ScheduleMessage>();
                                            eventMessages = _context.ScheduleMessages.Where(p => p.ScheduleMessage_TriggerEventID == item.Appointment_Trigger_EventId && p.Appointment_Schedule == item.Day.Value).ToList();

                                            foreach (var em in eventMessages)
                                            {
                                                var userStatusDetails = _context.UserEnrollments.FirstOrDefault(c => c.UserId == item.UserId && c.TriggerEventId == em.ScheduleMessage_TriggerEventID);
                                                if (userStatusDetails == null)
                                                {
                                                    string Sid = String.Empty;
                                                    var message0 = MessageResource.Create(to: to, from: from, body: msgToBeSend);
                                                    Sid = Convert.ToString(message0.Sid);
                                                    Library.WriteErrorLog("Message Sid " + Sid);
                                                    if (!string.IsNullOrEmpty(Sid))
                                                    {
                                                        SmsReply _smsReply = new SmsReply();
                                                        _smsReply.From = Convert.ToString(from);
                                                        _smsReply.To = Convert.ToString(to);
                                                        _smsReply.Sid = Convert.ToString(Sid);
                                                        _smsReply.ParentSid = null;
                                                        _smsReply.MessageId = item.MessageId;
                                                        _smsReply.UserId = item.UserId;
                                                        _smsReply.SentDate = DateTime.Now;
                                                        _context.SmsReplies.Add(_smsReply);
                                                        _context.SaveChanges();
                                                        Library.WriteErrorLog("Appoinment Message saved in the SMSReply table");
                                                    }
                                                }
                                                Library.WriteErrorLog("Successfully send message for appointment");
                                            }

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Library.WriteErrorLog("Appointment : " + ex.Message);
                                }

                            }

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Library.WriteErrorLog(ex.Message);
            }

        }

        public void SendScheduledMessage()
        {
            try
            {
                Library.WriteErrorLog("Inside Send Scheduled Messages");
                TwilioClient.Init(accountSid, authToken);
                using (DevinTwilioEntities dc = new DevinTwilioEntities())
                {
                    // var statusList = dc.UserStatus.ToList();
                    var users = dc.Users.Where(c => c.EnrollmentDate != null).ToList();
                    // var userEnrollmentList = dc.UserEnrollments.ToList();

                    //if(users.id)
                    //{

                    //}

                    var list = (from schMessage in dc.ScheduleMessages
                                join trgEvent in dc.TriggerEvents.Where(c => c.TriggerTypeId == 5)
                                on schMessage.ScheduleMessage_TriggerEventID equals trgEvent.TriggerEvent_ID
                                select new ScheduleViewModels
                                {
                                    Message = schMessage.ScheduleMessage_Message,
                                    // TriggerEventName = trgEvent.TriggerEvent_Event,
                                    TriggerEventDay = trgEvent.TriggerEvent_Day,
                                    TriggerEventId = trgEvent.TriggerEvent_ID,
                                    ScheduleMessageID = schMessage.ScheduleMessage_ID,
                                    Time = schMessage.ScheduleMessage_Time
                                }).ToList();


                    try
                    {
                        foreach (var item in users)
                        {
                            if (item.StatusId == 1) // 1- Active
                            {

                                Library.WriteErrorLog("Inside Patient loop");
                                var to = new PhoneNumber(item.MobileNumber.ToString());
                                Library.WriteErrorLog("To Number: " + to);
                                var from = new PhoneNumber("+16467591379");
                                Library.WriteErrorLog("From Number: " + from);
                                var EnrollmentDate = item.EnrollmentDate;
                                Library.WriteErrorLog("EnrollmentDate: " + EnrollmentDate);
                                if (EnrollmentDate != null)
                                {
                                    DateTime currentDate = DateTime.Now;
                                    Library.WriteErrorLog("Current Date: " + currentDate);
                                    DateTime currentEnrollmentDate = Convert.ToDateTime(item.EnrollmentDate);
                                    Library.WriteErrorLog("Current EnrollmentDate: " + currentEnrollmentDate);
                                    int totalDays = Convert.ToInt32((currentDate - currentEnrollmentDate).TotalDays);
                                    Library.WriteErrorLog(" scheduled days: " + totalDays);
                                    //if (totalDays >= 0 && totalDays <= 50)
                                    //{
                                    Library.WriteErrorLog("Inside total days comparison");
                                    string Sid = String.Empty;
                                    // var messageDetails = list.FirstOrDefault(c => c.TriggerEventName.ToLower() == "day " + totalDays);
                                    //var messageDetails = list.FirstOrDefault(c => c.TriggerEventDay == totalDays.ToString());
                                    var messageList = list.Where(c => c.TriggerEventDay == totalDays.ToString());

                                    if (messageList.Count() > 0)
                                    {
                                        foreach (var msg in messageList)
                                        {
                                            var userStatusDetails = dc.UserEnrollments.FirstOrDefault(c => c.UserId == item.User_ID && c.TriggerEventId == msg.TriggerEventId);
                                            if (userStatusDetails == null)
                                            {
                                                var calculatedTime = DateTime.Now.ToString("htt");

                                                var timeAfterRemovingSpace = "";

                                                if (!string.IsNullOrEmpty(msg.Time))
                                                {
                                                    timeAfterRemovingSpace = msg.Time.Trim().Replace(" ", "");
                                                    timeAfterRemovingSpace = timeAfterRemovingSpace.Replace(":00", "");
                                                    timeAfterRemovingSpace = timeAfterRemovingSpace.Replace(":30", "");

                                                    //string no_start_zeros = s.TrimStart('0');
                                                    timeAfterRemovingSpace = timeAfterRemovingSpace.TrimStart('0');
                                                }

                                                var setTime = msg.Time == null ? "9am" : timeAfterRemovingSpace;

                                                if (calculatedTime.ToLower() == setTime.ToLower())
                                                {
                                                    var message0 = MessageResource.Create(to: to, from: from, body: msg.Message);
                                                    Sid = Convert.ToString(message0.Sid);
                                                    Library.WriteErrorLog("Schedule Message Sid " + Sid);
                                                    if (!string.IsNullOrEmpty(Sid))
                                                    {
                                                        SmsReply _smsReply = new SmsReply();
                                                        _smsReply.From = Convert.ToString(from);
                                                        _smsReply.To = Convert.ToString(to);
                                                        _smsReply.Sid = Convert.ToString(Sid);
                                                        _smsReply.ParentSid = null;
                                                        _smsReply.MessageId = msg.ScheduleMessageID;
                                                        _smsReply.UserId = item.User_ID;
                                                        _smsReply.SentDate = DateTime.Now;
                                                        dc.SmsReplies.Add(_smsReply);
                                                        dc.SaveChanges();
                                                        Library.WriteErrorLog("Schedule Message saved in the SMSReply table");
                                                    }
                                                    Library.WriteErrorLog("Successfully send message for scheduled");
                                                }


                                            }
                                            else
                                            {
                                                Library.WriteErrorLog("Already user send, skip or pause for days: " + totalDays);
                                            }
                                        }
                                    }

                                    //if admin alredy skip, send adn pause message for this this user
                                    //var userStatusDetails = userEnrollmentList.FirstOrDefault(c => c.UserId == item.User_ID && c.TriggerEventId == messageDetails.TriggerEventId);
                                    //if (userStatusDetails == null)
                                    //{
                                    //    if (messageDetails != null)
                                    //    {
                                    //        // check the time

                                    //        var calculatedTime = DateTime.Now.ToString("htt");

                                    //        var timeAfterRemovingSpace = "";

                                    //        if (!string.IsNullOrEmpty(messageDetails.Time))
                                    //        {
                                    //            timeAfterRemovingSpace = messageDetails.Time.Trim().Replace(" ", "");
                                    //            timeAfterRemovingSpace = timeAfterRemovingSpace.Replace(":00", "");
                                    //            timeAfterRemovingSpace = timeAfterRemovingSpace.Replace(":30", "");

                                    //            //string no_start_zeros = s.TrimStart('0');
                                    //            timeAfterRemovingSpace = timeAfterRemovingSpace.TrimStart('0');
                                    //        }

                                    //        var setTime = messageDetails.Time == null ? "9am" : timeAfterRemovingSpace;

                                    //        if (calculatedTime.ToLower() == setTime.ToLower())
                                    //        {
                                    //            var message0 = MessageResource.Create(to: to, from: from, body: messageDetails.Message);
                                    //            Sid = Convert.ToString(message0.Sid);
                                    //            Library.WriteErrorLog("Schedule Message Sid " + Sid);
                                    //            if (!string.IsNullOrEmpty(Sid))
                                    //            {
                                    //                SmsReply _smsReply = new SmsReply();
                                    //                _smsReply.From = Convert.ToString(from);
                                    //                _smsReply.To = Convert.ToString(to);
                                    //                _smsReply.Sid = Convert.ToString(Sid);
                                    //                _smsReply.ParentSid = null;
                                    //                _smsReply.MessageId = messageDetails.ScheduleMessageID;
                                    //                _smsReply.UserId = item.User_ID;
                                    //                _smsReply.SentDate = DateTime.Now;
                                    //                dc.SmsReplies.Add(_smsReply);
                                    //                dc.SaveChanges();
                                    //                Library.WriteErrorLog("Schedule Message saved in the SMSReply table");
                                    //            }
                                    //            Library.WriteErrorLog("Successfully send message for scheduled");
                                    //        }

                                    //    }
                                    //    else
                                    //    {

                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    Library.WriteErrorLog("Already user send, skip or pause for days: " + totalDays);
                                    //}



                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Library.WriteErrorLog("Scheduled : " + ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {

                Library.WriteErrorLog(ex.Message);
            }

        }
    }
}
