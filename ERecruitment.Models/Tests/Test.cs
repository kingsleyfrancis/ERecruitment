using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ERecruitment.Models.Questions;
using ERecruitment.Patterns.Infrastructure;

namespace ERecruitment.Models.Tests
{
    public class Test : IEntity
    {
        private ICollection<Question> _questions;
        private ICollection<TestDetail> _testDetails;

        public string TestTitle { get; set; }

        public int QuestionsCount { get; set; }

        public float TotalScore { get; set; }

        public bool IsReady { get; set; }

        public DateTime AddedOn { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public int Id { get; set; }

        public byte[] TimeStamp { get; set; }

        public virtual ICollection<Question> Questions
        {
            get { return _questions ?? (_questions = new List<Question>()); }
            set { _questions = value; }
        }

        public ICollection<TestDetail> TestDetails
        {
            get { return _testDetails ?? (_testDetails = new List<TestDetail>()); }
            set { _testDetails = value; }
        }
    }
}