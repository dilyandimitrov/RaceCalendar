using System;
using System.Collections.Generic;

namespace RaceCalendar.Domain.Models.Authentication
{
    public class ErrorResult
    {
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class AuthResult : ErrorResult
    {
        public string Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool Result { get; set; }
    }
}