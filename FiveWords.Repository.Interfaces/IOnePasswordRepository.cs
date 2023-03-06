namespace FiveWords.Repository.Interfaces;

public interface IOnePasswordRepository : IBaseRepository
{
    byte[] GetPasswordHash();
    void SavePasswordHash(byte[] data);
}
