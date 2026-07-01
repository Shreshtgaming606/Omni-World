using System;
using System.Drawing;

namespace OmniWorld
{
    public sealed class Enemy : Entity
    {
        public EnemyKind Kind;
        public int Direction;
        public float LeftBound;
        public float RightBound;
        public bool OnGround;
        public float StunTimer;
        public float BrainTimer;
        public float HomeY;

        public Enemy(EnemyKind kind, float x, float y, float leftBound, float rightBound)
            : base(x, y, kind == EnemyKind.Seeker || kind == EnemyKind.Drifter ? 28f : 26f, kind == EnemyKind.Hopper ? 30f : kind == EnemyKind.Drifter ? 22f : 24f)
        {
            Kind = kind;
            Direction = 1;
            LeftBound = leftBound;
            RightBound = rightBound;
            OnGround = false;
            StunTimer = 0f;
            BrainTimer = 0f;
            HomeY = y;
        }

        public void Update(Level level, Player player, float dt)
        {
            if (!Active) return;

            BrainTimer += dt;

            if (Kind == EnemyKind.Roller)
            {
                Velocity.X = Direction * 58f;
            }
            else if (Kind == EnemyKind.Hopper)
            {
                Velocity.X = Direction * 36f;
                if (OnGround && BrainTimer > 1.2f)
                {
                    Velocity.Y = -420f;
                    BrainTimer = 0f;
                }
            }
            else if (Kind == EnemyKind.Seeker)
            {
                float distanceToPlayer = player.Position.X - Position.X;
                if (Math.Abs(distanceToPlayer) < 330f)
                {
                    Direction = distanceToPlayer >= 0f ? 1 : -1;
                    Velocity.X = Direction * 82f;
                }
                else
                {
                    Velocity.X = Direction * 44f;
                }
            }
            else if (Kind == EnemyKind.Drifter)
            {
                float distanceToPlayer = player.Position.X - Position.X;
                if (Math.Abs(distanceToPlayer) < 230f)
                {
                    Direction = distanceToPlayer >= 0f ? 1 : -1;
                }

                Velocity.X = Direction * 62f;
                Position.X += Velocity.X * dt;
                Position.Y = HomeY + (float)Math.Sin(BrainTimer * 2.7f) * 22f;

                if (Position.X < LeftBound)
                {
                    Position.X = LeftBound;
                    Direction = 1;
                }
                else if (Position.X + Size.Width > RightBound)
                {
                    Position.X = RightBound - Size.Width;
                    Direction = -1;
                }

                return;
            }

            Velocity.Y += 1250f * dt;
            if (Velocity.Y > 720f) Velocity.Y = 720f;

            Physics.MoveEnemy(this, level, dt);

            if (Position.X < LeftBound)
            {
                Position.X = LeftBound;
                Direction = 1;
            }
            else if (Position.X + Size.Width > RightBound)
            {
                Position.X = RightBound - Size.Width;
                Direction = -1;
            }

            if (Kind == EnemyKind.Roller || Kind == EnemyKind.Seeker)
            {
                float probeX = Direction > 0 ? Position.X + Size.Width + 4f : Position.X - 4f;
                float probeY = Position.Y + Size.Height + 8f;
                if (!level.IsSolidWorld(probeX, probeY))
                {
                    Direction *= -1;
                }
            }
        }

        public void Defeat(Player player, AudioManager audio)
        {
            Active = false;
            player.Velocity.Y = -330f;
            player.AddScore(250);
            audio.PlayStomp();
        }

        public RectangleF StompWindow
        {
            get { return new RectangleF(Position.X + 2f, Position.Y - 8f, Size.Width - 4f, 16f); }
        }
    }
}
