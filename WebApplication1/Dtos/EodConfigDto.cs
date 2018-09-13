using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class EodConfigDto
    {
        public int StartTimeHour { get; set; }
        public int StartTimeMins { get; set; }
        public string StartTimePeriod { get; set; }
        public int EndTimeHour { get; set; }
        public int EndTimeMins { get; set; }
        public string EndTimePeriod { get; set; }
        public int FixedTimeHour { get; set; }
        public int FixedTimeMins { get; set; }
        public string FixedTimePeriod { get; set; }
    }
}