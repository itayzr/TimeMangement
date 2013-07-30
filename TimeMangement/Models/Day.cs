using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeMangement.Models
{
    public enum Activity
    {
        None,
        Holiday,
        HalfHoliday
    }

    public class TimeInfo
    {
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        public double HoursRange()
        {
            //check for mins
            return (FinishTime - StartTime).TotalHours;
        }
    }

    public class Day
    {
        public DateTime Date { get; set; }
        public List<Activity> Activities { get; set; } 
        public List<TimeInfo> TimeInfos { get; set; } 
        public List<string> Projects { get; set; }
        public string Description { get; set; }

        public Day(DateTime today)
        {
            Date = today.Date;
            Activities = new List<Activity> {new Activity()};
            TimeInfos = new List<TimeInfo> {new TimeInfo()};
            Projects= new List<string>();
        }
    }

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