using System.Drawing;

namespace YMH219_FlappyBird
{
    /// <summary>
    /// Bir engel çiftini (üst ve alt sütun) temsil eder.
    /// Kuş, iki sütun arasındaki boşluktan geçmeye çalışır.
    /// </summary>
    public class Pipe
    {
        public float X { get; private set; }
        public int GapY { get; private set; }      // boşluğun üst kenarı
        public bool Passed { get; set; }           // kuş bu engeli geçti mi (skor için)

        public const int Width = 60;
        public const int GapSize = 150;            // kuşun geçeceği boşluk
        private readonly int groundY;              // alt sütun zemine kadar uzanır

        public Pipe(float x, int gapY, int groundY)
        {
            this.groundY = groundY;
            Reset(x, gapY);
        }

        // Engeli sola kaydırır
        public void Move(float speed)
        {
            X -= speed;
        }

        // Engel tamamen ekranın solundan çıktı mı?
        public bool IsOffScreen
        {
            get { return X + Width < 0; }
        }

        // Engeli yeni bir konuma taşır (silip yeniden oluşturmak yerine yeniden kullanım)
        public void Reset(float x, int gapY)
        {
            X = x;
            GapY = gapY;
            Passed = false;
        }

        public Rectangle TopRect
        {
            get { return new Rectangle((int)X, 0, Width, GapY); }
        }

        public Rectangle BottomRect
        {
            get
            {
                int top = GapY + GapSize;
                return new Rectangle((int)X, top, Width, groundY - top);
            }
        }

        // Verilen dikdörtgen (kuş) üst veya alt sütuna çarpıyor mu?
        public bool Collides(Rectangle other)
        {
            return other.IntersectsWith(TopRect) || other.IntersectsWith(BottomRect);
        }

        // Kendi stilimiz: gri taş sütunlar
        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.SlateGray, TopRect);
            g.FillRectangle(Brushes.SlateGray, BottomRect);
            g.DrawRectangle(Pens.DarkSlateGray, TopRect);
            g.DrawRectangle(Pens.DarkSlateGray, BottomRect);
        }
    }
}
