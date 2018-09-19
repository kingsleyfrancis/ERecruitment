using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ERecruitment.Models.Questions;

namespace ERecruitment.Data.Configurations
{
    public class QuestionConfiguration : EntityTypeConfiguration<Question>
    {
        public QuestionConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.QuestionBody).IsRequired().HasMaxLength(5000);
            Property(a => a.TimeStamp).IsRowVersion();
          

            HasRequired(a => a.Test).WithMany(a => a.Questions)
                .HasForeignKey(a => a.TestId);

            HasMany(a => a.Answers)
                .WithRequired(a => a.Question)
                .HasForeignKey(a => a.QuestionId);
        }
    }
}