using FluentValidation;
using GoAuthSystem.Application.DTOs.Auth;

namespace GoAuthSystem.Application.Validators;

/// <summary>
/// Login isteği doğrulama kuralları.
/// FluentValidation ile e-posta formatı ve minimum şifre uzunluğu kontrol edilir.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
{
    public LoginRequestValidator()
    {
        // E-posta: boş olamaz, geçerli e-posta formatında olmalı
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        // Şifre: boş olamaz, en az 8 karakter
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.");
    }
}
