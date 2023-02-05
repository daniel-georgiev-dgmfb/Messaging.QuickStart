using System.Threading.Tasks;

namespace DependencyResolver.QuickStart
{
    interface IRule
    {
        Task ApplyRule();
    }
}