using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<MemberLike?> GetMemberLikeAsync(string sourceMemberId, string targetMemberId);

    Task<PaginatedResult<Member>> GetMemberLikesAsync(LikesParams likesParams);

    Task<IReadOnlyList<string>> GetCurrentMemberLikeIdsAsync(string memberId);

    void DeleteLike(MemberLike like);

    void AddLike(MemberLike like);
}
