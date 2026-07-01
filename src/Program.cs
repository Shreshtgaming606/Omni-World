using System;
using System.Windows.Forms;

namespace OmniWorld
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--smoke-test")
            {
                SmokeTest.Run();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameForm());
        }
    }

    internal static class SmokeTest
    {
        public static void Run()
        {
            Level[] levels = LevelFactory.CreateAllCourses();
            if (levels.Length != 6) throw new InvalidOperationException("Expected exactly six courses.");

            for (int i = 0; i < levels.Length; i++)
            {
                Level level = levels[i];
                if (level.Enemies.Count == 0) throw new InvalidOperationException("Course has no enemies: " + level.Name);
                if (level.Collectibles.Count == 0) throw new InvalidOperationException("Course has no collectibles: " + level.Name);
                if (level.Checkpoints.Count == 0) throw new InvalidOperationException("Course has no checkpoints: " + level.Name);
                if (level.MovingPlatforms.Count == 0) throw new InvalidOperationException("Course has no moving platforms: " + level.Name);
                if (level.BouncePads.Count == 0) throw new InvalidOperationException("Course has no bounce pads: " + level.Name);
                if (level.Goal == null) throw new InvalidOperationException("Course has no goal: " + level.Name);
                if (level.ObjectiveOrbGoal <= 0) throw new InvalidOperationException("Course has no orb objective: " + level.Name);
                if (level.ObjectiveEnemyGoal <= 0) throw new InvalidOperationException("Course has no enemy objective: " + level.Name);
                if (level.ObjectiveEnemyGoal > level.Enemies.Count) throw new InvalidOperationException("Enemy objective exceeds enemy count: " + level.Name);

                int dataCores = 0;
                int aegisCores = 0;
                int energyOrbs = 0;
                for (int d = 0; d < level.Collectibles.Count; d++)
                {
                    if (level.Collectibles[d].Kind == CollectibleKind.DataCore)
                    {
                        dataCores++;
                    }
                    else if (level.Collectibles[d].Kind == CollectibleKind.AegisCore)
                    {
                        aegisCores++;
                    }
                    else if (level.Collectibles[d].Kind == CollectibleKind.EnergyOrb)
                    {
                        energyOrbs++;
                    }
                }

                if (dataCores == 0) throw new InvalidOperationException("Course has no Data Cores: " + level.Name);
                if (aegisCores == 0) throw new InvalidOperationException("Course has no Aegis Cores: " + level.Name);
                if (level.ObjectiveOrbGoal > energyOrbs) throw new InvalidOperationException("Orb objective exceeds orb count: " + level.Name);

                int drifters = 0;
                for (int e = 0; e < level.Enemies.Count; e++)
                {
                    if (level.Enemies[e].Kind == EnemyKind.Drifter)
                    {
                        drifters++;
                    }
                }

                if (level.CourseNumber > 1 && drifters == 0) throw new InvalidOperationException("Advanced course has no Drifters: " + level.Name);

                for (int c = 0; c < level.Checkpoints.Count; c++)
                {
                    Checkpoint checkpoint = level.Checkpoints[c];
                    float footX = checkpoint.RespawnPoint.X + 12f;
                    float footY = checkpoint.RespawnPoint.Y + 34f;
                    if (!level.IsSolidWorld(footX, footY))
                    {
                        throw new InvalidOperationException("Checkpoint respawn is not on solid ground: " + level.Name);
                    }
                }

                for (int b = 0; b < level.BouncePads.Count; b++)
                {
                    BouncePad pad = level.BouncePads[b];
                    if (!level.IsSolidWorld(pad.Bounds.Left + pad.Bounds.Width / 2f, pad.Bounds.Bottom + 2f))
                    {
                        throw new InvalidOperationException("Bounce pad is not mounted on solid ground: " + level.Name);
                    }
                }
            }
        }
    }
}
