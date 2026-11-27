using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMH219_FlappyBird
{
    public partial class GameForm : Form
    {
        private Player bird;   // formun kuşu
        private List<Pipe> pipes;
        private GameState state = new GameState();    // skor, hız ve oyun durumu
        private HighScoreManager highScores = new HighScoreManager();
        private bool newRecord = false;               // son oyunda rekor kırıldı mı?
        private Random random = new Random();
        private const int GroundY = 550;              // zeminin başladığı Y
        private const int PipeSpacing = 220;          // iki engel arası mesafe

        public GameForm()
        {
            InitializeComponent();
            ClientSize = new Size(400, 600);
            DoubleBuffered = true;
            KeyPreview = true;

            // Yeni Oyun butonunu ekranın ortasına yerleştir
            // (boyut kodla verilir, böylece ekran ölçeklendirmesinde yazı kesilmez)
            newGameButton.Size = new Size(160, 44);
            newGameButton.Left = (ClientSize.Width - newGameButton.Width) / 2;
            newGameButton.Top = 370;

            state.LoadHighScore(highScores.Load());
            StartNewGame();
        }

        // Kuşu, engelleri ve oyun durumunu başlangıca döndürür
        private void StartNewGame()
        {
            bird = new Player(80, 250);

            pipes = new List<Pipe>();
            pipes.Add(new Pipe(400, RandomGapY(), GroundY));
            pipes.Add(new Pipe(400 + PipeSpacing, RandomGapY(), GroundY));

            state.Reset();
            newRecord = false;
            newGameButton.Visible = false;
            ActiveControl = null;   // klavye odağı forma geri dönsün
            gameTimer.Start();
            Invalidate();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Sadece oyun oynanırken hareket olur (Hazır / Duraklatıldı / Bitti durumlarında beklenir)
            if (!state.IsPlaying) return;

            bird.Update();   // konumu hesapla
            bird.ClampToTop();

            foreach (Pipe pipe in pipes)
            {
                pipe.Move(state.Speed);

                // Engelin sağ kenarı kuşun soluna geçtiyse engel başarıyla geçilmiştir
                if (!pipe.Passed && pipe.X + Pipe.Width < bird.X)
                {
                    pipe.Passed = true;
                    state.AddPoint();
                }

                if (pipe.IsOffScreen)
                {
                    // en sağdaki engelin arkasına geri gönder
                    float maxX = 0;
                    foreach (Pipe p in pipes)
                        if (p.X > maxX) maxX = p.X;
                    pipe.Reset(maxX + PipeSpacing, RandomGapY());
                }
            }

            CheckCollisions();
            Invalidate();    // yeniden çiz
        }

        // Kuş zemine veya bir engele çarptı mı?
        private void CheckCollisions()
        {
            Rectangle birdRect = bird.Bounds;

            if (birdRect.Bottom >= GroundY)
            {
                EndGame();
                return;
            }

            foreach (Pipe pipe in pipes)
            {
                if (pipe.Collides(birdRect))
                {
                    EndGame();
                    return;
                }
            }
        }

        // Oyunu durdurur ve Game Over ekranını gösterir
        private void EndGame()
        {
            state.End();
            gameTimer.Stop();

            if (state.UpdateHighScore())
            {
                newRecord = true;
                if (!highScores.Save(state.HighScore))
                {
                    MessageBox.Show("En yüksek skor dosyaya kaydedilemedi.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            newGameButton.Visible = true;
            Invalidate();
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.SaddleBrown, 0, GroundY, ClientSize.Width, 50);
            foreach (Pipe pipe in pipes)
                pipe.Draw(e.Graphics);
            bird.Draw(e.Graphics);
            DrawScore(e.Graphics);

            switch (state.Status)
            {
                case GameStatus.Ready:
                    DrawMessage(e.Graphics, "Hazır!",
                        "Başlamak için SPACE\n\nSPACE / Yukarı ok: zıpla\nP: duraklat   R: yeniden başlat\n\nEn yüksek skor: " + state.HighScore);
                    break;
                case GameStatus.Paused:
                    DrawMessage(e.Graphics, "DURAKLATILDI", "Devam etmek için P");
                    break;
                case GameStatus.GameOver:
                    string record = newRecord ? "\nYENİ REKOR!" : "";
                    DrawMessage(e.Graphics, "GAME OVER",
                        "Skor: " + state.Score + "   En yüksek: " + state.HighScore + record + "\n\nYeniden başlamak için R");
                    break;
            }
        }

        // Skoru ekranın üst ortasına gölgeli olarak yazar
        private void DrawScore(Graphics g)
        {
            using (Font font = new Font("Segoe UI", 28, FontStyle.Bold))
            using (StringFormat center = new StringFormat())
            {
                center.Alignment = StringAlignment.Center;
                string text = state.Score.ToString();
                g.DrawString(text, font, Brushes.Black, new RectangleF(2, 22, ClientSize.Width, 50), center);
                g.DrawString(text, font, Brushes.White, new RectangleF(0, 20, ClientSize.Width, 50), center);
            }

            // Sol üst köşede seviye bilgisi
            using (Font small = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                g.DrawString("Seviye " + state.Level, small, Brushes.White, 10, 10);
            }
        }

        // Yarı saydam bir perde üzerine başlık ve açıklama yazar
        private void DrawMessage(Graphics g, string title, string detail)
        {
            using (SolidBrush shade = new SolidBrush(Color.FromArgb(150, Color.Black)))
            {
                g.FillRectangle(shade, 0, 0, ClientSize.Width, ClientSize.Height);
            }

            using (Font titleFont = new Font("Segoe UI", 30, FontStyle.Bold))
            using (Font detailFont = new Font("Segoe UI", 13))
            using (StringFormat center = new StringFormat())
            {
                center.Alignment = StringAlignment.Center;
                g.DrawString(title, titleFont, Brushes.White, new RectangleF(0, 170, ClientSize.Width, 60), center);
                g.DrawString(detail, detailFont, Brushes.White, new RectangleF(0, 240, ClientSize.Width, 200), center);
            }
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Up)
            {
                if (state.Status == GameStatus.Ready)
                {
                    state.Start();
                }

                if (state.IsPlaying)
                {
                    bird.Jump();
                }

                // Space tuşunun görünür bir butona "tıklama" olarak gitmesini engelle
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.P)
            {
                state.TogglePause();
                Invalidate();
            }
            else if (e.KeyCode == Keys.R)
            {
                StartNewGame();
            }
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        // Boşluğu ne çok yukarıda ne çok aşağıda olacak şekilde rastgele seçer
        private int RandomGapY()
        {
            return random.Next(60, GroundY - Pipe.GapSize - 60);
        }
    }
}
