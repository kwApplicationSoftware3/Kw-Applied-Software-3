using System;

namespace TeamMatching.Shared.DTOs
{
    public class SetAvailableTimesResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
