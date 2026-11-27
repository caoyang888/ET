using System;
using System.IO;
using System.Text.Json;

namespace ET.Slot.Postgres
{
    public static class SlotPostgresSettings
    {
        public static string Resolve(string root)
        {
            string env = Environment.GetEnvironmentVariable("SLOT_POSTGRES_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(env))
            {
                return env;
            }

            string url = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrWhiteSpace(url))
            {
                return FromUrl(url);
            }

            string path = Path.Combine(root ?? ".", "Config", "postgres.json");
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.TryGetProperty("ConnectionString", out JsonElement value))
            {
                return value.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        public static string FromUrl(string url)
        {
            if (!url.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            Uri uri = new Uri(url);
            string user = uri.UserInfo;
            string password = string.Empty;
            int colon = user.IndexOf(':');
            if (colon >= 0)
            {
                password = Uri.UnescapeDataString(user.Substring(colon + 1));
                user = Uri.UnescapeDataString(user.Substring(0, colon));
            }

            string database = uri.AbsolutePath.Trim('/');
            return "Host=" + uri.Host + ";Port=" + uri.Port + ";Username=" + user + ";Password=" + password + ";Database=" + database;
        }
    }
}
