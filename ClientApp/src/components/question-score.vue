<template>
    <h3 class="text-center scoring">
        <button class="btn btn-link btn-lg p-0 d-block mx-auto" @click.stop="onUpVote">
            <i class="fas fa-sort-up"/>
        </button>
        <span class="d-block mx-auto">{{question.score}}</span>
        <button class="btn btn-link btn-lg p-0 d-block mx-auto" @click.stop="onDownVote">
            <i class="fas fa-sort-down"/>
        </button>
    </h3>
</template>

<script>
export default {
  props: {
    question: {
      type: Object,
      required: true
    }
  },
  created () {
    this.$questionHub.$on('score-changed', this.onScoreChanged)
  },
  methods: {
    onUpVote () {
      this.$http.patch(`/api/question/${this.question.id}/upvote`).then(res => {
        Object.assign(this.question, res.data)
      })
    },
    onDownVote () {
      this.$http.patch(`api/question/${this.question.id}/downvote`).then(res => {
        Object.assign(this.question, res.data)
      })
    },
    onScoreChanged ({ questionId, score }) {
      if (this.question.Id !== questionId) { return Object.assign(this.question, { score }) }
    }
  },
  beforeDestroy () {
    // make sure to cleanup signalR event handlers when removing the component
    this.$questionHub.$off('score-changed', this.onScoreChanged)
  }
}
</script>

<style scoped>
.scoring .btn-link{
    line-height: 1;
}
</style>
