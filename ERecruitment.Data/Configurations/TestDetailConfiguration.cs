using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ERecruitment.Models.Questions;
using ERecruitment.Models.Tests;

namespace ERecruitment.Data.Configurations
{
    public class TestDetailConfiguration : EntityTypeConfiguration<TestDetail>
    {
        public TestDetailConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.TimeStamp).IsRowVersion();

            HasRequired(a => a.Account)
                .WithMany(a => a.Tests)
                .HasForeignKey(a => a.AccountId);

            HasRequired(a => a.Test)
                .WithMany(a => a.TestDetails)
                .HasForeignKey(a => a.TestId);

        }
    }
}