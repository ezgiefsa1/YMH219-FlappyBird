using System;
using System.IO;

namespace YMH219_FlappyBird
{
    /// <summary>
    /// En yüksek skoru bir metin dosyasında saklar ve geri okur.
    /// Dosya: %AppData%\YMH219-FlappyBird\highscore.txt
    /// </summary>
    public class HighScoreManager
    {
        private readonly string filePath;

        public HighScoreManager()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "YMH219-FlappyBird");
            filePath = Path.Combine(folder, "highscore.txt");
        }

        // Dosyadan en yüksek skoru okur. Dosya yoksa veya bozuksa 0 döner.
        public int Load()
        {
            try
            {
                if (!File.Exists(filePath))
                    return 0;

                int value;
                string text = File.ReadAllText(filePath).Trim();
                return int.TryParse(text, out value) && value >= 0 ? value : 0;
            }
            catch (IOException)
            {
                return 0;
            }
            catch (UnauthorizedAccessException)
            {
                return 0;
            }
        }

        // En yüksek skoru dosyaya yazar. Başarılı olursa true döner.
        public bool Save(int score)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, score.ToString());
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
