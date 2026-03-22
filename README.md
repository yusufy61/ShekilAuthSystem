## Shekil Auhtorize API Uygulaması

### Çalıştırılması için yapılması gerekenler

#### 1) JWT Key

- API uygulaması içerisinden appsetttings.json dosyasında bulunan key alanına kendi keyinizi eklemeniz önerilir. (Çalıştırmak için zorunlu değil.)
- Sadece güvenlik amacıyla bu işlemi yapıyoruz.

``` json
"JwtSettings": {
  "SecretKey": "eCY1IUBMxv7+xAp9atQwzReZKXN20ABmIpxCK9yDk1Ppa07WRvZR34axyDQkGWX7OpBNt9nVY5crHFx3JKlHOw==",
  "Issuer": "GoAuthSystem",
  "Audience": "GoAuthSystemClients",
  "AccessTokenExpirationMinutes": 15,
  "RefreshTokenExpirationDays": 7
}
```

#### 2) Veri Tabanı bağlantısı

- ORM : Entity Framework Core
- Veri tabanı bağlantısını yine API katmanında bulunan appsetttings.json dosyası üzerinden güncelleyebilirsiniz.

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GoAuthSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```
Veri Tabanı adı : localhost


#### 3) Veri tabanı Entegre

- Veri tabanına entegre etmek için terminal üzerinden;

eğer ki visual studio kullanıyorsanız (Package Manager Console) üzerinden proje olarak "Persistence" katmanı seçili olacak şekilde
```
update-database
```

eğer ki visual studio code (vs code) kullanıyorsanız (powershell üzerinden)
```
dotnet ef database updaate
```

Bu işlemler başarılı olduktan sonra panel üzerinden gelip gelmediğini, bir sorun olup olmadığını kontrol ediniz.

#### 4) Projeyi Çalıştırmak

- Veri tabanı bağlantısını kontrol ettikten sonra uygulamayı çalıştıralım. eğer ki visual studio kullanıyorsanız Run komutunun altında Set Startup Project alanında çalışma projesini API katmanına ayaralıyoruz. ardından RUN butonu ile çalıştırabiliriz.

#### 5) Swagger Üzerinden Test

Swagger üzerinden giriş yapmadan ve authorize vermeden diğer kontrolleri yapamıyoruz. Bir kullanıcı olarak kaydolup kendimizi Admin olarak atayamayacağımız için başlangıçta kolayca test edilebilirlik olsun diye Seed data ekledim.

``` json
{
  "email": "admin@goathletic.com",
  "password": "Admin123!@#"
}
```

Bu veriler ile Login fonksiyonunu execute edelim.

Ardından bize verilen paket içerisinde Access Token bulunuyor. Bu token ı kopyalayalım ve aşağıda bulunan Authorize butonunda istenilen yere yapıştırıp Yetki verelim. Artık diğer fonksiyonları kullanabiliriz.

<img width="1445" height="540" alt="image" src="https://github.com/user-attachments/assets/15610c61-06b8-485d-8b99-bcd980a663ea" />


Teşekkürler
~Shekil
