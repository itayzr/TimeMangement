using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimeMangement.Indexes;
using Raven.Client;



namespace TimeMangement.Models
{
    public enum Activity
    {
        Work,
        Holiday,
        HalfHoliday
    }

    public class TimeInfo
    {
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public Activity Activity { get; set; }
        public string Description { get; set; }
        public string Project { get; set; }

        public double CalcProjectHours()
        {
//            DateTime startTime = DateTime.Parse(StartTime);
//            DateTime finishTime = DateTime.Parse(FinishTime);
//            TimeSpan ts = finishTime.Subtract(startTime);
//            return (ts.TotalHours);
            return 5;
        }
    }

    public class Day
        {
            public DateTime Date { get; set; }
            public List<TimeInfo> TimeInfos { get; set; }

            public Day(DateTime today)
            {
                Date = today.Date;
                TimeInfos = new List<TimeInfo> {new TimeInfo()};
            }
        }

        [CollectionDataContractAttribute]
        public class Month
        {
            public string Id { get; set; }
            public string UserName { get; set; }

            public Dictionary<DateTime, Day> Days { get; set; }

            public Month()
            {
                Days = new Dictionary<DateTime, Day>();
            }




        }
    }
