using System.Drawing;

namespace OmniWorld
{
    public abstract class Entity
    {
        public Vec2 Position;
        public Vec2 Velocity;
        public SizeF Size;
        public bool Active;

        protected Entity(float x, float y, float width, float height)
        {
            Position = new Vec2(x, y);
            Velocity = Vec2.Zero;
            Size = new SizeF(width, height);
            Active = true;
        }

        public RectangleF Bounds
        {
            get { return new RectangleF(Position.X, Position.Y, Size.Width, Size.Height); }
        }
    }

    public sealed class Collectible
    {
        public CollectibleKind Kind;
        public Vec2 Position;
        public SizeF Size;
        public bool Collected;
        public float BobTimer;

        public Collectible(CollectibleKind kind, float x, float y)
        {
            Kind = kind;
            Position = new Vec2(x, y);
            Size = kind == CollectibleKind.FluxCore || kind == CollectibleKind.BurstCell ? new SizeF(24f, 24f) : new SizeF(18f, 18f);
            Collected = false;
            BobTimer = 0f;
        }

        public RectangleF Bounds
        {
            get { return new RectangleF(Position.X, Position.Y, Size.Width, Size.Height); }
        }
    }

    public sealed class MovingPlatform
    {
        public Vec2 Start;
        public Vec2 End;
        public Vec2 Position;
        public Vec2 PreviousPosition;
        public SizeF Size;
        public float Speed;
        private int direction;

        public MovingPlatform(float startX, float startY, float endX, float endY, float width, float height, float speed)
        {
            Start = new Vec2(startX, startY);
            End = new Vec2(endX, endY);
            Position = Start;
            PreviousPosition = Start;
            Size = new SizeF(width, height);
            Speed = speed;
            direction = 1;
        }

        public RectangleF Bounds
        {
            get { return new RectangleF(Position.X, Position.Y, Size.Width, Size.Height); }
        }

        public RectangleF PreviousBounds
        {
            get { return new RectangleF(PreviousPosition.X, PreviousPosition.Y, Size.Width, Size.Height); }
        }

        public Vec2 Delta
        {
            get { return Position - PreviousPosition; }
        }

        public void Update(float dt)
        {
            PreviousPosition = Position;

            Vec2 target = direction > 0 ? End : Start;
            Vec2 toTarget = target - Position;
            float distance = (float)System.Math.Sqrt(toTarget.X * toTarget.X + toTarget.Y * toTarget.Y);

            if (distance < 0.001f)
            {
                direction *= -1;
                return;
            }

            float step = Speed * dt;
            if (step >= distance)
            {
                Position = target;
                direction *= -1;
            }
            else
            {
                Position = new Vec2(Position.X + toTarget.X / distance * step, Position.Y + toTarget.Y / distance * step);
            }
        }
    }

    public sealed class Checkpoint
    {
        public RectangleF Bounds;
        public Vec2 RespawnPoint;
        public bool Activated;

        public Checkpoint(float x, float groundY)
        {
            Bounds = new RectangleF(x + 5f, groundY - 64f, 22f, 64f);
            RespawnPoint = new Vec2(x, groundY - 32f);
            Activated = false;
        }
    }

    public sealed class BouncePad
    {
        public RectangleF Bounds;
        public float PulseTimer;

        public BouncePad(float x, float groundY)
        {
            Bounds = new RectangleF(x, groundY - 14f, 34f, 14f);
            PulseTimer = 0f;
        }
    }

    public sealed class GoalGate
    {
        public RectangleF Bounds;

        public GoalGate(float x, float groundY)
        {
            Bounds = new RectangleF(x, groundY - 96f, 40f, 96f);
        }
    }
}
