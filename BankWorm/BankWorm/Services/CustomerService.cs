using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankWorm.Models;
using BankWorm.Enums;

namespace BankWorm.Services
{
    public class CustomerService
    {
        private readonly List<Customer> _customers;
        private readonly Random _random = new Random();

        public CustomerService()
        {
            _customers = new List<Customer>
            {
                new Customer
                {
                    Id = 101,
                    Name = "Gauri Bansal",
                    Email = "reach2gauri@gmail.com",
                    Accounts = new List<Account>
                    {
                        new Account
                        {
                            Id = 1001,
                            AcctType = AccountType.Checkings,
                            Name = "Bansal Treasures 1",
                            Transactions = new List<Transaction>
                            {
                                new Transaction
                                {
                                    Amount = 123.78M,
                                    Memo = "Intial Transaction on 1001",
                                    TType = TransactionType.Credit,
                                    TransactionDate = DateTime.Now
                                }
                            }
                        },
                        new Account
                        {
                            Id = 1002,
                            AcctType = AccountType.Savings,
                            Name = "Bansal Treasures 2",
                            Transactions = new List<Transaction>
                            {
                                new Transaction
                                {
                                    Amount = 100.78M,
                                    Memo = "Intial Transaction on 1002",
                                    TType = TransactionType.Debit,
                                    TransactionDate = DateTime.Now
                                },
                                new Transaction
                                {
                                    Amount = 187654.78M,
                                    Memo = "2nd Transaction on 1002",
                                    TType = TransactionType.Credit,
                                    TransactionDate = DateTime.Now
                                }
                            }
                         }
                    }
                }
            };
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _customers.ToList();
        }

        public Customer GetCustomerById(int customerId)
        {
            return _customers.FirstOrDefault(c => c.Id == customerId);
        }

        public int Create(string name, string email)
        {
            var customer = new Customer
            {
                Id = _customers.Max(c => c.Id) + 1,
                Name = name,
                Email = email
            };
            
            _customers.Add(customer);
            return customer.Id;
        }

        public bool CreateDebitTransaction(int custId, int accountId, decimal amount, string memo)
        {
            var customer = GetCustomerById(custId);
            var account = customer.Accounts.Where(a => a.Id == accountId).FirstOrDefault();

            if (account.AcctType == AccountType.Savings)
            {
                var currentMonth = DateTime.Now.Month;
                var counter = 0;
                foreach (var t in account.Transactions)
                {
                    var transMonth = t.TransactionDate.Month;
                    if(transMonth == currentMonth)
                    {
                        counter += 1;
                    }
                }
                if(counter >=3)
                {
                    return false;
                }
            }

            if (account.Balance < amount)
            {
                if(account.AcctType == AccountType.Checkings)
                {
                    CreateTransaction(15, "Overdraft fee", TransactionType.Debit);
                }
                else
                {
                    CreateTransaction(20, "Overdraft fee", TransactionType.Debit);
                }
            }

            CreateTransaction(amount, memo, TransactionType.Debit);
            _customers.Add(customer);
            return true;
        }

        public void CreateTransaction(decimal amount, string memo, TransactionType type)
        {
            var transaction = new Transaction
            {
                Amount = amount,
                Memo = memo,
                TransactionDate = DateTime.Now,
                TType = type
            };
        }

        public void CreateCreditTransaction(int custId, int acctId, decimal amount, string memo)
        {
            var customer = GetCustomerById(custId);
            var account = customer.Accounts.Where(a => a.Id == acctId).FirstOrDefault();

            CreateTransaction(amount, memo, TransactionType.Credit);
            _customers.Add(customer);
        }

        public void CreateCheckingAccount(int custId, string name)
        {
            //var customer = _customers.FirstOrDefault(c => c.Id == custId);
            var customer = GetCustomerById(custId);

            if (customer.Accounts != null)
            {
                customer.Accounts.ToList().Add(new Account
                {
                    Id = _random.Next(1, 10000000),
                    Name = name,
                    AcctType = AccountType.Checkings
                });
            }
            else
            {
                customer.Accounts = new List<Account>
                {
                    new Account
                    {
                    Id = _random.Next(1, 10000000),
                    Name = name,
                    AcctType = AccountType.Checkings
                    }
                };
            }
            _customers.Add(customer);
            
        }

        public bool CreateSavingsAccount(int custId, string name)
        {
            var customer = GetCustomerById(custId);

            if(customer.Accounts != null)
            {
                if(customer.Accounts.Any(a => a.AcctType == AccountType.Checkings))
                {
                    var savingAccountCount = customer.Accounts.Count(a => a.AcctType == AccountType.Savings);
                    if (savingAccountCount < 2)
                    {
                        customer.Accounts.ToList().Add(new Account
                        {
                            Id = _random.Next(1, 10000000),
                            Name = name,
                            AcctType = AccountType.Savings
                        });
                        _customers.Add(customer);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
