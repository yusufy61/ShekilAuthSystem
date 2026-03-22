using FluentValidation;
using GoAuthSystem.Application.DTOs.Auth;

namespace GoAuthSystem.Application.Validators;

/// <summary>
/// Şifre değiştirme doğrulama kuralları.
/// Mevcut şifre ile yeni şifre aynı olamaz.
/// Yeni şifre güçlü şifre kurallarına uymalı.
/// </summary>
public class ChangePasswordValidator : AbstractValidator<ChangePasswordDTO>
{
    public ChangePasswordValidator()
    {
        // Mevcut şifre: boş olamaz
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mevcut şifre boş olamaz.");

        // Yeni şifre: boş olamaz, minimum 8 karakter, güçlü şifre kuralları
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Yeni şifre en az 8 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Yeni şifre en az 1 büyük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Yeni şifre en az 1 rakam içermelidir.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Yeni şifre en az 1 özel karakter içermelidir.")
            .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifre mevcut şifre ile aynı olamaz.");

        // Yeni şifre tekrarı: NewPassword ile eşleşmeli
        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Şifre tekrarı boş olamaz.")
            .Equal(x => x.NewPassword).WithMessage("Yeni şifreler eşleşmiyor.");
    }
}
