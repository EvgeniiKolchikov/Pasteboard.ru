using System.Text.Json;
using PasteboardProject.Models;

namespace PasteboardProject.Repositories;

public class PasteboardRepositoryJson
{
    public List<Pasteboard> Pasteboards { get; set; }
    public List<PasteboardField> PasteboardFields { get; set; }
    
    public PasteboardRepositoryJson()
    {
        Pasteboards = GetAllPasteboardsFromJsonAsync().Result;
    }

    public Pasteboard GetPasteboardById(int pasteboardId)
    {
        return Pasteboards.First(pb => pb.Id == pasteboardId);
    }

    public async Task AddCardToJsonAsync()
    {
        using (var fs = new FileStream("pasteboard.json",FileMode.OpenOrCreate))
        {
            await JsonSerializer.SerializeAsync(fs, Pasteboards);
        }
    }

    public async Task<List<Pasteboard>> GetAllPasteboardsFromJsonAsync()
    {
        var list = new List<Pasteboard>();
        using (var fs = new FileStream("pasteboard.json", FileMode.Open))
        {
            list = await JsonSerializer.DeserializeAsync<List<Pasteboard>>(fs);
        }

        return list;
    }

    
}