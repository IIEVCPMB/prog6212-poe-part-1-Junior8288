namespace ProgPart1.ClaimStore;
using System.Text.Json;
using ProgPart1.Models;

    public static class ClaimsDataStore
    {
        public static List<Claims> Claims { get; set; } = new List<Claims>();
        private static string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Claims.json");

        // Load claims from JSON on app start
        static ClaimsDataStore()
        {
            if (System.IO.File.Exists(FilePath))
            {
                var json = System.IO.File.ReadAllText(FilePath);
                Claims = JsonSerializer.Deserialize<List<Claims>>(json) ?? new List<Claims>();
            }
        }

        // Add a new claim
        public static void AddClaim(Claims claim)
        {
            Claims.Add(claim);
            SaveToFile();
        }

        // Update an existing claim
        public static void UpdateClaim(Claims updatedClaim)
        {
            for (int i = 0; i < Claims.Count; i++)
            {
                if (Claims[i].Id == updatedClaim.Id)
                {
                    Claims[i] = updatedClaim;
                    break;
                }
            }
            SaveToFile();
        }

        // Save all claims to JSON
        private static void SaveToFile()
        {
            var json = JsonSerializer.Serialize(Claims, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(FilePath, json);
        }
    }
