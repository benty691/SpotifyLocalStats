using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs;

public class CreateUserRequest
{
    [Required]
    [StringLength(35)]
    public string? UserName { get; set; }

    [Required]
    [StringLength(35)]
    public string? UserFirstName { get; set; }
}
