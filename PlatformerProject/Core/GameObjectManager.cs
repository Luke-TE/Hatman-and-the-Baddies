using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatformerProject.Effects;
using PlatformerProject.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Core
{
    class GameObjectManager
    {
        public enum GameState { Paused, Playing, Won, Lost }

        #region Properties

        public GameState CurrentGameState { get; set; }
        public List<IGameObject> GameObjects { get; set; }
        public List<IGameObject> PauseMenuObjects { get; set; }
        public List<IGameObject> WinObjects { get; set; }
        public List<IGameObject> LoseObjects { get; set; }
        public List<ISprite> Sprites { get; set; }
        public List<ICollidable> Collidables { get; set; }
        public List<IInteractable> Interactables { get; set; }
        public Dictionary<string, Dictionary<string, TextureAnimation>> EnemyTextures { get; private set; }
        public Dictionary<string, Dictionary<string, SoundEffect>> EnemySounds { get; private set; }

        public List<Player> Players { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Rectangle> TileColls { get; private set; }
        public List<Rectangle> JumpThroughColls { get; private set; }
        public TileMap Map { get; private set; }
        public KeyboardState OldKeyState { get; private set; }
        public KeyboardState NewKeyState { get; private set; }
        public Camera Camera { get; private set; }
        public Rectangle Goal { get; set; }

        #endregion


        #region Methods

        public GameObjectManager(TileMap tileMap)
        {
            Map = tileMap;

            CurrentGameState = GameState.Playing;
            GameObjects = new List<IGameObject>();
            PauseMenuObjects = new List<IGameObject>();
            WinObjects = new List<IGameObject>();
            LoseObjects = new List<IGameObject>();
            Sprites = new List<ISprite>();
            Collidables = new List<ICollidable>();
            Interactables = new List<IInteractable>();            
            TileColls = tileMap.GetColls();
            JumpThroughColls = tileMap.GetRectObjects("jump through");
            Camera = new Camera();
            Players = new List<Player>();
            Enemies = new List<Enemy>();
            EnemyTextures = new Dictionary<string, Dictionary<string, TextureAnimation>>();
            EnemySounds = new Dictionary<string, Dictionary<string, SoundEffect>>();
            Goal = tileMap.GetRectObjects("exit")[0];
        }

        public void AddGameObject(IGameObject gameObject)
        {
            GameObjects.Add(gameObject);
            if (gameObject is ISprite) Sprites.Add(gameObject as ISprite);
            if (gameObject is ICollidable) Collidables.Add(gameObject as ICollidable);
            if (gameObject is IInteractable) Interactables.Add(gameObject as IInteractable);
            if (gameObject is Player) Players.Add(gameObject as Player);
            if (gameObject is Enemy) Enemies.Add(gameObject as Enemy);
        }

        public void AddPauseMenuObject(IGameObject gameObject)
        {
            PauseMenuObjects.Add(gameObject);
        }

        public void AddWinObject(IGameObject gameObject)
        {
            WinObjects.Add(gameObject);
        }

        public void AddLoseObject(IGameObject gameObject)
        {
            LoseObjects.Add(gameObject);
        }

        public void UpdateGameObjects(GameTime gameTime)
        {
            OldKeyState = NewKeyState;
            NewKeyState = Keyboard.GetState();

            switch (CurrentGameState)
            {
                case GameState.Playing:
                    for (var i = GameObjects.Count - 1; i >= 0; i--)
                    {
                        GameObjects[i].Update(gameTime);

                        if (CurrentGameState == GameState.Playing && GameObjects[i] is Player player)
                            Camera.Follow(player);

                        if ((!GameObjects[i].Active))
                            GameObjects.RemoveAt(i);
                    }
                    break;

                case GameState.Lost:
                    for (var i = GameObjects.Count - 1; i >= 0; i--)
                    {
                        GameObjects[i].Update(gameTime);

                        if (CurrentGameState == GameState.Playing && GameObjects[i] is Player player)
                            Camera.Follow(player);

                        if ((!GameObjects[i].Active))
                            GameObjects.RemoveAt(i);
                    }

                    for (var i = LoseObjects.Count - 1; i >= 0; i--)
                    {
                        LoseObjects[i].Update(gameTime);
                    }

                    break;

                case GameState.Won:
                    for (var i = GameObjects.Count - 1; i >= 0; i--)
                    {
                        GameObjects[i].Update(gameTime);

                        if (CurrentGameState == GameState.Playing && GameObjects[i] is Player player)
                            Camera.Follow(player);

                        if ((!GameObjects[i].Active))
                            GameObjects.RemoveAt(i);
                    }

                    for (var i = WinObjects.Count - 1; i >= 0; i--)
                    {
                        WinObjects[i].Update(gameTime);
                    }
                    break;

                case GameState.Paused:
                    for (var i = PauseMenuObjects.Count - 1; i >= 0; i--)
                    {
                        PauseMenuObjects[i].Update(gameTime);
                    }
                    break;
            }                
        }

        public void DrawGameObjects(GameTime gameTime, SpriteBatch spriteBatch)
        {            
            spriteBatch.Begin(transformMatrix: Camera.Transform);

            Map.DrawTileLayer(gameTime, spriteBatch, "Background 3", 0.5f);
            Map.DrawTileLayer(gameTime, spriteBatch, "Background 2", 0.6f);            
            Map.DrawTileLayer(gameTime, spriteBatch, "Background 1", 0.7f);
                        
            Map.DrawTileLayer(gameTime, spriteBatch, "Foreground");

            foreach (var gameObject in GameObjects)
                if (!gameObject.Fixed)
                    gameObject.Draw(gameTime, spriteBatch);

            
            Map.DrawTileLayer(gameTime, spriteBatch, "Fringe 3");
            Map.DrawTileLayer(gameTime, spriteBatch, "Fringe 2");
            Map.DrawTileLayer(gameTime, spriteBatch, "Fringe 1");                        

            spriteBatch.End();


            spriteBatch.Begin();

            foreach (var gameObject in GameObjects)
                if (gameObject.Fixed)
                    gameObject.Draw(gameTime, spriteBatch);

            if (CurrentGameState == GameState.Paused)
                foreach (var menuObject in PauseMenuObjects)
                    menuObject.Draw(gameTime, spriteBatch);

            if (CurrentGameState == GameState.Won)
                foreach (var winObject in WinObjects)
                    winObject.Draw(gameTime, spriteBatch);

            if (CurrentGameState == GameState.Lost)
                foreach (var loseObject in LoseObjects)
                    loseObject.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }        

        public void AddEnemyTextures(string enemyName, Dictionary<string, TextureAnimation> textures)
        {
            EnemyTextures.Add(enemyName, textures);
        }

        public void AddEnemySounds(string enemyName, Dictionary<string, SoundEffect> sounds)
        {
            EnemySounds.Add(enemyName, sounds);
        }        

        public void CreateSkeleton(Vector2 pos, bool facingRight, bool waitingForever)
        {            
            Dictionary<string, TextureAnimation> textures = new Dictionary<string, TextureAnimation>();
            foreach (var anim in EnemyTextures["skeleton"])
                textures[anim.Key] = (TextureAnimation)anim.Value.Clone();

            AddGameObject(new Skeleton(this, textures, pos, facingRight, waitingForever) { Sounds = EnemySounds["skeleton"] });            
        }

        public void CreateGoblin(Vector2 pos, bool facingRight)
        {
            Dictionary<string, TextureAnimation> textures = new Dictionary<string, TextureAnimation>();
            foreach (var anim in EnemyTextures["goblin"])
                textures[anim.Key] = (TextureAnimation)anim.Value.Clone();

            AddGameObject(new Goblin(this, textures, pos, facingRight) { Sounds = EnemySounds["goblin"] });
        }

        public void CreateBlobMan(Vector2 pos, bool facingRight)
        {
            Dictionary<string, TextureAnimation> textures = new Dictionary<string, TextureAnimation>();
            foreach (var anim in EnemyTextures["blobman"])
                textures[anim.Key] = (TextureAnimation)anim.Value.Clone();

            AddGameObject(new Blob_Man(this, textures, pos, facingRight) { Sounds = EnemySounds["blobman"] });
        }

        public void CreateBlob(Vector2 pos, bool facingRight)
        {
            Dictionary<string, TextureAnimation> textures = new Dictionary<string, TextureAnimation>();
            foreach (var anim in EnemyTextures["blob"])
                textures[anim.Key] = (TextureAnimation)anim.Value.Clone();

            AddGameObject(new Blob(this, textures, pos, facingRight) { Sounds = EnemySounds["blob"] });
        }

        #endregion

    }

}
