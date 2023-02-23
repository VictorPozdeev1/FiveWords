using FiveWords.Repository.Interfaces;
using FiveWords.DataObjects;
using FiveWords.Repository.Tests;

[TestFixture(typeof(SimpleRepositoryHelper), typeof(User), typeof(string))]
internal class IUsersRepository_Get_Tests<TRepositoryHelper, TEntity, TId>
    where TRepositoryHelper : ISimpleRepositoryHelper<TEntity, TId>, new()
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

    [Test]
    [TestCaseSource(nameof(TestCasesOfType))]
    public void IfSingleSuchUserExists_ThenReturnsIt(TEntity singleEntity)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(singleEntity);
        TEntity expected = singleEntity;
        TEntity? actual = systemUnderTests.Get(singleEntity.Id);

        Assert.That(actual, Is.EqualTo(expected));
    }

    public void IfNoSuchUserExists_ThenReturnsNull()
    {
        Assert.Pass();
    }

    // Такая ситуация в принципе не должна быть допущена любой реализацией интерфейса
    //public void IfMultipleSuchUsersExist_ThenWhatToDo()
    //{
    //    Assert.Fail();
    //}

    public static IEnumerable<TestCaseData> TestCasesOfType => TestCasesByTypes[typeof(TEntity).FullName!];

    private static Dictionary<string, IEnumerable<TestCaseData>> TestCasesByTypes = new()
    {
        {
            typeof(User).FullName!,  new TestCaseData[]
            {
                new TestCaseData(new User("Vasya Petrov", Guid.Parse("6F9619FF-8B86-D011-B42D-00CF4FC964FF"))),
                new TestCaseData(new User("Misha Hrenov", Guid.Parse("12400a97-10b9-42f8-86d3-a00568f8e0c2"))),
                new TestCaseData(new User("Sveta Buleva",  Guid.Parse("5733abe5-f3ae-40db-a787-1f311b5a188b"))),
            }
        }
    };
}

public class IUsersRepository_Get_TestsData
{

}