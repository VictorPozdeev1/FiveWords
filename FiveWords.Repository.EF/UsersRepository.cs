using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FiveWords.Repository.EF;

public class UsersRepository : ISimpleEntityRepository<User, string>
{
    public UsersRepository(CommonDbContext dbContext)
        => this.dbContext = dbContext;
    private readonly CommonDbContext dbContext;

    public bool Exists(string id)
    {
        // Отправляет запрос в бд или только локально ищет?
        return dbContext.Set<User>().Any(it => it.Id.Equals(id));
    }

    public User? Get(string id)
    {
        /* ??????
         * More importantly is the difference in behaviour when using EntityFramework, since .FirstOrDefault() executes a query and adds the result to the change tracker (by default; use .AsTracking()/.AsNoTracking() and the settings of the DBContext for override). The execution of .FirstOrDefault() might fail with an exception, when the object is already tracked by the change tracker.
         * .Find() will return the object that is already tracked by the change tracker in memory (ie. after an update or add). So this will not cause a problem when the object is already tracked.
         */

        return dbContext.Set<User>().FirstOrDefault(it => it.Id.Equals(id));
    }

    public IReadOnlyDictionary<string, User> GetAll()
    {
        return dbContext.Set<User>().ToDictionary(it => it.Id);
    }

    public void AddAndImmediatelySave(User entity)
    {
        dbContext.Set<User>().Add(entity);
        dbContext.SaveChanges();
    }

    public void UpdateAndImmediatelySave(string id, User entity)
    {
        //dbContext.Set<User>().Remove(id);
        if (dbContext.Set<User>().Single(it => it.Id.Equals(id)) is User foundUser)
        {
            dbContext.Entry(foundUser).State = EntityState.Detached;
            //DbContext.Set<User>().Attach(entity); // not needed?
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
    }

    public void DeleteAndImmediatelySave(string id)
    {
        // Find(), then Remove()?
        dbContext.Entry(new User() { Id = id }).State = EntityState.Deleted;
        dbContext.SaveChanges();
    }
}
