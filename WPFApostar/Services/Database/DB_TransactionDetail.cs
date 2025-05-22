using System;

namespace WPFApostar.Services.Database
{
    public class DB_TransactionDetail
    {
        public long Id { get; set; }
        public long IdTransaction { get; set; }
        public int CurrencyDenomination { get; set; }
        public int IdTypeOperation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 