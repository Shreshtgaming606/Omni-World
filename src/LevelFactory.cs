using System.Drawing;

namespace OmniWorld
{
    public static class LevelFactory
    {
        public static Level CreateCourse(int courseNumber)
        {
            if (courseNumber == 6) return CreatePrismReactor();
            if (courseNumber == 5) return CreateSkylineCircuit();
            if (courseNumber == 4) return CreateEmberworks();
            if (courseNumber == 3) return CreateCrystalCanopy();
            if (courseNumber == 2) return CreateVoltageVale();
            return CreateGreenlitGrove();
        }

        public static Level[] CreateAllCourses()
        {
            return new Level[] { CreateGreenlitGrove(), CreateVoltageVale(), CreateCrystalCanopy(), CreateEmberworks(), CreateSkylineCircuit(), CreatePrismReactor() };
        }

        private static Level CreateGreenlitGrove()
        {
            // Courses are authored through helper calls so the data stays easy to extend.
            Level level = new Level(1, "Greenlit Grove", "A breezy first run through soft hills and bright energy.", 172, 18);
            level.SkyTop = Color.FromArgb(94, 195, 252);
            level.SkyBottom = Color.FromArgb(203, 244, 255);
            level.GroundTop = Color.FromArgb(79, 210, 90);
            level.GroundBody = Color.FromArgb(99, 142, 67);
            level.Spawn = new Vec2(2 * Level.TileSize, 15 * Level.TileSize - 30f);

            Ground(level, 0, 30);
            Ground(level, 34, 60);
            Ground(level, 64, 91);
            Ground(level, 97, 129);
            Ground(level, 135, 170);

            Platform(level, 11, 12, 6);
            Platform(level, 22, 12, 5);
            Platform(level, 42, 12, 6);
            Platform(level, 51, 12, 5);
            Platform(level, 73, 12, 7);
            Platform(level, 83, 12, 6);
            Platform(level, 107, 12, 6);
            Platform(level, 121, 12, 6);
            Platform(level, 145, 12, 7);

            Breakables(level, 38, 12, 4);
            Breakables(level, 86, 12, 3);
            Breakables(level, 116, 12, 4);

            level.SetTile(68, 14, TileType.Spike);
            level.SetTile(69, 14, TileType.Spike);
            level.SetTile(130, 14, TileType.Spike);
            level.SetTile(131, 14, TileType.Spike);

            level.MovingPlatforms.Add(new MovingPlatform(92 * Level.TileSize, 12 * Level.TileSize, 96 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 68f));

            level.AddOrbLine(5, 13, 5);
            level.AddOrbLine(12, 10, 4);
            level.AddOrbLine(23, 10, 3);
            level.AddOrbLine(43, 10, 4);
            level.AddOrbLine(52, 9, 4);
            level.AddOrbLine(74, 10, 5);
            level.AddOrbLine(84, 9, 5);
            level.AddOrbLine(108, 10, 5);
            level.AddOrbLine(122, 10, 4);
            level.AddOrbLine(145, 10, 5);
            level.AddPowerUp(75, 10);
            level.AddBurstCell(95, 10);
            level.AddHeart(125, 10);
            level.AddDataCore(24, 9);
            level.AddDataCore(87, 9);
            level.AddDataCore(148, 9);
            level.AddAegisCore(57, 10);

            level.AddBouncePad(27, 15);
            level.AddBouncePad(58, 15);
            level.AddBouncePad(128, 15);

            level.AddGroundEnemy(EnemyKind.Roller, 19, 15, 3, 28);
            level.AddGroundEnemy(EnemyKind.Roller, 53, 15, 35, 58);
            level.AddGroundEnemy(EnemyKind.Hopper, 80, 15, 66, 89);
            level.AddGroundEnemy(EnemyKind.Seeker, 118, 15, 100, 127);
            level.AddGroundEnemy(EnemyKind.Roller, 151, 15, 137, 168);

            level.AddCheckpoint(66, 15);
            level.AddCheckpoint(121, 15);
            level.SetGoal(166, 15);
            level.SetObjectives(25, 4);

            return level;
        }

        private static Level CreateVoltageVale()
        {
            // The second course keeps the same rules but combines them more densely.
            Level level = new Level(2, "Voltage Vale", "Moving platforms, sharp hazards, and a tight final climb.", 226, 18);
            level.SkyTop = Color.FromArgb(65, 109, 178);
            level.SkyBottom = Color.FromArgb(199, 225, 239);
            level.GroundTop = Color.FromArgb(88, 224, 177);
            level.GroundBody = Color.FromArgb(67, 85, 122);
            level.Spawn = new Vec2(2 * Level.TileSize, 15 * Level.TileSize - 30f);

            Ground(level, 0, 23);
            Ground(level, 28, 46);
            Ground(level, 52, 74);
            Ground(level, 81, 106);
            Ground(level, 113, 138);
            Ground(level, 145, 162);
            Ground(level, 169, 188);
            Ground(level, 196, 224);

            Platform(level, 14, 12, 5);
            Platform(level, 34, 12, 4);
            Platform(level, 42, 12, 4);
            Platform(level, 57, 12, 5);
            Platform(level, 67, 12, 5);
            Platform(level, 89, 12, 5);
            Platform(level, 100, 12, 5);
            Platform(level, 121, 12, 4);
            Platform(level, 132, 12, 5);
            Platform(level, 151, 12, 5);
            Platform(level, 177, 12, 5);
            Platform(level, 204, 12, 4);
            Platform(level, 211, 12, 4);

            Breakables(level, 36, 12, 3);
            Breakables(level, 91, 12, 3);
            Breakables(level, 123, 12, 4);
            Breakables(level, 179, 12, 3);
            Breakables(level, 206, 12, 3);

            Spikes(level, 19, 14, 3);
            Spikes(level, 58, 14, 2);
            Spikes(level, 70, 14, 3);
            Spikes(level, 101, 14, 3);
            Spikes(level, 126, 14, 3);
            Spikes(level, 153, 14, 4);
            Spikes(level, 181, 14, 3);
            Spikes(level, 202, 14, 2);

            level.MovingPlatforms.Add(new MovingPlatform(24 * Level.TileSize, 12 * Level.TileSize, 28 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 82f));
            level.MovingPlatforms.Add(new MovingPlatform(75 * Level.TileSize, 13 * Level.TileSize, 81 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 74f));
            level.MovingPlatforms.Add(new MovingPlatform(139 * Level.TileSize, 12 * Level.TileSize, 145 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 92f));
            level.MovingPlatforms.Add(new MovingPlatform(189 * Level.TileSize, 12 * Level.TileSize, 195 * Level.TileSize, 13 * Level.TileSize, 88f, 16f, 88f));

            level.AddOrbLine(6, 13, 5);
            level.AddOrbLine(14, 10, 5);
            level.AddOrbLine(35, 10, 3);
            level.AddOrbLine(57, 10, 5);
            level.AddOrbLine(67, 9, 5);
            level.AddOrbLine(90, 10, 4);
            level.AddOrbLine(101, 10, 4);
            level.AddOrbLine(122, 10, 4);
            level.AddOrbLine(132, 9, 5);
            level.AddOrbLine(151, 10, 5);
            level.AddOrbLine(177, 10, 5);
            level.AddOrbLine(205, 10, 4);
            level.AddOrbLine(211, 9, 4);
            level.AddPowerUp(93, 9);
            level.AddPowerUp(180, 10);
            level.AddBurstCell(79, 10);
            level.AddBurstCell(191, 10);
            level.AddHeart(135, 9);
            level.AddDataCore(38, 9);
            level.AddDataCore(133, 8);
            level.AddDataCore(212, 8);
            level.AddAegisCore(158, 10);

            level.AddBouncePad(44, 15);
            level.AddBouncePad(105, 15);
            level.AddBouncePad(160, 15);
            level.AddBouncePad(186, 15);

            level.AddGroundEnemy(EnemyKind.Roller, 15, 15, 2, 22);
            level.AddGroundEnemy(EnemyKind.Hopper, 39, 15, 29, 45);
            level.AddGroundEnemy(EnemyKind.Seeker, 61, 15, 53, 73);
            level.AddGroundEnemy(EnemyKind.Roller, 95, 15, 83, 105);
            level.AddGroundEnemy(EnemyKind.Hopper, 124, 15, 114, 137);
            level.AddGroundEnemy(EnemyKind.Seeker, 154, 15, 146, 161);
            level.AddGroundEnemy(EnemyKind.Hopper, 179, 15, 170, 187);
            level.AddGroundEnemy(EnemyKind.Seeker, 210, 15, 198, 223);
            level.AddFlyingEnemy(EnemyKind.Drifter, 132, 8, 118, 145);

            level.AddCheckpoint(82, 15);
            level.AddCheckpoint(171, 15);
            level.SetGoal(220, 15);
            level.SetObjectives(32, 6);

            return level;
        }

        private static Level CreateCrystalCanopy()
        {
            Level level = new Level(3, "Crystal Canopy", "A twilight climb through luminous stone and suspended platforms.", 246, 18);
            level.SkyTop = Color.FromArgb(53, 41, 112);
            level.SkyBottom = Color.FromArgb(211, 148, 202);
            level.GroundTop = Color.FromArgb(117, 238, 211);
            level.GroundBody = Color.FromArgb(86, 66, 147);
            level.Spawn = new Vec2(2 * Level.TileSize, 15 * Level.TileSize - 30f);

            Ground(level, 0, 26);
            Ground(level, 32, 56);
            Ground(level, 62, 91);
            Ground(level, 99, 126);
            Ground(level, 134, 164);
            Ground(level, 172, 202);
            Ground(level, 211, 244);

            Platform(level, 10, 12, 5);
            Platform(level, 22, 12, 4);
            Platform(level, 38, 12, 5);
            Platform(level, 51, 12, 4);
            Platform(level, 68, 12, 6);
            Platform(level, 83, 12, 5);
            Platform(level, 105, 12, 5);
            Platform(level, 119, 12, 5);
            Platform(level, 144, 12, 5);
            Platform(level, 158, 12, 5);
            Platform(level, 181, 12, 5);
            Platform(level, 195, 12, 5);
            Platform(level, 218, 12, 5);
            Platform(level, 230, 12, 4);

            Breakables(level, 40, 12, 3);
            Breakables(level, 107, 12, 3);
            Breakables(level, 146, 12, 3);
            Breakables(level, 220, 12, 3);

            Spikes(level, 20, 14, 3);
            Spikes(level, 70, 14, 3);
            Spikes(level, 114, 14, 4);
            Spikes(level, 151, 14, 4);
            Spikes(level, 184, 14, 3);
            Spikes(level, 224, 14, 4);

            level.MovingPlatforms.Add(new MovingPlatform(27 * Level.TileSize, 12 * Level.TileSize, 32 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 86f));
            level.MovingPlatforms.Add(new MovingPlatform(92 * Level.TileSize, 12 * Level.TileSize, 98 * Level.TileSize, 13 * Level.TileSize, 88f, 16f, 84f));
            level.MovingPlatforms.Add(new MovingPlatform(127 * Level.TileSize, 13 * Level.TileSize, 134 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 92f));
            level.MovingPlatforms.Add(new MovingPlatform(203 * Level.TileSize, 12 * Level.TileSize, 211 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 96f));

            level.AddOrbLine(6, 13, 5);
            level.AddOrbLine(11, 10, 4);
            level.AddOrbLine(38, 10, 5);
            level.AddOrbLine(52, 10, 4);
            level.AddOrbLine(69, 10, 5);
            level.AddOrbLine(84, 10, 4);
            level.AddOrbLine(106, 10, 4);
            level.AddOrbLine(120, 10, 4);
            level.AddOrbLine(145, 10, 4);
            level.AddOrbLine(159, 10, 4);
            level.AddOrbLine(182, 10, 4);
            level.AddOrbLine(196, 10, 4);
            level.AddOrbLine(219, 10, 4);
            level.AddOrbLine(230, 10, 4);
            level.AddPowerUp(108, 10);
            level.AddPowerUp(197, 10);
            level.AddBurstCell(94, 10);
            level.AddBurstCell(130, 10);
            level.AddBurstCell(205, 10);
            level.AddHeart(160, 10);
            level.AddDataCore(53, 9);
            level.AddDataCore(146, 9);
            level.AddDataCore(221, 9);
            level.AddAegisCore(119, 9);

            level.AddBouncePad(25, 15);
            level.AddBouncePad(55, 15);
            level.AddBouncePad(124, 15);
            level.AddBouncePad(200, 15);

            level.AddGroundEnemy(EnemyKind.Roller, 14, 15, 2, 25);
            level.AddGroundEnemy(EnemyKind.Hopper, 44, 15, 33, 55);
            level.AddGroundEnemy(EnemyKind.Seeker, 76, 15, 63, 90);
            level.AddGroundEnemy(EnemyKind.Roller, 111, 15, 100, 125);
            level.AddGroundEnemy(EnemyKind.Hopper, 149, 15, 135, 163);
            level.AddGroundEnemy(EnemyKind.Seeker, 188, 15, 173, 201);
            level.AddGroundEnemy(EnemyKind.Hopper, 225, 15, 212, 242);
            level.AddFlyingEnemy(EnemyKind.Drifter, 70, 8, 62, 91);
            level.AddFlyingEnemy(EnemyKind.Drifter, 146, 8, 135, 164);
            level.AddFlyingEnemy(EnemyKind.Drifter, 221, 8, 212, 244);

            level.AddCheckpoint(86, 15);
            level.AddCheckpoint(176, 15);
            level.SetGoal(239, 15);
            level.SetObjectives(34, 5);

            return level;
        }

        private static Level CreateEmberworks()
        {
            Level level = new Level(4, "Emberworks", "A hot industrial run with vents, bursts, and pressure enemies.", 258, 18);
            level.SkyTop = Color.FromArgb(71, 47, 61);
            level.SkyBottom = Color.FromArgb(238, 130, 71);
            level.GroundTop = Color.FromArgb(246, 164, 77);
            level.GroundBody = Color.FromArgb(104, 72, 80);
            level.Spawn = new Vec2(2 * Level.TileSize, 15 * Level.TileSize - 30f);

            Ground(level, 0, 28);
            Ground(level, 35, 63);
            Ground(level, 70, 99);
            Ground(level, 106, 136);
            Ground(level, 144, 174);
            Ground(level, 182, 214);
            Ground(level, 222, 254);

            Platform(level, 12, 12, 5);
            Platform(level, 24, 12, 4);
            Platform(level, 42, 12, 5);
            Platform(level, 57, 12, 4);
            Platform(level, 78, 12, 5);
            Platform(level, 94, 12, 4);
            Platform(level, 116, 12, 5);
            Platform(level, 130, 12, 4);
            Platform(level, 153, 12, 5);
            Platform(level, 168, 12, 4);
            Platform(level, 193, 12, 5);
            Platform(level, 208, 12, 4);
            Platform(level, 232, 12, 5);
            Platform(level, 244, 12, 4);

            Breakables(level, 43, 12, 3);
            Breakables(level, 118, 12, 3);
            Breakables(level, 195, 12, 3);
            Breakables(level, 233, 12, 3);

            Spikes(level, 18, 14, 4);
            Spikes(level, 51, 14, 3);
            Spikes(level, 84, 14, 4);
            Spikes(level, 122, 14, 4);
            Spikes(level, 160, 14, 5);
            Spikes(level, 199, 14, 4);
            Spikes(level, 237, 14, 3);

            level.MovingPlatforms.Add(new MovingPlatform(29 * Level.TileSize, 12 * Level.TileSize, 35 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 92f));
            level.MovingPlatforms.Add(new MovingPlatform(100 * Level.TileSize, 13 * Level.TileSize, 106 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 86f));
            level.MovingPlatforms.Add(new MovingPlatform(175 * Level.TileSize, 12 * Level.TileSize, 182 * Level.TileSize, 13 * Level.TileSize, 88f, 16f, 92f));
            level.MovingPlatforms.Add(new MovingPlatform(215 * Level.TileSize, 12 * Level.TileSize, 222 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 102f));

            level.AddOrbLine(6, 13, 5);
            level.AddOrbLine(13, 10, 4);
            level.AddOrbLine(43, 10, 4);
            level.AddOrbLine(58, 10, 4);
            level.AddOrbLine(79, 10, 4);
            level.AddOrbLine(95, 10, 3);
            level.AddOrbLine(117, 10, 5);
            level.AddOrbLine(131, 10, 3);
            level.AddOrbLine(154, 10, 5);
            level.AddOrbLine(169, 10, 3);
            level.AddOrbLine(194, 10, 5);
            level.AddOrbLine(209, 10, 3);
            level.AddOrbLine(233, 10, 5);
            level.AddPowerUp(118, 10);
            level.AddPowerUp(195, 10);
            level.AddBurstCell(31, 10);
            level.AddBurstCell(102, 10);
            level.AddBurstCell(178, 10);
            level.AddBurstCell(218, 10);
            level.AddHeart(132, 10);
            level.AddDataCore(58, 9);
            level.AddDataCore(154, 9);
            level.AddDataCore(233, 9);
            level.AddAegisCore(95, 9);

            level.AddBouncePad(26, 15);
            level.AddBouncePad(62, 15);
            level.AddBouncePad(134, 15);
            level.AddBouncePad(172, 15);
            level.AddBouncePad(212, 15);

            level.AddGroundEnemy(EnemyKind.Hopper, 17, 15, 2, 27);
            level.AddGroundEnemy(EnemyKind.Seeker, 47, 15, 36, 62);
            level.AddGroundEnemy(EnemyKind.Roller, 86, 15, 71, 98);
            level.AddGroundEnemy(EnemyKind.Hopper, 124, 15, 107, 135);
            level.AddGroundEnemy(EnemyKind.Seeker, 160, 15, 145, 173);
            level.AddGroundEnemy(EnemyKind.Hopper, 198, 15, 183, 213);
            level.AddGroundEnemy(EnemyKind.Seeker, 238, 15, 223, 253);
            level.AddFlyingEnemy(EnemyKind.Drifter, 80, 8, 70, 99);
            level.AddFlyingEnemy(EnemyKind.Drifter, 153, 8, 144, 174);
            level.AddFlyingEnemy(EnemyKind.Drifter, 232, 8, 222, 254);

            level.AddCheckpoint(72, 15);
            level.AddCheckpoint(184, 15);
            level.SetGoal(249, 15);
            level.SetObjectives(34, 5);

            return level;
        }

        private static Level CreateSkylineCircuit()
        {
            Level level = new Level(5, "Skyline Circuit", "A brisk rooftop course built around bounce pads and chained bursts.", 288, 18);
            level.SkyTop = Color.FromArgb(48, 74, 152);
            level.SkyBottom = Color.FromArgb(186, 226, 255);
            level.GroundTop = Color.FromArgb(255, 217, 92);
            level.GroundBody = Color.FromArgb(54, 92, 145);
            level.Spawn = new Vec2(2 * Level.TileSize, 15 * Level.TileSize - 30f);

            Ground(level, 0, 24);
            Ground(level, 31, 58);
            Ground(level, 66, 94);
            Ground(level, 102, 132);
            Ground(level, 140, 166);
            Ground(level, 175, 204);
            Ground(level, 213, 244);
            Ground(level, 253, 286);

            Platform(level, 10, 12, 5);
            Platform(level, 20, 12, 4);
            Platform(level, 39, 12, 5);
            Platform(level, 52, 12, 4);
            Platform(level, 73, 12, 5);
            Platform(level, 88, 12, 4);
            Platform(level, 110, 12, 5);
            Platform(level, 126, 12, 4);
            Platform(level, 147, 12, 5);
            Platform(level, 160, 12, 4);
            Platform(level, 184, 12, 5);
            Platform(level, 199, 12, 4);
            Platform(level, 222, 12, 5);
            Platform(level, 239, 12, 4);
            Platform(level, 265, 12, 5);
            Platform(level, 276, 12, 4);

            Breakables(level, 40, 12, 3);
            Breakables(level, 111, 12, 3);
            Breakables(level, 185, 12, 3);
            Breakables(level, 266, 12, 3);

            Spikes(level, 16, 14, 3);
            Spikes(level, 75, 14, 4);
            Spikes(level, 119, 14, 4);
            Spikes(level, 153, 14, 4);
            Spikes(level, 190, 14, 4);
            Spikes(level, 229, 14, 4);
            Spikes(level, 268, 14, 3);

            level.MovingPlatforms.Add(new MovingPlatform(24 * Level.TileSize, 12 * Level.TileSize, 31 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 104f));
            level.MovingPlatforms.Add(new MovingPlatform(95 * Level.TileSize, 12 * Level.TileSize, 102 * Level.TileSize, 13 * Level.TileSize, 88f, 16f, 98f));
            level.MovingPlatforms.Add(new MovingPlatform(167 * Level.TileSize, 13 * Level.TileSize, 175 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 108f));
            level.MovingPlatforms.Add(new MovingPlatform(245 * Level.TileSize, 12 * Level.TileSize, 253 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 116f));

            level.AddOrbLine(6, 13, 5);
            level.AddOrbLine(11, 10, 4);
            level.AddOrbLine(40, 10, 4);
            level.AddOrbLine(53, 10, 3);
            level.AddOrbLine(74, 10, 5);
            level.AddOrbLine(89, 10, 3);
            level.AddOrbLine(111, 10, 5);
            level.AddOrbLine(127, 10, 3);
            level.AddOrbLine(148, 10, 5);
            level.AddOrbLine(161, 10, 3);
            level.AddOrbLine(185, 10, 5);
            level.AddOrbLine(200, 10, 3);
            level.AddOrbLine(223, 10, 5);
            level.AddOrbLine(240, 10, 3);
            level.AddOrbLine(266, 10, 5);
            level.AddPowerUp(111, 10);
            level.AddPowerUp(224, 10);
            level.AddBurstCell(26, 10);
            level.AddBurstCell(98, 10);
            level.AddBurstCell(170, 10);
            level.AddBurstCell(248, 10);
            level.AddHeart(188, 10);
            level.AddDataCore(89, 9);
            level.AddDataCore(161, 9);
            level.AddDataCore(266, 9);
            level.AddAegisCore(148, 9);

            level.AddBouncePad(22, 15);
            level.AddBouncePad(57, 15);
            level.AddBouncePad(92, 15);
            level.AddBouncePad(130, 15);
            level.AddBouncePad(164, 15);
            level.AddBouncePad(202, 15);
            level.AddBouncePad(242, 15);

            level.AddGroundEnemy(EnemyKind.Seeker, 14, 15, 2, 23);
            level.AddGroundEnemy(EnemyKind.Hopper, 43, 15, 32, 57);
            level.AddGroundEnemy(EnemyKind.Seeker, 78, 15, 67, 93);
            level.AddGroundEnemy(EnemyKind.Roller, 116, 15, 103, 131);
            level.AddGroundEnemy(EnemyKind.Hopper, 152, 15, 141, 165);
            level.AddGroundEnemy(EnemyKind.Seeker, 190, 15, 176, 203);
            level.AddGroundEnemy(EnemyKind.Hopper, 228, 15, 214, 243);
            level.AddGroundEnemy(EnemyKind.Seeker, 270, 15, 254, 285);
            level.AddFlyingEnemy(EnemyKind.Drifter, 73, 8, 66, 94);
            level.AddFlyingEnemy(EnemyKind.Drifter, 147, 8, 140, 166);
            level.AddFlyingEnemy(EnemyKind.Drifter, 222, 8, 213, 244);
            level.AddFlyingEnemy(EnemyKind.Drifter, 265, 8, 253, 286);

            level.AddCheckpoint(104, 15);
            level.AddCheckpoint(214, 15);
            level.SetGoal(281, 15);
            level.SetObjectives(36, 6);

            return level;
        }

        private static Level CreatePrismReactor()
        {
            Level level = new Level(6, "Prism Reactor", "A neon finale that mixes every movement skill.", 316, 18);
            level.SkyTop = Color.FromArgb(42, 31, 96);
            level.SkyBottom = Color.FromArgb(64, 203, 208);
            level.GroundTop = Color.FromArgb(225, 133, 255);
            level.GroundBody = Color.FromArgb(45, 66, 126);
            level.Spawn = new Vec2(2 * Level.TileSize, 15 * Level.TileSize - 30f);

            Ground(level, 0, 22);
            Ground(level, 29, 53);
            Ground(level, 61, 87);
            Ground(level, 96, 124);
            Ground(level, 133, 161);
            Ground(level, 172, 201);
            Ground(level, 211, 240);
            Ground(level, 252, 283);
            Ground(level, 292, 314);

            Platform(level, 10, 12, 5);
            Platform(level, 19, 12, 4);
            Platform(level, 34, 12, 5);
            Platform(level, 48, 12, 4);
            Platform(level, 68, 12, 5);
            Platform(level, 82, 12, 4);
            Platform(level, 105, 12, 5);
            Platform(level, 119, 12, 4);
            Platform(level, 142, 12, 5);
            Platform(level, 156, 12, 4);
            Platform(level, 181, 12, 5);
            Platform(level, 196, 12, 4);
            Platform(level, 221, 12, 5);
            Platform(level, 236, 12, 4);
            Platform(level, 263, 12, 5);
            Platform(level, 278, 12, 4);
            Platform(level, 300, 12, 5);

            Breakables(level, 35, 12, 3);
            Breakables(level, 107, 12, 3);
            Breakables(level, 143, 12, 3);
            Breakables(level, 222, 12, 3);
            Breakables(level, 264, 12, 3);

            Spikes(level, 15, 14, 3);
            Spikes(level, 70, 14, 4);
            Spikes(level, 114, 14, 4);
            Spikes(level, 151, 14, 4);
            Spikes(level, 187, 14, 5);
            Spikes(level, 231, 14, 4);
            Spikes(level, 270, 14, 4);
            Spikes(level, 303, 14, 3);

            level.MovingPlatforms.Add(new MovingPlatform(23 * Level.TileSize, 12 * Level.TileSize, 29 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 104f));
            level.MovingPlatforms.Add(new MovingPlatform(54 * Level.TileSize, 12 * Level.TileSize, 61 * Level.TileSize, 13 * Level.TileSize, 88f, 16f, 94f));
            level.MovingPlatforms.Add(new MovingPlatform(88 * Level.TileSize, 13 * Level.TileSize, 96 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 112f));
            level.MovingPlatforms.Add(new MovingPlatform(125 * Level.TileSize, 12 * Level.TileSize, 133 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 108f));
            level.MovingPlatforms.Add(new MovingPlatform(162 * Level.TileSize, 13 * Level.TileSize, 172 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 116f));
            level.MovingPlatforms.Add(new MovingPlatform(202 * Level.TileSize, 12 * Level.TileSize, 211 * Level.TileSize, 13 * Level.TileSize, 88f, 16f, 110f));
            level.MovingPlatforms.Add(new MovingPlatform(241 * Level.TileSize, 12 * Level.TileSize, 252 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 122f));
            level.MovingPlatforms.Add(new MovingPlatform(284 * Level.TileSize, 13 * Level.TileSize, 292 * Level.TileSize, 12 * Level.TileSize, 88f, 16f, 118f));

            level.AddOrbLine(6, 13, 5);
            level.AddOrbLine(11, 10, 4);
            level.AddOrbLine(35, 10, 4);
            level.AddOrbLine(49, 10, 3);
            level.AddOrbLine(69, 10, 5);
            level.AddOrbLine(83, 10, 3);
            level.AddOrbLine(106, 10, 5);
            level.AddOrbLine(120, 10, 3);
            level.AddOrbLine(143, 10, 5);
            level.AddOrbLine(157, 10, 3);
            level.AddOrbLine(182, 10, 5);
            level.AddOrbLine(197, 10, 3);
            level.AddOrbLine(222, 10, 5);
            level.AddOrbLine(237, 10, 3);
            level.AddOrbLine(264, 10, 5);
            level.AddOrbLine(279, 10, 3);
            level.AddOrbLine(301, 10, 4);
            level.AddPowerUp(107, 10);
            level.AddPowerUp(222, 10);
            level.AddBurstCell(25, 10);
            level.AddBurstCell(90, 10);
            level.AddBurstCell(164, 10);
            level.AddBurstCell(244, 10);
            level.AddBurstCell(286, 10);
            level.AddHeart(198, 10);
            level.AddDataCore(49, 9);
            level.AddDataCore(143, 9);
            level.AddDataCore(222, 9);
            level.AddDataCore(300, 9);
            level.AddAegisCore(182, 9);
            level.AddAegisCore(264, 9);

            level.AddBouncePad(20, 15);
            level.AddBouncePad(51, 15);
            level.AddBouncePad(85, 15);
            level.AddBouncePad(121, 15);
            level.AddBouncePad(158, 15);
            level.AddBouncePad(199, 15);
            level.AddBouncePad(238, 15);
            level.AddBouncePad(281, 15);

            level.AddGroundEnemy(EnemyKind.Seeker, 14, 15, 2, 21);
            level.AddGroundEnemy(EnemyKind.Hopper, 38, 15, 30, 52);
            level.AddGroundEnemy(EnemyKind.Seeker, 72, 15, 62, 86);
            level.AddGroundEnemy(EnemyKind.Roller, 110, 15, 97, 123);
            level.AddGroundEnemy(EnemyKind.Hopper, 148, 15, 134, 160);
            level.AddGroundEnemy(EnemyKind.Seeker, 187, 15, 173, 200);
            level.AddGroundEnemy(EnemyKind.Hopper, 226, 15, 212, 239);
            level.AddGroundEnemy(EnemyKind.Seeker, 268, 15, 253, 282);
            level.AddGroundEnemy(EnemyKind.Hopper, 297, 15, 293, 302);
            level.AddFlyingEnemy(EnemyKind.Drifter, 69, 8, 61, 87);
            level.AddFlyingEnemy(EnemyKind.Drifter, 142, 8, 133, 161);
            level.AddFlyingEnemy(EnemyKind.Drifter, 222, 8, 211, 240);
            level.AddFlyingEnemy(EnemyKind.Drifter, 300, 8, 292, 314);

            level.AddCheckpoint(99, 15);
            level.AddCheckpoint(213, 15);
            level.SetGoal(309, 15);
            level.SetObjectives(42, 7);

            return level;
        }

        private static void Ground(Level level, int startTileX, int endTileX)
        {
            level.FillRect(startTileX, 15, endTileX - startTileX, 3, TileType.Ground);
        }

        private static void Platform(Level level, int x, int y, int width)
        {
            level.FillRect(x, y, width, 1, TileType.Ground);
        }

        private static void Breakables(Level level, int x, int y, int width)
        {
            for (int i = 0; i < width; i++)
            {
                level.SetTile(x + i, y, TileType.Breakable);
            }
        }

        private static void Spikes(Level level, int x, int y, int width)
        {
            for (int i = 0; i < width; i++)
            {
                level.SetTile(x + i, y, TileType.Spike);
            }
        }
    }
}
