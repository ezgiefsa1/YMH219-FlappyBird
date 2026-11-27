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

        public int Score { get; private set; }
        public float Speed { get; private set; }
        public GameStatus Status { get; private set; }

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

        // Bir engel geçildiğinde çağrılır
        public void AddPoint()
        {
            Score++;
        }
    }
}
