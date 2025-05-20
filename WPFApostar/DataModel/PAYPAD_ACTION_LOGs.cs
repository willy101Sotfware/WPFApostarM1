﻿namespace WPFApostar.DataModel
{
    public partial class PAYPAD_ACTION_LOG
    {
        public int PAYPAD_ACTION_LOG_ID { get; set; }
        public int ACTION_LOG_ID { get; set; }
        public Nullable<int> DEVICE_PAYPAD_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public System.DateTime DATE_EXECUTE { get; set; }
        public int QUANTITY_INTENTS { get; set; }
        public int INTENTS { get; set; }
        public int STATE { get; set; }
    }
}
