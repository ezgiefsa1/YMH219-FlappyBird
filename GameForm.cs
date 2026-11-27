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

        public GameForm()
        {
            InitializeComponent();
            ClientSize = new Size(400, 600);
            DoubleBuffered = true;
            KeyPreview = true;

            bird = new Player(80, 250);   // önce kuşu oluştur
            gameTimer.Start();            // sonra döngüyü başlat
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            bird.Update();   // konumu hesapla
            Invalidate();    // yeniden çiz
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.SaddleBrown, 0, 550, ClientSize.Width, 50);
            bird.Draw(e.Graphics);
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                bird.Jump();
            }
        }
    }
}
