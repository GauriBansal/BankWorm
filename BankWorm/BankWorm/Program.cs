﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankWorm.Models;
using BankWorm.Services;
using BankWorm.Enums;

namespace BankWorm
{
    public class Program
    {
        private static readonly CustomerService _customerService = new CustomerService();

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to BankWorm.....Your protection from the early birds.");

            var isRunning = true;
            while (isRunning)
            {
                PrintMenu("Main Menu");
                Console.WriteLine("-- 'C' to manage customers");
                Console.WriteLine("-- 'R' for reports");
                Console.WriteLine("-- 'Q' to quit");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "Q":
                        isRunning = false;
                        break;

                    case "C":
                        ManageCustomerPrompts();
                        break;

                    case "R":
                        ManageReporting();
                        break;

                    default:
                        break;
                }

                Console.WriteLine("GoodBye");
                Console.ReadLine();
            }
        }

        public static void PrintMenu(string menuText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(menuText);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AccountCreation(int acctCustId)
        {
            Console.WriteLine("Enter the Name of the Account");
            var accountName = Console.ReadLine();
            Console.WriteLine("Enter the type of the Account (C for checking / S for savings)");
            var accountType = Console.ReadLine().ToUpper();

            switch (accountType)
            {
                case "C":
                    _customerService.CreateCheckingAccount(acctCustId, accountName);
                    Console.WriteLine("Congratulations! you have opened a Checking Account");
                    break;
                case "S":
                    var isAcctcreated = _customerService.CreateSavingsAccount(acctCustId, accountName);
                    if (!isAcctcreated)
                    {
                        Console.WriteLine("Cannot add the saving accounts for this customer");
                    }
                    else
                    {
                        Console.WriteLine("Congratulations! you have opened a Savings Account");
                    }
                    break;
                default:
                    Console.WriteLine("Unrecognized Account type");
                    break;
            }
        }

        public static void ManageCustomerPrompts()
        {
            var manageCustomers = true;
            while (manageCustomers)
            {
                PrintMenu("Customer Menu");
                Console.WriteLine("-- 'C' to Add a new customers");
                Console.WriteLine("-- 'I' to search for existing customers by Id");
                Console.WriteLine("-- 'A' to open account(s) for existing customers");
                Console.WriteLine("-- 'M' to manipulate account(s) for existing customers");
                Console.WriteLine("-- 'W' to withdraw from an account");
                Console.WriteLine("-- 'D' to deposit into an account");
                Console.WriteLine("-- 'X' to return to the Main Menu");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "C":
                        Console.WriteLine("Please Enter the name of the customer");
                        var name = Console.ReadLine();
                        Console.WriteLine("Please Enter the email of the customer");
                        var email = Console.ReadLine();

                        var customerId = _customerService.Create(name, email);

                        Console.WriteLine($"Congratulation! The customer {name} has been Added.");
                        Console.WriteLine("Do you wanna open an account? (Y / N)");
                        var isNewAccountCreate = (Console.ReadLine().ToUpper() == "Y")
                                            ? true
                                            : false;

                        while (isNewAccountCreate)
                        {
                            AccountCreation(customerId);

                            Console.WriteLine("Do you wanna open another account for same customer? (Y / N)");
                            isNewAccountCreate = (Console.ReadLine().ToUpper() == "Y")
                                            ? true
                                            : false;
                        }

                        break;

                    case "I":
                        Console.WriteLine("Enter the Id to be searched");
                        var custId = Convert.ToInt32(Console.ReadLine());

                        var customerSearched = _customerService.GetCustomerById(custId);

                        if (customerSearched != null)
                        {
                            Console.WriteLine($"The Customer {customerSearched.Id} belongs to {customerSearched.Name}.");
                            Console.WriteLine($"the email of this customer is {customerSearched.Email}");
                            Console.WriteLine($"This customer has {customerSearched.Accounts.Count()} Accounts");

                            foreach (var a in customerSearched.Accounts)
                            {
                                Console.WriteLine($"-- {a.Name} {a.AcctType} ({a.Balance})");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not found this customer");
                        }
                        break;

                    case "A":
                        Console.WriteLine("Enter the customer Id for which you want to open account.");
                        var acctCustId = Convert.ToInt32(Console.ReadLine());

                        var isAccountCreate = true;
                        while (isAccountCreate)
                        {
                            AccountCreation(acctCustId);

                            Console.WriteLine("Do you wanna open another account for same customer? (Y / N)");
                            isAccountCreate = (Console.ReadLine().ToUpper() == "Y")
                                            ? true
                                            : false;
                        }
                        break;

                    case "M":
                        Console.WriteLine("Enter the customer Id for which you want to manipulate the account.");
                        var accountCustId = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter the account Id you want to manipulate.");
                        var accountId = Convert.ToInt32(Console.ReadLine());

                        var customer = _customerService.GetCustomerById(accountCustId);
                        var account = customer.Accounts.Where(a => a.Id == accountId).FirstOrDefault();

                        Console.WriteLine("you have selected the following account");
                        Console.WriteLine($"-- {account.Id}, {account.Name}, {account.AcctType} ({account.Balance})");
                        Console.WriteLine("Please select the option below");
                        Console.WriteLine("-- 'D' to delete an account");
                        Console.WriteLine("-- 'U' update an account");
                        Console.WriteLine("-- 'X' Go back to customer menu");

                        var input1 = Console.ReadLine();

                        switch (input1)
                        {
                            case "D":
                                account = null;
                                break;

                            case "U":
                                Console.WriteLine("Enter the new Account Name");
                                account.Name = Console.ReadLine();
                                Console.WriteLine("The new account details are: ");
                                Console.WriteLine($"-- {account.Id}, {account.Name}, {account.AcctType} ({account.Balance})");
                                break;

                            case "X":
                                break;

                            default:
                                Console.WriteLine("Unrecognized option");
                                break;
                        }
                        break;

                    case "D":
                        Console.WriteLine("Enter the customer Id.");
                        var custId2 = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter the account Id.");
                        var acctId = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter the amount you want to deposit");
                        var amount1 = Convert.ToDecimal(Console.ReadLine());
                        Console.WriteLine("Enter a comment (optional)");
                        var memo1 = Console.ReadLine();

                        _customerService.CreateCreditTransaction(custId2, acctId, amount1, memo1);
                        Console.WriteLine("your account has been credited");
                        break;

                    case "W":
                        Console.WriteLine("Enter the customer Id.");
                        var CustId1 = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter the account Id.");
                        var acctId1 = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter the amount you want to withdrawl");
                        var amount = Convert.ToDecimal(Console.ReadLine());
                        Console.WriteLine("Enter a comment (optional)");
                        var memo = Console.ReadLine();

                        var isDebit = _customerService.CreateDebitTransaction(CustId1, acctId1, amount, memo);
                        if (isDebit)
                        {
                            Console.WriteLine("your account has been debited");
                        }
                        else
                        {
                            Console.WriteLine("you cannot withdraw from this account more than 3 times a month");
                        }
                        break;

                    case "X":
                        manageCustomers = false;
                        break;

                    default:
                        Console.WriteLine("Unrecognizd Option");
                        break;
                }
            }
        }
        
        public static void ManageReporting()
        {
            var manageReports = true;
            while (manageReports)
            {
                PrintMenu("Report Menu");
                Console.WriteLine("-- 'A' to show account(s) with current balance");
                Console.WriteLine("-- 'T' to show all Transactions by start/end date for an account");
                Console.WriteLine("-- 'X' to return to the Main Menu");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "A":
                        var customers = _customerService.GetAllCustomers();

                        Console.WriteLine($"-- AccountId --\t-- AccountName --\t-- AccountType --\t-- Balance--");
                        foreach (var c in customers)
                        {
                            foreach(var a in c.Accounts)
                            {
                                Console.WriteLine($"-- {a.Id} --\t-- {a.Name} --\t-- {a.AcctType} --\t-- {a.Balance}--");
                            }
                        }
                        break;

                    case "T":
                        Console.WriteLine("Enter the account Id for transactions");
                        var acctId = Convert.ToInt32(Console.ReadLine());

                        var customers1 = _customerService.GetAllCustomers();
                        Console.WriteLine("-- TransactionType --\t-- Amount --\t-- Memo --\t-- Date--");

                        foreach (var c in customers1)
                        {
                            var account = c.Accounts.Where(a => a.Id == acctId).FirstOrDefault();
                            
                            if(account != null)
                            {
                                foreach(var t in account.Transactions)
                                {
                                    Console.WriteLine($"-- {t.TType} --\t-- {t.Amount} --\t-- {t.Memo} --\t-- {t.TransactionDate}--");
                                }
                            }
                        }
                        break;

                    case "X":
                        manageReports = false;
                        break;

                    default:
                        Console.WriteLine("Unrecognized option");
                        break;
                }
            }
        }
    }
}


