namespace ERecruitment.Patterns.Infrastructure
{
    public interface IEntity : IObjectState
    {
        int Id { get; set; }

        byte[] TimeStamp { get; set; }
    }
}