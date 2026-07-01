using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace OmniWorld
{
    public sealed class GameForm : Form
    {
        private readonly Timer timer;
        private readonly Stopwatch clock;
        private readonly InputState input;
        private readonly AudioManager audio;
        private readonly Game game;
        private readonly Renderer renderer;
        private double lastSeconds;

        public GameForm()
        {
            Text = "Omni World";
            ClientSize = new Size(960, 540);
            MinimumSize = new Size(960, 540);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;

            input = new InputState();
            audio = new AudioManager();
            game = new Game(audio);
            renderer = new Renderer();

            clock = Stopwatch.StartNew();
            lastSeconds = clock.Elapsed.TotalSeconds;

            timer = new Timer();
            timer.Interval = 16;
            timer.Tick += OnTick;
            timer.Start();

            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
        }

        private void OnTick(object sender, EventArgs e)
        {
            double now = clock.Elapsed.TotalSeconds;
            float dt = (float)(now - lastSeconds);
            lastSeconds = now;

            game.Update(input, dt);
            input.EndFrame();
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            input.OnKeyDown(e.KeyCode);
            if (IsGameKey(e.KeyCode)) e.Handled = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            input.OnKeyUp(e.KeyCode);
            if (IsGameKey(e.KeyCode)) e.Handled = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            renderer.Render(e.Graphics, game, ClientSize);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            audio.StopMusic();
            base.OnFormClosed(e);
        }

        private static bool IsGameKey(Keys key)
        {
            return key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down ||
                   key == Keys.A || key == Keys.D || key == Keys.W || key == Keys.S ||
                   key == Keys.Space || key == Keys.Z || key == Keys.X || key == Keys.C ||
                   key == Keys.ControlKey || key == Keys.LControlKey || key == Keys.RControlKey ||
                   key == Keys.Enter ||
                   key == Keys.Escape || key == Keys.P || key == Keys.R || key == Keys.M;
        }
    }
}
