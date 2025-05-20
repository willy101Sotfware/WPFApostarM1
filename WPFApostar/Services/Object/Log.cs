﻿using WPFApostar.Classes;

namespace WPFApostar.Services.Object
{
    public class LogDispenser
    {
        public string SendMessage { get; set; }

        public string ResponseMessage { get; set; }
    }

    public class RequestLog
    {
        public string Reference { get; set; }

        public string Description { get; set; }

        public int State { get; set; }

        public DateTime Date { get; set; }
    }

    public class RequestLogDevice
    {
        public int TransactionId { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Code { get; set; }

        public ELevelError Level { get; set; }
    }
}
