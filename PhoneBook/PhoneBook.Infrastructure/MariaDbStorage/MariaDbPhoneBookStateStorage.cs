using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.State;
using PhoneBook.Core.Storage;

namespace PhoneBook.Infrastructure.MariaDbStorage;

public sealed class MariaDbPhoneBookStateStorage: IPhoneBookStateStorage
{
     private readonly MariaDbStorageOptions _options;
    private readonly ILogger<MariaDbPhoneBookStateStorage> _logger;

    public MariaDbPhoneBookStateStorage(
        IOptions<MariaDbStorageOptions> options,
        ILogger<MariaDbPhoneBookStateStorage> logger)
    {
        _options = options.Value;
        _logger = logger;
        _logger.LogInformation("MariaDb connection string configured: {HasValue}", !string.IsNullOrWhiteSpace(_options.ConnectionString));

    }

    public PhoneBookState Load()
    {
        try
        {
            using var conn = new MySqlConnection(_options.ConnectionString);
            conn.Open();

            const string sql = @"
SELECT
  PhoneE164, PhoneRaw, FirstName, LastName, Email, Pronouns, Ringtone, Birthday, Notes
FROM contacts
ORDER BY FirstName, LastName, PhoneE164;";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var state = new PhoneBookState();

            while (reader.Read())
            {
                var phoneE164 = reader.GetString("PhoneE164");
                var phoneRaw = reader.GetString("PhoneRaw");

                var firstName = reader.GetString("FirstName");
                var lastName = ReadNullableString(reader, "LastName");
                var email = ReadNullableString(reader, "Email");
                var pronouns = ReadNullableString(reader, "Pronouns");
                var ringtoneRaw = ReadNullableString(reader, "Ringtone");
                var notes = ReadNullableString(reader, "Notes");

                DateOnly? birthday = null;
                if (!reader.IsDBNull(reader.GetOrdinal("Birthday")))
                {
                    birthday = DateOnly.FromDateTime(reader.GetDateTime("Birthday"));
                }
                
                var phone = PhoneNumber.FromE164(phoneE164, phoneRaw);

                var ringtone = ParseRingtoneOrDefault(ringtoneRaw);

                var contact = new Contact(
                    phoneNumber: phone,
                    firstName: firstName,
                    lastName: lastName,
                    email: email,
                    pronouns: pronouns,
                    ringtone: ringtone,
                    birthday: birthday,
                    notes: notes
                );

                state.Contacts.Add(contact);
            }

            return state;
        }
        catch (Exception ex) when (ex is MySqlException or InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to load phone book state from MariaDB.");
            throw new StorageException("Could not load the phone book from the database.", ex);
        }
    }

    public void Save(PhoneBookState state)
    {
        try
        {
            using var conn = new MySqlConnection(_options.ConnectionString);
            conn.Open();

            using var tx = conn.BeginTransaction();

            using (var deleteCmd = new MySqlCommand("DELETE FROM contacts;", conn, tx))
            {
                deleteCmd.ExecuteNonQuery();
            }

            const string insertSql = @"
INSERT INTO contacts
(PhoneE164, PhoneRaw, FirstName, LastName, Email, Pronouns, Ringtone, Birthday, Notes, CreatedAtUtc, UpdatedAtUtc)
VALUES
(@PhoneE164, @PhoneRaw, @FirstName, @LastName, @Email, @Pronouns, @Ringtone, @Birthday, @Notes, @CreatedAtUtc, @UpdatedAtUtc);";

            var now = DateTime.UtcNow;

            foreach (var c in state.Contacts)
            {
                using var cmd = new MySqlCommand(insertSql, conn, tx);

                cmd.Parameters.AddWithValue("@PhoneE164", c.PhoneNumber.E164);
                cmd.Parameters.AddWithValue("@PhoneRaw", c.PhoneNumber.Raw);

                cmd.Parameters.AddWithValue("@FirstName", c.FirstName);
                cmd.Parameters.AddWithValue("@LastName", (object?)c.LastName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)c.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Pronouns", (object?)c.Pronouns ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Ringtone", c.Ringtone.ToString());
                
                cmd.Parameters.AddWithValue("@Birthday",
                    c.Birthday is null
                        ? DBNull.Value
                        : c.Birthday.Value.ToDateTime(TimeOnly.MinValue));

                cmd.Parameters.AddWithValue("@Notes", (object?)c.Notes ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CreatedAtUtc", now);
                cmd.Parameters.AddWithValue("@UpdatedAtUtc", now);

                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }
        catch (Exception ex) when (ex is MySqlException or InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to save phone book state to MariaDB.");
            throw new StorageException("Could not save the phone book to the database.", ex);
        }
    }

    private static string? ReadNullableString(MySqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static Ringtone ParseRingtoneOrDefault(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Ringtone.Default;
        }

        return Enum.TryParse<Ringtone>(value, ignoreCase: true, out var parsed)
            ? parsed
            : Ringtone.Default;
    }

}