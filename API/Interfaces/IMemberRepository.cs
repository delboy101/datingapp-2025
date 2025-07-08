using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMemberRepository
{
    void Update(Member member);

    Task<bool> SaveAllAsync();

    Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);

    Task<Member?> GetMemberByIdAsync(string id);

    Task<Member?> GetMemberForUpdateAsync(string id);

    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
}
