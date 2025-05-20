namespace WPFApostar.DataModel
{
    public partial class PAYPAD_CONSOLE_ERROR
    {
        public int PAYPAD_CONSOLE_ERROR_ID { get; set; }
        public int PAYPAD_ID { get; set; }
        public int ERROR_ID { get; set; }
        public int ERROR_LEVEL_ID { get; set; }
        public Nullable<int> DEVICE_PAYPAD_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public System.DateTime DATE { get; set; }
        public string OBSERVATION { get; set; }
        public int STATE { get; set; }
        public int REFERENCE { get; set; }
    }
}
