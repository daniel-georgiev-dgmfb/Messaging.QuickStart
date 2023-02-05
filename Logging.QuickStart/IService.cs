using System.Threading.Tasks;

namespace Logging.QuickStart
{
    internal interface IService
    {
        Task DoWork(object input);
        Task LogSomething(object input);
    }
}