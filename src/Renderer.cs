using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OmniWorld
{
    public sealed class Renderer
    {
        private readonly UiRenderer ui;
        private readonly Font smallFont;

        public Renderer()
        {
            ui = new UiRenderer();
            smallFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        }

        public void Render(Graphics g, Game game, Size viewport)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (game.Screen == GameScreen.CourseSelect)
            {
                ui.DrawCourseSelect(g, viewport, game);
                return;
            }

            DrawWorld(g, game, viewport);
            ui.DrawHud(g, viewport, game);

            if (game.Screen == GameScreen.Paused)
            {
                ui.DrawPause(g, viewport, game);
            }
            else if (game.Screen == GameScreen.CourseComplete)
            {
                ui.DrawCourseComplete(g, viewport, game);
            }
            else if (game.Screen == GameScreen.GameOver)
            {
                ui.DrawGameOver(g, viewport, game);
            }
            else if (game.Screen == GameScreen.Victory)
            {
                ui.DrawVictory(g, viewport, game);
            }
        }

        private void DrawWorld(Graphics g, Game game, Size viewport)
        {
            Level level = game.CurrentLevel;
            Vec2 camera = game.Camera + game.CameraShake;
            DrawBackground(g, level, viewport, camera, game.WorldTime);
            DrawTiles(g, level, viewport, camera);
            DrawMovingPlatforms(g, level, camera);
            DrawBouncePads(g, level, camera);
            DrawCollectibles(g, level, camera);
            DrawCheckpoints(g, level, camera);
            DrawGoal(g, level.Goal, camera, game.WorldTime);
            DrawParticles(g, game.Particles, camera);
            DrawEnemies(g, level, camera);
            DrawPlayer(g, game.Player, camera, game.WorldTime);
            DrawForegroundDecor(g, level, viewport, camera, game.WorldTime);
            DrawAtmosphereOverlay(g, level, viewport);
        }

        private void DrawBackground(Graphics g, Level level, Size viewport, Vec2 camera, float time)
        {
            using (LinearGradientBrush sky = new LinearGradientBrush(new Rectangle(0, 0, viewport.Width, viewport.Height), level.SkyTop, level.SkyBottom, LinearGradientMode.Vertical))
            {
                g.FillRectangle(sky, 0, 0, viewport.Width, viewport.Height);
            }

            DrawSkyAccent(g, level, viewport, time);
            DrawBackgroundMotes(g, level, viewport, camera, time);
            DrawParallaxLayer(g, viewport, camera, 0.08f, viewport.Height - 250, Color.FromArgb(80, 255, 255, 255), 330f, 60f);
            DrawParallaxLayer(g, viewport, camera, 0.16f, viewport.Height - 205, Color.FromArgb(110, 104, 166, 184), 260f, 120f);
            DrawParallaxLayer(g, viewport, camera, 0.28f, viewport.Height - 145, Color.FromArgb(160, 70, 136, 112), 230f, 115f);

            if (level.CourseNumber == 1)
            {
                DrawGroveDecor(g, viewport, camera);
            }
            else if (level.CourseNumber == 2)
            {
                DrawValeDecor(g, viewport, camera);
            }
            else if (level.CourseNumber == 3)
            {
                DrawCrystalDecor(g, viewport, camera, time);
            }
            else if (level.CourseNumber == 4)
            {
                DrawEmberDecor(g, viewport, camera, time);
            }
            else
            {
                DrawSkylineDecor(g, viewport, camera, time);
            }
        }

        private void DrawSkyAccent(Graphics g, Level level, Size viewport, float time)
        {
            RectangleF glow;
            if (level.CourseNumber == 1)
            {
                glow = new RectangleF(viewport.Width - 230f, 58f, 96f, 96f);
            }
            else if (level.CourseNumber == 2)
            {
                glow = new RectangleF(viewport.Width - 250f, 52f, 120f, 76f);
            }
            else if (level.CourseNumber == 3)
            {
                float pulse = (float)Math.Sin(time * 1.7f) * 5f;
                glow = new RectangleF(viewport.Width - 260f - pulse, 42f - pulse, 130f + pulse * 2f, 130f + pulse * 2f);
            }
            else if (level.CourseNumber == 4)
            {
                float pulse = (float)Math.Sin(time * 2.1f) * 6f;
                glow = new RectangleF(viewport.Width - 285f - pulse, 34f - pulse, 150f + pulse * 2f, 105f + pulse);
            }
            else
            {
                glow = new RectangleF(viewport.Width - 260f, 48f, 112f, 112f);
            }

            Color softColor = level.CourseNumber == 1 ? Color.FromArgb(160, 255, 235, 118) :
                level.CourseNumber == 2 ? Color.FromArgb(150, 124, 237, 255) :
                level.CourseNumber == 3 ? Color.FromArgb(140, 231, 132, 255) :
                level.CourseNumber == 4 ? Color.FromArgb(150, 255, 138, 67) :
                Color.FromArgb(135, 255, 235, 118);
            Color hotColor = level.CourseNumber == 1 ? Color.FromArgb(230, 255, 249, 186) :
                level.CourseNumber == 2 ? Color.FromArgb(220, 241, 255, 255) :
                level.CourseNumber == 3 ? Color.FromArgb(215, 255, 246, 255) :
                level.CourseNumber == 4 ? Color.FromArgb(220, 255, 227, 137) :
                Color.FromArgb(215, 255, 255, 255);

            using (SolidBrush soft = new SolidBrush(softColor))
            using (SolidBrush hot = new SolidBrush(hotColor))
            {
                g.FillEllipse(soft, glow);
                g.FillEllipse(hot, glow.X + glow.Width * 0.22f, glow.Y + glow.Height * 0.22f, glow.Width * 0.56f, glow.Height * 0.56f);
            }
        }

        private void DrawBackgroundMotes(Graphics g, Level level, Size viewport, Vec2 camera, float time)
        {
            Color moteColor = level.CourseNumber == 1 ? Color.FromArgb(58, 255, 255, 255) :
                level.CourseNumber == 2 ? Color.FromArgb(70, 74, 241, 225) :
                level.CourseNumber == 3 ? Color.FromArgb(90, 255, 216, 255) :
                level.CourseNumber == 4 ? Color.FromArgb(92, 255, 186, 79) :
                Color.FromArgb(76, 255, 255, 255);

            using (SolidBrush mote = new SolidBrush(moteColor))
            {
                for (int i = 0; i < 34; i++)
                {
                    float lane = (i * 73f) % viewport.Width;
                    float drift = (float)Math.Sin(time * 0.7f + i * 1.9f) * 18f;
                    float x = (lane - camera.X * 0.05f + drift) % (viewport.Width + 40f);
                    if (x < -20f) x += viewport.Width + 40f;

                    float y = 34f + ((i * 47f + time * (level.CourseNumber >= 3 ? 18f : 8f)) % (viewport.Height - 135f));
                    float size = level.CourseNumber == 3 ? 3f + (i % 3) : level.CourseNumber == 4 ? 2.5f + (i % 2) : 2f;
                    g.FillEllipse(mote, x, y, size, size);
                }
            }
        }

        private void DrawParallaxLayer(Graphics g, Size viewport, Vec2 camera, float rate, float baseY, Color color, float spacing, float height)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                float start = -camera.X * rate % spacing - spacing;
                for (float x = start; x < viewport.Width + spacing; x += spacing)
                {
                    PointF[] ridge = new PointF[]
                    {
                        new PointF(x, viewport.Height),
                        new PointF(x + spacing * 0.18f, baseY + height * 0.32f),
                        new PointF(x + spacing * 0.44f, baseY),
                        new PointF(x + spacing * 0.70f, baseY + height * 0.22f),
                        new PointF(x + spacing, viewport.Height),
                    };
                    g.FillPolygon(brush, ridge);
                }
            }
        }

        private void DrawGroveDecor(Graphics g, Size viewport, Vec2 camera)
        {
            using (SolidBrush trunk = new SolidBrush(Color.FromArgb(112, 83, 57)))
            using (SolidBrush leaf = new SolidBrush(Color.FromArgb(145, 74, 176, 112)))
            using (SolidBrush leafLight = new SolidBrush(Color.FromArgb(125, 113, 224, 132)))
            {
                float start = -camera.X * 0.42f % 180f - 180f;
                for (float x = start; x < viewport.Width + 220f; x += 180f)
                {
                    float y = viewport.Height - 118f;
                    g.FillRectangle(trunk, x + 38f, y + 34f, 16f, 76f);
                    g.FillEllipse(leaf, x, y, 88f, 58f);
                    g.FillEllipse(leafLight, x + 28f, y - 14f, 72f, 56f);
                    g.FillEllipse(leaf, x + 56f, y + 16f, 74f, 52f);
                }
            }
        }

        private void DrawValeDecor(Graphics g, Size viewport, Vec2 camera)
        {
            using (Pen cable = new Pen(Color.FromArgb(90, 34, 64, 103), 3f))
            using (SolidBrush tower = new SolidBrush(Color.FromArgb(120, 42, 65, 103)))
            using (SolidBrush light = new SolidBrush(Color.FromArgb(150, 67, 236, 216)))
            {
                float start = -camera.X * 0.34f % 210f - 210f;
                for (float x = start; x < viewport.Width + 240f; x += 210f)
                {
                    float y = viewport.Height - 190f;
                    g.FillRectangle(tower, x + 70f, y, 18f, 164f);
                    g.FillRectangle(tower, x + 42f, y + 36f, 74f, 12f);
                    g.DrawLine(cable, x - 10f, y + 50f, x + 244f, y + 20f);
                    g.FillRectangle(light, x + 72f, y - 10f, 14f, 14f);
                }
            }
        }

        private void DrawCrystalDecor(Graphics g, Size viewport, Vec2 camera, float time)
        {
            using (SolidBrush trunk = new SolidBrush(Color.FromArgb(80, 49, 80, 125)))
            using (SolidBrush crystalA = new SolidBrush(Color.FromArgb(145, 105, 241, 232)))
            using (SolidBrush crystalB = new SolidBrush(Color.FromArgb(125, 232, 130, 255)))
            using (Pen glow = new Pen(Color.FromArgb(120, 255, 238, 255), 2f))
            {
                float start = -camera.X * 0.32f % 220f - 220f;
                for (float x = start; x < viewport.Width + 260f; x += 220f)
                {
                    float y = viewport.Height - 172f;
                    float bob = (float)Math.Sin(time * 1.8f + x * 0.02f) * 5f;
                    g.FillRectangle(trunk, x + 82f, y + 35f, 18f, 150f);
                    DrawCrystalCluster(g, crystalA, glow, x + 30f, y + 66f + bob, 54f, 96f);
                    DrawCrystalCluster(g, crystalB, glow, x + 102f, y + 34f - bob, 62f, 126f);
                    DrawCrystalCluster(g, crystalA, glow, x + 162f, y + 78f + bob * 0.6f, 42f, 76f);
                }
            }
        }

        private void DrawCrystalCluster(Graphics g, Brush fill, Pen edge, float x, float y, float width, float height)
        {
            PointF[] shard = new PointF[]
            {
                new PointF(x + width * 0.5f, y),
                new PointF(x + width, y + height * 0.55f),
                new PointF(x + width * 0.65f, y + height),
                new PointF(x + width * 0.28f, y + height * 0.92f),
                new PointF(x, y + height * 0.52f)
            };
            g.FillPolygon(fill, shard);
            g.DrawPolygon(edge, shard);
        }

        private void DrawEmberDecor(Graphics g, Size viewport, Vec2 camera, float time)
        {
            using (SolidBrush stack = new SolidBrush(Color.FromArgb(122, 53, 58, 70)))
            using (SolidBrush furnace = new SolidBrush(Color.FromArgb(142, 72, 56, 64)))
            using (SolidBrush glow = new SolidBrush(Color.FromArgb(105, 255, 138, 67)))
            using (Pen pipe = new Pen(Color.FromArgb(100, 52, 38, 48), 5f))
            {
                float start = -camera.X * 0.36f % 240f - 240f;
                for (float x = start; x < viewport.Width + 260f; x += 240f)
                {
                    float y = viewport.Height - 205f;
                    float pulse = (float)Math.Sin(time * 3.5f + x * 0.02f) * 10f;
                    g.FillRectangle(stack, x + 42f, y + 12f, 34f, 182f);
                    g.FillRectangle(stack, x + 132f, y + 44f, 26f, 150f);
                    g.DrawLine(pipe, x - 10f, y + 112f, x + 230f, y + 84f);
                    g.FillRectangle(furnace, x + 78f, y + 108f, 72f, 66f);
                    g.FillEllipse(glow, x + 92f, y + 123f - pulse * 0.15f, 44f, 26f + pulse * 0.25f);
                }
            }
        }

        private void DrawSkylineDecor(Graphics g, Size viewport, Vec2 camera, float time)
        {
            using (SolidBrush tower = new SolidBrush(Color.FromArgb(118, 48, 82, 137)))
            using (SolidBrush glass = new SolidBrush(Color.FromArgb(105, 212, 242, 255)))
            using (Pen rail = new Pen(Color.FromArgb(110, 255, 217, 92), 3f))
            {
                float start = -camera.X * 0.30f % 210f - 210f;
                for (float x = start; x < viewport.Width + 240f; x += 210f)
                {
                    float y = viewport.Height - 214f;
                    float antenna = (float)Math.Sin(time * 2f + x * 0.03f) * 4f;
                    g.FillRectangle(tower, x + 44f, y + 58f, 46f, 182f);
                    g.FillRectangle(tower, x + 128f, y + 18f, 38f, 222f);
                    g.FillRectangle(glass, x + 54f, y + 76f, 10f, 10f);
                    g.FillRectangle(glass, x + 72f, y + 104f, 10f, 10f);
                    g.FillRectangle(glass, x + 137f, y + 42f, 10f, 10f);
                    g.FillRectangle(glass, x + 151f, y + 82f, 10f, 10f);
                    g.DrawLine(rail, x - 12f, y + 72f + antenna, x + 230f, y + 42f - antenna);
                }
            }
        }

        private void DrawTiles(Graphics g, Level level, Size viewport, Vec2 camera)
        {
            int left = GameMath.Clamp((int)Math.Floor(camera.X / Level.TileSize) - 1, 0, level.WidthTiles - 1);
            int right = GameMath.Clamp((int)Math.Ceiling((camera.X + viewport.Width) / Level.TileSize) + 1, 0, level.WidthTiles - 1);
            int top = GameMath.Clamp((int)Math.Floor(camera.Y / Level.TileSize) - 1, 0, level.HeightTiles - 1);
            int bottom = GameMath.Clamp((int)Math.Ceiling((camera.Y + viewport.Height) / Level.TileSize) + 1, 0, level.HeightTiles - 1);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    TileType tile = level.GetTile(x, y);
                    if (tile == TileType.Empty) continue;

                    RectangleF rect = new RectangleF(x * Level.TileSize - camera.X, y * Level.TileSize - camera.Y, Level.TileSize, Level.TileSize);
                    if (tile == TileType.Ground)
                    {
                        DrawGroundTile(g, level, rect, x, y);
                    }
                    else if (tile == TileType.Breakable)
                    {
                        DrawBreakable(g, rect);
                    }
                    else if (tile == TileType.Spike)
                    {
                        DrawSpike(g, rect);
                    }
                }
            }
        }

        private void DrawGroundTile(Graphics g, Level level, RectangleF rect, int tx, int ty)
        {
            bool topExposed = !level.IsSolidTile(tx, ty - 1);
            using (LinearGradientBrush body = new LinearGradientBrush(rect, Mix(level.GroundBody, Color.White, 0.12f), Mix(level.GroundBody, Color.Black, 0.18f), LinearGradientMode.Vertical))
            using (SolidBrush top = new SolidBrush(level.GroundTop))
            using (SolidBrush lip = new SolidBrush(Color.FromArgb(125, 255, 255, 255)))
            using (Pen line = new Pen(Color.FromArgb(90, 24, 42, 56)))
            using (Pen groove = new Pen(Color.FromArgb(75, 255, 255, 255)))
            using (Pen deepGroove = new Pen(Color.FromArgb(65, 0, 0, 0)))
            {
                g.FillRectangle(body, rect);
                if (topExposed)
                {
                    g.FillRectangle(top, rect.X, rect.Y, rect.Width, 11f);
                    g.FillRectangle(lip, rect.X + 2f, rect.Y + 2f, rect.Width - 4f, 2f);
                    g.DrawLine(groove, rect.X + 4f, rect.Y + 8f, rect.X + 10f, rect.Y + 4f);
                    g.DrawLine(groove, rect.X + 21f, rect.Y + 9f, rect.X + 28f, rect.Y + 5f);
                    DrawSurfaceDecor(g, level.CourseNumber, rect, tx);
                }

                g.DrawRectangle(line, rect.X, rect.Y, rect.Width, rect.Height);
                g.DrawLine(line, rect.X + 5f, rect.Y + 20f, rect.X + 25f, rect.Y + 20f);
                g.DrawLine(line, rect.X + 11f, rect.Y + 27f, rect.X + 18f, rect.Y + 27f);
                g.DrawLine(deepGroove, rect.X + 2f, rect.Bottom - 5f, rect.Right - 2f, rect.Bottom - 5f);

                if ((tx + ty) % 4 == 0)
                {
                    g.DrawLine(deepGroove, rect.X + 8f, rect.Y + 15f, rect.X + 17f, rect.Y + 23f);
                }
            }
        }

        private void DrawBreakable(Graphics g, RectangleF rect)
        {
            using (LinearGradientBrush fill = new LinearGradientBrush(rect, Color.FromArgb(226, 104, 220), Color.FromArgb(111, 52, 151), LinearGradientMode.Vertical))
            using (SolidBrush inset = new SolidBrush(Color.FromArgb(150, 58, 185)))
            using (SolidBrush shine = new SolidBrush(Color.FromArgb(244, 177, 238)))
            using (SolidBrush core = new SolidBrush(Color.FromArgb(71, 238, 213)))
            using (Pen edge = new Pen(Color.FromArgb(64, 33, 102), 2f))
            using (Pen bevel = new Pen(Color.FromArgb(160, 255, 255, 255), 1.4f))
            using (Pen crack = new Pen(Color.FromArgb(89, 41, 111), 2f))
            {
                g.FillRectangle(fill, rect);
                g.FillRectangle(inset, rect.X + 5f, rect.Y + 6f, rect.Width - 10f, rect.Height - 12f);
                g.FillRectangle(shine, rect.X + 6f, rect.Y + 5f, rect.Width - 12f, 3f);
                g.DrawRectangle(edge, rect.X + 2f, rect.Y + 2f, rect.Width - 4f, rect.Height - 4f);
                g.DrawLine(bevel, rect.X + 4f, rect.Y + 4f, rect.Right - 5f, rect.Y + 4f);
                g.DrawLine(bevel, rect.X + 4f, rect.Y + 4f, rect.X + 4f, rect.Bottom - 5f);
                g.DrawEllipse(edge, rect.X + 10f, rect.Y + 10f, 12f, 12f);
                g.FillEllipse(core, rect.X + 13f, rect.Y + 13f, 6f, 6f);
                g.DrawLine(crack, rect.X + 20f, rect.Y + 6f, rect.X + 15f, rect.Y + 14f);
                g.DrawLine(crack, rect.X + 15f, rect.Y + 14f, rect.X + 23f, rect.Y + 24f);
            }
        }

        private void DrawSpike(Graphics g, RectangleF rect)
        {
            PointF[] points = new PointF[]
            {
                new PointF(rect.X + 3f, rect.Bottom),
                new PointF(rect.X + rect.Width * 0.5f, rect.Y + 6f),
                new PointF(rect.Right - 3f, rect.Bottom)
            };

            using (SolidBrush fill = new SolidBrush(Color.FromArgb(242, 76, 96)))
            using (SolidBrush shine = new SolidBrush(Color.FromArgb(255, 169, 178)))
            using (Pen edge = new Pen(Color.FromArgb(105, 31, 58), 2f))
            {
                g.FillPolygon(fill, points);
                g.FillPolygon(shine, new PointF[]
                {
                    new PointF(rect.X + rect.Width * 0.5f, rect.Y + 10f),
                    new PointF(rect.X + rect.Width * 0.5f + 5f, rect.Bottom - 5f),
                    new PointF(rect.X + rect.Width * 0.5f - 2f, rect.Bottom - 5f)
                });
                g.DrawPolygon(edge, points);
            }
        }

        private void DrawMovingPlatforms(Graphics g, Level level, Vec2 camera)
        {
            using (Pen rail = new Pen(Color.FromArgb(105, 27, 51, 82), 3f))
            using (SolidBrush fill = new SolidBrush(Color.FromArgb(48, 205, 207)))
            using (SolidBrush glow = new SolidBrush(Color.FromArgb(90, 195, 255, 245)))
            using (SolidBrush dark = new SolidBrush(Color.FromArgb(32, 78, 110)))
            using (Pen edge = new Pen(Color.FromArgb(19, 45, 78), 2f))
            {
                for (int i = 0; i < level.MovingPlatforms.Count; i++)
                {
                    MovingPlatform platform = level.MovingPlatforms[i];
                    g.DrawLine(rail, platform.Start.X - camera.X + platform.Size.Width / 2f, platform.Start.Y - camera.Y + platform.Size.Height / 2f, platform.End.X - camera.X + platform.Size.Width / 2f, platform.End.Y - camera.Y + platform.Size.Height / 2f);

                    RectangleF rect = platform.Bounds;
                    rect.X -= camera.X;
                    rect.Y -= camera.Y;
                    DrawShadow(g, rect.X + 7f, rect.Y + rect.Height + 3f, rect.Width - 14f, 8f);
                    g.FillRectangle(dark, rect.X, rect.Y + 10f, rect.Width, rect.Height - 6f);
                    g.FillRectangle(fill, rect.X, rect.Y, rect.Width, 12f);
                    g.FillRectangle(glow, rect.X + 8f, rect.Y + 3f, rect.Width - 16f, 3f);
                    g.DrawRectangle(edge, rect.X, rect.Y, rect.Width, rect.Height);
                    g.FillEllipse(dark, rect.X + 8f, rect.Y + 5f, 5f, 5f);
                    g.FillEllipse(dark, rect.Right - 14f, rect.Y + 5f, 5f, 5f);
                }
            }
        }

        private void DrawBouncePads(Graphics g, Level level, Vec2 camera)
        {
            using (SolidBrush baseBrush = new SolidBrush(Color.FromArgb(37, 51, 82)))
            using (SolidBrush padBrush = new SolidBrush(Color.FromArgb(255, 247, 91)))
            using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(95, 64, 236, 210)))
            using (Pen edge = new Pen(Color.FromArgb(83, 55, 18), 2f))
            using (Pen spring = new Pen(Color.FromArgb(64, 236, 210), 2f))
            {
                for (int i = 0; i < level.BouncePads.Count; i++)
                {
                    BouncePad pad = level.BouncePads[i];
                    RectangleF rect = pad.Bounds;
                    rect.X -= camera.X;
                    rect.Y -= camera.Y;

                    float pulse = 1f + (float)Math.Sin(pad.PulseTimer * 8f) * 0.12f;
                    RectangleF glow = new RectangleF(rect.X - 6f, rect.Y - 8f, rect.Width + 12f, rect.Height + 16f);
                    RectangleF top = new RectangleF(rect.X - 1f, rect.Y - 3f * pulse, rect.Width + 2f, 8f * pulse);

                    DrawShadow(g, rect.X + 2f, rect.Bottom - 1f, rect.Width - 4f, 6f);
                    g.FillEllipse(glowBrush, glow);
                    g.FillRectangle(baseBrush, rect.X, rect.Y + 5f, rect.Width, rect.Height - 5f);
                    g.FillEllipse(padBrush, top);
                    g.DrawEllipse(edge, top);
                    g.DrawLine(spring, rect.X + 7f, rect.Bottom - 3f, rect.X + 14f, rect.Y + 8f);
                    g.DrawLine(spring, rect.X + 17f, rect.Y + 8f, rect.X + 25f, rect.Bottom - 3f);
                }
            }
        }

        private void DrawCollectibles(Graphics g, Level level, Vec2 camera)
        {
            for (int i = 0; i < level.Collectibles.Count; i++)
            {
                Collectible item = level.Collectibles[i];
                if (item.Collected) continue;

                float bob = (float)Math.Sin(item.BobTimer * 5f) * 3f;
                RectangleF rect = item.Bounds;
                rect.X -= camera.X;
                rect.Y = rect.Y - camera.Y + bob;

                if (item.Kind == CollectibleKind.EnergyOrb)
                {
                    DrawEnergyCoin(g, rect, item.BobTimer);
                }
                else if (item.Kind == CollectibleKind.FluxCore)
                {
                    DrawFluxCore(g, rect, item.BobTimer);
                }
                else if (item.Kind == CollectibleKind.BurstCell)
                {
                    DrawBurstCell(g, rect, item.BobTimer);
                }
                else
                {
                    DrawHeart(g, rect);
                }
            }
        }

        private void DrawCheckpoints(Graphics g, Level level, Vec2 camera)
        {
            for (int i = 0; i < level.Checkpoints.Count; i++)
            {
                Checkpoint checkpoint = level.Checkpoints[i];
                RectangleF rect = checkpoint.Bounds;
                rect.X -= camera.X;
                rect.Y -= camera.Y;

                Color flag = checkpoint.Activated ? Color.FromArgb(52, 243, 194) : Color.FromArgb(245, 226, 91);
                using (SolidBrush pole = new SolidBrush(Color.FromArgb(45, 52, 69)))
                using (SolidBrush poleLight = new SolidBrush(Color.FromArgb(104, 120, 144)))
                using (SolidBrush baseBrush = new SolidBrush(Color.FromArgb(26, 39, 58)))
                using (SolidBrush flagBrush = new SolidBrush(flag))
                using (Pen thread = new Pen(Color.FromArgb(130, 255, 255, 255), 1.5f))
                {
                    DrawShadow(g, rect.X - 6f, rect.Bottom - 2f, 28f, 7f);
                    g.FillRectangle(baseBrush, rect.X - 3f, rect.Bottom - 6f, 18f, 6f);
                    g.FillRectangle(pole, rect.X + 2f, rect.Y, 5f, rect.Height);
                    g.FillRectangle(poleLight, rect.X + 3f, rect.Y + 2f, 1f, rect.Height - 6f);
                    PointF[] banner = new PointF[]
                    {
                        new PointF(rect.X + 7f, rect.Y + 4f),
                        new PointF(rect.X + 28f, rect.Y + 12f),
                        new PointF(rect.X + 7f, rect.Y + 23f)
                    };
                    g.FillPolygon(flagBrush, banner);
                    g.DrawLine(thread, rect.X + 9f, rect.Y + 8f, rect.X + 25f, rect.Y + 13f);
                }
            }
        }

        private void DrawGoal(Graphics g, GoalGate goal, Vec2 camera, float time)
        {
            RectangleF rect = goal.Bounds;
            rect.X -= camera.X;
            rect.Y -= camera.Y;
            float pulseSize = 8f + (float)Math.Sin(time * 5.5f) * 4f;
            float innerPulse = 3f + (float)Math.Sin(time * 8.0f) * 1.5f;

            using (SolidBrush baseBrush = new SolidBrush(Color.FromArgb(24, 37, 59)))
            using (SolidBrush mast = new SolidBrush(Color.FromArgb(235, 242, 255)))
            using (SolidBrush mastShadow = new SolidBrush(Color.FromArgb(96, 132, 166)))
            using (SolidBrush flag = new SolidBrush(Color.FromArgb(75, 238, 211)))
            using (SolidBrush flagShade = new SolidBrush(Color.FromArgb(39, 157, 190)))
            using (Pen outer = new Pen(Color.FromArgb(28, 64, 112), 6f))
            using (Pen inner = new Pen(Color.FromArgb(75, 238, 211), 3f))
            using (SolidBrush shine = new SolidBrush(Color.FromArgb(95, 255, 255, 255)))
            using (SolidBrush pulse = new SolidBrush(Color.FromArgb(55, 75, 238, 211)))
            {
                g.FillEllipse(pulse, rect.X - pulseSize, rect.Y + 18f - pulseSize * 0.6f, rect.Width + pulseSize * 2.4f, rect.Height - 26f + pulseSize * 1.2f);
                g.FillRectangle(baseBrush, rect.X - 8f, rect.Bottom - 12f, rect.Width + 16f, 12f);
                g.FillRectangle(mastShadow, rect.X + 18f, rect.Y + 6f, 9f, rect.Height - 12f);
                g.FillRectangle(mast, rect.X + 16f, rect.Y + 4f, 7f, rect.Height - 10f);

                PointF[] banner = new PointF[]
                {
                    new PointF(rect.X + 23f, rect.Y + 10f),
                    new PointF(rect.X + 68f, rect.Y + 19f + (float)Math.Sin(time * 4f) * 3f),
                    new PointF(rect.X + 54f, rect.Y + 41f),
                    new PointF(rect.X + 23f, rect.Y + 34f)
                };
                g.FillPolygon(flag, banner);
                g.FillPolygon(flagShade, new PointF[]
                {
                    new PointF(rect.X + 23f, rect.Y + 27f),
                    new PointF(rect.X + 54f, rect.Y + 41f),
                    new PointF(rect.X + 23f, rect.Y + 34f)
                });

                RectangleF ring = new RectangleF(rect.X - 2f, rect.Y + 34f, rect.Width + 6f, rect.Height - 42f);
                g.DrawEllipse(outer, ring);
                g.DrawEllipse(inner, ring.X + 4f + innerPulse, ring.Y + 4f + innerPulse, ring.Width - 8f - innerPulse * 2f, ring.Height - 8f - innerPulse * 2f);
                g.FillRectangle(shine, rect.X + 19f, rect.Y + 44f, 6f, rect.Height - 62f);
            }

            using (SolidBrush text = new SolidBrush(Color.FromArgb(35, 52, 70)))
            {
                g.DrawString("BEACON", smallFont, text, rect.X - 11f, rect.Y - 22f);
            }
        }

        private void DrawEnemies(Graphics g, Level level, Vec2 camera)
        {
            for (int i = 0; i < level.Enemies.Count; i++)
            {
                Enemy enemy = level.Enemies[i];
                if (!enemy.Active) continue;

                RectangleF rect = enemy.Bounds;
                rect.X -= camera.X;
                rect.Y -= camera.Y;
                DrawShadow(g, rect.X + 2f, rect.Bottom - 1f, rect.Width - 4f, 7f);

                if (enemy.Kind == EnemyKind.Roller)
                {
                    using (SolidBrush fill = new SolidBrush(Color.FromArgb(92, 75, 186)))
                    using (SolidBrush shell = new SolidBrush(Color.FromArgb(128, 100, 220)))
                    using (SolidBrush eye = new SolidBrush(Color.White))
                    using (SolidBrush pupil = new SolidBrush(Color.FromArgb(32, 27, 70)))
                    using (Pen edge = new Pen(Color.FromArgb(42, 35, 91), 2f))
                    {
                        g.FillEllipse(fill, rect);
                        g.FillPie(shell, rect.X + 4f, rect.Y + 3f, rect.Width - 8f, rect.Height - 6f, 200f, 145f);
                        g.DrawEllipse(edge, rect);
                        g.FillRectangle(eye, rect.X + (enemy.Direction > 0 ? 15f : 6f), rect.Y + 7f, 5f, 5f);
                        g.FillRectangle(pupil, rect.X + (enemy.Direction > 0 ? 18f : 6f), rect.Y + 9f, 2f, 2f);
                        g.DrawArc(edge, rect.X + 5f, rect.Y + 8f, rect.Width - 10f, rect.Height - 10f, 25f, 135f);
                    }
                }
                else if (enemy.Kind == EnemyKind.Hopper)
                {
                    using (SolidBrush fill = new SolidBrush(Color.FromArgb(235, 115, 73)))
                    using (SolidBrush belly = new SolidBrush(Color.FromArgb(253, 171, 91)))
                    using (SolidBrush eye = new SolidBrush(Color.White))
                    using (SolidBrush foot = new SolidBrush(Color.FromArgb(114, 50, 41)))
                    using (Pen edge = new Pen(Color.FromArgb(111, 46, 34), 2f))
                    {
                        g.FillRectangle(fill, rect.X + 3f, rect.Y + 6f, rect.Width - 6f, rect.Height - 6f);
                        g.FillRectangle(fill, rect.X + 8f, rect.Y, rect.Width - 16f, 8f);
                        g.FillRectangle(belly, rect.X + 8f, rect.Y + 18f, rect.Width - 16f, 7f);
                        g.DrawRectangle(edge, rect.X + 3f, rect.Y + 6f, rect.Width - 6f, rect.Height - 6f);
                        g.FillRectangle(eye, rect.X + (enemy.Direction > 0 ? 16f : 7f), rect.Y + 11f, 5f, 5f);
                        g.FillRectangle(foot, rect.X + 3f, rect.Bottom - 4f, 9f, 4f);
                        g.FillRectangle(foot, rect.Right - 12f, rect.Bottom - 4f, 9f, 4f);
                    }
                }
                else
                {
                    using (SolidBrush fill = new SolidBrush(Color.FromArgb(225, 64, 112)))
                    using (SolidBrush core = new SolidBrush(Color.FromArgb(255, 231, 90)))
                    using (SolidBrush highlight = new SolidBrush(Color.FromArgb(255, 151, 179)))
                    using (Pen edge = new Pen(Color.FromArgb(95, 28, 76), 2f))
                    {
                        PointF[] body = new PointF[]
                        {
                            new PointF(rect.X + rect.Width / 2f, rect.Y),
                            new PointF(rect.Right, rect.Y + rect.Height / 2f),
                            new PointF(rect.X + rect.Width / 2f, rect.Bottom),
                            new PointF(rect.X, rect.Y + rect.Height / 2f)
                        };
                        g.FillPolygon(fill, body);
                        g.FillPolygon(highlight, new PointF[]
                        {
                            new PointF(rect.X + rect.Width / 2f, rect.Y + 4f),
                            new PointF(rect.X + rect.Width * 0.72f, rect.Y + rect.Height * 0.48f),
                            new PointF(rect.X + rect.Width * 0.47f, rect.Y + rect.Height * 0.38f)
                        });
                        g.DrawPolygon(edge, body);
                        g.FillEllipse(core, rect.X + 9f, rect.Y + 8f, 9f, 9f);
                    }
                }
            }
        }

        private void DrawParticles(Graphics g, ParticleSystem particleSystem, Vec2 camera)
        {
            for (int i = 0; i < particleSystem.Particles.Count; i++)
            {
                Particle particle = particleSystem.Particles[i];
                float remaining = 1f - particle.Age / particle.Life;
                int alpha = GameMath.Clamp((int)(remaining * 230f), 0, 230);
                Color color = Color.FromArgb(alpha, particle.Color);
                float x = particle.Position.X - camera.X;
                float y = particle.Position.Y - camera.Y;

                if (particle.Kind == ParticleKind.Smoke)
                {
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        float size = particle.Size * (1.0f + (1f - remaining) * 1.2f);
                        g.FillEllipse(brush, x - size / 2f, y - size / 2f, size, size * 0.75f);
                    }
                }
                else if (particle.Kind == ParticleKind.Spark)
                {
                    using (Pen pen = new Pen(color, Math.Max(1f, particle.Size * 0.28f)))
                    {
                        float size = particle.Size * (0.7f + remaining);
                        g.DrawLine(pen, x - size, y, x + size, y);
                        g.DrawLine(pen, x, y - size, x, y + size);
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        float size = particle.Size * (0.6f + remaining);
                        PointF[] shard = new PointF[]
                        {
                            new PointF(x, y - size),
                            new PointF(x + size, y),
                            new PointF(x, y + size),
                            new PointF(x - size, y)
                        };
                        g.FillPolygon(brush, shard);
                    }
                }
            }
        }

        private void DrawPlayer(Graphics g, Player player, Vec2 camera, float time)
        {
            RectangleF rect = player.Bounds;
            rect.X -= camera.X;
            rect.Y -= camera.Y;

            bool blink = player.InvincibleTimer > 0f && ((int)(player.InvincibleTimer * 12f) % 2 == 0);
            if (blink) return;

            if (player.Powered)
            {
                using (SolidBrush glow = new SolidBrush(Color.FromArgb(90, 62, 244, 221)))
                using (Pen ring = new Pen(Color.FromArgb(150, 255, 247, 92), 2f))
                {
                    g.FillEllipse(glow, rect.X - 10f, rect.Y - 10f, rect.Width + 20f, rect.Height + 20f);
                    g.DrawEllipse(ring, rect.X - 14f, rect.Y - 13f, rect.Width + 28f, rect.Height + 26f);
                }
            }

            if (player.Bursting)
            {
                using (SolidBrush streak = new SolidBrush(Color.FromArgb(115, 255, 247, 91)))
                using (SolidBrush coreTrail = new SolidBrush(Color.FromArgb(90, 64, 236, 210)))
                {
                    float tailX = player.Facing > 0 ? rect.X - 34f : rect.Right;
                    PointF[] flare = new PointF[]
                    {
                        new PointF(player.Facing > 0 ? rect.X + 5f : rect.Right - 5f, rect.Y + 7f),
                        new PointF(tailX, rect.Y + rect.Height * 0.5f),
                        new PointF(player.Facing > 0 ? rect.X + 5f : rect.Right - 5f, rect.Bottom - 5f)
                    };
                    g.FillPolygon(streak, flare);
                    g.FillEllipse(coreTrail, rect.X - 8f, rect.Y - 6f, rect.Width + 16f, rect.Height + 12f);
                }
            }

            DrawShadow(g, rect.X + 2f, rect.Bottom - 1f, rect.Width - 4f, 7f);
            float stride = player.OnGround && Math.Abs(player.Velocity.X) > 50f ? (float)Math.Sin(time * 18f) * 2.5f : 0f;
            float lean = player.OnGround ? Math.Sign(player.Velocity.X) * Math.Min(2.5f, Math.Abs(player.Velocity.X) * 0.01f) : player.Facing * 1.5f;

            using (SolidBrush suit = new SolidBrush(Color.FromArgb(41, 70, 120)))
            using (SolidBrush suitLight = new SolidBrush(Color.FromArgb(63, 103, 166)))
            using (SolidBrush boot = new SolidBrush(Color.FromArgb(18, 32, 67)))
            using (SolidBrush helmet = new SolidBrush(Color.FromArgb(236, 243, 255)))
            using (SolidBrush visor = new SolidBrush(Color.FromArgb(67, 220, 235)))
            using (SolidBrush visorShine = new SolidBrush(Color.FromArgb(230, 255, 255, 255)))
            using (SolidBrush core = new SolidBrush(player.Powered ? Color.FromArgb(255, 247, 92) : Color.FromArgb(45, 245, 184)))
            using (Pen edge = new Pen(Color.FromArgb(20, 32, 62), 2f))
            {
                g.FillRectangle(suit, rect.X + 4f + lean, rect.Y + 12f, rect.Width - 8f, rect.Height - 8f);
                g.FillRectangle(suitLight, rect.X + 7f + lean, rect.Y + 14f, 5f, rect.Height - 13f);
                g.DrawRectangle(edge, rect.X + 4f + lean, rect.Y + 12f, rect.Width - 8f, rect.Height - 8f);
                g.FillEllipse(helmet, rect.X + 2f + lean, rect.Y, rect.Width - 4f, 18f);
                g.DrawEllipse(edge, rect.X + 2f + lean, rect.Y, rect.Width - 4f, 18f);
                g.FillRectangle(visor, player.Facing > 0 ? rect.X + 13f + lean : rect.X + 5f + lean, rect.Y + 7f, 8f, 5f);
                g.FillRectangle(visorShine, player.Facing > 0 ? rect.X + 17f + lean : rect.X + 5f + lean, rect.Y + 7f, 2f, 2f);
                g.FillEllipse(core, rect.X + 8f + lean, rect.Y + 17f, 8f, 8f);
                g.DrawEllipse(edge, rect.X + 8f + lean, rect.Y + 17f, 8f, 8f);
                g.FillRectangle(suit, rect.X + (player.Facing > 0 ? 18f : -1f) + lean, rect.Y + 15f - stride * 0.4f, 7f, 5f);
                g.DrawRectangle(edge, rect.X + (player.Facing > 0 ? 18f : -1f) + lean, rect.Y + 15f - stride * 0.4f, 7f, 5f);
                g.FillRectangle(boot, rect.X + 5f + stride, rect.Bottom - 4f, 8f, 4f);
                g.FillRectangle(boot, rect.Right - 13f - stride, rect.Bottom - 4f, 8f, 4f);
            }
        }

        private void DrawPickupSpark(Graphics g, RectangleF rect, float timer)
        {
            float pulse = 2f + (float)Math.Sin(timer * 7f) * 1.5f;
            using (Pen sparkle = new Pen(Color.FromArgb(150, 255, 255, 255), 1.5f))
            {
                g.DrawLine(sparkle, rect.X - pulse, rect.Y + rect.Height / 2f, rect.X - pulse - 5f, rect.Y + rect.Height / 2f);
                g.DrawLine(sparkle, rect.Right + pulse, rect.Y + rect.Height / 2f, rect.Right + pulse + 5f, rect.Y + rect.Height / 2f);
                g.DrawLine(sparkle, rect.X + rect.Width / 2f, rect.Y - pulse, rect.X + rect.Width / 2f, rect.Y - pulse - 5f);
            }
        }

        private void DrawEnergyCoin(Graphics g, RectangleF rect, float timer)
        {
            DrawPickupSpark(g, rect, timer);

            float squash = 1f + (float)Math.Sin(timer * 6.5f) * 0.12f;
            RectangleF coin = new RectangleF(rect.X + rect.Width * (1f - squash) * 0.5f, rect.Y, rect.Width * squash, rect.Height);

            using (LinearGradientBrush outer = new LinearGradientBrush(coin, Color.FromArgb(255, 250, 128), Color.FromArgb(228, 146, 36), LinearGradientMode.ForwardDiagonal))
            using (SolidBrush inner = new SolidBrush(Color.FromArgb(255, 255, 205)))
            using (Pen edge = new Pen(Color.FromArgb(143, 91, 22), 2f))
            using (Pen groove = new Pen(Color.FromArgb(165, 122, 49), 1.5f))
            {
                g.FillEllipse(outer, coin);
                g.DrawEllipse(edge, coin);
                g.DrawEllipse(groove, coin.X + 4f, coin.Y + 4f, coin.Width - 8f, coin.Height - 8f);
                g.FillRectangle(inner, coin.X + coin.Width * 0.48f, coin.Y + 4f, 3f, coin.Height - 8f);
            }
        }

        private void DrawFluxCore(Graphics g, RectangleF rect, float timer)
        {
            DrawPickupSpark(g, rect, timer);

            float pulse = 2f + (float)Math.Sin(timer * 8f) * 2f;
            RectangleF glow = new RectangleF(rect.X - pulse, rect.Y - pulse, rect.Width + pulse * 2f, rect.Height + pulse * 2f);

            using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(75, 64, 236, 210)))
            using (LinearGradientBrush outer = new LinearGradientBrush(rect, Color.FromArgb(55, 246, 215), Color.FromArgb(29, 112, 188), LinearGradientMode.ForwardDiagonal))
            using (SolidBrush inner = new SolidBrush(Color.FromArgb(238, 255, 255)))
            using (Pen edge = new Pen(Color.FromArgb(24, 99, 117), 2f))
            {
                g.FillEllipse(glowBrush, glow);
                PointF[] diamond = new PointF[]
                {
                    new PointF(rect.X + rect.Width / 2f, rect.Y),
                    new PointF(rect.Right, rect.Y + rect.Height / 2f),
                    new PointF(rect.X + rect.Width / 2f, rect.Bottom),
                    new PointF(rect.X, rect.Y + rect.Height / 2f)
                };
                g.FillPolygon(outer, diamond);
                g.DrawPolygon(edge, diamond);
                g.FillEllipse(inner, rect.X + 8f, rect.Y + 8f, 8f, 8f);
            }
        }

        private void DrawBurstCell(Graphics g, RectangleF rect, float timer)
        {
            DrawPickupSpark(g, rect, timer);

            float pulse = 2f + (float)Math.Sin(timer * 10f) * 2f;
            RectangleF glow = new RectangleF(rect.X - pulse, rect.Y - pulse, rect.Width + pulse * 2f, rect.Height + pulse * 2f);

            using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(80, 255, 247, 91)))
            using (LinearGradientBrush body = new LinearGradientBrush(rect, Color.FromArgb(255, 247, 91), Color.FromArgb(64, 236, 210), LinearGradientMode.ForwardDiagonal))
            using (SolidBrush notch = new SolidBrush(Color.FromArgb(37, 51, 82)))
            using (Pen edge = new Pen(Color.FromArgb(92, 61, 16), 2f))
            {
                g.FillEllipse(glowBrush, glow);
                PointF[] hex = new PointF[]
                {
                    new PointF(rect.X + rect.Width * 0.50f, rect.Y),
                    new PointF(rect.Right, rect.Y + rect.Height * 0.28f),
                    new PointF(rect.Right - 2f, rect.Y + rect.Height * 0.76f),
                    new PointF(rect.X + rect.Width * 0.50f, rect.Bottom),
                    new PointF(rect.X + 2f, rect.Y + rect.Height * 0.76f),
                    new PointF(rect.X, rect.Y + rect.Height * 0.28f)
                };
                g.FillPolygon(body, hex);
                g.DrawPolygon(edge, hex);
                g.FillRectangle(notch, rect.X + rect.Width * 0.42f, rect.Y + 5f, rect.Width * 0.16f, rect.Height - 10f);
            }
        }

        private void DrawHeart(Graphics g, RectangleF rect)
        {
            using (LinearGradientBrush fill = new LinearGradientBrush(rect, Color.FromArgb(255, 131, 157), Color.FromArgb(210, 49, 91), LinearGradientMode.Vertical))
            using (SolidBrush shine = new SolidBrush(Color.FromArgb(255, 221, 230)))
            using (Pen edge = new Pen(Color.FromArgb(123, 30, 61), 1.6f))
            {
                g.FillEllipse(fill, rect.X, rect.Y, rect.Width * 0.56f, rect.Height * 0.58f);
                g.FillEllipse(fill, rect.X + rect.Width * 0.44f, rect.Y, rect.Width * 0.56f, rect.Height * 0.58f);
                PointF[] point = new PointF[]
                {
                    new PointF(rect.X + 2f, rect.Y + rect.Height * 0.42f),
                    new PointF(rect.Right - 2f, rect.Y + rect.Height * 0.42f),
                    new PointF(rect.X + rect.Width / 2f, rect.Bottom)
                };
                g.FillPolygon(fill, point);
                g.DrawEllipse(edge, rect.X, rect.Y, rect.Width * 0.56f, rect.Height * 0.58f);
                g.DrawEllipse(edge, rect.X + rect.Width * 0.44f, rect.Y, rect.Width * 0.56f, rect.Height * 0.58f);
                g.DrawPolygon(edge, point);
                g.FillEllipse(shine, rect.X + 5f, rect.Y + 4f, 5f, 5f);
            }
        }

        private static Color Mix(Color a, Color b, float amount)
        {
            amount = GameMath.Clamp(amount, 0f, 1f);
            int r = (int)(a.R + (b.R - a.R) * amount);
            int g = (int)(a.G + (b.G - a.G) * amount);
            int bl = (int)(a.B + (b.B - a.B) * amount);
            return Color.FromArgb(a.A, r, g, bl);
        }

        private void DrawShadow(Graphics g, float x, float y, float width, float height)
        {
            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(65, 8, 18, 28)))
            {
                g.FillEllipse(shadow, x, y, width, height);
            }
        }

        private void DrawSurfaceDecor(Graphics g, int courseNumber, RectangleF rect, int tileX)
        {
            int pattern = Math.Abs(tileX * 37 + courseNumber * 19) % 6;

            if (courseNumber == 1)
            {
                using (Pen blade = new Pen(Color.FromArgb(170, 37, 139, 68), 1.4f))
                using (SolidBrush flower = new SolidBrush(pattern == 0 ? Color.FromArgb(255, 247, 91) : Color.FromArgb(255, 126, 171)))
                {
                    if (pattern != 4)
                    {
                        float x = rect.X + 5f + pattern * 3f;
                        g.DrawLine(blade, x, rect.Y + 9f, x - 3f, rect.Y + 2f);
                        g.DrawLine(blade, x + 7f, rect.Y + 10f, x + 10f, rect.Y + 3f);
                    }

                    if (pattern == 0 || pattern == 3)
                    {
                        g.FillEllipse(flower, rect.X + 18f, rect.Y - 3f, 5f, 5f);
                    }
                }
            }
            else if (courseNumber == 2)
            {
                using (Pen circuit = new Pen(Color.FromArgb(145, 73, 238, 212), 1.3f))
                using (SolidBrush node = new SolidBrush(Color.FromArgb(190, 241, 255, 255)))
                {
                    if (pattern == 1 || pattern == 4)
                    {
                        g.DrawLine(circuit, rect.X + 5f, rect.Y + 5f, rect.X + 18f, rect.Y + 5f);
                        g.DrawLine(circuit, rect.X + 18f, rect.Y + 5f, rect.X + 24f, rect.Y + 10f);
                        g.FillEllipse(node, rect.X + 23f, rect.Y + 8f, 4f, 4f);
                    }
                }
            }
            else if (courseNumber == 3)
            {
                using (SolidBrush chip = new SolidBrush(Color.FromArgb(170, 232, 130, 255)))
                using (Pen glint = new Pen(Color.FromArgb(150, 255, 255, 255), 1.2f))
                {
                    if (pattern == 0 || pattern == 2 || pattern == 5)
                    {
                        PointF[] shard = new PointF[]
                        {
                            new PointF(rect.X + 8f + pattern, rect.Y + 2f),
                            new PointF(rect.X + 15f + pattern, rect.Y + 8f),
                            new PointF(rect.X + 5f + pattern, rect.Y + 10f)
                        };
                        g.FillPolygon(chip, shard);
                        g.DrawLine(glint, shard[0], shard[1]);
                    }
                }
            }
            else if (courseNumber == 4)
            {
                using (Pen seam = new Pen(Color.FromArgb(145, 255, 167, 75), 1.4f))
                using (SolidBrush ember = new SolidBrush(Color.FromArgb(180, 255, 105, 55)))
                {
                    if (pattern == 0 || pattern == 4)
                    {
                        g.DrawLine(seam, rect.X + 5f, rect.Y + 7f, rect.X + 24f, rect.Y + 7f);
                        g.FillEllipse(ember, rect.X + 23f, rect.Y + 4f, 5f, 5f);
                    }
                }
            }
            else
            {
                using (Pen stripe = new Pen(Color.FromArgb(150, 255, 217, 92), 1.5f))
                using (SolidBrush light = new SolidBrush(Color.FromArgb(180, 212, 242, 255)))
                {
                    if (pattern == 2 || pattern == 5)
                    {
                        g.DrawLine(stripe, rect.X + 4f, rect.Y + 4f, rect.X + 24f, rect.Y + 10f);
                        g.FillRectangle(light, rect.X + 10f, rect.Y + 3f, 5f, 5f);
                    }
                }
            }
        }

        private void DrawForegroundDecor(Graphics g, Level level, Size viewport, Vec2 camera, float time)
        {
            if (level.CourseNumber == 1)
            {
                using (Pen grass = new Pen(Color.FromArgb(120, 29, 112, 58), 2f))
                {
                    float start = -camera.X * 0.85f % 34f - 34f;
                    for (float x = start; x < viewport.Width + 34f; x += 17f)
                    {
                        float sway = (float)Math.Sin(time * 2.5f + x * 0.05f) * 3f;
                        float y = viewport.Height - 28f + (x % 3f);
                        g.DrawLine(grass, x, y, x + sway, y - 18f);
                    }
                }
            }
            else if (level.CourseNumber == 2)
            {
                using (Pen spark = new Pen(Color.FromArgb(90, 80, 255, 230), 1.5f))
                {
                    float start = -camera.X * 0.65f % 96f - 96f;
                    for (float x = start; x < viewport.Width + 120f; x += 96f)
                    {
                        float y = viewport.Height - 62f + (float)Math.Sin(time * 3f + x) * 9f;
                        g.DrawLine(spark, x, y, x + 16f, y - 12f);
                        g.DrawLine(spark, x + 16f, y - 12f, x + 29f, y - 4f);
                    }
                }
            }
            else if (level.CourseNumber == 3)
            {
                using (Pen ray = new Pen(Color.FromArgb(90, 255, 221, 255), 2f))
                using (SolidBrush shard = new SolidBrush(Color.FromArgb(75, 225, 133, 255)))
                {
                    float start = -camera.X * 0.72f % 78f - 78f;
                    for (float x = start; x < viewport.Width + 100f; x += 78f)
                    {
                        float y = viewport.Height - 50f + (float)Math.Sin(time * 2.2f + x * 0.03f) * 8f;
                        g.DrawLine(ray, x, y - 24f, x + 22f, y - 58f);
                        g.FillPolygon(shard, new PointF[]
                        {
                            new PointF(x + 26f, y - 38f),
                            new PointF(x + 36f, y - 14f),
                            new PointF(x + 18f, y - 18f)
                        });
                    }
                }
            }
            else if (level.CourseNumber == 4)
            {
                using (Pen heat = new Pen(Color.FromArgb(70, 255, 151, 71), 2f))
                using (SolidBrush spark = new SolidBrush(Color.FromArgb(85, 255, 207, 91)))
                {
                    float start = -camera.X * 0.80f % 58f - 58f;
                    for (float x = start; x < viewport.Width + 80f; x += 58f)
                    {
                        float y = viewport.Height - 38f;
                        float rise = (float)Math.Sin(time * 4.0f + x * 0.04f) * 12f;
                        g.DrawLine(heat, x, y, x + 8f, y - 35f - rise);
                        g.FillEllipse(spark, x + 18f, y - 20f - rise * 0.4f, 5f, 5f);
                    }
                }
            }
            else
            {
                using (Pen wind = new Pen(Color.FromArgb(85, 255, 255, 255), 2f))
                using (SolidBrush light = new SolidBrush(Color.FromArgb(70, 255, 217, 92)))
                {
                    float start = -camera.X * 0.90f % 105f - 105f;
                    for (float x = start; x < viewport.Width + 130f; x += 105f)
                    {
                        float y = viewport.Height - 82f + (float)Math.Sin(time * 2.5f + x * 0.02f) * 14f;
                        g.DrawLine(wind, x, y, x + 42f, y - 14f);
                        g.DrawLine(wind, x + 16f, y + 9f, x + 64f, y - 5f);
                        g.FillEllipse(light, x + 70f, y - 20f, 9f, 9f);
                    }
                }
            }
        }

        private void DrawAtmosphereOverlay(Graphics g, Level level, Size viewport)
        {
            Color top = level.CourseNumber == 1 ? Color.FromArgb(24, 255, 255, 255) :
                level.CourseNumber == 2 ? Color.FromArgb(24, 75, 238, 211) :
                level.CourseNumber == 3 ? Color.FromArgb(32, 255, 170, 255) :
                level.CourseNumber == 4 ? Color.FromArgb(35, 255, 134, 67) :
                Color.FromArgb(28, 255, 255, 255);
            Color bottom = Color.FromArgb(26, 7, 14, 28);

            using (LinearGradientBrush wash = new LinearGradientBrush(new Rectangle(0, 0, viewport.Width, viewport.Height), top, bottom, LinearGradientMode.Vertical))
            {
                g.FillRectangle(wash, 0, 0, viewport.Width, viewport.Height);
            }
        }
    }
}
