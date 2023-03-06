using CsvHelper.Configuration;
using FiveWords.CommonModels;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.Csv;

public class UsersCsvRepository : OneFileCsvRepository<User, string>, IUsersRepository
{
    public UsersCsvRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath, fileName, new UsersMapping()) { }

    private class UsersMapping : ClassMap<User>
    {
        public UsersMapping()
        {
            Map(w => w.Id).Name("Id").Index(0);
            Map(w => w.Guid).Name("Guid").Index(1);
        }
    }
}
