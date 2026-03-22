using AutoMapper;
using GoAuthSystem.Application.Common;
using GoAuthSystem.Application.DTOs.Auth;
using GoAuthSystem.Application.DTOs.User;
using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GoAuthSystem.API.Controllers;

/// <summary>
/// Hesap yönetimi controller'ı.
/// Profil görüntüleme, güncelleme ve şifre değiştirme.
/// Oturum açmış kullanıcılar erişebilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public AccountController(
        UserManager<AppUser> userManager,
        IAuthService authService,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _userManager = userManager;
        _authService = authService;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    /// <summary>
    /// Mevcut kullanıcının profil bilgilerini getir.
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserInfoDTO>>> GetProfile()
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString()!);
        if (user == null)
            return NotFound(ApiResponse<UserInfoDTO>.Fail("Kullanıcı bulunamadı."));

        var roles = await _userManager.GetRolesAsync(user);
        var userInfo = _mapper.Map<UserInfoDTO>(user);
        userInfo.Role = roles.FirstOrDefault() ?? string.Empty;

        return Ok(ApiResponse<UserInfoDTO>.Ok(userInfo, "Profil bilgileri getirildi."));
    }

    /// <summary>
    /// Profil bilgilerini güncelle (ad, soyad, adres, resim).
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UpdateProfileDTO request)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString()!);
        if (user == null)
            return NotFound(ApiResponse.Fail("Kullanıcı bulunamadı."));

        // AutoMapper ile DTO → Entity mapping (sadece izin verilen alanlar güncellenir)
        _mapper.Map(request, user);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse.Fail("Profil güncellenemedi.", errors));
        }

        return Ok(ApiResponse.Ok("Profil başarıyla güncellendi."));
    }

    /// <summary>
    /// Şifre değiştir — mevcut şifre doğrulanır, tüm oturumlar sonlandırılır.
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordDTO request)
    {
        await _authService.ChangePasswordAsync(
            _currentUser.UserId!.Value,
            request.CurrentPassword,
            request.NewPassword);

        return Ok(ApiResponse.Ok("Şifre başarıyla değiştirildi. Lütfen tekrar giriş yapın."));
    }
}
