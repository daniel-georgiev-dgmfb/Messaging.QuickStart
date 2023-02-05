using System.Threading.Tasks;

namespace Logging.QuickStart
{
    internal interface IOtherService
    {
        Task DoWork(object input);
    }
}