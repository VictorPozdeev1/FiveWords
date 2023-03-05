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
    private DbSet<TEntity> Entities => dbContext.Set<TEntity>();

    public async Task<bool> ExistsAsync(TId id)
    {
        return await Entities.AnyAsync(it => it.Id.Equals(id)); // or this?: return await Entities.FindAsync(id) is not null;
    }

    public bool Exists(TId id) => ExistsAsync(id).GetAwaiter().GetResult();

    public async Task<TEntity?> GetAsync(TId id)
    {
        return await Entities.FindAsync(id);
    }

    public TEntity? Get(TId id) => GetAsync(id).GetAwaiter().GetResult();

    public async Task<IReadOnlyDictionary<TId, TEntity>> GetAllAsync()
    {
        return await Entities.ToDictionaryAsync(it => it.Id);
    }

    public IReadOnlyDictionary<TId, TEntity> GetAll() => GetAllAsync().GetAwaiter().GetResult();

    public async Task AddAndImmediatelySaveAsync(TEntity entity)
    {
        if (await ExistsAsync(entity.Id))
            throw new ArgumentException($"Entity with key {entity.Id} already exists.");
        Entities.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public void AddAndImmediatelySave(TEntity entity) => AddAndImmediatelySaveAsync(entity).GetAwaiter().GetResult();

    public async Task UpdateAndImmediatelySaveAsync(TId id, TEntity entity)
    {
        var foundEntity = await dbContext.FindAsync<User>(id);
        if (foundEntity is null)
            throw new ArgumentException($"Entity with key {id} not found.");
        if (await ExistsAsync(entity.Id))
            throw new ArgumentException($"Entity with key {entity.Id} already exists.");
        dbContext.Entry(foundEntity).State = EntityState.Deleted;
        dbContext.Entry(entity).State = EntityState.Added;
        await dbContext.SaveChangesAsync();
    }

    public void UpdateAndImmediatelySave(TId id, TEntity entity) => UpdateAndImmediatelySaveAsync(id, entity).GetAwaiter().GetResult();

    public async Task DeleteAndImmediatelySaveAsync(TId id)
    {
        var entityToDelete = await dbContext.FindAsync<TEntity>(id);
        if (entityToDelete is null)
            throw new ArgumentException($"Entity with key {id} not found.");
        dbContext.Entry(entityToDelete).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
    }

    public void DeleteAndImmediatelySave(TId id) => DeleteAndImmediatelySaveAsync(id).GetAwaiter().GetResult();
}
