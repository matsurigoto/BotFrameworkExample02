using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using RestSharp;

namespace BotApplication.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if(activity.Text.IndexOf("weather") >=0 || activity.Text.IndexOf("天氣") >= 0)
            {
                var client = new RestClient("http://opendata.cwb.gov.tw");
                var request = new RestRequest("api/v1/rest/datastore/F-C0032-001?Authorization=CWB-xxxx&locationName=臺北市&elementName=Wx,PoP", Method.GET);

                var response = await client.ExecuteTaskAsync<RootObject>(request);
                var weather = response.Data.records.location[0].weatherElement[0].time[1].parameter.parameterName;
                var rainPercentage = response.Data.records.location[0].weatherElement[1].time[1].parameter.parameterName;
                await context.PostAsync($"天氣：{weather}，降雨機率{rainPercentage}");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
    public class Field
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Result
    {
        public string resource_id { get; set; }
        public List<Field> fields { get; set; }
    }

    public class Parameter
    {
        public string parameterName { get; set; }
        public string parameterValue { get; set; }
        public string parameterUnit { get; set; }
    }

    public class Time
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public Parameter parameter { get; set; }
    }

    public class WeatherElement
    {
        public string elementName { get; set; }
        public List<Time> time { get; set; }
    }

    public class Location
    {
        public string locationName { get; set; }
        public List<WeatherElement> weatherElement { get; set; }
    }

    public class Records
    {
        public string datasetDescription { get; set; }
        public List<Location> location { get; set; }
    }

    public class RootObject
    {
        public string success { get; set; }
        public Result result { get; set; }
        public Records records { get; set; }
    }

}