namespace WPFApostar.DataModel
{
    public partial class TRANSACTION_DESCRIPTION
    {
        public int TRANSACTION_DESCRIPTION_ID { get; set; }
        public int TRANSACTION_ID { get; set; }
        public int TRANSACTION_PRODUCT_ID { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public string EXTRA_DATA { get; set; }
        public Nullable<bool> STATE { get; set; }
    }
}
