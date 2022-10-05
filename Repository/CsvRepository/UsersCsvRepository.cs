using CsvHelper.Configuration;
using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.CsvRepository;

internal class UsersCsvRepository : OneFileCsvRepository<User, string>, IUsersRepository
{
    protected internal UsersCsvRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath, fileName) { }

    protected override ClassMap<User> Mapping => new UsersMapping();

    private class UsersMapping : ClassMap<User>
    {
        public UsersMapping()
        {
            Map(w => w.Id).Name("Id").Index(0);
            Map(w => w.Guid).Name("Guid").Index(1);
        }
    }
}
