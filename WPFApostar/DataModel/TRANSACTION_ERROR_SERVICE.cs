﻿namespace WPFApostar.DataModel
{
    public class TRANSACTION_ERROR_SERVICE
    {
        public int TRANSACTION_ERROR_SERVICE_ID { get; set; }
        public string JSON { get; set; }
        public int TRANSACTION_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public int STATE { get; set; }
        public Nullable<int> NOTIFICATION_INTENT { get; set; }
    }
}
