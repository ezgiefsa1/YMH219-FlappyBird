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
            bird.Update();   // konumu hesapla

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

            Invalidate();    // yeniden çiz
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.SaddleBrown, 0, GroundY, ClientSize.Width, 50);
            foreach (Pipe pipe in pipes)
                pipe.Draw(e.Graphics);
            bird.Draw(e.Graphics);
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
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
