using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        /// <summary>
        /// Format: HH:mm
        /// </summary>
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
    }

    public class Day
    {
        public DateTime Date { get; set; }
        public Activity Activity { get; set; } 
        public List<TimeInfo> TimeInfos { get; set; } 
        public List<string> Projects { get; set; }
        public string Description { get; set; }

        public Day(DateTime today)
        {
            Date = today.Date;
            TimeInfos = new List<TimeInfo> {new TimeInfo()};
            Projects= new List<string>();
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
            Days=new Dictionary<DateTime, Day>();
        }
   } 
}