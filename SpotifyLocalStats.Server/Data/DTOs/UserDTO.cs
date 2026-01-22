namespace WebApi.Data.DTOs;

// essentially the main user data passed likely to a user profile page?
public class UserProfileDTO
{

    public UserProfileDTO(Guid userId) // then use gets to retriev that info??
    {
        
        UserName = getUserName()

    }
    public string UserName { get; set; }


}
