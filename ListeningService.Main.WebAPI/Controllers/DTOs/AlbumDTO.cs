using ListeningService.Domain.Entities;
using Zack.DomainCommons.Models;

namespace ListeningService.Main.WebAPI.Controllers.DTOs;

public record AlbumDTO(Guid Id, MultilingualString Name, Guid CategoryId)
{
    public static AlbumDTO? Create(Album? album)
    {
        if (album == null) return null;
        return new AlbumDTO(album.Id, album.Name, album.CategoryId);
    }
    public static AlbumDTO[] Create(Album[] albums)
        => albums.Select(a => Create(a)!).ToArray();
}
