using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;

namespace StockManagementSystem.Services.Scheduler
{
    public class SchedulerJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobKey key = context.JobDetail.Key; //todo need?
                JobDataMap dataMap = context.JobDetail.JobDataMap;

                //Get data
                string title = dataMap.GetString("title");
                string description = dataMap.GetString("description");
                string tokenID = dataMap.GetString("tokenID");
                string apiKey = dataMap.GetString("apiKey");
                string senderID = dataMap.GetString("senderID");

                HttpWebRequest tRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "POST";
                tRequest.ContentType = "application/json";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", apiKey));
                ////Sender Id - From firebase project setting  
                //tRequest.Headers.Add(string.Format("Sender: id={0}", senderID));

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
            }

            catch (Exception ex)
            {
                string exs = ex.Message;
            }
            return Task.CompletedTask;
        }
    }
}
