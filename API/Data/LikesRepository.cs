using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(AppDbContext context) : ILikesRepository
{
    public void AddLike(MemberLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(MemberLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIdsAsync(string memberId)
    {
        return await context.Likes
            .Where(x => x.SourceMemberId == memberId)
            .Select(x => x.TargetMemberId)
            .ToListAsync();
    }

    public async Task<MemberLike?> GetMemberLikeAsync(string sourceMemberId, string targetMemberId)
    {
        return await context.Likes
            .FindAsync(sourceMemberId, targetMemberId);
    }

    public async Task<PaginatedResult<Member>> GetMemberLikesAsync(LikesParams likesParams)
    {
        var query = context.Likes.AsQueryable();
        IQueryable<Member> result;

        switch (likesParams.Predicate)
        {
            case "liked":
                result = query
                    .Where(x => x.SourceMemberId == likesParams.CurrentMemberId)
                    .Select(x => x.TargetMember);
                break;
            case "likedBy":
                result = query
                    .Where(x => x.TargetMemberId == likesParams.CurrentMemberId)
                    .Select(x => x.SourceMember);
                break;
            default: // mutual
                var likeIds = await GetCurrentMemberLikeIdsAsync(likesParams.CurrentMemberId);

                result = query
                    .Where(x => x.TargetMemberId == likesParams.CurrentMemberId && likeIds.Contains(x.SourceMemberId))
                    .Select(x => x.SourceMember);
                break;
        }

        return await PaginationHelper.CreateAsync(
            result,
            likesParams.PageNumber,
            likesParams.PageSize);
    }

    public async Task<bool> SaveAllChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
