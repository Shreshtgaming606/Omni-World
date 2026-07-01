using System.Drawing;
using System.Drawing.Drawing2D;

namespace OmniWorld
{
    public sealed class UiRenderer
    {
        private readonly Font titleFont;
        private readonly Font headerFont;
        private readonly Font bodyFont;
        private readonly Font tinyFont;

        public UiRenderer()
        {
            titleFont = new Font("Segoe UI", 34f, FontStyle.Bold);
            headerFont = new Font("Segoe UI", 18f, FontStyle.Bold);
            bodyFont = new Font("Segoe UI", 12f, FontStyle.Bold);
            tinyFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        }

        public void DrawHud(Graphics g, Size viewport, Game game)
        {
            Player p = game.Player;

            using (SolidBrush panel = new SolidBrush(Color.FromArgb(205, 20, 30, 48)))
            using (SolidBrush text = new SolidBrush(Color.White))
            using (SolidBrush accent = new SolidBrush(Color.FromArgb(64, 236, 210)))
            using (Pen edge = new Pen(Color.FromArgb(110, 255, 255, 255)))
            {
                g.FillRectangle(panel, 12, 12, viewport.Width - 24, 48);
                g.DrawRectangle(edge, 12, 12, viewport.Width - 24, 48);
                g.DrawString("OMNI WORLD", bodyFont, accent, 24, 17);
                g.DrawString("Course " + game.CurrentLevel.CourseNumber + ": " + game.CurrentLevel.Name, bodyFont, text, 156, 17);
                g.DrawString("Lives " + p.Lives, bodyFont, text, 455, 17);
                g.DrawString("Health", bodyFont, text, 545, 17);
                DrawHealthPips(g, p.Health, p.MaxHealth, 612, 24);
                g.DrawString("Orbs " + p.Orbs, bodyFont, text, 695, 17);
                g.DrawString("Score " + p.Score, bodyFont, text, viewport.Width - 150, 17);
                DrawBurstMeter(g, p, viewport.Width - 198, 64);

                if (p.Powered)
                {
                    g.DrawString("FLUX " + ((int)p.PowerTimer + 1), bodyFont, accent, viewport.Width - 128, 83);
                }

                if (game.AirStompChain > 1 && game.ComboPopupTimer > 0f)
                {
                    int alpha = GameMath.Clamp((int)(game.ComboPopupTimer * 230f), 0, 230);
                    using (SolidBrush combo = new SolidBrush(Color.FromArgb(alpha, 255, 247, 91)))
                    {
                        g.DrawString("COMBO x" + game.AirStompChain, headerFont, combo, viewport.Width / 2 - 62, 68);
                    }
                }
            }
        }

        public void DrawCourseSelect(Graphics g, Size viewport, Game game)
        {
            using (LinearGradientBrush bg = new LinearGradientBrush(new Rectangle(0, 0, viewport.Width, viewport.Height), Color.FromArgb(38, 82, 120), Color.FromArgb(102, 205, 183), LinearGradientMode.Vertical))
            {
                g.FillRectangle(bg, 0, 0, viewport.Width, viewport.Height);
            }

            DrawDecorativeGrid(g, viewport);

            using (SolidBrush text = new SolidBrush(Color.White))
            using (SolidBrush muted = new SolidBrush(Color.FromArgb(220, 232, 249, 255)))
            using (SolidBrush selected = new SolidBrush(Color.FromArgb(235, 20, 34, 54)))
            using (SolidBrush complete = new SolidBrush(Color.FromArgb(255, 247, 91)))
            using (Pen edge = new Pen(Color.FromArgb(255, 255, 255), 2f))
            {
                g.DrawString("Omni World", titleFont, text, 54, 24);
                g.DrawString("Core-Man Course Select", headerFont, muted, 60, 80);

                for (int i = 0; i < game.CourseNames.Length; i++)
                {
                    Rectangle rect = new Rectangle(76, 118 + i * 74, viewport.Width - 152, 60);
                    bool isSelected = i == game.SelectedCourseIndex;

                    if (isSelected)
                    {
                        g.FillRectangle(selected, rect);
                        g.DrawRectangle(edge, rect);
                    }
                    else
                    {
                        using (SolidBrush normal = new SolidBrush(Color.FromArgb(130, 12, 30, 52)))
                        {
                            g.FillRectangle(normal, rect);
                        }
                    }

                    DrawCoursePreview(g, i + 1, new Rectangle(rect.X + 14, rect.Y + 8, 96, 44));

                    g.DrawString("Course " + (i + 1) + "  " + game.CourseNames[i], bodyFont, text, rect.X + 128, rect.Y + 7);
                    g.DrawString(game.CourseTaglines[i], tinyFont, muted, rect.X + 130, rect.Y + 34);
                    if (game.CourseCompleted[i])
                    {
                        g.DrawString("CLEAR", bodyFont, complete, rect.Right - 92, rect.Y + 20);
                    }
                }

                g.DrawString("Up/Down choose    Enter start", bodyFont, text, 88, viewport.Height - 82);
            }
        }

        public void DrawPause(Graphics g, Size viewport, Game game)
        {
            DrawOverlay(g, viewport);
            using (SolidBrush text = new SolidBrush(Color.White))
            using (SolidBrush accent = new SolidBrush(Color.FromArgb(64, 236, 210)))
            {
                Center(g, "Paused", titleFont, text, viewport.Width, 170);
                Center(g, "Enter or Esc to resume", bodyFont, accent, viewport.Width, 246);
                Center(g, "R restart course    M course select", bodyFont, text, viewport.Width, 286);
            }
        }

        public void DrawCourseComplete(Graphics g, Size viewport, Game game)
        {
            DrawOverlay(g, viewport);
            using (SolidBrush text = new SolidBrush(Color.White))
            using (SolidBrush accent = new SolidBrush(Color.FromArgb(255, 247, 91)))
            {
                Center(g, "Course Clear!", titleFont, accent, viewport.Width, 170);
                Center(g, game.CurrentLevel.Name + " complete", headerFont, text, viewport.Width, 242);
                Center(g, "Enter returns to course select", bodyFont, text, viewport.Width, 296);
            }
        }

        public void DrawGameOver(Graphics g, Size viewport, Game game)
        {
            DrawOverlay(g, viewport);
            using (SolidBrush text = new SolidBrush(Color.White))
            using (SolidBrush accent = new SolidBrush(Color.FromArgb(255, 95, 126)))
            {
                Center(g, "Game Over", titleFont, accent, viewport.Width, 178);
                Center(g, "R retry course    Enter course select", bodyFont, text, viewport.Width, 264);
            }
        }

        public void DrawVictory(Graphics g, Size viewport, Game game)
        {
            DrawOverlay(g, viewport);
            using (SolidBrush text = new SolidBrush(Color.White))
            using (SolidBrush accent = new SolidBrush(Color.FromArgb(64, 236, 210)))
            {
                Center(g, "Victory!", titleFont, accent, viewport.Width, 154);
                Center(g, "Core-Man cleared every first-version course.", headerFont, text, viewport.Width, 232);
                Center(g, "Enter returns to course select", bodyFont, text, viewport.Width, 298);
            }
        }

        private void DrawOverlay(Graphics g, Size viewport)
        {
            using (SolidBrush shade = new SolidBrush(Color.FromArgb(195, 8, 13, 27)))
            {
                g.FillRectangle(shade, 0, 0, viewport.Width, viewport.Height);
            }

            using (Pen line = new Pen(Color.FromArgb(80, 64, 236, 210), 2f))
            {
                g.DrawLine(line, 0, viewport.Height / 2 - 76, viewport.Width, viewport.Height / 2 - 126);
                g.DrawLine(line, 0, viewport.Height / 2 + 118, viewport.Width, viewport.Height / 2 + 72);
            }
        }

        private void DrawDecorativeGrid(Graphics g, Size viewport)
        {
            using (Pen line = new Pen(Color.FromArgb(38, 255, 255, 255), 1f))
            {
                for (int x = 0; x < viewport.Width; x += 32)
                {
                    g.DrawLine(line, x, 0, x, viewport.Height);
                }

                for (int y = 0; y < viewport.Height; y += 32)
                {
                    g.DrawLine(line, 0, y, viewport.Width, y);
                }
            }
        }

        private void DrawHealthPips(Graphics g, int health, int maxHealth, int x, int y)
        {
            using (SolidBrush empty = new SolidBrush(Color.FromArgb(85, 255, 255, 255)))
            using (SolidBrush full = new SolidBrush(Color.FromArgb(255, 95, 126)))
            using (SolidBrush shine = new SolidBrush(Color.FromArgb(255, 206, 218)))
            {
                for (int i = 0; i < maxHealth; i++)
                {
                    Rectangle rect = new Rectangle(x + i * 18, y, 12, 12);
                    g.FillEllipse(i < health ? full : empty, rect);
                    if (i < health)
                    {
                        g.FillEllipse(shine, rect.X + 3, rect.Y + 2, 4, 4);
                    }
                }
            }
        }

        private void DrawBurstMeter(Graphics g, Player player, int x, int y)
        {
            using (SolidBrush label = new SolidBrush(Color.FromArgb(230, 255, 255, 255)))
            using (SolidBrush back = new SolidBrush(Color.FromArgb(140, 8, 13, 27)))
            using (SolidBrush fill = new SolidBrush(player.Bursting ? Color.FromArgb(255, 247, 91) : Color.FromArgb(64, 236, 210)))
            using (Pen edge = new Pen(Color.FromArgb(130, 255, 255, 255), 1f))
            {
                g.DrawString("BURST", tinyFont, label, x, y - 2);
                Rectangle bar = new Rectangle(x + 50, y + 2, 82, 9);
                g.FillRectangle(back, bar);
                g.FillRectangle(fill, bar.X, bar.Y, (int)(bar.Width * player.BurstCharge), bar.Height);
                g.DrawRectangle(edge, bar);
            }
        }

        private void DrawCoursePreview(Graphics g, int courseNumber, Rectangle rect)
        {
            Color skyTop = courseNumber == 1 ? Color.FromArgb(94, 195, 252) :
                courseNumber == 2 ? Color.FromArgb(65, 109, 178) :
                courseNumber == 3 ? Color.FromArgb(53, 41, 112) :
                courseNumber == 4 ? Color.FromArgb(71, 47, 61) :
                Color.FromArgb(48, 74, 152);
            Color skyBottom = courseNumber == 1 ? Color.FromArgb(203, 244, 255) :
                courseNumber == 2 ? Color.FromArgb(199, 225, 239) :
                courseNumber == 3 ? Color.FromArgb(211, 148, 202) :
                courseNumber == 4 ? Color.FromArgb(238, 130, 71) :
                Color.FromArgb(186, 226, 255);
            Color ground = courseNumber == 1 ? Color.FromArgb(79, 210, 90) :
                courseNumber == 2 ? Color.FromArgb(88, 224, 177) :
                courseNumber == 3 ? Color.FromArgb(117, 238, 211) :
                courseNumber == 4 ? Color.FromArgb(246, 164, 77) :
                Color.FromArgb(255, 217, 92);
            Color body = courseNumber == 1 ? Color.FromArgb(99, 142, 67) :
                courseNumber == 2 ? Color.FromArgb(67, 85, 122) :
                courseNumber == 3 ? Color.FromArgb(86, 66, 147) :
                courseNumber == 4 ? Color.FromArgb(104, 72, 80) :
                Color.FromArgb(54, 92, 145);

            using (LinearGradientBrush sky = new LinearGradientBrush(rect, skyTop, skyBottom, LinearGradientMode.Vertical))
            using (SolidBrush cloud = new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
            using (SolidBrush groundBrush = new SolidBrush(ground))
            using (SolidBrush bodyBrush = new SolidBrush(body))
            using (Pen edge = new Pen(Color.FromArgb(120, 255, 255, 255)))
            {
                g.FillRectangle(sky, rect);
                g.FillEllipse(cloud, rect.X + 8, rect.Y + 10, 42, 18);
                g.FillRectangle(bodyBrush, rect.X, rect.Bottom - 18, rect.Width, 18);
                g.FillRectangle(groundBrush, rect.X, rect.Bottom - 22, rect.Width, 7);

                if (courseNumber == 1)
                {
                    using (SolidBrush tree = new SolidBrush(Color.FromArgb(100, 80, 50)))
                    using (SolidBrush leaf = new SolidBrush(Color.FromArgb(74, 176, 112)))
                    {
                        g.FillRectangle(tree, rect.X + 68, rect.Bottom - 36, 6, 18);
                        g.FillEllipse(leaf, rect.X + 55, rect.Bottom - 50, 32, 22);
                    }
                }
                else if (courseNumber == 2)
                {
                    using (Pen cable = new Pen(Color.FromArgb(82, 255, 243), 2f))
                    using (SolidBrush spike = new SolidBrush(Color.FromArgb(242, 76, 96)))
                    {
                        g.DrawLine(cable, rect.X + 12, rect.Y + 17, rect.Right - 12, rect.Y + 8);
                        g.FillPolygon(spike, new Point[] { new Point(rect.X + 72, rect.Bottom - 18), new Point(rect.X + 80, rect.Bottom - 34), new Point(rect.X + 88, rect.Bottom - 18) });
                    }
                }
                else if (courseNumber == 3)
                {
                    using (SolidBrush crystal = new SolidBrush(Color.FromArgb(180, 225, 133, 255)))
                    using (Pen ray = new Pen(Color.FromArgb(130, 255, 235, 255), 2f))
                    {
                        g.DrawLine(ray, rect.X + 10, rect.Y + 44, rect.X + 44, rect.Y + 10);
                        g.FillPolygon(crystal, new Point[] { new Point(rect.X + 65, rect.Bottom - 20), new Point(rect.X + 80, rect.Y + 12), new Point(rect.X + 97, rect.Bottom - 20) });
                        g.FillPolygon(crystal, new Point[] { new Point(rect.X + 37, rect.Bottom - 20), new Point(rect.X + 49, rect.Y + 23), new Point(rect.X + 59, rect.Bottom - 20) });
                    }
                }
                else if (courseNumber == 4)
                {
                    using (SolidBrush stack = new SolidBrush(Color.FromArgb(122, 53, 58, 70)))
                    using (SolidBrush fire = new SolidBrush(Color.FromArgb(255, 138, 67)))
                    {
                        g.FillRectangle(stack, rect.X + 20, rect.Y + 8, 12, 34);
                        g.FillRectangle(stack, rect.X + 62, rect.Y + 16, 14, 28);
                        g.FillEllipse(fire, rect.X + 38, rect.Bottom - 35, 26, 14);
                    }
                }
                else
                {
                    using (SolidBrush tower = new SolidBrush(Color.FromArgb(48, 82, 137)))
                    using (Pen rail = new Pen(Color.FromArgb(255, 217, 92), 2f))
                    {
                        g.FillRectangle(tower, rect.X + 18, rect.Y + 17, 15, 28);
                        g.FillRectangle(tower, rect.X + 68, rect.Y + 8, 13, 37);
                        g.DrawLine(rail, rect.X + 6, rect.Y + 18, rect.Right - 6, rect.Y + 8);
                    }
                }

                g.DrawRectangle(edge, rect);
            }
        }

        private void Center(Graphics g, string textValue, Font font, Brush brush, int width, int y)
        {
            SizeF size = g.MeasureString(textValue, font);
            g.DrawString(textValue, font, brush, (width - size.Width) * 0.5f, y);
        }

    }
}
