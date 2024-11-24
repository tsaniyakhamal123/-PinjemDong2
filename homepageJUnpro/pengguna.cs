using Npgsql;

public class Pengguna
{
    public int UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    public Pengguna(int userId, string pass, string username)
    {
        UserId = userId;
        Password = pass;
        Username = username;
    }

    // Generic Login logic (using BCrypt for password hashing)
    public virtual bool Login(string enteredPassword)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, Password);
    }

    public void Logout() { /* Implement logout method */ }

    public virtual void SignUp() { }
    public virtual void TambahBarang() { }
    public void ViewBarang() { }
    public virtual void EditBarang() { }
    public virtual void HapusBarang() { }
}
public class Admin : Pengguna
{
    public Admin(int userId, string pass, string username) : base(userId, pass, username) { }

    public override void SignUp()
    {
        throw new InvalidOperationException("Admin cannot sign up. Use the provided username and password.");
    }

    public override void EditBarang() { /* Method to update barang */ }
    public override void HapusBarang() { /* Method to delete barang */ }
}
public class Pemilik : Pengguna
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public Pemilik(int userId, string pass, string username, string name, string email) : base(userId, pass, username)
    {
        Name = name;
        Email = email;
    }

    public void InsertPemilik(NpgsqlConnection conn)
    {
        string query = "INSERT INTO pemilik (user_id, name, email) VALUES (@userId, @name, @email)";
        using (var cmd = new NpgsqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@userId", UserId);
            cmd.Parameters.AddWithValue("@name", Name);
            cmd.Parameters.AddWithValue("@email", Email);
            cmd.ExecuteNonQuery();
        }
    }

    public override void TambahBarang() { /* Method to add barang */ }
    public override void EditBarang() { /* Method to update barang */ }
    public override void HapusBarang() { /* Method to delete barang */ }
}
public class Penyewa : Pengguna
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public Penyewa(int userId, string pass, string username, string name, string email) : base(userId, pass, username)
    {
        Name = name;
        Email = email;
    }

    public void Checkout() { /* Implement checkout method */ }
}
public class LoginService
{
    private string connstring = "Host=your_host;Port=5432;Username=postgres;Password=your_password;Database=your_database";

    public Pengguna AuthenticateUser(string username, string password)
    {
        string query = "SELECT password, user_id, username FROM public.admin WHERE username = @username"; // Modify for other types

        using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
        {
            conn.Open();

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHashedPassword = reader["password"]?.ToString() ?? string.Empty;
                        int userId = (int)reader["user_id"];

                        // Assuming you return an Admin for now, but you can modify this to check for different user types
                        if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                        {
                            return new Admin(userId, storedHashedPassword, username);
                        }
                    }
                }
            }
        }

        return null;  // Return null if no match found
    }
}
