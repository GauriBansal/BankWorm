using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankWorm.Models;
using BankWorm.Services;

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
                Console.WriteLine("-- 'X' to return to the Main Menu");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "C":
                        Console.WriteLine("Please Enter the name of the customer");
                        var name = Console.ReadLine();
                        Console.WriteLine("Please Enter the email of the customer");
                        var email = Console.ReadLine();

                        var customer = _customerService.Create(name, email);

                        Console.WriteLine($"Congratulation! The customer {name} has been Added.");
                        Console.WriteLine("Do you wanna open an account? (Y / N)");
                        var isNewAccountCreate = (Console.ReadLine().ToUpper() == "Y")
                                            ? true
                                            : false;

                        while (isNewAccountCreate)
                        {
                            AccountCreation(customer.Id);

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

                        if (customerSearched.Id != 0)
                        {
                            Console.WriteLine($"The Customer {customerSearched.Id} belongs to {customerSearched.Name}.");
                            Console.WriteLine($"the email of this customer is {customerSearched.Email}");
                            Console.WriteLine($"This customer has {customerSearched.Accounts.Count()} Accounts");

                            foreach (var a in customerSearched.Accounts)
                            {
                                Console.WriteLine($"-- {a.Name} {a.AcctType} ({a.Balance})");
                            }

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

                    case "X":
                        manageCustomers = false;
                        break;

                    default:
                        Console.WriteLine("Unrecognizd Option");
                        break;
                }
            }
        }
    }
}


