using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ERecruitment.Models.Accounts;

namespace ERecruitment.Data.Configurations
{
    public class AccountConfiguration : EntityTypeConfiguration<Account>
    {
        public AccountConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.Firstname).IsRequired().HasMaxLength(200);
            Property(a => a.Lastname).IsRequired().HasMaxLength(200);
            Property(a => a.State).IsRequired().HasMaxLength(200);
            Property(a => a.TimeStamp).IsRowVersion();

            HasMany(a => a.Tests)
                .WithRequired(a => a.Account)
                .HasForeignKey(a => a.AccountId);
        }
    }
}