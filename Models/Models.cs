using System;
using System.Collections.Generic;

namespace so_signalr.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public List<Answer> Answers { get;set; }
    }
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Body { get; set; }
    }
}