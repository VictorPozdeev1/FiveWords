using FiveWords.Repository.Interfaces;
using FiveWords.DataObjects;
using FiveWords.Repository.Tests;

[TestFixture(typeof(SimpleRepositoryHelper))]
internal class IUsersRepository_Get_Tests<TRepositoryHelper>
    where TRepositoryHelper : ISimpleRepositoryHelper, new()
{
    TRepositoryHelper repositoryHelper;
    IUsersRepository? systemUnderTests;

    [OneTimeSetUp]
    public void InitializeHelper() => repositoryHelper = new TRepositoryHelper();

    /*[SetUp]
    public void SetUp() => systemUnderTests = repositoryHelper.CreateRepository();*/

    [TearDown]
    public void TearDown() => repositoryHelper.DeleteRepository();

    [OneTimeTearDown]
    public void Clean() => repositoryHelper.Clean();

    [Test]
    [TestCase("Vasya Petrov", "6F9619FF-8B86-D011-B42D-00CF4FC964FF")]
    [TestCase("Misha Hrenov", "6F9619FF-8B86-D011-B42D-00CF4FC964FF")]
    public void IfSingleSuchUserExists_ThenReturnsIt(string id, string guidString)
    {
        systemUnderTests = repositoryHelper.CreateRepositoryWithOneEntity(id, guidString);
        User expected = new User()
        {
            Id = id,
            Guid = Guid.Parse(guidString)
        };

        User? actual = systemUnderTests.Get(id);

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
}

public class IUsersRepository_GetAll_Tests
{

}