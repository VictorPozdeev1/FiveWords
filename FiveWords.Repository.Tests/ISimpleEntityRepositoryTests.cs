using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using FiveWords.Repository.Tests.Helpers;
using FiveWords.Repository.Tests.Helpers.Csv;

namespace FiveWords.Repository.Tests;

[TestFixture(typeof(UsersRepositoryHelper), typeof(User), typeof(string))]
internal class ISimpleEntityRepository_Tests<TRepositoryHelper, TEntity, TId>
    where TRepositoryHelper : ISimpleEntityRepositoryHelper<TEntity, TId>, new()
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>
{
    TRepositoryHelper repositoryHelper;
    ISimpleEntityRepository<TEntity, TId>? systemUnderTests;

    [OneTimeSetUp]
    public void InitializeHelper() => repositoryHelper = new TRepositoryHelper();

    /*[SetUp]
    public void SetUp() => systemUnderTests = repositoryHelper.CreateRepository();*/

    [TearDown]
    public void TearDown() => repositoryHelper.DeleteRepository();

    [OneTimeTearDown]
    public void Clean() => repositoryHelper.Clean();

    [TestCaseSource(nameof(SingleEntity))]
    public void Get_IfSuchEntityExists_ThenReturnsIt(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        TEntity expected = exampleEntity;
        TEntity? actual = systemUnderTests.Get(exampleEntity.Id);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(SingleEntity))]
    public void Get_IfNoSuchEntityExists_ThenReturnsNull(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        TId idToFind = repositoryHelper.GetSimilarButNotExistingId(exampleEntity.Id);
        TEntity? actual = systemUnderTests.Get(idToFind);

        Assert.That(actual, Is.Null);
    }

    [TestCaseSource(nameof(SingleEntity))]
    public void Exists_IfSuchEntityExists_ThenReturnsTrue(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        bool actual = systemUnderTests.Exists(exampleEntity.Id);

        Assert.That(actual, Is.True);
    }

    [TestCaseSource(nameof(SingleEntity))]
    public void Exists_IfNoSuchEntityExists_ThenReturnsFalse(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        TId idToFind = repositoryHelper.GetSimilarButNotExistingId(exampleEntity.Id);
        bool actual = systemUnderTests.Exists(idToFind);

        Assert.That(actual, Is.False);
    }

    [TestCase]
    public void GetAll_IfNoEntitiesExist_ThenReturnsEmptyEnumerable()
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(Enumerable.Empty<TEntity>());
        IReadOnlyDictionary<TId, TEntity> actual = systemUnderTests.GetAll();

        Assert.That(actual, Is.Empty);
    }

    [TestCaseSource(nameof(EntityEnumerable))]
    public void GetAll_IfSomeEntitiesExist_ThenReturnsThem(TEntity[] exampleEntities)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(exampleEntities);
        var expected = exampleEntities.ToDictionary(it => it.Id);
        IReadOnlyDictionary<TId, TEntity> actual = systemUnderTests.GetAll();

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [TestCaseSource(nameof(SingleEntity))]
    public void AddAndImmediatelySave_IfNoEntitiesExist_ThenAddsSuccesfully(TEntity entityToAdd)
    {
        // arrange
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(Enumerable.Empty<TEntity>());
        // act
        systemUnderTests.AddAndImmediatelySave(entityToAdd);
        // assert
        var expected = new TEntity[] { entityToAdd };
        var actual = repositoryHelper.GetAllEntitiesFromRepository();

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [TestCaseSource(nameof(NotEmptyEntityEnumerable))]
    public void AddAndImmediatelySave_IfSomeEntitiesExistButNoConflicts_ThenAddsSuccesfully(TEntity[] exampleEntities)
    {
        // arrange
        Assume.That(exampleEntities.Count() > 0);
        TEntity entityToAdd = exampleEntities.First();
        IEnumerable<TEntity> alreadyExistingEntities = exampleEntities.Skip(1);
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(alreadyExistingEntities);
        // act
        systemUnderTests.AddAndImmediatelySave(entityToAdd);
        // assert
        var expected = alreadyExistingEntities.Append(entityToAdd);
        var actual = repositoryHelper.GetAllEntitiesFromRepository();

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [TestCaseSource(nameof(NotEmptyEntityEnumerable))]
    public void AddAndImmediatelySave_IfKeyAlreadyExistsConflict_ThenThrowsArgumentException(TEntity[] exampleEntities)
    {
        // arrange
        Assume.That(exampleEntities.Count() > 0);
        TEntity entityToAdd = exampleEntities.First();
        IEnumerable<TEntity> alreadyExistingEntities = exampleEntities;
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(alreadyExistingEntities);
        // act, assert
        Assert.Throws<ArgumentException>(() => systemUnderTests.AddAndImmediatelySave(entityToAdd));
    }

    [TestCaseSource(nameof(NotEmptyEntityEnumerable))]
    public void DeleteAndImmediatelySave_IfExists_ThenDeletesSuccessfully(TEntity[] exampleEntities)
    {
        // arrange
        Assume.That(exampleEntities.Count() > 0);
        IEnumerable<TEntity> existingEntities = exampleEntities;
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(existingEntities);
        // act
        TEntity entityToDelete = exampleEntities.First();
        systemUnderTests.DeleteAndImmediatelySave(entityToDelete.Id);
        // assert
        var expected = existingEntities.Skip(1);
        var actual = repositoryHelper.GetAllEntitiesFromRepository();
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [TestCaseSource(nameof(NotEmptyEntityEnumerable))]
    public void DeleteAndImmediatelySave_IfNotExists_ThenThrowsArgumentException(TEntity[] exampleEntities)
    {
        // arrange
        Assume.That(exampleEntities.Count() > 0);
        IEnumerable<TEntity> existingEntities = exampleEntities;
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(existingEntities);
        // act, assert
        TId notExistingId = repositoryHelper.GetSimilarButNotExistingId(exampleEntities.First().Id);
        Assert.Throws<ArgumentException>(() => systemUnderTests.DeleteAndImmediatelySave(notExistingId));
    }

    //void UpdateAndImmediatelySave(TEntityId id, TEntity entity);

    private static IEnumerable<TestCaseData> SingleEntity => singleEntitiesByTypes[typeof(TEntity).FullName!];
    private static readonly Dictionary<string, IEnumerable<TestCaseData>> singleEntitiesByTypes = new()
    {
        {
            typeof(User).FullName!,  new TestCaseData[]
            {
                new TestCaseData(new User("Vasya Petrov", Guid.Parse("6F9619FF-8B86-D011-B42D-00CF4FC964FF"))),
                new TestCaseData(new User("Misha Hrenov", Guid.Parse("12400a97-10b9-42f8-86d3-a00568f8e0c2"))),
                new TestCaseData(new User("Sveta Buleva",  Guid.Parse("5733abe5-f3ae-40db-a787-1f311b5a188b")))
            }
        }
    };


    private static IEnumerable<TestCaseData> EntityEnumerable => entityEnumerablesByTypes[typeof(TEntity).FullName!];
    private static IEnumerable<TestCaseData> NotEmptyEntityEnumerable => EntityEnumerable.Where(it => ((TEntity[])it.OriginalArguments[0]!).Length > 0);
    private static readonly Dictionary<string, IEnumerable<TestCaseData>> entityEnumerablesByTypes = new()
    {
        {
            typeof(User).FullName!,  new TestCaseData[]
            {
                new TestCaseData(new [] { new []
                {
                    new User("Vasya Petrov", Guid.Parse("6F9619FF-8B86-D011-B42D-00CF4FC964FF")),
                    new User("Misha Hrenov", Guid.Parse("12400a97-10b9-42f8-86d3-a00568f8e0c2")),
                    new User("Sveta Buleva",  Guid.Parse("5733abe5-f3ae-40db-a787-1f311b5a188b"))
                } }),
                new TestCaseData(new [] { new User[] { } }),
            }
        }
    };
}