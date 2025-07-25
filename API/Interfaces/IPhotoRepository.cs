using API.DTO;
using API.Entities;

namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos();

    Task<Photo?> GetPhotoById(int id);

    void RemovePhoto(Photo photo);
}
