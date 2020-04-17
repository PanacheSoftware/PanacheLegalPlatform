using System;

namespace PanacheSoftware.Database.Core
{
    public interface IPanacheSoftwareUnitOfWork : IDisposable
    {
        int Complete();
    }
}
