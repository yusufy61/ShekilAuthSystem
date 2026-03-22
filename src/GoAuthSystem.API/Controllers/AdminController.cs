using AutoMapper;
using GoAuthSystem.Application.Common;
using GoAuthSystem.Application.DTOs.User;
using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoAuthSystem.API.Controllers;

/// <summary>
/// Admin paneli controller'ı.
/// Kullanıcı yönetimi, rol atama, eğitmen/öğrenci oluşturma.
/// Sadece Admin rolüne sahip kullanıcılar erişebilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public AdminController(
        UserManager<AppUser> userManager,
        IAuthService authService,
        IMapper mapper,
        AppDbContext context)
    {
        _userManager = userManager;
        _authService = authService;
        _mapper = mapper;
        _context = context;
    }

    /// <summary>
    /// Tüm kullanıcıları listele.
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<List<UserInfoDTO>>>> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userList = new List<UserInfoDTO>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var dto = _mapper.Map<UserInfoDTO>(user);
            dto.Role = roles.FirstOrDefault() ?? string.Empty;
            userList.Add(dto);
        }

        return Ok(ApiResponse<List<UserInfoDTO>>.Ok(userList, $"{userList.Count} kullanıcı listelendi."));
    }

    /// <summary>
    /// Kullanıcıya rol ata.
    /// </summary>
    [HttpPost("assign-role")]
    public async Task<ActionResult<ApiResponse>> AssignRole([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return NotFound(ApiResponse.Fail("Kullanıcı bulunamadı."));

        // Mevcut rolleri kaldır
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Yeni rol ata
        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse.Fail("Rol ataması başarısız.", errors));
        }

        return Ok(ApiResponse.Ok($"'{request.Role}' rolü başarıyla atandı."));
    }

    /// <summary>
    /// Trainer oluştur + AppUser + Trainer rolü ata.
    /// </summary>
    [HttpPost("create-trainer")]
    public async Task<ActionResult<ApiResponse>> CreateTrainer([FromBody] CreateTrainerRequest request)
    {
        // 1. Trainer entity oluştur
        var trainer = new Trainer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Speciality = request.Speciality
        };

        _context.Trainers.Add(trainer);
        await _context.SaveChangesAsync();

        // 2. AppUser oluştur ve Trainer rolü ata
        await _authService.RegisterAsync(
            request.FirstName, request.LastName, request.Email, request.Password, "Trainer");

        // 3. AppUser'a TrainerId ata
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            user.TrainerId = trainer.PersonId;
            await _userManager.UpdateAsync(user);
        }

        return Ok(ApiResponse.Ok($"Eğitmen '{request.FirstName} {request.LastName}' başarıyla oluşturuldu."));
    }

    /// <summary>
    /// Student oluştur + AppUser + Student rolü ata.
    /// </summary>
    [HttpPost("create-student")]
    public async Task<ActionResult<ApiResponse>> CreateStudent([FromBody] CreateStudentRequest request)
    {
        // 1. Trainee entity oluştur
        var trainee = new Trainee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            TrainerId = request.TrainerId
        };

        _context.Trainees.Add(trainee);
        await _context.SaveChangesAsync();

        // 2. AppUser oluştur ve Student rolü ata
        await _authService.RegisterAsync(
            request.FirstName, request.LastName, request.Email, request.Password, "Student");

        // 3. AppUser'a TraineeId ata
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            user.TraineeId = trainee.PersonId;
            await _userManager.UpdateAsync(user);
        }

        return Ok(ApiResponse.Ok($"Öğrenci '{request.FirstName} {request.LastName}' başarıyla oluşturuldu."));
    }

    /// <summary>
    /// Hesabı pasifleştir — kullanıcı giriş yapamaz.
    /// </summary>
    [HttpPost("deactivate-user/{id}")]
    public async Task<ActionResult<ApiResponse>> DeactivateUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return NotFound(ApiResponse.Fail("Kullanıcı bulunamadı."));

        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        // Tüm tokenları iptal et
        await _authService.RevokeAllTokensAsync(id);

        return Ok(ApiResponse.Ok("Hesap pasifleştirildi ve tüm oturumlar sonlandırıldı."));
    }

    /// <summary>
    /// Hesabı aktifleştir.
    /// </summary>
    [HttpPost("activate-user/{id}")]
    public async Task<ActionResult<ApiResponse>> ActivateUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            return NotFound(ApiResponse.Fail("Kullanıcı bulunamadı."));

        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse.Ok("Hesap aktifleştirildi."));
    }
}

// ── Admin Request DTOs (Controller'a özel) ──────────────
public record AssignRoleRequest(int UserId, string Role);

public record CreateTrainerRequest(
    string FirstName, string LastName, string Email,
    string PhoneNumber, string Speciality, string Password);

public record CreateStudentRequest(
    string FirstName, string LastName, string Email,
    string PhoneNumber, int? TrainerId, string Password);
