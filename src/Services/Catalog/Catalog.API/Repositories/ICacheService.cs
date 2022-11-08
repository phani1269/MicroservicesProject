

namespace Catalog.API.Repositories
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, int expirationTime);
        object RemoveData(string key);
    }
}







