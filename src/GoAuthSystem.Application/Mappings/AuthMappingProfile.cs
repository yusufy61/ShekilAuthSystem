using AutoMapper;
using GoAuthSystem.Application.DTOs.User;
using GoAuthSystem.Domain.Entities;

namespace GoAuthSystem.Application.Mappings;

/// <summary>
/// AutoMapper profili — kimlik doğrulama ile ilgili mapping tanımları.
/// AppUser entity'si ile DTO'lar arasındaki dönüşümleri tanımlar.
/// </summary>
public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // AppUser → UserInfoDTO
        // FullName computed property olduğu için otomatik map edilir
        // Role alanı mapping sırasında manuel set edilecek (UserManager'dan alınır)
        CreateMap<AppUser, UserInfoDTO>()
            .ForMember(dest => dest.Role, opt => opt.Ignore()); // Rol, servis katmanında set edilecek

        // UpdateProfileDTO → AppUser (profil güncelleme)
        // Sadece belirtilen alanlar güncellenir, diğerleri korunur
        CreateMap<UpdateProfileDTO, AppUser>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
            .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.TrainerId, opt => opt.Ignore())
            .ForMember(dest => dest.TraineeId, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
    }
}
