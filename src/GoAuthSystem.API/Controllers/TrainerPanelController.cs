using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoAuthSystem.API.Controllers;

/// <summary>
/// Eğitmen paneli controller'ı.
/// Sadece Trainer rolüne sahip kullanıcılar erişebilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trainer")]
public class TrainerPanelController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly AppDbContext _context;

    public TrainerPanelController(ICurrentUserService currentUser, AppDbContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    /// <summary>
    /// Trainer'a özel dashboard verisi.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = _currentUser.UserId;

        // Trainer bilgilerini getir
        var trainer = await _context.Trainers
            .FirstOrDefaultAsync(t => t.AppUser != null && t.AppUser.Id == userId);

        var studentCount = trainer != null
            ? await _context.Trainees.CountAsync(t => t.TrainerId == trainer.PersonId)
            : 0;

        return Ok(new
        {
            message = $"Hoş geldiniz, {_currentUser.Email}!",
            role = "Trainer",
            studentCount,
            trainerId = trainer?.PersonId
        });
    }

    /// <summary>
    /// Sadece kendi öğrencilerini listele.
    /// </summary>
    [HttpGet("my-students")]
    public async Task<IActionResult> GetMyStudents()
    {
        var userId = _currentUser.UserId;

        var trainer = await _context.Trainers
            .FirstOrDefaultAsync(t => t.AppUser != null && t.AppUser.Id == userId);

        if (trainer == null)
            return NotFound("Eğitmen profili bulunamadı.");

        var students = await _context.Trainees
            .Where(t => t.TrainerId == trainer.PersonId)
            .Select(t => new
            {
                t.PersonId,
                t.FirstName,
                t.LastName,
                t.Email,
                t.PhoneNumber
            })
            .ToListAsync();

        return Ok(students);
    }
}
