﻿using Lab2Expense.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab2Expense.ViewModels
{
    public class CommentGetModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Important { get; set; }
        public int? ExpenseId { get; set; }




        public static CommentGetModel FromComment(Comment c)
        {
            return new CommentGetModel
            {
                Id = c.Id,
                ExpenseId = c.Expense?.Id,
                Important = c.Important,
                Text = c.Text
            };
        }

        //public static CommentGetModel FromComment(Comment comment)
        //{
        //    return new CommentGetModel
        //    {
        //        Id = comment.Id,
        //        Text = comment.Text,
        //        Important = comment.Important,
        //        ExpenseId = comment.ExpenseId,

        //    };
        //}
    }
}