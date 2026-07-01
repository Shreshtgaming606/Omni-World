using System.Drawing;

namespace OmniWorld
{
    public sealed class Player : Entity
    {
        private const float WalkSpeed = 165f;
        private const float RunSpeed = 250f;
        private const float GroundAccel = 2100f;
        private const float AirAccel = 1350f;
        private const float Friction = 2500f;
        private const float Gravity = 1450f;
        private const float JumpVelocity = -565f;
        private const float MaxFallSpeed = 780f;
        private const float BurstSpeed = 430f;
        private const float BurstDuration = 0.17f;
        private const float BurstCooldownDuration = 0.82f;

        public int Lives;
        public int Health;
        public int MaxHealth;
        public int Score;
        public int Orbs;
        public bool OnGround;
        public int Facing;
        public Vec2 RespawnPoint;
        public float InvincibleTimer;
        public float PowerTimer;
        public float BurstTimer;
        public float BurstCooldown;
        public bool BurstStartedThisFrame;

        private float coyoteTimer;
        private float jumpBufferTimer;

        public Player(float x, float y)
            : base(x, y, 24f, 30f)
        {
            MaxHealth = 3;
            Lives = 3;
            Health = MaxHealth;
            Facing = 1;
            RespawnPoint = new Vec2(x, y);
        }

        public bool Powered
        {
            get { return PowerTimer > 0f; }
        }

        public bool Bursting
        {
            get { return BurstTimer > 0f; }
        }

        public float BurstCharge
        {
            get { return BurstCooldown <= 0f ? 1f : 1f - GameMath.Clamp(BurstCooldown / BurstCooldownDuration, 0f, 1f); }
        }

        public void ResetForCourse(Vec2 spawn)
        {
            Position = spawn;
            Velocity = Vec2.Zero;
            RespawnPoint = spawn;
            Lives = 3;
            Health = MaxHealth;
            Score = 0;
            Orbs = 0;
            OnGround = false;
            Facing = 1;
            InvincibleTimer = 0f;
            PowerTimer = 0f;
            BurstTimer = 0f;
            BurstCooldown = 0f;
            BurstStartedThisFrame = false;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }

        public void Respawn()
        {
            Position = RespawnPoint;
            Velocity = Vec2.Zero;
            Health = MaxHealth;
            OnGround = false;
            InvincibleTimer = 1.4f;
            PowerTimer = 0f;
            BurstTimer = 0f;
            BurstCooldown = 0.35f;
            BurstStartedThisFrame = false;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }

        public void UpdateInput(InputState input, float dt, AudioManager audio)
        {
            BurstStartedThisFrame = false;
            if (InvincibleTimer > 0f) InvincibleTimer -= dt;
            if (PowerTimer > 0f) PowerTimer -= dt;
            if (BurstCooldown > 0f) BurstCooldown -= dt;
            if (BurstTimer > 0f) BurstTimer -= dt;

            float move = 0f;
            if (input.Left) move -= 1f;
            if (input.Right) move += 1f;

            if (move != 0f) Facing = move > 0f ? 1 : -1;

            if (input.BurstPressed && BurstCooldown <= 0f)
            {
                BurstTimer = BurstDuration;
                BurstCooldown = BurstCooldownDuration;
                BurstStartedThisFrame = true;
                Velocity.X = Facing * (Powered ? BurstSpeed * 1.12f : BurstSpeed);
                if (Velocity.Y > 60f) Velocity.Y = 60f;
                if (Velocity.Y < -260f) Velocity.Y = -260f;
                audio.PlayDash();
            }

            if (Bursting)
            {
                Velocity.X = Facing * (Powered ? BurstSpeed * 1.12f : BurstSpeed);
            }
            else
            {
                float maxSpeed = input.Run ? RunSpeed : WalkSpeed;
                if (Powered) maxSpeed *= 1.16f;

                float targetSpeed = move * maxSpeed;
                float accel = OnGround ? GroundAccel : AirAccel;
                if (move == 0f && OnGround)
                {
                    Velocity.X = GameMath.Approach(Velocity.X, 0f, Friction * dt);
                }
                else
                {
                    Velocity.X = GameMath.Approach(Velocity.X, targetSpeed, accel * dt);
                }
            }

            if (OnGround) coyoteTimer = 0.11f;
            else coyoteTimer -= dt;

            if (input.JumpPressed) jumpBufferTimer = 0.12f;
            else jumpBufferTimer -= dt;

            if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            {
                Velocity.Y = JumpVelocity;
                OnGround = false;
                coyoteTimer = 0f;
                jumpBufferTimer = 0f;
                audio.PlayJump();
            }

            // Variable jump height: releasing jump early trims upward velocity.
            if (!input.JumpHeld && Velocity.Y < -190f)
            {
                Velocity.Y = -190f;
            }

            Velocity.Y += (Bursting ? Gravity * 0.38f : Gravity) * dt;
            if (Velocity.Y > MaxFallSpeed) Velocity.Y = MaxFallSpeed;
        }

        public void AddScore(int amount)
        {
            Score += amount;
            if (Score < 0) Score = 0;
        }

        public bool TakeDamage(AudioManager audio)
        {
            if (InvincibleTimer > 0f || Powered) return false;

            Health--;
            InvincibleTimer = 1.1f;
            Velocity.X = -Facing * 135f;
            Velocity.Y = -250f;
            audio.PlayHurt();

            return Health <= 0;
        }
    }
}
