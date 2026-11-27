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
        private Random random = new Random();
        private float pipeSpeed = 3f;                 // engellerin hızı
        private const int GroundY = 550;              // zeminin başladığı Y
        private const int PipeSpacing = 220;          // iki engel arası mesafe
        private bool gameOver = false;                // oyun bitti mi?

        public GameForm()
        {
            InitializeComponent();
            ClientSize = new Size(400, 600);
            DoubleBuffered = true;
            KeyPreview = true;

            bird = new Player(80, 250);   // önce kuşu oluştur

            pipes = new List<Pipe>();
            pipes.Add(new Pipe(400, RandomGapY(), GroundY));
            pipes.Add(new Pipe(400 + PipeSpacing, RandomGapY(), GroundY));

            gameTimer.Start();            // sonra döngüyü başlat
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            bird.Update();   // konumu hesapla
            bird.ClampToTop();

            foreach (Pipe pipe in pipes)
            {
                pipe.Move(pipeSpeed);
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
            gameOver = true;
            gameTimer.Stop();
            Invalidate();
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.SaddleBrown, 0, GroundY, ClientSize.Width, 50);
            foreach (Pipe pipe in pipes)
                pipe.Draw(e.Graphics);
            bird.Draw(e.Graphics);

            if (gameOver)
            {
                DrawGameOver(e.Graphics);
            }
        }

        // Yarı saydam bir perde üzerine "GAME OVER" yazar
        private void DrawGameOver(Graphics g)
        {
            using (SolidBrush shade = new SolidBrush(Color.FromArgb(150, Color.Black)))
            {
                g.FillRectangle(shade, 0, 0, ClientSize.Width, ClientSize.Height);
            }

            using (Font font = new Font("Segoe UI", 32, FontStyle.Bold))
            using (StringFormat center = new StringFormat())
            {
                center.Alignment = StringAlignment.Center;
                center.LineAlignment = StringAlignment.Center;
                g.DrawString("GAME OVER", font, Brushes.White,
                    new RectangleF(0, 0, ClientSize.Width, ClientSize.Height - 100), center);
            }
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && !gameOver)
            {
                bird.Jump();
            }
        }

        // Boşluğu ne çok yukarıda ne çok aşağıda olacak şekilde rastgele seçer
        private int RandomGapY()
        {
            return random.Next(60, GroundY - Pipe.GapSize - 60);
        }
    }
}
