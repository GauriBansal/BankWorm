﻿using System;
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
        public decimal Balance { get; set; }

    }
}
