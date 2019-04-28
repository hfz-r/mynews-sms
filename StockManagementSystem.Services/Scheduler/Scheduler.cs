using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using Quartz.Impl.Triggers;

namespace StockManagementSystem.Services.Scheduler
{
    public enum RepeatEnum
    {
        [Description("Every Day")]
        EveryDay = 1,

        [Description("Every Week")]
        EveryWeek = 2,

        [Description("Every Two Weeks")]
        EveryTwoWeeks = 3,

        [Description("Every Month")]
        EveryMonth = 4,

        [Description("Every Year")]
        EveryYear = 5
    }

    public static class Scheduler
    {
        public static void TriggerScheduler(string title, string description, string tokenID, DateTime startTime, DateTime endTime, int? repeat, string apiKey, string senderID)
        {
            // This Scheduler will start at hour:minute and call after every repeat time
            try
            {
                int numberDays = 0;

                if (repeat == (int)RepeatEnum.EveryDay)
                {
                    numberDays = 1;
                }
                else if (repeat == (int)RepeatEnum.EveryMonth)
                {
                    numberDays = 30;
                }
                else if (repeat == (int)RepeatEnum.EveryTwoWeeks)
                {
                    numberDays = 14;
                }
                else if (repeat == (int)RepeatEnum.EveryWeek)
                {
                    numberDays = 7;
                }
                else if (repeat == (int)RepeatEnum.EveryYear)
                {
                    numberDays = 365;
                }

                Scheduler.IntervalInSeconds(startTime.Hour, startTime.Minute, numberDays,
                () => {
                    HttpWebRequest tRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "POST";
                    tRequest.ContentType = "application/json";
                    //serverKey - Key from Firebase cloud messaging server  
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", apiKey));
                    ////Sender Id - From firebase project setting  
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderID));

                    var payload = new
                    {
                        to = tokenID,
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = description,
                            title = title
                        },
                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (HttpWebResponse tResponse = (HttpWebResponse)tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

        public static void IntervalInSeconds(int hour, int sec, double interval, Action task)
        {
            interval = interval / 3600;
            SchedulerService.Instance.ScheduleTask(hour, sec, interval, task);
        }

        public static void IntervalInMinutes(int hour, int min, double interval, Action task)
        {
            interval = interval / 60;
            SchedulerService.Instance.ScheduleTask(hour, min, interval, task);
        }

        public static void IntervalInHours(int hour, int min, double interval, Action task)
        {
            SchedulerService.Instance.ScheduleTask(hour, min, interval, task);
        }

        public static void IntervalInDays(int hour, int min, double interval, Action task)
        {
            interval = interval * 24;
            SchedulerService.Instance.ScheduleTask(hour, min, interval, task);
        }

        private static int GetNumDays(int? repeat, DateTime startTime)
        {
            int numberDays = 0;

            if (repeat == (int)RepeatEnum.EveryDay)
            {
                numberDays = 1;
            }
            else if (repeat == (int)RepeatEnum.EveryMonth)
            {
                numberDays = startTime.Day;
            }
            else if (repeat == (int)RepeatEnum.EveryTwoWeeks)
            {
                numberDays = 14;
            }
            else if (repeat == (int)RepeatEnum.EveryWeek)
            {
                numberDays = 7;
            }
            else if (repeat == (int)RepeatEnum.EveryYear)
            {
                numberDays = 365;
            }
            return numberDays;
        }

        public static void Start(string jobTitle, string groupTitle, string title, string description, string tokenID, DateTime startTime, DateTime endTime, int? repeat, string apiKey/*, string senderID*/)
        {
            int numberDays = GetNumDays(repeat, startTime);

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

            scheduler.Start();

            //Build a trigger that will fire on start date, then repeat every interval selected by user, until end of day of end date
            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(jobTitle, groupTitle)
            .StartAt(startTime) // if a start time is not given (if this line were omitted), "now" is implied
            .EndAt(endTime)
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(numberDays * 24)
                .RepeatForever())
            .Build();

            IJobDetail job = JobBuilder.Create<SchedulerJob>()
                .WithIdentity(jobTitle, groupTitle)
                .UsingJobData("title", title)
                .UsingJobData("description", description)
                .UsingJobData("tokenID", tokenID)
                .UsingJobData("apiKey", apiKey)
                //.UsingJobData("senderID", senderID)
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

    }
}

