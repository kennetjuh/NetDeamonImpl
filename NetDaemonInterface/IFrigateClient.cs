using System.Threading.Tasks;

namespace NetDaemonInterface
{
    public interface IFrigateClient
    {
        Task MarkReviewedAsync(string? id);
        Task MarkReviewedWithLoginAsync(string? id);
    }
}
