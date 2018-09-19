using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Tests;
using ERecruitment.Patterns.Infrastructure;

namespace ERecruitment.Models.Questions
{
    public class Question : IEntity
    {
        private ICollection<ValidAnswer> _answers;

        public string QuestionBody { get; set; }

        public DateTime AddedOn { get; set; }

        public QuestionType QuestionType { get; set; }

        public float Score { get; set; }

        public virtual ICollection<ValidAnswer> Answers
        {
            get { return _answers ?? (_answers = new List<ValidAnswer>()); }
            set { _answers = value; }
        }

        public virtual Test Test { get; set; }

        public int TestId { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public int Id { get; set; }

        public byte[] TimeStamp { get; set; }
    }
}