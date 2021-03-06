﻿using Lab2Expense.Models;
using Lab2Expense.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestUsersService
{
    class ExpensesServiceTest
    {
        [Test]
        public void GetAllShouldReturnCorrectNumberOfPages()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAllShouldReturnCorrectNumberOfPages))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {

                // var commentService = new CommentService(context);
                var expenseService = new ExpenseService(context);
                var addedFlower = expenseService.Create(new Lab2Expense.ViewModels.ExpensePostModel
                {
                    Description = "jshdkhsakjd",
                    Sum = 1.23,
                    Location = "jsfkdsf",
                    Date = new DateTime(),
                    Currency = "euro",
                    ExpenseType = "food",

                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "asd",
                            Owner = null
                        }
                    },

                }, null);

                var allComments = expenseService.GetAll(1);
                Assert.NotNull(allComments);
            }
        }

        [Test]
        public void CreateAndGetByIdTest()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(CreateAndGetByIdTest))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {


                var expenseService = new ExpenseService(context);
                var addedExpense = expenseService.Create(new Lab2Expense.ViewModels.ExpensePostModel
                {
                    Description = "jshdkhsakjd",
                    Sum = 1.23,
                    Location = "jsfkdsf",
                    Date = new DateTime(),
                    Currency = "euro",
                    ExpenseType = "food",

                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "asd",
                            Owner = null
                        }
                    },

                }, null);

                var expenseCreated = expenseService.GetById(addedExpense.Id);
                Assert.NotNull(expenseCreated);
                Assert.AreEqual(addedExpense, expenseCreated);
            }
        }

        [Test]
        public void DeleteTest()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(DeleteTest))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {


                var expenseService = new ExpenseService(context);
                var addedExpense = expenseService.Create(new Lab2Expense.ViewModels.ExpensePostModel
                {
                    Description = "jshdkhsakjd",
                    Sum = 1.23,
                    Location = "jsfkdsf",
                    Date = new DateTime(),
                    Currency = "euro",
                    ExpenseType = "food",

                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "asd",
                            Owner = null
                        }
                    },

                }, null);

                var expenseDeleted = expenseService.Delete(addedExpense.Id);
                Assert.IsNotNull(expenseDeleted);
            }
        }

        [Test]
        public void UpdateTest1()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(UpdateTest1))
              .Options;

            using (var context = new ExpensesDbContext(options))
            {


                var expenseService = new ExpenseService(context);

                var addedExpense = expenseService.Create(new Lab2Expense.ViewModels.ExpensePostModel
                {
                    Description = "lksajdaksld",
                    Sum = 1.23,
                    Location = "sjdasjldls",
                    Date = new DateTime(),
                    Currency = "euro",
                    ExpenseType = "food",

                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "asd",
                            Owner = null
                        }
                    },

                }, null);
                var addedExpenseForUpdate = new Expense
                {   
                    Description = "update",
                    Sum = 1.23,
                    Location = "update",
                    Date = new DateTime(),
                    Currency = "euro",
                    ExpenseType = ExpenseType.food,

                    Comments = new List<Comment>()
                    {
                        new Comment
                        {
                            Important = true,
                            Text = "asd",
                            Owner = null
                        }
                    },
                    Owner = null
                };




                var updateResult = expenseService.Upsert(addedExpense.Id, addedExpenseForUpdate);
                

            }
        }


    }
}
