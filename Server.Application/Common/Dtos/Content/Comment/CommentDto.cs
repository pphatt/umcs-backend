namespace Server.Application.Common.Dtos.Content.Comment;

public class CommentDto
{
    public string Content { get; set; }

    public string Username { get; set; }

    public string Avatar { get; set; }

    public DateTime DateCreated { get; set; }
}
