using System.Threading.Tasks;

namespace Logging.QuickStart
{
    internal interface ITransactionalService
    {
        Task DoWork(object input);
    }
}