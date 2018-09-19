using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ERecruitment.Models.Questions;

namespace ERecruitment.Data.Configurations
{
    public class ValidAnswerConfiguration : EntityTypeConfiguration<ValidAnswer>
    {
        public ValidAnswerConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.Answer).IsRequired().HasMaxLength(500);
            Property(a => a.TimeStamp).IsRowVersion();

            HasRequired(a => a.Question)
                .WithMany(a => a.Answers)
                .HasForeignKey(a => a.QuestionId);
        }
    }
}