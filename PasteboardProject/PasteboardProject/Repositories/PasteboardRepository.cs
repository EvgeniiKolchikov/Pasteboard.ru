using PasteboardProject.Models;

namespace PasteboardProject.Repositories;

public class PasteboardRepository
{
    private List<Pasteboard> Pasteboards { get; set; }
    public List<PasteboardField> PasteboardFields { get; set; }

    public PasteboardRepository()
    {
        PasteboardFields = new List<PasteboardField>
        {
            new PasteboardField{FieldName = "Vk",FieldValue = "vk.com",Id = 1,PasteboardId = 1},
            new PasteboardField{FieldName = "ok",FieldValue = "ok.ru",Id = 2,PasteboardId = 1},
            new PasteboardField{FieldName = "Vk",FieldValue = "vk.com",Id = 3,PasteboardId = 2},
            new PasteboardField{FieldName = "ok",FieldValue = "ok.ru",Id = 4,PasteboardId = 2},
            new PasteboardField{FieldName = "Vk",FieldValue = "vk.com",Id = 5,PasteboardId = 3},
            new PasteboardField{FieldName = "ok",FieldValue = "ok.ru",Id = 6,PasteboardId = 3}
        };
        
        Pasteboards = new List<Pasteboard>
        {
            new Pasteboard { Id = 1, Name = "Первый"},
            new Pasteboard { Id = 2, Name = "Второй"},
            new Pasteboard { Id = 3, Name = "Третий"}
        };
        
    }

    public Pasteboard GetPasteboardById(int pasteboardId)
    {
        return Pasteboards[pasteboardId - 1];
    }

    public List<PasteboardField> GetFieldsByPasteboardId(int pasteboardId)
    {
        return PasteboardFields.Where(pf => pf.PasteboardId == pasteboardId).ToList();
    }
}