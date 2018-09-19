using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ERecruitment.Models.Questions;
using ERecruitment.Models.Tests;

namespace ERecruitment.Data.Configurations
{
    public class TestConfiguration : EntityTypeConfiguration<Test>
    {
        public TestConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.TestTitle).IsRequired().HasMaxLength(500);
            Property(a => a.TimeStamp).IsRowVersion();

            HasMany(a => a.Questions)
                .WithRequired(a => a.Test)
                .HasForeignKey(a => a.TestId);

            HasMany(a => a.TestDetails)
                .WithRequired(a => a.Test)
                .HasForeignKey(a => a.TestId);
        }
    }
}