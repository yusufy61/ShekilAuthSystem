using Microsoft.AspNetCore.Identity;

namespace GoAuthSystem.Domain.Entities;

/// <summary>
/// Uygulama rol entity'si.
/// IdentityRole<int> türetilmiş — PK tipi int.
/// </summary>
public class AppRole : IdentityRole<int>
{
    /// <summary>
    /// Rolün açıklaması (opsiyonel)
    /// </summary>
    public string? Description { get; set; }
}
