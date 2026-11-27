using System.Drawing;

namespace YMH219_FlappyBird
{
    public class Player
    {
        // --- Özellikler: dışarıdan okunur, sadece Player değiştirir (kapsülleme) ---
        public float X { get; private set; }
        public float Y { get; private set; }
        public float VelocityY { get; private set; }   // dikey hız (+ aşağı, - yukarı)
        public int Width { get; private set; }
        public int Height { get; private set; }

        // --- Sabitler: oyunun fizik ayarları ---
        private const float Gravity = 0.5f;   // her tikte hıza eklenen çekim
        private const float JumpForce = 8f;   // zıplayınca verilen yukarı hız

        public Player(float x, float y)
        {
            X = x;
            Y = y;
            Width = 34;
            Height = 26;
            VelocityY = 0;
        }

        // Her timer tikinde çağrılır: yer çekimi hızı artırır, hız konumu değiştirir
        public void Update()
        {
            VelocityY += Gravity;
            Y += VelocityY;
        }

        // Ekranda yukarı = eksi Y, bu yüzden hızı eksi yapıyoruz
        public void Jump()
        {
            VelocityY = -JumpForce;
        }

        // Çarpışma kontrolünde kullanılacak dikdörtgen
        public Rectangle Bounds
        {
            get { return new Rectangle((int)X, (int)Y, Width, Height); }
        }

        // Kuşu kendi stilimizle çizer (yuvarlak mor gövde, kanat, göz, gaga)
        public void Draw(Graphics g)
        {
            // Gövde
            g.FillEllipse(Brushes.MediumPurple, X, Y, Width, Height);
            g.DrawEllipse(Pens.Indigo, X, Y, Width, Height);

            // Kanat
            g.FillEllipse(Brushes.Plum, X + 4, Y + 11, 14, 9);

            // Göz (beyaz + siyah bebek)
            g.FillEllipse(Brushes.White, X + 20, Y + 4, 10, 10);
            g.FillEllipse(Brushes.Black, X + 25, Y + 7, 4, 4);

            // Gaga
            PointF[] beak =
            {
                new PointF(X + Width - 2, Y + 12),
                new PointF(X + Width + 8, Y + 15),
                new PointF(X + Width - 2, Y + 18)
            };
            g.FillPolygon(Brushes.Gold, beak);
        }
    }
}