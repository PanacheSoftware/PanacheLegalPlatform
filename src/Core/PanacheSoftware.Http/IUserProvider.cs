using System.Threading.Tasks;

namespace PanacheSoftware.Http
{
    public interface IUserProvider
    {
        string GetTenantId();
        string GetUserId();
    }
}
