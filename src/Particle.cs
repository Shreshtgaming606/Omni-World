using System;
using System.Collections.Generic;
using System.Drawing;

namespace OmniWorld
{
    public enum ParticleKind
    {
        Smoke,
        Spark,
        Shard
    }

    public struct Particle
    {
        public ParticleKind Kind;
        public Vec2 Position;
        public Vec2 Velocity;
        public Color Color;
        public float Age;
        public float Life;
        public float Size;
        public float Gravity;
    }

    public sealed class ParticleSystem
    {
        private readonly List<Particle> particles;
        private readonly Random random;

        public ParticleSystem()
        {
            particles = new List<Particle>();
            random = new Random(31);
        }

        public IList<Particle> Particles
        {
            get { return particles; }
        }

        public void Clear()
        {
            particles.Clear();
        }

        public void Update(float dt)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle particle = particles[i];
                particle.Age += dt;
                if (particle.Age >= particle.Life)
                {
                    particles.RemoveAt(i);
                    continue;
                }

                particle.Velocity.Y += particle.Gravity * dt;
                particle.Position += particle.Velocity * dt;
                particles[i] = particle;
            }
        }

        public void EmitLandingDust(RectangleF bounds, float fallSpeed)
        {
            if (fallSpeed < 170f) return;

            int count = fallSpeed > 520f ? 12 : 7;
            Vec2 origin = new Vec2(bounds.Left + bounds.Width / 2f, bounds.Bottom - 2f);
            for (int i = 0; i < count; i++)
            {
                AddParticle(
                    ParticleKind.Smoke,
                    new Vec2(origin.X + RandomRange(-10f, 10f), origin.Y),
                    new Vec2(RandomRange(-85f, 85f), RandomRange(-58f, -18f)),
                    Color.FromArgb(196, 217, 187),
                    RandomRange(0.28f, 0.46f),
                    RandomRange(5f, 10f),
                    120f);
            }
        }

        public void EmitRunDust(RectangleF bounds, int facing)
        {
            Vec2 origin = new Vec2(facing > 0 ? bounds.Left + 2f : bounds.Right - 2f, bounds.Bottom - 2f);
            for (int i = 0; i < 3; i++)
            {
                AddParticle(
                    ParticleKind.Smoke,
                    new Vec2(origin.X + RandomRange(-3f, 3f), origin.Y),
                    new Vec2(-facing * RandomRange(45f, 90f), RandomRange(-36f, -10f)),
                    Color.FromArgb(186, 205, 174),
                    RandomRange(0.22f, 0.34f),
                    RandomRange(3f, 6f),
                    80f);
            }
        }

        public void EmitBurstTrail(RectangleF bounds, int facing, bool bright)
        {
            Vec2 origin = new Vec2(facing > 0 ? bounds.Left + 2f : bounds.Right - 2f, bounds.Top + bounds.Height * 0.52f);
            Color spark = bright ? Color.FromArgb(255, 247, 91) : Color.FromArgb(64, 236, 210);

            for (int i = 0; i < 7; i++)
            {
                AddParticle(
                    i % 2 == 0 ? ParticleKind.Spark : ParticleKind.Shard,
                    new Vec2(origin.X + RandomRange(-5f, 5f), origin.Y + RandomRange(-11f, 11f)),
                    new Vec2(-facing * RandomRange(125f, 260f), RandomRange(-70f, 70f)),
                    spark,
                    RandomRange(0.18f, 0.32f),
                    RandomRange(3f, 7f),
                    20f);
            }
        }

        public void EmitCollect(RectangleF bounds, Color color)
        {
            EmitBurst(Center(bounds), color, 12, 120f, 0.38f, 4f, ParticleKind.Spark, -70f);
        }

        public void EmitPower(RectangleF bounds)
        {
            EmitBurst(Center(bounds), Color.FromArgb(64, 236, 210), 24, 175f, 0.55f, 5f, ParticleKind.Spark, -110f);
            EmitBurst(Center(bounds), Color.FromArgb(255, 247, 91), 12, 120f, 0.45f, 4f, ParticleKind.Shard, 80f);
        }

        public void EmitBurstRecharge(RectangleF bounds)
        {
            EmitBurst(Center(bounds), Color.FromArgb(255, 247, 91), 20, 180f, 0.44f, 5f, ParticleKind.Spark, -130f);
            EmitBurst(Center(bounds), Color.FromArgb(64, 236, 210), 14, 140f, 0.40f, 4f, ParticleKind.Shard, 30f);
        }

        public void EmitDataCore(RectangleF bounds)
        {
            Vec2 center = Center(bounds);
            EmitBurst(center, Color.FromArgb(255, 247, 91), 24, 210f, 0.55f, 5f, ParticleKind.Spark, -145f);
            EmitBurst(center, Color.FromArgb(225, 133, 255), 18, 175f, 0.52f, 5f, ParticleKind.Shard, 40f);
            EmitBurst(center, Color.FromArgb(64, 236, 210), 14, 145f, 0.46f, 4f, ParticleKind.Spark, -50f);
        }

        public void EmitShieldBlock(RectangleF bounds)
        {
            Vec2 center = Center(bounds);
            EmitBurst(center, Color.FromArgb(122, 208, 255), 22, 170f, 0.42f, 5f, ParticleKind.Spark, -80f);
            EmitBurst(center, Color.FromArgb(255, 255, 255), 12, 115f, 0.34f, 4f, ParticleKind.Shard, 10f);
        }

        public void EmitBounce(RectangleF bounds)
        {
            Vec2 origin = new Vec2(bounds.Left + bounds.Width / 2f, bounds.Top + 2f);
            for (int i = 0; i < 18; i++)
            {
                AddParticle(
                    i % 3 == 0 ? ParticleKind.Shard : ParticleKind.Spark,
                    new Vec2(origin.X + RandomRange(-16f, 16f), origin.Y + RandomRange(-3f, 3f)),
                    new Vec2(RandomRange(-105f, 105f), RandomRange(-210f, -70f)),
                    i % 2 == 0 ? Color.FromArgb(255, 247, 91) : Color.FromArgb(64, 236, 210),
                    RandomRange(0.26f, 0.48f),
                    RandomRange(3f, 6f),
                    120f);
            }
        }

        public void EmitSlamStart(RectangleF bounds)
        {
            Vec2 origin = new Vec2(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height * 0.35f);
            for (int i = 0; i < 16; i++)
            {
                AddParticle(
                    i % 2 == 0 ? ParticleKind.Spark : ParticleKind.Shard,
                    new Vec2(origin.X + RandomRange(-10f, 10f), origin.Y + RandomRange(-8f, 8f)),
                    new Vec2(RandomRange(-60f, 60f), RandomRange(110f, 260f)),
                    i % 2 == 0 ? Color.FromArgb(255, 247, 91) : Color.FromArgb(64, 236, 210),
                    RandomRange(0.16f, 0.30f),
                    RandomRange(3f, 6f),
                    30f);
            }
        }

        public void EmitSlamImpact(RectangleF bounds)
        {
            Vec2 origin = new Vec2(bounds.Left + bounds.Width / 2f, bounds.Bottom - 2f);
            for (int i = 0; i < 34; i++)
            {
                AddParticle(
                    i % 3 == 0 ? ParticleKind.Smoke : ParticleKind.Shard,
                    new Vec2(origin.X + RandomRange(-34f, 34f), origin.Y + RandomRange(-5f, 5f)),
                    new Vec2(RandomRange(-240f, 240f), RandomRange(-150f, -28f)),
                    i % 3 == 0 ? Color.FromArgb(196, 217, 187) : Color.FromArgb(255, 247, 91),
                    RandomRange(0.28f, 0.54f),
                    RandomRange(4f, 10f),
                    180f);
            }
        }

        public void EmitImpact(RectangleF bounds, Color color)
        {
            EmitBurst(Center(bounds), color, 15, 150f, 0.34f, 5f, ParticleKind.Shard, 180f);
        }

        public void EmitCheckpoint(RectangleF bounds)
        {
            EmitBurst(new Vec2(bounds.Left + 12f, bounds.Top + 18f), Color.FromArgb(245, 226, 91), 18, 140f, 0.52f, 4f, ParticleKind.Spark, -120f);
        }

        public void EmitGoal(RectangleF bounds)
        {
            Vec2 center = Center(bounds);
            EmitBurst(center, Color.FromArgb(75, 238, 211), 40, 210f, 0.75f, 5f, ParticleKind.Spark, -90f);
            EmitBurst(center, Color.FromArgb(255, 255, 255), 20, 145f, 0.55f, 4f, ParticleKind.Shard, 50f);
        }

        public void EmitDamage(RectangleF bounds)
        {
            EmitBurst(Center(bounds), Color.FromArgb(255, 95, 126), 18, 165f, 0.42f, 5f, ParticleKind.Shard, 210f);
        }

        private void EmitBurst(Vec2 origin, Color color, int count, float speed, float life, float size, ParticleKind kind, float gravity)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = RandomRange(0f, (float)(Math.PI * 2.0));
                float velocity = RandomRange(speed * 0.35f, speed);
                AddParticle(
                    kind,
                    new Vec2(origin.X + RandomRange(-4f, 4f), origin.Y + RandomRange(-4f, 4f)),
                    new Vec2((float)Math.Cos(angle) * velocity, (float)Math.Sin(angle) * velocity),
                    color,
                    RandomRange(life * 0.65f, life),
                    RandomRange(size * 0.65f, size * 1.25f),
                    gravity);
            }
        }

        private void AddParticle(ParticleKind kind, Vec2 position, Vec2 velocity, Color color, float life, float size, float gravity)
        {
            if (particles.Count > 260)
            {
                particles.RemoveAt(0);
            }

            Particle particle = new Particle();
            particle.Kind = kind;
            particle.Position = position;
            particle.Velocity = velocity;
            particle.Color = color;
            particle.Age = 0f;
            particle.Life = life;
            particle.Size = size;
            particle.Gravity = gravity;
            particles.Add(particle);
        }

        private float RandomRange(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private static Vec2 Center(RectangleF bounds)
        {
            return new Vec2(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height / 2f);
        }
    }
}
