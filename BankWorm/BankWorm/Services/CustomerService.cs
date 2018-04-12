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
        private readonly IEnumerable<Customer> _customers;
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
                            Balance = 156789.09M
                        },
                        new Account
                        {
                            Id = 1002,
                            AcctType = AccountType.Savings,
                            Name = "Bansal Treasures 2",
                            Balance = 100009.09M
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

        public Customer Create(string name, string email)
        {
            var customer = new Customer
            {
                Id = _customers.Max(c => c.Id) + 1,
                Name = name,
                Email = email
            };
            
            _customers.ToList().Add(customer);
            return customer;
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
            _customers.PersistChanges(customer);
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
                        _customers.PersistChanges(customer);
                        return true;
                    }
                }
            }
            return false;
        }

    }

    public static class FakeDatabase
    {
        public static void PersistChanges(this IEnumerable<Customer> customers, Customer newOrUpdatedCustomer)
        {
            var existingCustomer = customers.FirstOrDefault(c => c.Id == newOrUpdatedCustomer.Id);
            if (existingCustomer == null)
            {
                customers.ToList().Add(newOrUpdatedCustomer);
            }

            if (existingCustomer.Accounts != null && !existingCustomer.Accounts.Any() && newOrUpdatedCustomer.Accounts.Any())
            {
                existingCustomer.Accounts.ToList().AddRange(newOrUpdatedCustomer.Accounts);
            }

            foreach (var a in newOrUpdatedCustomer.Accounts)
            {
                var existingAccount = existingCustomer.Accounts?.FirstOrDefault(c => c.Id == a.Id);
                if (existingAccount == null)
                {
                    existingCustomer.Accounts.ToList().Add(a);
                }
            }
        }
    }
}
