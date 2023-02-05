using System.Threading.Tasks;

namespace DependencyResolver.QuickStart
{
    internal interface IService
    {
        Task DoWork(object input);
    }
}