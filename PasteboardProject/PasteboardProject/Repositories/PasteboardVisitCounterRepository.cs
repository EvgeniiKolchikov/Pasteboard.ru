using PasteboardProject.Context;
using PasteboardProject.Interfaces;

namespace PasteboardProject.Repositories;

public class PasteboardVisitCounterRepository : IVisitCounterRepository
{
    private readonly ApplicationContext _db;
    public PasteboardVisitCounterRepository(ApplicationContext context)
    {
        _db = context;
    }
    
    
}