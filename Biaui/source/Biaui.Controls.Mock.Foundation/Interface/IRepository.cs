using System.Threading.Tasks;

namespace Biaui.Controls.Mock.Foundation.Interface;

public interface IRepository<T>
{
    Task SaveAsync(T obj);
    Task<T?> LoadAsync();
}