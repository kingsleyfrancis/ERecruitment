using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using ERecruitment.Models.Logs;

namespace ERecruitment.Data.Configurations
{
    public class LogConfiguration : EntityTypeConfiguration<Log>
    {
        public LogConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.FullMessage).HasMaxLength(25000).IsRequired();
            Property(a => a.ShortMessage).HasMaxLength(5000);
            Property(a => a.StackTrace).HasMaxLength(25000);
            Property(a => a.ReferrerUrl).HasMaxLength(500);
            Property(a => a.PageUrl).HasMaxLength(500);
            Property(a => a.Source).HasMaxLength(25000);
            Property(a => a.IpAddress).HasMaxLength(100);
            Property(a => a.TimeStamp).IsRowVersion();
        }
    }
}