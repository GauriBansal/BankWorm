using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankWorm.Enums
{
    public static class EnumExtensions
    {
        public static TransactionType ConvertTransactionType(string transType)
        {
            if(transType == "Credit")
            {
                return TransactionType.Credit;
            }
            return TransactionType.Debit;
        }
    }
}
