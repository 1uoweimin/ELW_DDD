using ListeningService.Domain.Entities;
using Zack.DomainCommons.Models;

namespace ListeningService.Main.WebAPI.Controllers.DTOs;

public record CategoryDTO(Guid Id, MultilingualString Name, Uri CoverUrl)
{
    public static CategoryDTO? Create(Category? category)
    {
        if (category == null) return null;
        return new CategoryDTO(category.Id, category.Name, category.CoverUrl);
    }
    public static CategoryDTO[] Create(Category[] categories)
        => categories.Select(c => Create(c)!).ToArray();
}
