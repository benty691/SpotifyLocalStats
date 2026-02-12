using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.DTO;

public class CreateUserRequest
{
    [Required]
    [StringLength(35)]
    public string? UserName { get; set; }

    [Required]
    [StringLength(35)]
    public string? UserFirstName { get; set; }
}
