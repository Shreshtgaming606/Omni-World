using System;
using System.Drawing;

namespace OmniWorld
{
    public sealed class Game
    {
        private const int ViewWidth = 960;
        private const int ViewHeight = 540;
        public const int ObjectiveTotal = 4;

        public GameScreen Screen;
        public Level CurrentLevel;
        public Player Player;
        public Vec2 Camera;
        public Vec2 CameraShake;
        public ParticleSystem Particles;
        public int SelectedCourseIndex;
        public bool[] CourseCompleted;
        public int[] CourseBestScores;
        public int[] CourseBestDataCores;
        public int[] CourseDataCoreTotals;
        public int[] CourseBestObjectiveStars;
        public float CourseClearTimer;
        public int AirStompChain;
        public float ComboPopupTimer;
        public float WorldTime;

        private readonly AudioManager audio;
        private readonly string[] courseNames;
        private readonly string[] courseTaglines;
        private readonly Random random;
        private float shakeTimer;
        private float shakeDuration;
        private float shakeMagnitude;
        private float cameraLookAhead;
        private float runDustTimer;
        private float burstTrailTimer;

        public Game(AudioManager audioManager)
        {
            audio = audioManager;
            courseNames = new string[] { "Greenlit Grove", "Voltage Vale", "Crystal Canopy", "Emberworks", "Skyline Circuit", "Prism Reactor" };
            courseTaglines = new string[]
            {
                "Bright hills, friendly jumps, and starter enemies.",
                "Moving platforms, sharper hazards, and a tense finish.",
                "Glowing cliffs, longer gaps, and careful platform timing.",
                "Hot vents, heavy enemy pressure, and burst-cell routes.",
                "High platforms, precision bounce pads, and a fast final run.",
                "A neon finale with layered hazards and rare Data Cores."
            };
            CourseCompleted = new bool[courseNames.Length];
            CourseBestScores = new int[courseNames.Length];
            CourseBestDataCores = new int[courseNames.Length];
            CourseDataCoreTotals = new int[courseNames.Length];
            CourseBestObjectiveStars = new int[courseNames.Length];
            for (int i = 0; i < CourseDataCoreTotals.Length; i++)
            {
                CourseDataCoreTotals[i] = CountDataCores(LevelFactory.CreateCourse(i + 1));
            }

            SelectedCourseIndex = 0;
            Screen = GameScreen.CourseSelect;
            Player = new Player(64f, 400f);
            Camera = Vec2.Zero;
            CameraShake = Vec2.Zero;
            Particles = new ParticleSystem();
            CurrentLevel = LevelFactory.CreateCourse(1);
            random = new Random(17);
        }

        public string[] CourseNames
        {
            get { return courseNames; }
        }

        public string[] CourseTaglines
        {
            get { return courseTaglines; }
        }

        public int CurrentDataCoreTotal
        {
            get { return CourseDataCoreTotals[SelectedCourseIndex]; }
        }

        public int CurrentObjectiveStars
        {
            get { return CountCurrentObjectiveStars(); }
        }

        public void Update(InputState input, float dt)
        {
            if (dt > 0.033f) dt = 0.033f;
            WorldTime += dt;

            if (Screen == GameScreen.CourseSelect)
            {
                UpdateCourseSelect(input);
            }
            else if (Screen == GameScreen.Playing)
            {
                UpdatePlaying(input, dt);
            }
            else if (Screen == GameScreen.Paused)
            {
                UpdatePaused(input);
            }
            else if (Screen == GameScreen.CourseComplete)
            {
                UpdateCourseComplete(input, dt);
            }
            else if (Screen == GameScreen.GameOver)
            {
                UpdateGameOver(input);
            }
            else if (Screen == GameScreen.Victory)
            {
                UpdateVictory(input);
            }

            UpdateFeedback(dt);
        }

        public void StartCourse(int index)
        {
            SelectedCourseIndex = GameMath.Clamp(index, 0, courseNames.Length - 1);
            CurrentLevel = LevelFactory.CreateCourse(SelectedCourseIndex + 1);
            Player.ResetForCourse(CurrentLevel.Spawn);
            Camera = Vec2.Zero;
            CameraShake = Vec2.Zero;
            Particles.Clear();
            AirStompChain = 0;
            ComboPopupTimer = 0f;
            shakeTimer = 0f;
            cameraLookAhead = 0f;
            runDustTimer = 0f;
            burstTrailTimer = 0f;
            Screen = GameScreen.Playing;
            audio.StartMusic();
        }

        private void UpdateCourseSelect(InputState input)
        {
            audio.StopMusic();

            if (input.UpPressed)
            {
                SelectedCourseIndex--;
                if (SelectedCourseIndex < 0) SelectedCourseIndex = courseNames.Length - 1;
            }
            else if (input.DownPressed)
            {
                SelectedCourseIndex++;
                if (SelectedCourseIndex >= courseNames.Length) SelectedCourseIndex = 0;
            }

            if (input.ConfirmPressed)
            {
                StartCourse(SelectedCourseIndex);
            }
        }

        private void UpdatePaused(InputState input)
        {
            if (input.PausePressed || input.ConfirmPressed)
            {
                Screen = GameScreen.Playing;
                audio.StartMusic();
            }
            else if (input.RestartPressed)
            {
                StartCourse(SelectedCourseIndex);
            }
            else if (input.MenuPressed)
            {
                Screen = GameScreen.CourseSelect;
                audio.StopMusic();
            }
        }

        private void UpdateCourseComplete(InputState input, float dt)
        {
            CourseClearTimer += dt;

            if (input.ConfirmPressed || CourseClearTimer > 4.0f)
            {
                if (AllCoursesCompleted())
                {
                    Screen = GameScreen.Victory;
                }
                else
                {
                    Screen = GameScreen.CourseSelect;
                }

                audio.StopMusic();
            }
        }

        private void UpdateGameOver(InputState input)
        {
            audio.StopMusic();

            if (input.RestartPressed)
            {
                StartCourse(SelectedCourseIndex);
            }
            else if (input.ConfirmPressed || input.MenuPressed)
            {
                Screen = GameScreen.CourseSelect;
            }
        }

        private void UpdateVictory(InputState input)
        {
            audio.StopMusic();
            if (input.ConfirmPressed || input.MenuPressed)
            {
                Screen = GameScreen.CourseSelect;
            }
        }

        private void UpdatePlaying(InputState input, float dt)
        {
            if (input.PausePressed)
            {
                Screen = GameScreen.Paused;
                audio.StopMusic();
                return;
            }

            // Platform motion is advanced before player physics so riders inherit the platform delta.
            for (int i = 0; i < CurrentLevel.MovingPlatforms.Count; i++)
            {
                CurrentLevel.MovingPlatforms[i].Update(dt);
            }

            for (int i = 0; i < CurrentLevel.BouncePads.Count; i++)
            {
                CurrentLevel.BouncePads[i].PulseTimer += dt;
            }

            bool wasOnGround = Player.OnGround;

            Player.UpdateInput(input, dt, audio);
            if (Player.BurstStartedThisFrame)
            {
                Particles.EmitBurstTrail(Player.Bounds, Player.Facing, Player.Powered);
                AddShake(2.4f, 0.14f);
                burstTrailTimer = 0.025f;
            }
            if (Player.SlamStartedThisFrame)
            {
                Particles.EmitSlamStart(Player.Bounds);
                AddShake(1.6f, 0.11f);
            }

            float fallSpeed = Player.Velocity.Y;
            Physics.MovePlayer(Player, CurrentLevel, dt, audio);
            ResolveMovementFeedback(wasOnGround, fallSpeed, dt);
            ResolveBurstFeedback(dt);
            ResolveBouncePads();
            ResolveGameplayCollisions();

            for (int i = 0; i < CurrentLevel.Enemies.Count; i++)
            {
                CurrentLevel.Enemies[i].Update(CurrentLevel, Player, dt);
            }

            ResolveEnemyCollisions();
            ResolveCollectibles(dt);
            ResolveCheckpoints();
            ResolveHazardsAndPits();
            ResolveGoal();
            UpdateCamera(dt);
        }

        private void ResolveGameplayCollisions()
        {
            if (Player.Position.Y > CurrentLevel.WorldHeight + 128f)
            {
                LoseLife();
            }
        }

        private void ResolveEnemyCollisions()
        {
            RectangleF playerBounds = Player.Bounds;

            for (int i = 0; i < CurrentLevel.Enemies.Count; i++)
            {
                Enemy enemy = CurrentLevel.Enemies[i];
                if (!enemy.Active) continue;

                RectangleF enemyBounds = enemy.Bounds;
                if (!playerBounds.IntersectsWith(enemyBounds)) continue;

                bool stomp = Player.Velocity.Y > 80f && Player.Bounds.Bottom - enemy.Bounds.Top < 18f;

                if (stomp)
                {
                    AirStompChain++;
                    enemy.Defeat(Player, audio);
                    Player.EnemiesDefeated++;
                    if (AirStompChain > 1)
                    {
                        Player.AddScore((AirStompChain - 1) * 100);
                        ComboPopupTimer = 1.15f;
                    }

                    Particles.EmitImpact(enemyBounds, Color.FromArgb(255, 247, 91));
                    AddShake(2.5f, 0.13f);
                    playerBounds = Player.Bounds;
                }
                else if (Player.Powered || Player.Bursting || Player.Slamming)
                {
                    bool slamAttack = Player.Slamming;
                    enemy.Active = false;
                    Player.EnemiesDefeated++;
                    if (slamAttack)
                    {
                        Player.SlamHits++;
                        Player.Velocity.Y = -265f;
                        Player.Slamming = false;
                    }

                    Player.AddScore(slamAttack ? 325 : Player.Bursting ? 275 : 200);
                    Particles.EmitImpact(enemyBounds, Player.Bursting || slamAttack ? Color.FromArgb(255, 247, 91) : Color.FromArgb(64, 236, 210));
                    AddShake(Player.Bursting || slamAttack ? 3.2f : 2.0f, 0.14f);
                    audio.PlayStomp();
                }
                else
                {
                    bool canDamage = Player.InvincibleTimer <= 0f && !Player.Powered;
                    bool emptyHealth = Player.TakeDamage(audio);
                    if (Player.ShieldBlockedThisFrame)
                    {
                        Particles.EmitShieldBlock(Player.Bounds);
                        AddShake(3.2f, 0.18f);
                        AirStompChain = 0;
                    }
                    else if (canDamage)
                    {
                        Particles.EmitDamage(Player.Bounds);
                        AddShake(5.0f, 0.25f);
                        AirStompChain = 0;
                    }

                    if (emptyHealth)
                    {
                        LoseLife();
                        return;
                    }
                }
            }
        }

        private void ResolveBurstFeedback(float dt)
        {
            if (Player.Bursting)
            {
                burstTrailTimer -= dt;
                if (burstTrailTimer <= 0f)
                {
                    Particles.EmitBurstTrail(Player.Bounds, Player.Facing, Player.Powered);
                    burstTrailTimer = 0.035f;
                }
            }
            else
            {
                burstTrailTimer = 0f;
            }
        }

        private void ResolveCollectibles(float dt)
        {
            RectangleF playerBounds = Player.Bounds;

            for (int i = 0; i < CurrentLevel.Collectibles.Count; i++)
            {
                Collectible item = CurrentLevel.Collectibles[i];
                item.BobTimer += dt;
                if (item.Collected || !playerBounds.IntersectsWith(item.Bounds)) continue;

                item.Collected = true;

                if (item.Kind == CollectibleKind.EnergyOrb)
                {
                    Player.Orbs++;
                    Player.AddScore(25);
                    Particles.EmitCollect(item.Bounds, Color.FromArgb(255, 218, 70));
                    audio.PlayCollect();

                    if (Player.Orbs % 25 == 0)
                    {
                        Player.Lives++;
                    }
                }
                else if (item.Kind == CollectibleKind.FluxCore)
                {
                    Player.PowerTimer = 12f;
                    Player.AddScore(500);
                    Particles.EmitPower(item.Bounds);
                    AddShake(3.0f, 0.22f);
                    audio.PlayPower();
                }
                else if (item.Kind == CollectibleKind.Heart)
                {
                    if (Player.Health < Player.MaxHealth) Player.Health++;
                    Player.AddScore(150);
                    Particles.EmitCollect(item.Bounds, Color.FromArgb(255, 95, 126));
                    audio.PlayCollect();
                }
                else if (item.Kind == CollectibleKind.BurstCell)
                {
                    Player.BurstCooldown = 0f;
                    Player.AddScore(125);
                    Particles.EmitBurstRecharge(item.Bounds);
                    AddShake(1.7f, 0.12f);
                    audio.PlayDash();
                }
                else if (item.Kind == CollectibleKind.DataCore)
                {
                    Player.DataCores++;
                    Player.BurstCooldown = 0f;
                    Player.AddScore(750);
                    if (Player.DataCores == CurrentDataCoreTotal && CurrentDataCoreTotal > 0)
                    {
                        Player.Lives++;
                        Player.AddScore(500);
                    }

                    Particles.EmitDataCore(item.Bounds);
                    AddShake(4.4f, 0.30f);
                    audio.PlayDataCore();
                }
                else if (item.Kind == CollectibleKind.AegisCore)
                {
                    if (Player.AegisCharges < 2) Player.AegisCharges++;
                    Player.AddScore(350);
                    Particles.EmitShieldBlock(item.Bounds);
                    AddShake(2.0f, 0.14f);
                    audio.PlayShield();
                }
            }
        }

        private void ResolveBouncePads()
        {
            RectangleF playerBounds = Player.Bounds;

            for (int i = 0; i < CurrentLevel.BouncePads.Count; i++)
            {
                BouncePad pad = CurrentLevel.BouncePads[i];
                if (!playerBounds.IntersectsWith(pad.Bounds)) continue;
                if (Player.Velocity.Y < -80f) continue;

                Player.Position.Y = pad.Bounds.Top - Player.Size.Height - 0.5f;
                Player.Velocity.Y = -700f;
                Player.OnGround = false;
                Player.Slamming = false;
                Player.BurstCooldown = 0f;
                pad.PulseTimer = 0f;
                Particles.EmitBounce(pad.Bounds);
                AddShake(2.0f, 0.15f);
                audio.PlayBounce();
                break;
            }
        }

        private void ResolveCheckpoints()
        {
            RectangleF playerBounds = Player.Bounds;

            for (int i = 0; i < CurrentLevel.Checkpoints.Count; i++)
            {
                Checkpoint checkpoint = CurrentLevel.Checkpoints[i];
                if (playerBounds.IntersectsWith(checkpoint.Bounds))
                {
                    Player.RespawnPoint = checkpoint.RespawnPoint;
                    if (!checkpoint.Activated)
                    {
                        checkpoint.Activated = true;
                        Player.AddScore(300);
                        Particles.EmitCheckpoint(checkpoint.Bounds);
                        AddShake(1.5f, 0.12f);
                        audio.PlayCollect();
                    }
                }
            }
        }

        private void ResolveHazardsAndPits()
        {
            if (Physics.IsTouchingHazard(Player.Bounds, CurrentLevel))
            {
                bool canDamage = Player.InvincibleTimer <= 0f && !Player.Powered;
                bool emptyHealth = Player.TakeDamage(audio);
                if (Player.ShieldBlockedThisFrame)
                {
                    Particles.EmitShieldBlock(Player.Bounds);
                    AddShake(3.2f, 0.18f);
                    AirStompChain = 0;
                }
                else if (canDamage)
                {
                    Particles.EmitDamage(Player.Bounds);
                    AddShake(5.5f, 0.25f);
                    AirStompChain = 0;
                }

                if (emptyHealth)
                {
                    LoseLife();
                }
            }

            if (Player.Position.Y > CurrentLevel.WorldHeight + 64f)
            {
                LoseLife();
            }
        }

        private void ResolveGoal()
        {
            if (!Player.Bounds.IntersectsWith(CurrentLevel.Goal.Bounds)) return;

            CourseCompleted[SelectedCourseIndex] = true;
            Player.AddScore(1000);
            StoreCourseBest();
            CourseClearTimer = 0f;
            Screen = GameScreen.CourseComplete;
            Particles.EmitGoal(CurrentLevel.Goal.Bounds);
            AddShake(4.0f, 0.35f);
            audio.PlayGoal();
        }

        private void LoseLife()
        {
            Player.Lives--;
            if (Player.Lives <= 0)
            {
                Screen = GameScreen.GameOver;
                audio.StopMusic();
                return;
            }

            Player.Respawn();
            Particles.EmitDamage(Player.Bounds);
            AddShake(5.5f, 0.26f);
            AirStompChain = 0;
        }

        private void UpdateCamera(float dt)
        {
            float desiredLookAhead = Math.Abs(Player.Velocity.X) > 40f ? Player.Facing * 86f : 0f;
            cameraLookAhead = GameMath.Approach(cameraLookAhead, desiredLookAhead, 320f * dt);

            float verticalBias = Player.Slamming ? 0.45f : Player.Velocity.Y < -120f ? 0.62f : 0.56f;
            float targetX = Player.Position.X + Player.Size.Width * 0.5f - ViewWidth * 0.45f + cameraLookAhead;
            float targetY = Player.Position.Y + Player.Size.Height * 0.5f - ViewHeight * verticalBias;

            targetX = GameMath.Clamp(targetX, 0f, Math.Max(0f, CurrentLevel.WorldWidth - ViewWidth));
            targetY = GameMath.Clamp(targetY, 0f, Math.Max(0f, CurrentLevel.WorldHeight - ViewHeight));

            float smooth = 1f - (float)Math.Pow(0.001, dt);
            Camera = new Vec2(
                GameMath.Lerp(Camera.X, targetX, smooth),
                GameMath.Lerp(Camera.Y, targetY, smooth));
        }

        private void ResolveMovementFeedback(bool wasOnGround, float fallSpeed, float dt)
        {
            bool slamLanded = Player.Slamming && !wasOnGround && Player.OnGround;

            if (!wasOnGround && Player.OnGround)
            {
                Particles.EmitLandingDust(Player.Bounds, fallSpeed);
                if (fallSpeed > 520f)
                {
                    AddShake(1.8f, 0.11f);
                }

                if (slamLanded)
                {
                    ResolveCoreSlamImpact();
                }

                AirStompChain = 0;
            }

            if (Player.OnGround && Math.Abs(Player.Velocity.X) > 160f)
            {
                runDustTimer -= dt;
                if (runDustTimer <= 0f)
                {
                    Particles.EmitRunDust(Player.Bounds, Player.Facing);
                    runDustTimer = 0.075f;
                }
            }
            else
            {
                runDustTimer = 0f;
            }
        }

        private void UpdateFeedback(float dt)
        {
            Particles.Update(dt);

            if (ComboPopupTimer > 0f)
            {
                ComboPopupTimer -= dt;
            }

            if (shakeTimer > 0f)
            {
                shakeTimer -= dt;
                float fade = shakeDuration > 0f ? shakeTimer / shakeDuration : 0f;
                float strength = shakeMagnitude * Math.Max(0f, fade);
                CameraShake = new Vec2(RandomRange(-strength, strength), RandomRange(-strength, strength));
            }
            else
            {
                CameraShake = Vec2.Zero;
            }
        }

        private void AddShake(float magnitude, float duration)
        {
            if (magnitude < shakeMagnitude && shakeTimer > 0f) return;

            shakeMagnitude = magnitude;
            shakeDuration = duration;
            shakeTimer = duration;
        }

        private float RandomRange(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private void ResolveCoreSlamImpact()
        {
            RectangleF playerBounds = Player.Bounds;
            RectangleF shockwave = new RectangleF(playerBounds.Left - 58f, playerBounds.Bottom - 26f, playerBounds.Width + 116f, 58f);
            int defeated = 0;
            int broken = 0;

            for (int i = 0; i < CurrentLevel.Enemies.Count; i++)
            {
                Enemy enemy = CurrentLevel.Enemies[i];
                if (!enemy.Active || !enemy.Bounds.IntersectsWith(shockwave)) continue;

                enemy.Active = false;
                defeated++;
                Player.EnemiesDefeated++;
                Player.SlamHits++;
                Player.AddScore(325);
                Particles.EmitImpact(enemy.Bounds, Color.FromArgb(255, 247, 91));
            }

            int left = (int)Math.Floor(shockwave.Left / Level.TileSize);
            int right = (int)Math.Floor((shockwave.Right - 1f) / Level.TileSize);
            int top = (int)Math.Floor(shockwave.Top / Level.TileSize);
            int bottom = (int)Math.Floor((shockwave.Bottom - 1f) / Level.TileSize);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    if (CurrentLevel.GetTile(x, y) != TileType.Breakable) continue;

                    CurrentLevel.BreakTile(x, y);
                    broken++;
                    Player.AddScore(90);
                    Particles.EmitImpact(new RectangleF(x * Level.TileSize, y * Level.TileSize, Level.TileSize, Level.TileSize), Color.FromArgb(225, 133, 255));
                }
            }

            Player.Slamming = false;
            Player.BurstCooldown = Math.Min(Player.BurstCooldown, 0.25f);
            Particles.EmitSlamImpact(playerBounds);
            AddShake(5.7f + defeated * 0.45f + broken * 0.18f, 0.24f);
            audio.PlaySlamImpact();
            if (defeated > 0) audio.PlayStomp();
            if (broken > 0) audio.PlayBreak();
        }

        private bool AllCoursesCompleted()
        {
            for (int i = 0; i < CourseCompleted.Length; i++)
            {
                if (!CourseCompleted[i]) return false;
            }

            return true;
        }

        private void StoreCourseBest()
        {
            if (Player.Score > CourseBestScores[SelectedCourseIndex])
            {
                CourseBestScores[SelectedCourseIndex] = Player.Score;
            }

            if (Player.DataCores > CourseBestDataCores[SelectedCourseIndex])
            {
                CourseBestDataCores[SelectedCourseIndex] = Player.DataCores;
            }

            int objectives = CountCurrentObjectiveStars();
            if (objectives > CourseBestObjectiveStars[SelectedCourseIndex])
            {
                CourseBestObjectiveStars[SelectedCourseIndex] = objectives;
            }
        }

        public bool IsCurrentObjectiveComplete(int index)
        {
            if (index == 0) return CourseCompleted[SelectedCourseIndex];
            if (index == 1) return CurrentDataCoreTotal > 0 && Player.DataCores >= CurrentDataCoreTotal;
            if (index == 2) return Player.Orbs >= CurrentLevel.ObjectiveOrbGoal;
            if (index == 3) return Player.EnemiesDefeated >= CurrentLevel.ObjectiveEnemyGoal;
            return false;
        }

        public string GetCurrentObjectiveText(int index)
        {
            if (index == 0) return "Reach the end beacon";
            if (index == 1) return "Data Cores " + Player.DataCores + "/" + CurrentDataCoreTotal;
            if (index == 2) return "Energy Orbs " + Player.Orbs + "/" + CurrentLevel.ObjectiveOrbGoal;
            if (index == 3) return "Enemies " + Player.EnemiesDefeated + "/" + CurrentLevel.ObjectiveEnemyGoal;
            return "";
        }

        private int CountCurrentObjectiveStars()
        {
            int count = 0;
            for (int i = 0; i < ObjectiveTotal; i++)
            {
                if (IsCurrentObjectiveComplete(i))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountDataCores(Level level)
        {
            int count = 0;
            for (int i = 0; i < level.Collectibles.Count; i++)
            {
                if (level.Collectibles[i].Kind == CollectibleKind.DataCore)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
