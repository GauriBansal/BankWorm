using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankWorm.Models;
using BankWorm.Enums;
using System.IO;

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

        public bool CreateDebitTransaction(Customer customer, Account account, decimal amount, string memo)
        {
            if (account.AcctType == AccountType.Savings)
            {
                var currentMonth = DateTime.Now.Month;
                var counter = 0;
                if (account.Transactions != null)
                {
                    foreach (var t in account.Transactions)
                    {
                        var transMonth = t.TransactionDate.Month;
                        if (transMonth == currentMonth)
                        {
                            counter += 1;
                        }
                    }
                    if (counter >= 3)
                    {
                        return false;
                    }
                }
            }

            _customers.Remove(customer);
            customer.Accounts.Remove(account);

            if (account.Transactions == null)
            {
                account.Transactions = new List<Transaction>() { };
            }

            if (account.Balance < amount)
            {
                if(account.AcctType == AccountType.Checkings)
                {
                    var transactionOC = CreateTransaction(15, "Overdraft fee", TransactionType.Debit);
                    account.Transactions.Add(transactionOC);
                }
                else
                {
                    var transactionOS = CreateTransaction(20, "Overdraft fee", TransactionType.Debit);
                    account.Transactions.Add(transactionOS);
                }
            }

            var transaction = CreateTransaction(amount, memo, TransactionType.Debit);
            account.Transactions.Add(transaction);
            customer.Accounts.Add(account);
            _customers.Add(customer);

            return true;
        }

        public void CreateCreditTransaction(Customer customer, Account account, decimal amount, string memo)
        {
             var transaction = CreateTransaction(amount, memo, TransactionType.Credit);
            _customers.Remove(customer);
            customer.Accounts.Remove(account);

            if (account.Transactions != null)
            {
                account.Transactions.Add(transaction);
            }
            else
            {
                account.Transactions = new List<Transaction>
                {
                    transaction
                };
            }

            customer.Accounts.Add(account);
            _customers.Add(customer);
        }

        public Transaction CreateTransaction(decimal amount, string memo, TransactionType type)
        {
            return (new Transaction
            {
                Amount = amount,
                Memo = memo,
                TransactionDate = DateTime.Now,
                TType = type
            });
        }

        public void CreateCheckingAccount(int custId, string name)
        {
            //var customer = _customers.FirstOrDefault(c => c.Id == custId);
            var customer = GetCustomerById(custId);

            if (customer.Accounts != null)
            {
                _customers.Remove(customer);
                customer.Accounts.Add(new Account
                {
                    Id = _random.Next(1, 10000000),
                    Name = name,
                    AcctType = AccountType.Checkings
                });
            }
            else
            {
                _customers.Remove(customer);
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
                        _customers.Remove(customer);
                        customer.Accounts.Add(new Account
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

        public bool RemoveAccount(Customer customer, Account account)
        {
            if (account.Balance == 0.0M)
            {
                _customers.Remove(customer);
                customer.Accounts.Remove(account);
                _customers.Add(customer);
                return true;
            }
            return false;
        }

        public void UpdateAccount(Customer customer, Account account, string newAcctName)
        {
            _customers.Remove(customer);
            customer.Accounts.Remove(account);

            account.Name = newAcctName;
            customer.Accounts.Add(account);
            _customers.Add(customer);
            
        }

        public void PopulateAccount(Account accountToPopulate)
        {
            if (accountToPopulate.Transactions == null)
            {
                accountToPopulate.Transactions = new List<Transaction>();
            }

            var fileName1 = "C:\\Source\\acadotnet\\BankWorm\\transactionfile-data.csv";

            var fileContents = File.ReadAllLines(fileName1).ToList().Skip(1);
            foreach (var line in fileContents)
            {
                var cells = line.Split(',');

                var transaction = new Transaction
                {
                    TransactionDate = DateTime.Parse(cells[0]),
                    Memo = cells[1],
                    TType = EnumExtensions.ConvertTransactionType(cells[2]),
                    Amount = Convert.ToDecimal(cells[3])
                };

                accountToPopulate.Transactions.Add(transaction);
            }

        }

    }
}
