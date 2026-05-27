using System;
using System.Collections.Generic;

namespace TeamMatching.Shared.DTOs
{
    public class SetAvailableTimesRequest
    {
        public List<DateTime> AvailableTimes { get; set; } = new List<DateTime>(); // 가능 시간
    }
}
