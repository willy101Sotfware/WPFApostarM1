using System;

namespace WPFApostar.Domain.UIServices.ObjectIntegration
{
    public class ResponseApi
    {
        public int codeError { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }
}
