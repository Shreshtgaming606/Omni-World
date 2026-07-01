using System.Collections.Generic;
using System.Drawing;

namespace OmniWorld
{
    public sealed class Level
    {
        public const int TileSize = 32;

        public int CourseNumber;
        public string Name;
        public string Tagline;
        public int WidthTiles;
        public int HeightTiles;
        public TileType[,] Tiles;
        public Vec2 Spawn;
        public GoalGate Goal;
        public Color SkyTop;
        public Color SkyBottom;
        public Color GroundTop;
        public Color GroundBody;
        public int ObjectiveOrbGoal;
        public int ObjectiveEnemyGoal;
        public readonly List<Enemy> Enemies;
        public readonly List<Collectible> Collectibles;
        public readonly List<MovingPlatform> MovingPlatforms;
        public readonly List<Checkpoint> Checkpoints;
        public readonly List<BouncePad> BouncePads;

        public Level(int courseNumber, string name, string tagline, int widthTiles, int heightTiles)
        {
            CourseNumber = courseNumber;
            Name = name;
            Tagline = tagline;
            WidthTiles = widthTiles;
            HeightTiles = heightTiles;
            Tiles = new TileType[widthTiles, heightTiles];
            Spawn = new Vec2(64f, 320f);
            Goal = new GoalGate((widthTiles - 4) * TileSize, 15 * TileSize);
            SkyTop = Color.FromArgb(91, 190, 255);
            SkyBottom = Color.FromArgb(198, 241, 255);
            GroundTop = Color.FromArgb(70, 203, 93);
            GroundBody = Color.FromArgb(91, 128, 62);
            ObjectiveOrbGoal = 20;
            ObjectiveEnemyGoal = 3;
            Enemies = new List<Enemy>();
            Collectibles = new List<Collectible>();
            MovingPlatforms = new List<MovingPlatform>();
            Checkpoints = new List<Checkpoint>();
            BouncePads = new List<BouncePad>();
        }

        public float WorldWidth
        {
            get { return WidthTiles * TileSize; }
        }

        public float WorldHeight
        {
            get { return HeightTiles * TileSize; }
        }

        public TileType GetTile(int x, int y)
        {
            if (x < 0 || x >= WidthTiles || y < 0 || y >= HeightTiles)
            {
                return TileType.Empty;
            }

            return Tiles[x, y];
        }

        public void SetTile(int x, int y, TileType tile)
        {
            if (x < 0 || x >= WidthTiles || y < 0 || y >= HeightTiles)
            {
                return;
            }

            Tiles[x, y] = tile;
        }

        public void FillRect(int x, int y, int width, int height, TileType tile)
        {
            for (int tx = x; tx < x + width; tx++)
            {
                for (int ty = y; ty < y + height; ty++)
                {
                    SetTile(tx, ty, tile);
                }
            }
        }

        public bool IsSolidTile(int x, int y)
        {
            TileType tile = GetTile(x, y);
            return tile == TileType.Ground || tile == TileType.Breakable;
        }

        public bool IsSolidWorld(float worldX, float worldY)
        {
            int tx = (int)(worldX / TileSize);
            int ty = (int)(worldY / TileSize);
            return IsSolidTile(tx, ty);
        }

        public bool IsHazardTile(int x, int y)
        {
            return GetTile(x, y) == TileType.Spike;
        }

        public void BreakTile(int x, int y)
        {
            if (GetTile(x, y) == TileType.Breakable)
            {
                SetTile(x, y, TileType.Empty);
            }
        }

        public void AddOrb(int tileX, int tileY)
        {
            Collectibles.Add(new Collectible(CollectibleKind.EnergyOrb, tileX * TileSize + 7f, tileY * TileSize + 7f));
        }

        public void AddOrbLine(int startTileX, int tileY, int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddOrb(startTileX + i, tileY);
            }
        }

        public void AddPowerUp(int tileX, int tileY)
        {
            Collectibles.Add(new Collectible(CollectibleKind.FluxCore, tileX * TileSize + 4f, tileY * TileSize + 4f));
        }

        public void AddHeart(int tileX, int tileY)
        {
            Collectibles.Add(new Collectible(CollectibleKind.Heart, tileX * TileSize + 7f, tileY * TileSize + 7f));
        }

        public void AddBurstCell(int tileX, int tileY)
        {
            Collectibles.Add(new Collectible(CollectibleKind.BurstCell, tileX * TileSize + 4f, tileY * TileSize + 4f));
        }

        public void AddDataCore(int tileX, int tileY)
        {
            Collectibles.Add(new Collectible(CollectibleKind.DataCore, tileX * TileSize + 4f, tileY * TileSize + 4f));
        }

        public void AddAegisCore(int tileX, int tileY)
        {
            Collectibles.Add(new Collectible(CollectibleKind.AegisCore, tileX * TileSize + 4f, tileY * TileSize + 4f));
        }

        public void AddGroundEnemy(EnemyKind kind, int tileX, int floorTileY, int leftBoundTile, int rightBoundTile)
        {
            float enemyHeight = kind == EnemyKind.Hopper ? 30f : 24f;
            Enemies.Add(new Enemy(
                kind,
                tileX * TileSize + 3f,
                floorTileY * TileSize - enemyHeight,
                leftBoundTile * TileSize,
                rightBoundTile * TileSize));
        }

        public void AddFlyingEnemy(EnemyKind kind, int tileX, int tileY, int leftBoundTile, int rightBoundTile)
        {
            Enemies.Add(new Enemy(
                kind,
                tileX * TileSize + 2f,
                tileY * TileSize + 5f,
                leftBoundTile * TileSize,
                rightBoundTile * TileSize));
        }

        public void AddCheckpoint(int tileX, int groundTileY)
        {
            Checkpoints.Add(new Checkpoint(tileX * TileSize, groundTileY * TileSize));
        }

        public void AddBouncePad(int tileX, int groundTileY)
        {
            BouncePads.Add(new BouncePad(tileX * TileSize, groundTileY * TileSize));
        }

        public void SetGoal(int tileX, int groundTileY)
        {
            Goal = new GoalGate(tileX * TileSize, groundTileY * TileSize);
        }

        public void SetObjectives(int orbGoal, int enemyGoal)
        {
            ObjectiveOrbGoal = orbGoal;
            ObjectiveEnemyGoal = enemyGoal;
        }
    }
}
