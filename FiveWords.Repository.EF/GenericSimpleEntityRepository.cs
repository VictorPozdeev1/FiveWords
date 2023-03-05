using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FiveWords.Repository.EF;

public class GenericSimpleEntityRepository<TEntity, TId> : ISimpleEntityRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>
{
    public GenericSimpleEntityRepository(CommonDbContext dbContext)
        => this.dbContext = dbContext;
    private readonly CommonDbContext dbContext;

    public bool Exists(TId id)
    {
        return dbContext.Set<TEntity>().Any(it => it.Id.Equals(id));
    }

    public TEntity? Get(TId id)
    {
        return dbContext.Set<TEntity>().FirstOrDefault(it => it.Id.Equals(id));
    }

    public IReadOnlyDictionary<TId, TEntity> GetAll()
    {
        return dbContext.Set<TEntity>().ToDictionary(it => it.Id);
    }

    public void AddAndImmediatelySave(TEntity entity)
    {
        if (Exists(entity.Id))
            throw new ArgumentException($"Entity with key {entity.Id} already exists.");
        dbContext.Set<TEntity>().Add(entity);
        dbContext.SaveChanges();
    }

    public void UpdateAndImmediatelySave(TId id, TEntity entity)
    {
        var foundEntity = dbContext.Find<User>(id);
        if (foundEntity is null)
            throw new ArgumentException($"Entity with key {id} not found.");
        if (Exists(entity.Id))
            throw new ArgumentException($"Entity with key {entity.Id} already exists.");
        dbContext.Entry(foundEntity).State = EntityState.Deleted;
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

    public void DeleteAndImmediatelySave(TId id)
    {
        var entityToDelete = dbContext.Find<TEntity>(id);
        if (entityToDelete is null)
            throw new ArgumentException($"Entity with key {id} not found.");
        dbContext.Entry(entityToDelete).State = EntityState.Deleted;
        dbContext.SaveChanges();
    }
}

