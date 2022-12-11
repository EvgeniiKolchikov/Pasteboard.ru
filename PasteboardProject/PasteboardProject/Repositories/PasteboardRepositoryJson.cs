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
        var pasteboard = new Pasteboard();
        try
        {
            pasteboard = Pasteboards[pasteboardId - 1];
        }
        catch (ArgumentOutOfRangeException)
        {
            var count = Pasteboards.Count;
            var rnd = new Random();
            var rndIndex = rnd.Next(1, count + 1);
            pasteboard = Pasteboards[rndIndex];
        }

        return pasteboard;
    }

    public async Task AddCardToJsonAsync(Pasteboard pasteboard)
    {
        var lastId = Pasteboards.Count;
        pasteboard.Id = lastId + 1;
        Pasteboards.Add(pasteboard);
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