namespace WebApi.Data.DTOs;

public class SpotifyAccessTokenResponseDto
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
}
