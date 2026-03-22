using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoAuthSystem.API.Controllers;

/// <summary>
/// Öğrenci paneli controller'ı.
/// Sadece Student rolüne sahip kullanıcılar erişebilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class StudentPanelController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly AppDbContext _context;

    public StudentPanelController(ICurrentUserService currentUser, AppDbContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    /// <summary>
    /// Student'a özel dashboard verisi.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = _currentUser.UserId;

        var trainee = await _context.Trainees
            .Include(t => t.Trainer)
            .FirstOrDefaultAsync(t => t.AppUser != null && t.AppUser.Id == userId);

        return Ok(new
        {
            message = $"Hoş geldiniz, {_currentUser.Email}!",
            role = "Student",
            trainerName = trainee?.Trainer != null
                ? $"{trainee.Trainer.FirstName} {trainee.Trainer.LastName}"
                : "Henüz eğitmen atanmadı",
            traineeId = trainee?.PersonId
        });
    }

    /// <summary>
    /// Kendi programlarını getir (demo — ileride genişletilecek).
    /// </summary>
    [HttpGet("my-programs")]
    public IActionResult GetMyPrograms()
    {
        return Ok(new
        {
            message = "Programlar henüz implementasyon aşamasında.",
            programs = new List<object>()
        });
    }
}
