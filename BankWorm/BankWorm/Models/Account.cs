using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankWorm.Enums;

namespace BankWorm.Models
{
    public class Account
    {
        public int Id { get; set; }
        public AccountType AcctType { get; set; }
        public string Name { get; set; }
        public decimal Balance
        {
            get
            {
                var accountBalance = 0.0M;
                if (Transactions != null)
                {
                    foreach (var t in Transactions)
                    {
                        if (t.TType == TransactionType.Credit)
                        {
                            accountBalance += t.Amount;
                        }
                        else
                        {
                            accountBalance -= t.Amount;
                        }
                    }
                }
                return accountBalance;
            }
        }
        public List<Transaction> Transactions { get; set; }
    }
}
