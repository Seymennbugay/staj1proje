eBA Süreç Takip Sistemi (CLI Proje)

Bu proje, C# dili ve SQLite veritabanı kullanılarak geliştirilen, terminal tabanlı bir "İş Süreç Takip Uygulamasıdır". Kullanıcılar sisteme giriş yaparak iş talebi oluşturabilir, yöneticiler ise bu talepleri onaylayabilir veya reddedebilir. Proje tek bir dosya (Program.cs) içerisinde yönetilmektedir ve kolay çalıştırılabilir bir yapıya sahiptir. Bu uygulama, gerçek dünyada kullanılan eBA tarzı talepli sistemlerin sadeleştirilmiş bir simülasyonudur.

Geliştirici: Seymen Bugay
Danışman: Dr. Öğr. Üyesi Zekeriya Tüfekci
Staj Projesi – Yaz 2025




PROJE VİDEOSU !!!!
https://www.youtube.com/watch?v=XMni3_jo8Xw




Özellikler:

Şifreli giriş sistemi (Personel ve Yönetici olarak iki rol)

Personelin talep oluşturabilmesi (başlık, açıklama ve kategori seçimi)

Taleplerin listelenebilmesi

Yöneticinin tüm talepleri görüntüleyebilmesi

Taleplere onay veya red verilmesi, ayrıca not eklenebilmesi

İstatistik ekranı: toplam talep, onaylı/retli talepler, en çok talep açan kullanıcı

SQLite veritabanı kullanımı (ilk çalıştırmada otomatik oluşturuluyor)

.NET CLI üzerinden çalışır

Nasıl Kurulur ve Çalıştırılır:

Bilgisayarında .NET SDK yüklü olmalıdır. (dotnet --version komutu ile kontrol edebilirsin)

Proje klasörüne gir:
Örnek: cd C:\Users\kamer\EBASurecTakip

Konsolda şu komutu çalıştır:
dotnet run

İlk çalıştırmada database.db otomatik oluşur. Varsayılan kullanıcılar da eklenir.

Varsayılan Kullanıcılar ve Şifreleri:

Personel:

ID: 1

Şifre: 1234

Yönetici:

ID: 2

Şifre: admin

Kullanım Talimatları:

Program açıldığında kullanıcı ID ve şifre girilir.
Doğru bilgiler girildiğinde aşağıdaki menüler görüntülenir:

Personel Menüsü:
1 - Talep Oluştur
2 - Taleplerimi Listele
0 - Çıkış

Talep oluştururken başlık, açıklama ve kategori (Yazılım, Donanım, Eğitim, Diğer) girilir.

Yönetici Menüsü:
1 - Tüm Talepleri Listele
2 - Talep Onayla/Reddet + Not
3 - İstatistikleri Göster
0 - Çıkış

Yönetici, talepleri tek tek seçip onaylayabilir veya reddedebilir. Ayrıca isteğe bağlı açıklama (not) eklenebilir.

Proje Dosyaları:

Program.cs → tüm iş mantığı burada

EBASurecTakip.csproj → proje yapılandırması

database.db → SQLite veritabanı (otomatik oluşur)

README.md → proje tanıtımı


