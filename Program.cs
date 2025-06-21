#nullable disable        //  null-warning’leri sustur
using System;
using Microsoft.Data.Sqlite;

class Program
{
    private const string Conn = "Data Source=database.db";
    private static readonly string[] Types = { "Yazılım", "Donanım", "Eğitim", "Diğer" };

    static void Main()
    {
        InitDb();

        int uid = Login();
        var (name, role) = GetUser(uid);

        Console.WriteLine($"\nHoş geldin {name}! Rol: {role}\n");

        if (role == "Personel")
            PersonelMenu(uid);
        else
            YoneticiMenu();
    }

    // ---------- DB Kur ----------
    static void InitDb()
    {
        using var con = new SqliteConnection(Conn);
        con.Open();
        var c = con.CreateCommand();

        // Users (şifre alanı dahil)
        c.CommandText = @"
CREATE TABLE IF NOT EXISTS Users(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Password TEXT NOT NULL,
    Role TEXT NOT NULL
);";
        c.ExecuteNonQuery();

        // Requests (tip ve not alanı dahil)
        c.CommandText = @"
CREATE TABLE IF NOT EXISTS Requests(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Title TEXT NOT NULL,
    Description TEXT,
    Type TEXT NOT NULL,
    Date TEXT NOT NULL,
    Status TEXT NOT NULL,
    Note TEXT,
    FOREIGN KEY(UserId) REFERENCES Users(Id)
);";
        c.ExecuteNonQuery();

        // Varsayılan kullanıcılar
        c.CommandText = "SELECT COUNT(*) FROM Users;";
        if ((long)c.ExecuteScalar() == 0)
        {
            c.CommandText = @"
INSERT INTO Users (Name, Password, Role) VALUES
('Seymen Bugay', '1234', 'Personel'),
('Zekeriya Tüfekci', 'admin', 'Yonetici');";
            c.ExecuteNonQuery();
            Console.WriteLine("→ Varsayılan kullanıcılar eklendi (şifreler: 1234 / admin)");
        }
    }

    // ---------- LOGIN ----------
    static int Login()
    {
        while (true)
        {
            ListUsers();
            Console.Write("ID gir: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Geçersiz.\n"); continue; }

            Console.Write("Şifre gir: ");
            string pass = Console.ReadLine();

            using var con = new SqliteConnection(Conn);
            con.Open();
            var c = con.CreateCommand();
            c.CommandText = "SELECT COUNT(*) FROM Users WHERE Id=@i AND Password=@p;";
            c.Parameters.AddWithValue("@i", id);
            c.Parameters.AddWithValue("@p", pass);

            if ((long)c.ExecuteScalar() == 1) return id;

            Console.WriteLine("❌ Hatalı ID veya şifre!\n");
        }
    }

    static void ListUsers()
    {
        using var con = new SqliteConnection(Conn);
        con.Open();
        var c = con.CreateCommand();
        c.CommandText = "SELECT Id, Name, Role FROM Users;";
        using var r = c.ExecuteReader();
        Console.WriteLine("\n--- KULLANICILAR ---");
        while (r.Read())
            Console.WriteLine($"{r.GetInt32(0)} | {r.GetString(1)} ({r.GetString(2)})");
    }

    static (string, string) GetUser(int id)
    {
        using var con = new SqliteConnection(Conn);
        con.Open();
        var c = con.CreateCommand();
        c.CommandText = "SELECT Name, Role FROM Users WHERE Id=@i;";
        c.Parameters.AddWithValue("@i", id);
        using var r = c.ExecuteReader();
        r.Read();
        return (r.GetString(0), r.GetString(1));
    }

    // ---------- PERSONEL ----------
    static void PersonelMenu(int uid)
    {
        while (true)
        {
            Console.WriteLine("\n--- PERSONEL MENÜ ---");
            Console.WriteLine("1) Talep Oluştur");
            Console.WriteLine("2) Taleplerimi Listele");
            Console.WriteLine("0) Çıkış");
            Console.Write("Seçim: ");
            string ch = Console.ReadLine();

            if (ch == "1") CreateRequest(uid);
            else if (ch == "2") ListRequests(uid);
            else if (ch == "0") return;
        }
    }

    static void CreateRequest(int uid)
    {
        Console.Write("Başlık: "); string title = Console.ReadLine();
        Console.Write("Açıklama: "); string desc = Console.ReadLine();

        Console.WriteLine("Talep Tipi Seç:");
        for (int i = 0; i < Types.Length; i++) Console.WriteLine($"{i + 1}) {Types[i]}");
        int tix = int.Parse(Console.ReadLine()) - 1;

        using var con = new SqliteConnection(Conn);
        con.Open();
        var c = con.CreateCommand();
        c.CommandText = @"INSERT INTO Requests
(UserId, Title, Description, Type, Date, Status) VALUES
(@u,@t,@d,@ty,@dt,'Pending');";
        c.Parameters.AddWithValue("@u", uid);
        c.Parameters.AddWithValue("@t", title);
        c.Parameters.AddWithValue("@d", desc);
        c.Parameters.AddWithValue("@ty", Types[tix]);
        c.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd"));
        c.ExecuteNonQuery();

        Console.WriteLine("✅ Talep eklendi.");
    }

    // ---------- YÖNETİCİ ----------
    static void YoneticiMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- YÖNETİCİ MENÜ ---");
            Console.WriteLine("1) Tüm Talepleri Listele");
            Console.WriteLine("2) Talep Onayla/Reddet + Not");
            Console.WriteLine("3) İstatistikleri Göster");
            Console.WriteLine("0) Çıkış");
            Console.Write("Seçim: ");
            string ch = Console.ReadLine();

            if (ch == "1") ListRequests();
            else if (ch == "2") UpdateRequest();
            else if (ch == "3") ShowStats();
            else if (ch == "0") return;
        }
    }

    // ---------- TALEP LİSTE ----------
    static void ListRequests(int uid = -1)
    {
        using var con = new SqliteConnection(Conn);
        con.Open();
        var c = con.CreateCommand();

        c.CommandText = uid == -1 ?
            @"SELECT r.Id,u.Name,r.Title,r.Type,r.Date,r.Status,r.Note
              FROM Requests r JOIN Users u ON u.Id=r.UserId;"
            :
            @"SELECT Id,Title,Type,Date,Status,Note
              FROM Requests WHERE UserId=@u;";
        if (uid != -1) c.Parameters.AddWithValue("@u", uid);

        using var r = c.ExecuteReader();
        Console.WriteLine("\nID | Kullanıcı | Başlık | Tip | Tarih | Durum | Not");
        while (r.Read())
        {
            int idx = 0;
            int id = r.GetInt32(idx++);
            string user = uid == -1 ? r.GetString(idx++) : "-";
            string title = r.GetString(idx++);
            string type = r.GetString(idx++);
            string date = r.GetString(idx++);
            string status = r.GetString(idx++);
            string note = !r.IsDBNull(idx) ? r.GetString(idx) : "";
            Console.WriteLine($"{id} | {user} | {title} | {type} | {date} | {status} | {note}");
        }
    }

    // ---------- ONAY/RED + NOT ----------
    static void UpdateRequest()
    {
        Console.Write("Talep ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Geçersiz."); return; }

        Console.Write("Durum (Approved/Rejected): ");
        string status = Console.ReadLine();
        if (status != "Approved" && status != "Rejected") { Console.WriteLine("Hata."); return; }

        Console.Write("Not: ");
        string note = Console.ReadLine();

        using var con = new SqliteConnection(Conn);
        con.Open();
        var c = con.CreateCommand();
        c.CommandText = "UPDATE Requests SET Status=@s, Note=@n WHERE Id=@id;";
        c.Parameters.AddWithValue("@s", status);
        c.Parameters.AddWithValue("@n", note);
        c.Parameters.AddWithValue("@id", id);

        Console.WriteLine(c.ExecuteNonQuery() > 0 ? "✅ Güncellendi." : "❌ Bulunamadı.");
    }

    // ---------- İSTATİSTİK ----------
    static void ShowStats()
    {
        using var con = new SqliteConnection(Conn);
        con.Open();
        var c = con.CreateCommand();

        c.CommandText = "SELECT COUNT(*) FROM Requests;";       long total = (long)c.ExecuteScalar();
        c.CommandText = "SELECT COUNT(*) FROM Requests WHERE Status='Approved';"; long ok = (long)c.ExecuteScalar();
        c.CommandText = "SELECT COUNT(*) FROM Requests WHERE Status='Rejected';"; long no = (long)c.ExecuteScalar();
        c.CommandText = @"SELECT u.Name, COUNT(*) cnt FROM Requests r
                          JOIN Users u ON u.Id=r.UserId
                          GROUP BY u.Name ORDER BY cnt DESC LIMIT 1;";
        string topUser = (string?)c.ExecuteScalar() ?? "Yok";

        Console.WriteLine($"\n--- İSTATİSTİK ---");
        Console.WriteLine($"Toplam Talep : {total}");
        Console.WriteLine($"Onaylı       : {ok}");
        Console.WriteLine($"Reddedilen   : {no}");
        Console.WriteLine($"En Çok Talep : {topUser}");
    }
}
