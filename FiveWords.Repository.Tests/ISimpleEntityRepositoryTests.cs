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

    [TestCaseSource(nameof(SingleEntities))]
    public void Get_IfSuchEntityExists_ThenReturnsIt(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        TEntity expected = exampleEntity;
        TEntity? actual = systemUnderTests.Get(exampleEntity.Id);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(SingleEntities))]
    public void Get_IfNoSuchEntityExists_ThenReturnsNull(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        TId idToFind = repositoryHelper.GetSomeSimilarId(exampleEntity.Id);
        TEntity? actual = systemUnderTests.Get(idToFind);

        Assert.That(actual, Is.Null);
    }

    [TestCaseSource(nameof(SingleEntities))]
    public void Exists_IfSuchEntityExists_ThenReturnsTrue(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        bool actual = systemUnderTests.Exists(exampleEntity.Id);

        Assert.That(actual, Is.True);
    }

    [TestCaseSource(nameof(SingleEntities))]
    public void Exists_IfNoSuchEntityExists_ThenReturnsFalse(TEntity exampleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(exampleEntity);
        TId idToFind = repositoryHelper.GetSomeSimilarId(exampleEntity.Id);
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

    [TestCaseSource(nameof(EntityEnumerables))]
    public void GetAll_IfSomeEntitiesExist_ThenReturnsThem(TEntity[] exampleEntities)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithSomeEntities(exampleEntities);
        var expected = exampleEntities.ToDictionary(it => it.Id);
        IReadOnlyDictionary<TId, TEntity> actual = systemUnderTests.GetAll();

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    private static IEnumerable<TestCaseData> SingleEntities => singleEntitiesByTypes[typeof(TEntity).FullName!];
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


    private static IEnumerable<TestCaseData> EntityEnumerables => entityEnumerablesByTypes[typeof(TEntity).FullName!];
    private static readonly Dictionary<string, IEnumerable<TestCaseData>> entityEnumerablesByTypes = new()
    {
        {
            typeof(User).FullName!,  new TestCaseData[]
            {
                new TestCaseData(new [] {new User[]
                {
                    new User("Vasya Petrov", Guid.Parse("6F9619FF-8B86-D011-B42D-00CF4FC964FF")),
                    new User("Misha Hrenov", Guid.Parse("12400a97-10b9-42f8-86d3-a00568f8e0c2")),
                    new User("Sveta Buleva",  Guid.Parse("5733abe5-f3ae-40db-a787-1f311b5a188b"))
                } })
            }
        }
    };
}