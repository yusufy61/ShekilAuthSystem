using FluentValidation;
using GoAuthSystem.Application.DTOs.Auth;

namespace GoAuthSystem.Application.Validators;

/// <summary>
/// Kayıt isteği doğrulama kuralları.
/// Güçlü şifre politikası: en az 8 karakter, 1 büyük harf, 1 rakam, 1 özel karakter.
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequestDTO>
{
    public RegisterRequestValidator()
    {
        // Ad: boş olamaz, maksimum 50 karakter
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

        // Soyad: boş olamaz, maksimum 50 karakter
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

        // E-posta: boş olamaz, geçerli format, maksimum 256 karakter
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(256).WithMessage("E-posta adresi en fazla 256 karakter olabilir.");

        // Şifre: boş olamaz, minimum 8 karakter, güçlü şifre kuralları
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az 1 büyük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az 1 rakam içermelidir.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az 1 özel karakter içermelidir.");

        // Şifre tekrarı: Password ile eşleşmeli
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Şifre tekrarı boş olamaz.")
            .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor.");
    }
}
