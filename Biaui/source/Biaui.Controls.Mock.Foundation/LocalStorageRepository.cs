using System.Threading.Tasks;
using Biaui.Controls.Mock.Foundation.Interface;

namespace Biaui.Controls.Mock.Foundation;

public class LocalStorageRepository<T> : IRepository<T>
{
    private readonly LocalStorage _localStorage;
    private string _path;

    public LocalStorageRepository(LocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public LocalStorageRepository<T> Setup(string path)
    {
        _path = path;

        return this;
    }

    public Task SaveAsync(T obj)
    {
        return _localStorage.WriteAsync(obj, _path);
    }

    public Task<T> LoadAsync()
    {
        return _localStorage.ReadAsync<T>(_path);
    }
}