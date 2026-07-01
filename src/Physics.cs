using System;
using System.Drawing;

namespace OmniWorld
{
    public static class Physics
    {
        private const float Epsilon = 0.01f;

        public static void MovePlayer(Player player, Level level, float dt, AudioManager audio)
        {
            float previousY = player.Position.Y;
            player.OnGround = false;

            // Axis-separated collision keeps wall, floor, and ceiling responses crisp.
            MoveHorizontal(player, level, dt, audio, true);
            MoveVertical(player, level, dt, audio, true);
            ResolveMovingPlatforms(player, level, previousY);
        }

        public static void MoveEnemy(Enemy enemy, Level level, float dt)
        {
            enemy.OnGround = false;
            MoveEnemyHorizontal(enemy, level, dt);
            MoveEnemyVertical(enemy, level, dt);
        }

        public static bool IsTouchingHazard(RectangleF bounds, Level level)
        {
            int left = (int)Math.Floor(bounds.Left / Level.TileSize);
            int right = (int)Math.Floor((bounds.Right - 1f) / Level.TileSize);
            int top = (int)Math.Floor(bounds.Top / Level.TileSize);
            int bottom = (int)Math.Floor((bounds.Bottom - 1f) / Level.TileSize);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    if (!level.IsHazardTile(x, y)) continue;

                    RectangleF spike = new RectangleF(x * Level.TileSize + 5f, y * Level.TileSize + 8f, Level.TileSize - 10f, Level.TileSize - 8f);
                    if (bounds.IntersectsWith(spike))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void MoveHorizontal(Player player, Level level, float dt, AudioManager audio, bool canBreak)
        {
            player.Position.X += player.Velocity.X * dt;
            RectangleF bounds = player.Bounds;

            int left = (int)Math.Floor(bounds.Left / Level.TileSize);
            int right = (int)Math.Floor((bounds.Right - 1f) / Level.TileSize);
            int top = (int)Math.Floor(bounds.Top / Level.TileSize);
            int bottom = (int)Math.Floor((bounds.Bottom - 1f) / Level.TileSize);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    TileType tile = level.GetTile(x, y);
                    if (tile != TileType.Ground && tile != TileType.Breakable) continue;

                    if (tile == TileType.Breakable && (player.Powered || player.Bursting))
                    {
                        BreakBlock(level, player, x, y, audio);
                        continue;
                    }

                    RectangleF tileRect = TileRect(x, y);
                    if (!player.Bounds.IntersectsWith(tileRect)) continue;

                    if (player.Velocity.X > 0f)
                    {
                        player.Position.X = tileRect.Left - player.Size.Width - Epsilon;
                    }
                    else if (player.Velocity.X < 0f)
                    {
                        player.Position.X = tileRect.Right + Epsilon;
                    }

                    player.Velocity.X = 0f;
                    bounds = player.Bounds;
                }
            }

            if (player.Position.X < 0f)
            {
                player.Position.X = 0f;
                player.Velocity.X = 0f;
            }
            else if (player.Position.X + player.Size.Width > level.WorldWidth)
            {
                player.Position.X = level.WorldWidth - player.Size.Width;
                player.Velocity.X = 0f;
            }
        }

        private static void MoveVertical(Player player, Level level, float dt, AudioManager audio, bool canBreak)
        {
            player.Position.Y += player.Velocity.Y * dt;
            RectangleF bounds = player.Bounds;

            int left = (int)Math.Floor(bounds.Left / Level.TileSize);
            int right = (int)Math.Floor((bounds.Right - 1f) / Level.TileSize);
            int top = (int)Math.Floor(bounds.Top / Level.TileSize);
            int bottom = (int)Math.Floor((bounds.Bottom - 1f) / Level.TileSize);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    TileType tile = level.GetTile(x, y);
                    if (tile != TileType.Ground && tile != TileType.Breakable) continue;

                    if (tile == TileType.Breakable && (player.Powered || player.Bursting))
                    {
                        BreakBlock(level, player, x, y, audio);
                        continue;
                    }

                    RectangleF tileRect = TileRect(x, y);
                    if (!player.Bounds.IntersectsWith(tileRect)) continue;

                    if (player.Velocity.Y > 0f)
                    {
                        player.Position.Y = tileRect.Top - player.Size.Height - Epsilon;
                        player.Velocity.Y = 0f;
                        player.OnGround = true;
                    }
                    else if (player.Velocity.Y < 0f)
                    {
                        if (tile == TileType.Breakable && canBreak)
                        {
                            BreakBlock(level, player, x, y, audio);
                        }

                        player.Position.Y = tileRect.Bottom + Epsilon;
                        player.Velocity.Y = 0f;
                    }

                    bounds = player.Bounds;
                }
            }
        }

        private static void ResolveMovingPlatforms(Player player, Level level, float previousY)
        {
            RectangleF playerBounds = player.Bounds;
            float previousBottom = previousY + player.Size.Height;

            for (int i = 0; i < level.MovingPlatforms.Count; i++)
            {
                MovingPlatform platform = level.MovingPlatforms[i];
                RectangleF platformBounds = platform.Bounds;
                RectangleF previousPlatformBounds = platform.PreviousBounds;

                bool horizontalOverlap = playerBounds.Right > platformBounds.Left + 3f && playerBounds.Left < platformBounds.Right - 3f;
                bool wasAbove = previousBottom <= previousPlatformBounds.Top + 9f;
                bool isLanding = playerBounds.Bottom >= platformBounds.Top && playerBounds.Bottom <= platformBounds.Top + 24f;

                if (horizontalOverlap && wasAbove && isLanding && player.Velocity.Y >= 0f)
                {
                    player.Position.Y = platformBounds.Top - player.Size.Height - Epsilon;
                    player.Position.X += platform.Delta.X;
                    player.Velocity.Y = 0f;
                    player.OnGround = true;
                    playerBounds = player.Bounds;
                }
            }
        }

        private static void MoveEnemyHorizontal(Enemy enemy, Level level, float dt)
        {
            enemy.Position.X += enemy.Velocity.X * dt;
            RectangleF bounds = enemy.Bounds;

            int left = (int)Math.Floor(bounds.Left / Level.TileSize);
            int right = (int)Math.Floor((bounds.Right - 1f) / Level.TileSize);
            int top = (int)Math.Floor(bounds.Top / Level.TileSize);
            int bottom = (int)Math.Floor((bounds.Bottom - 1f) / Level.TileSize);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    if (!level.IsSolidTile(x, y)) continue;

                    RectangleF tileRect = TileRect(x, y);
                    if (!enemy.Bounds.IntersectsWith(tileRect)) continue;

                    if (enemy.Velocity.X > 0f)
                    {
                        enemy.Position.X = tileRect.Left - enemy.Size.Width - Epsilon;
                    }
                    else if (enemy.Velocity.X < 0f)
                    {
                        enemy.Position.X = tileRect.Right + Epsilon;
                    }

                    enemy.Velocity.X = 0f;
                    enemy.Direction *= -1;
                }
            }
        }

        private static void MoveEnemyVertical(Enemy enemy, Level level, float dt)
        {
            enemy.Position.Y += enemy.Velocity.Y * dt;
            RectangleF bounds = enemy.Bounds;

            int left = (int)Math.Floor(bounds.Left / Level.TileSize);
            int right = (int)Math.Floor((bounds.Right - 1f) / Level.TileSize);
            int top = (int)Math.Floor(bounds.Top / Level.TileSize);
            int bottom = (int)Math.Floor((bounds.Bottom - 1f) / Level.TileSize);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    if (!level.IsSolidTile(x, y)) continue;

                    RectangleF tileRect = TileRect(x, y);
                    if (!enemy.Bounds.IntersectsWith(tileRect)) continue;

                    if (enemy.Velocity.Y > 0f)
                    {
                        enemy.Position.Y = tileRect.Top - enemy.Size.Height - Epsilon;
                        enemy.Velocity.Y = 0f;
                        enemy.OnGround = true;
                    }
                    else if (enemy.Velocity.Y < 0f)
                    {
                        enemy.Position.Y = tileRect.Bottom + Epsilon;
                        enemy.Velocity.Y = 0f;
                    }
                }
            }
        }

        private static RectangleF TileRect(int x, int y)
        {
            return new RectangleF(x * Level.TileSize, y * Level.TileSize, Level.TileSize, Level.TileSize);
        }

        private static void BreakBlock(Level level, Player player, int x, int y, AudioManager audio)
        {
            level.BreakTile(x, y);
            player.AddScore(75);
            audio.PlayBreak();
        }
    }
}
