namespace API.DTO;

public class PhotoForApprovalDto
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public required string MemberId { get; set; }
    public bool IsApproved { get; set; }
}
