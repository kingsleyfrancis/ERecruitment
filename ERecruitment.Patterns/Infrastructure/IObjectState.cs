using System.ComponentModel.DataAnnotations.Schema;

namespace ERecruitment.Patterns.Infrastructure
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}