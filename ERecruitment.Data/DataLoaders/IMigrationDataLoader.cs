namespace ERecruitment.Data.DataLoaders
{
    public interface IMigrationDataLoader
    {
        int Order { get; }

        void Load(DataContext context);
    }
}
