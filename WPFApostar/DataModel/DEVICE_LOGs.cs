namespace WPFApostar.DataModel
{
    public partial class DEVICE_LOG
    {
        public int DEVICE_LOG_ID { get; set; }
        public Nullable<int> TRANSACTION_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public System.DateTime DATETIME { get; set; }
        public string CODE { get; set; }
    }
}
