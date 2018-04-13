using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankWorm.Enums;

namespace BankWorm.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Memo { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TType { get; set; }
    }
}
