using System.Security.Claims;
using GoAuthSystem.Application.Common;
using GoAuthSystem.Application.DTOs.Auth;
using GoAuthSystem.Application.DTOs.User;
using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace GoAuthSystem.API.Controllers;

/// <summary>
/// Kimlik doğrulama controller'ı.
/// Login, Register, Refresh Token ve Revoke endpoint'leri.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public AuthController(
        IAuthService authService,
        IMapper mapper,
        UserManager<AppUser> userManager)
    {
        _authService = authService;
        _mapper = mapper;
        _userManager = userManager;
    }

    /// <summary>
    /// Kullanıcı girişi — e-posta ve şifre ile token al.
    /// Rate limit: dakikada max 10 istek.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginRequestDTO request)
    {
        var (accessToken, refreshToken, expiration) =
            await _authService.LoginAsync(request.Email, request.Password);

        // Kullanıcı bilgilerini al
        var user = await _userManager.FindByEmailAsync(request.Email);
        var roles = await _userManager.GetRolesAsync(user!);

        var userInfo = _mapper.Map<UserInfoDTO>(user);
        userInfo.Role = roles.FirstOrDefault() ?? string.Empty;

        var data = new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration,
            UserInfo = userInfo
        };

        return Ok(ApiResponse<AuthResponseDTO>.Ok(data, "Giriş başarılı."));
    }

    /// <summary>
    /// Yeni kullanıcı kaydı — varsayılan olarak Student rolü atanır.
    /// Rate limit: dakikada max 5 istek.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register([FromBody] RegisterRequestDTO request)
    {
        var (accessToken, refreshToken, expiration) =
            await _authService.RegisterAsync(
                request.FirstName, request.LastName,
                request.Email, request.Password);

        // Kullanıcı bilgilerini al
        var user = await _userManager.FindByEmailAsync(request.Email);
        var roles = await _userManager.GetRolesAsync(user!);

        var userInfo = _mapper.Map<UserInfoDTO>(user);
        userInfo.Role = roles.FirstOrDefault() ?? string.Empty;

        var data = new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration,
            UserInfo = userInfo
        };

        return Ok(ApiResponse<AuthResponseDTO>.Ok(data, "Kayıt başarılı."));
    }

    /// <summary>
    /// Refresh token rotation — süresi dolmuş access token ve geçerli refresh token
    /// ile yeni bir token çifti al.
    /// Rate limit: dakikada max 20 istek.
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [EnableRateLimiting("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        var (accessToken, refreshToken, expiration) =
            await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);

        var data = new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration
        };

        return Ok(ApiResponse<AuthResponseDTO>.Ok(data, "Token yenilendi."));
    }

    /// <summary>
    /// Belirli bir refresh token'ı iptal et (logout).
    /// </summary>
    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RevokeToken([FromBody] RevokeTokenRequestDTO request)
    {
        await _authService.RevokeTokenAsync(request.RefreshToken);
        return Ok(ApiResponse.Ok("Token başarıyla iptal edildi."));
    }

    /// <summary>
    /// Kullanıcının tüm aktif refresh token'larını iptal et.
    /// </summary>
    [HttpPost("revoke-all-tokens")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RevokeAllTokens()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _authService.RevokeAllTokensAsync(userId);
        return Ok(ApiResponse.Ok("Tüm tokenlar başarıyla iptal edildi."));
    }
}
