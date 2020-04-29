using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using so_signalr.Hubs;
using so_signalr.Models;
//using so-signalr.Models;

namespace VueSPATemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IHubContext<QuestionHub, IQuestionHub> hubContext;
        private static ConcurrentBag<Question> questions = new ConcurrentBag<Question>{
            new Question {
                Id = Guid.Parse("b00c58c0-df00-49ac-ae85-0a135f75e01b"),
                Title = "Welcome",
                Body = "Welcome to the _mini Stack Overflow_ rip-off!\nThis will help showcasing **SignalR** and its integration with **Vue**",
                Answers = new List<Answer>{ new Answer{ Body = "Sample answer"}}
            }
        };

        public QuestionController(IHubContext<QuestionHub, IQuestionHub> questionHub)
        {
            this.hubContext = questionHub;
        }

        // GET api/question
        [HttpGet()]
        public IEnumerable GetQuestions()
        {
            return questions.Select(q => new {
                Id = q.Id,
                Title = q.Title,
                Body = q.Body,
                Score = q.Score,
                AnswerCount = q.Answers.Count
            });
        }

        // GET api/question/5
        [HttpGet("{id}")]
        public ActionResult GetQuestion(Guid id)
        {
            var question = questions.SingleOrDefault( t => t.Id == id);
            if(question == null) return NotFound();

            return new JsonResult(question);
        }

        // POST api/question
        [HttpPost()]
        public Question AddQuestion ([FromBody]Question question)
        {
            question.Id = Guid.NewGuid();
            question.Answers = new List<Answer>();
            questions.Add(question);
            return question;
        }

        // POST api/question/5
        [HttpPost("{id}/answer")]
        public async Task<IActionResult> AddAnswerAsync(Guid id, [FromBody]Answer answer)
        {
            var question = questions.SingleOrDefault(m => m.Id == id);
            if(question == null) return NotFound();

            answer.Id = Guid.NewGuid();
            answer.QuestionId = id;
            question.Answers.Add(answer);

            //notify clients in that group
            await this.hubContext.Clients.Group(id.ToString()).AnswerAdded(answer);
            
            return new JsonResult(answer);
        }

        // PATCH api/question/5
        [HttpPatch("{id}/upvote")]
        public async Task<ActionResult> UpVoteQuestionAsync(Guid id)
        {
            var question = questions.SingleOrDefault( m=> m.Id == id);
            if(question == null) return NotFound();

            question.Score++;

            //notify every client
            await this.hubContext.Clients.All.QuestionScoreChange(question.Id, question.Score);

            return new JsonResult(question);
        }

        [HttpPatch("{id}/downvote")]
        public async Task<ActionResult> DownVoteQuestionAsync(Guid id)
        {
            var question = questions.SingleOrDefault( m=> m.Id == id);
            if(question == null) return NotFound();

            question.Score--;

            //notify every client
            await this.hubContext.Clients.All.QuestionScoreChange(question.Id, question.Score);

            return new JsonResult(question);
        }
    }
}