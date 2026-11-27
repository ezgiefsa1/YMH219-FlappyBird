using System;

namespace YMH219_FlappyBird
{
    /// <summary>
    /// Oyunun içinde bulunabileceği durumlar.
    /// </summary>
    public enum GameStatus
    {
        Ready,      // oyun başlamayı bekliyor
        Playing,    // oyun oynanıyor
        Paused,     // oyun duraklatıldı
        GameOver    // kuş çarptı, oyun bitti
    }

    /// <summary>
    /// Oyunun skor, hız ve durum bilgilerini tek bir yerde tutar.
    /// Form bu bilgileri doğrudan değiştirmez, bu sınıfın metotlarını çağırır.
    /// </summary>
    public class GameState
    {
        public const float StartSpeed = 3f;
        public const float MaxSpeed = 7f;
        public const int PointsPerLevel = 5;       // her 5 puanda bir seviye atlanır

        public int Score { get; private set; }
        public float Speed { get; private set; }
        public GameStatus Status { get; private set; }
        public int HighScore { get; private set; }

        // Seviye skordan hesaplanır: 0-4 puan = 1. seviye, 5-9 = 2. seviye ...
        public int Level
        {
            get { return Score / PointsPerLevel + 1; }
        }

        public bool IsPlaying
        {
            get { return Status == GameStatus.Playing; }
        }

        public GameState()
        {
            Reset();
        }

        // Yeni oyun için tüm değerleri başlangıca döndürür
        public void Reset()
        {
            Score = 0;
            Speed = StartSpeed;
            Status = GameStatus.Ready;
        }

        public void Start()
        {
            if (Status == GameStatus.Ready)
                Status = GameStatus.Playing;
        }

        // Oynarken duraklatır, duraklatılmışken devam ettirir
        public void TogglePause()
        {
            if (Status == GameStatus.Playing)
                Status = GameStatus.Paused;
            else if (Status == GameStatus.Paused)
                Status = GameStatus.Playing;
        }

        public void End()
        {
            Status = GameStatus.GameOver;
        }

        // Bir engel geçildiğinde çağrılır; seviye arttıkça hız da artar
        public void AddPoint()
        {
            Score++;
            Speed = Math.Min(MaxSpeed, StartSpeed + (Level - 1) * 0.5f);
        }

        // Dosyadan okunan en yüksek skoru yükler
        public void LoadHighScore(int value)
        {
            HighScore = value;
        }

        // Skor rekoru geçtiyse en yüksek skoru günceller ve true döner
        public bool UpdateHighScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                return true;
            }
            return false;
        }
    }
}
