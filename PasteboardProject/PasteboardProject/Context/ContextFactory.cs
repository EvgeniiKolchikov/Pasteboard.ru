using PasteboardProject.Interfaces;
using PasteboardProject.Repositories;

namespace PasteboardProject.Context;

public class ContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

    }
    
    public IRepository GetRepository(string repository)
    {
        if (repository == "PasteboardRepositoryPostgres")
            return (IRepository)_serviceProvider.GetService(typeof(PasteboardRepositoryPostgres));
                
        return (IRepository)_serviceProvider.GetService(typeof(PasteboardRepositoryJson));
    }
}