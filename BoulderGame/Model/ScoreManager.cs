using BoulderGame.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BoulderGame
{
    public static class ScoreManager
    {
        private const string FilePath = "scores.json";

        public static void SaveScore(PlayerRecord newRecord)
        {
            List<PlayerRecord> scores = LoadScores() ?? new List<PlayerRecord>();

            if (string.IsNullOrEmpty(newRecord.Username))
            {
                newRecord.Username = Session.CurrentUsername ?? "Guest";
            }

            var existingRecord = scores.FirstOrDefault(s => s.Username?.Trim().ToLower() == newRecord.Username.Trim().ToLower());

            if (existingRecord != null)
            {
                if (newRecord.Score > existingRecord.Score)
                {
                    existingRecord.Score = newRecord.Score;
                }
            }
            else
            {
                scores.Add(newRecord);
            }

            var sortedScores = scores.OrderByDescending(s => s.Score).Take(10).ToList();

            for (int i = 0; i < sortedScores.Count; i++)
            {
                sortedScores[i].Id = i + 1;
            }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                if (sortedScores.Count == 0)
                {
                    sortedScores.Add(newRecord);
                }

                string json = JsonSerializer.Serialize(sortedScores, options);
                File.WriteAllText(FilePath, json);

                System.Diagnostics.Debug.WriteLine($"RECORD SAVED: {json}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CRITICAL SAVE ERROR: {ex.Message}");
            }
        }

        public static int GetCurrentUserBestScore()
        {
            string currentUser = Session.CurrentUsername;
            
            return LoadScores()
                .Where(s => s.Username == currentUser)
                .Select(s => s.Score)
                .DefaultIfEmpty(0)
                .Max();
        }
        public static List<PlayerRecord> LoadScores()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new List<PlayerRecord>();
                }

                string json = File.ReadAllText(FilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<PlayerRecord>();
                }

                return JsonSerializer.Deserialize<List<PlayerRecord>>(json) ?? new List<PlayerRecord>();
            }
            catch(IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
                return new List<PlayerRecord>();
            }
        }

        public static bool IsSecretLevelUnlocked()
        {
            return GetCurrentUserBestScore() >= 1000;
        }
    }
}
