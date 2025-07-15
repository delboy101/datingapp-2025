using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);

    void DeleteMessage(Message message);

    Task<Message?> GetMessageAsync(string messageId);

    Task<PaginatedResult<MessageDto>> GetMessagesForMemberAsync(MessageParams messageParams);

    Task<IReadOnlyList<MessageDto>> GetMessageThreadAsync(string currentMemberId, string recipientid);

    void AddGroup(Group group);

    Task RemoveConnectionAsync(string connectionId);

    Task<Connection?> GetConnectionAsync(string connectionId);

    Task<Group?> GetMessageGroupAsync(string groupName);

    Task<Group?> GetGroupForConnectionAsync(string connectionId);
}
