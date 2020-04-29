using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using so_signalr.Models;

namespace so_signalr.Hubs
{
    public interface IQuestionHub
    {
        Task QuestionScoreChange(Guid questionId, int score);
        Task AnswerAdded(Answer answer);
    }
    public class QuestionHub : Hub <IQuestionHub>
    {        
        private readonly ILogger logger;
        public QuestionHub(ILogger<QuestionHub> logger)
        {
            this.logger = logger;
        }

        public async Task JoinQuestionGroup(Guid questionId)
        {
            this.logger.LogInformation($"{Context.ConnectionId} has joined the group");
            await Groups.AddToGroupAsync(Context.ConnectionId, questionId.ToString());
        }

        public async Task LeaveQuestionGroup(Guid questionId)
        {
            this.logger.LogInformation($"{Context.ConnectionId} has left the group");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, questionId.ToString());
        }
    }
}