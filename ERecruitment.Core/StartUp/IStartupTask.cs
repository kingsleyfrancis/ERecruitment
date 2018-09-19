namespace ERecruitment.Core.StartUp
{
    public interface IStartupTask
    {
        int Order { get; }
        void Execute();
    }
}