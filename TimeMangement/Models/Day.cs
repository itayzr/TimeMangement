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
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public Activity Activity { get; set; }
        public List<string> Projects { get; set; }
        public string Description { get; set; }

        public TimeInfo()
        {
            Projects = new List<string>();
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
            Days=new Dictionary<DateTime, Day>();
        }
   } 
}