namespace WebApi.Data.DTOs;

// essentially the main user data passed likely to a user profile page?
public class UserDto
{
    public UserDto(Guid id, string userName) // then use gets to retriev that info??
    {
        Id = id;
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }
    public string UserName { get; set; }
    public Guid Id {get; set;}
}
