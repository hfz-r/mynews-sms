using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace StockManagementSystem.Services.Scheduler
{
    public static class Scheduler
    {
        public static void TriggerScheduler(string title, string description, string tokenID, string serverKey, string senderID)
        {
            // For Interval in Seconds 
            // This Scheduler will start at 11:10 and call after every 15 Seconds
            // IntervalInSeconds(start_hour, start_minute, seconds)
            Scheduler.IntervalInSeconds(10, 35, 15,
            () => {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";

                //serverKey - Key from Firebase cloud messaging server  

                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));

                ////Sender Id - From firebase project setting  

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderID));
                tRequest.ContentType = "application/json";

                tRequest.Credentials = CredentialCache.DefaultCredentials;

                var payload = new
                {
                    to = tokenID,
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = description,
                        title = title,
                        badge = 1
                    },
                };

                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                        }
                    }
                }
            });
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
    }
}
