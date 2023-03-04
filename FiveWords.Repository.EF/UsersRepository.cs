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
        return dbContext.Set<User>().Any(it => it.Id.Equals(id));
    }

    public User? Get(string id)
    {
        return dbContext.Set<User>().FirstOrDefault(it => it.Id.Equals(id));
    }

    public IReadOnlyDictionary<string, User> GetAll()
    {
        return dbContext.Set<User>().ToDictionary(it => it.Id);
    }

    public void AddAndImmediatelySave(User entity)
    {
        if (Exists(entity.Id))
            throw new ArgumentException($"Entity with key {entity.Id} already exists.");
        dbContext.Set<User>().Add(entity);
        dbContext.SaveChanges();
    }

    public void UpdateAndImmediatelySave(string id, User entity)
    {
        var foundUser = dbContext.Find<User>(id);
        if (foundUser is null)
            throw new ArgumentException($"Entity with key {id} not found.");
        if (Exists(entity.Id))
            throw new ArgumentException($"Entity with key {entity.Id} already exists.");
        dbContext.Entry(foundUser).State = EntityState.Deleted;
        dbContext.Entry(entity).State = EntityState.Added;
        dbContext.SaveChanges();

        /*  if (dbContext.Set<User>().Single(it => it.Id.Equals(id)) is User foundUser)
          {
              //var t = await db.Teachers.FindAsync(id);
              //teacher.Id = t.Id; // you might need to return to original value 
              //db.Entry(t).CurrentValues.SetValues(teacher);

              dbContext.Entry(foundUser).State = EntityState.Detached;
              //DbContext.Set<User>().Attach(entity); // not needed?
              dbContext.Entry(entity).State = EntityState.Modified;
              dbContext.SaveChanges();
          }*/
    }

    public void DeleteAndImmediatelySave(string id)
    {
        var entityToDelete = dbContext.Find<User>(id);
        if (entityToDelete is null)
            throw new ArgumentException($"Entity with key {id} not found.");
        dbContext.Entry(entityToDelete).State = EntityState.Deleted;
        dbContext.SaveChanges();
    }
}
