import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

export default {
  install (Vue) {
    const questionHub = new Vue()
    Vue.prototype.$questionHub = questionHub
    let startedPromise = null

    const connection = new HubConnectionBuilder()
      .withUrl('/question-hub')
      .configureLogging(LogLevel.Information)
      .build()

    connection.on('QuestionScoreChange', (questionId, score) => {
      questionHub.$emit('score-changed', { questionId, score })
    })
    connection.on('AnswerAdded', answer => {
      questionHub.$emit('answer-added', answer)
    })

    function start () {
      startedPromise = connection.start().catch(err => {
        console.error('Failed to connect with hub', err)
        return new Promise((resolve, reject) =>
          setTimeout(() => start().then(resolve).catch(reject), 5000))
      })
      return startedPromise
    }
    connection.onclose(() => start())

    start()

    questionHub.questionOpened = (questionId) => {
      return startedPromise.then(() => connection.invoke('JoinQuestionGroup', questionId))
        .catch(console.error)
    }
    questionHub.questionClosed = (questionId) => {
      return startedPromise.then(() => connection.invoke('LeaveQuestionGroup', questionId))
        .catch(console.error)
    }
  }
}
