using Interface.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PlatformerProject.Core;
using PlatformerProject.Effects;
using PlatformerProject.Effects.ParticleEngines;
using PlatformerProject.Enemies;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace PlatformerProject
{
    public class Stage1 : Game
    {
        #region Statics
        public static int ScreenWidth;
        public static int ScreenHeight;
        #endregion

        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameObjectManager gameObjectManager;
        TmxMap map;        
        TileMap tm;        
        Player player;
        KeyboardState oldKeyState;
        KeyboardState newKeyState;
        SoundEffect buttonClickSound;
        Text winText, loseText;
        int finishTimer;

        #endregion


        #region Textures
        Texture2D bg;
        Texture2D tileset;
        TextureAnimation idleAnim;
        #endregion


        #region Core Methods

        public Stage1()
        {            
            graphics = new GraphicsDeviceManager(this)
            {
                //SynchronizeWithVerticalRetrace = false,
                IsFullScreen = false,
                GraphicsProfile = GraphicsProfile.HiDef              
            };
            ScreenWidth = graphics.PreferredBackBufferWidth = 1920;
            ScreenHeight = graphics.PreferredBackBufferHeight = 1080;            

            Content.RootDirectory = "Content";
            IsMouseVisible = false;            
        }

        protected override void Initialize()
        {
            finishTimer = 5000;
            base.Initialize();            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TILEMAP //
            map = new TmxMap("../../../../Content/TestMap.tmx");           
            tileset = Content.Load<Texture2D>("Tilesets/" + map.Tilesets[0].Name.ToString());
            tm = new TileMap(map, tileset);
            gameObjectManager = new GameObjectManager(tm);
            
            var spawnPoint = tm.GetPoints("spawn")[0];

            // SPRITE TEXTURES AND SOUNDS //
            var playerTextures = new Dictionary<string, TextureAnimation>
            {
                { "Idle", new TextureAnimation(Content.Load<Texture2D>("Player/hat_guy_idle"), spawnPoint, 3, 2, 60, true) { Scale = 0.15f }},
                { "Running", new TextureAnimation(Content.Load<Texture2D>("Player/hat_guy_run"), spawnPoint, 2, 4, 60, true) { Scale = 0.15f }},
                { "Attacking", new TextureAnimation(Content.Load<Texture2D>("Player/hat_guy_slash"), spawnPoint, 3, 2, 60, false) { Scale = 0.15f }},
                { "Jumping", new TextureAnimation(Content.Load<Texture2D>("Player/hat_guy_jump"), spawnPoint, 3, 2, 80, false) { Scale = 0.15f }},
                { "Hurt", new TextureAnimation(Content.Load<Texture2D>("Player/hat_guy_hurt"), spawnPoint, 2, 1, 150, false) { Scale = 0.15f }},
                { "Dying", new TextureAnimation(Content.Load<Texture2D>("Player/hat_guy_death"), spawnPoint, 3, 2, 60, false) { Scale = 0.15f }},
            };

            var playerSounds = new Dictionary<string, SoundEffect>
            {
                { "jump", Content.Load<SoundEffect>("Player/01")},
                { "attack", Content.Load<SoundEffect>("Player/swing")},
                { "hit", Content.Load<SoundEffect>("Player/00")},
                { "death", Content.Load<SoundEffect>("Player/06")},
            };

            player = new Player(gameObjectManager, playerTextures) { Sounds = playerSounds };            
            gameObjectManager.AddGameObject(player);
           
            var skeletonTextures = new Dictionary<string, TextureAnimation>
            {
                { "Idle", new TextureAnimation(Content.Load<Texture2D>("Enemies/Skeleton/skeleton_idle"), Vector2.Zero, 1, 11, 60, true) { Scale = 0.2f }},
                { "Walking", new TextureAnimation(Content.Load<Texture2D>("Enemies/Skeleton/skeleton_walk"), Vector2.Zero, 1, 13, 60, true) { Scale = 0.2f }},
                { "Reacting", new TextureAnimation(Content.Load<Texture2D>("Enemies/Skeleton/skeleton_react"), Vector2.Zero, 2, 2, 100, false) { Scale = 0.2f }},                
                { "Attacking", new TextureAnimation(Content.Load<Texture2D>("Enemies/Skeleton/skeleton_attack"), Vector2.Zero, 3, 6, 40, false) { Scale = 0.2f }},                
                { "Hurt", new TextureAnimation(Content.Load<Texture2D>("Enemies/Skeleton/skeleton_hit"), Vector2.Zero, 2, 4, 60, false) { Scale = 0.2f }},
                { "Dying", new TextureAnimation(Content.Load<Texture2D>("Enemies/Skeleton/skeleton_dead"), Vector2.Zero, 3, 5, 60, false) { Scale = 0.2f }},
            };

            var skeletonSounds = new Dictionary<string, SoundEffect>
            {
                { "react", Content.Load<SoundEffect>("Enemies/Skeleton/Blow 2")},
                { "attack", Content.Load<SoundEffect>("Enemies/Skeleton/05")},
                { "hit", Content.Load<SoundEffect>("Enemies/Skeleton/02")},
                { "death", Content.Load<SoundEffect>("Enemies/Skeleton/17")},
            };

            var goblinTextures = new Dictionary<string, TextureAnimation>
            {
                { "Idle", new TextureAnimation(Content.Load<Texture2D>("Enemies/Goblin/goblin idle"), Vector2.Zero, 1, 3, 100, true) { Scale = 0.1f }},
                { "Running", new TextureAnimation(Content.Load<Texture2D>("Enemies/Goblin/goblin run"), Vector2.Zero, 3, 2, 60, true) { Scale = 0.1f }},
                { "Attacking", new TextureAnimation(Content.Load<Texture2D>("Enemies/Goblin/goblin attack"), Vector2.Zero, 1, 7, 40, false) { Scale = 0.1f }},                
                { "Dying", new TextureAnimation(Content.Load<Texture2D>("Enemies/Goblin/goblin death"), Vector2.Zero, 4, 2, 60, false) { Scale = 0.1f }},
            };

            var goblinSounds = new Dictionary<string, SoundEffect>
            {
                { "attack", Content.Load<SoundEffect>("Enemies/Goblin/03")},
                { "death", Content.Load<SoundEffect>("Enemies/Goblin/17")},
            };

            var blobManTextures = new Dictionary<string, TextureAnimation>
            {
                { "Idle", new TextureAnimation(Content.Load<Texture2D>("Enemies/Blob Man/blob minion idle"), Vector2.Zero, 3, 2, 60, true) { Scale = 0.2f }},
                { "Walking", new TextureAnimation(Content.Load<Texture2D>("Enemies/Blob Man/blob minion walk"), Vector2.Zero, 2, 4, 100, true) { Scale = 0.2f }},                
            };

            var blobManSounds = new Dictionary<string, SoundEffect>
            {
                { "move", Content.Load<SoundEffect>("Enemies/Blob Man/slime1")},                
                { "hit", Content.Load<SoundEffect>("Enemies/Blob Man/slime5")},
                { "death", Content.Load<SoundEffect>("Enemies/Blob Man/slime8")},
            };

            var blobTextures = new Dictionary<string, TextureAnimation>
            {
                { "Idle", new TextureAnimation(Content.Load<Texture2D>("Enemies/Blob/blob idle"), Vector2.Zero, 2, 4, 60, true) { Scale = 0.2f }},
                { "Jumping", new TextureAnimation(Content.Load<Texture2D>("Enemies/Blob/blob move"), Vector2.Zero, 2, 4, 60, false) { Scale = 0.2f }},
                { "Dying", new TextureAnimation(Content.Load<Texture2D>("Enemies/Blob/blob death"), Vector2.Zero, 2, 4, 60, false) { Scale = 0.2f }},
            };

            var blobSounds = new Dictionary<string, SoundEffect>
            {
                { "jump", Content.Load<SoundEffect>("Enemies/Blob/slime2")},
                { "land", Content.Load<SoundEffect>("Enemies/Blob/slime4")},
                { "hit", Content.Load<SoundEffect>("Enemies/Blob/slime6")},
                { "death", Content.Load<SoundEffect>("Enemies/Blob/17")},
            };
        
            gameObjectManager.AddEnemyTextures("skeleton", skeletonTextures);
            gameObjectManager.AddEnemyTextures("goblin", goblinTextures);
            gameObjectManager.AddEnemyTextures("blobman", blobManTextures);
            gameObjectManager.AddEnemyTextures("blob", blobTextures);

            gameObjectManager.AddEnemySounds("skeleton", skeletonSounds);
            gameObjectManager.AddEnemySounds("goblin", goblinSounds);
            gameObjectManager.AddEnemySounds("blobman", blobManSounds);
            gameObjectManager.AddEnemySounds("blob", blobSounds);
            tm.AddEnemies(gameObjectManager);
                        

            // BACKGROUND //
            bg = Content.Load<Texture2D>("Backgrounds/rocks_2");


            // MUSIC //
            Song bgmusic = Content.Load<Song>("Music/06 Tonal Dissonance");
            MediaPlayer.Play(bgmusic);
            MediaPlayer.Volume = 0.03f;
            MediaPlayer.IsRepeating = true;


            // BUTTONS //
            buttonClickSound = Content.Load<SoundEffect>("Controls/interface5");
            var buttonTexture = Content.Load<Texture2D>("Controls/button");
            var buttonFont = Content.Load<SpriteFont>("Fonts/Button");
            var logoFont = Content.Load<SpriteFont>("Fonts/Logo");
            var healthFont = Content.Load<SpriteFont>("Fonts/Health");

            var continueButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(ScreenWidth / 2 - buttonTexture.Width / 2, ScreenHeight / 2 - 3 * buttonTexture.Height / 2),
                Text = "Continue"
            };

            var pauseMenuQuitButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(ScreenWidth / 2 - buttonTexture.Width / 2, ScreenHeight / 2 + buttonTexture.Height / 2),
                Text = "Quit"
            };

            continueButton.Click += ContinueButton_Click; 
            pauseMenuQuitButton.Click += QuitButton_Click; 

            gameObjectManager.AddPauseMenuObject(continueButton);
            gameObjectManager.AddPauseMenuObject(pauseMenuQuitButton);


            // HEALTH //
            gameObjectManager.AddGameObject(new HealthText(player, healthFont, new Vector2(200,100)) { Fixed = true, PenColour = Color.Red});

            // LOGO //
            gameObjectManager.AddGameObject(new Text("HAT MAN AND THE BADDIES", logoFont, new Vector2(ScreenWidth / 2, 350), 5000, true) { Fixed = true, });

            // INSTRUCTION TEXT //
            gameObjectManager.AddGameObject(new Text("ARROW KEYS TO MOVE", buttonFont, new Vector2(ScreenWidth / 2, 650), 5000, true) { Fixed = true});
            gameObjectManager.AddGameObject(new Text("SPACE TO JUMP", buttonFont, new Vector2(ScreenWidth / 2, 750), 5000, true) { Fixed = true });
            gameObjectManager.AddGameObject(new Text("Z TO ATTACK", buttonFont, new Vector2(ScreenWidth / 2, 850), 5000, true) { Fixed = true });


            winText = new Text("YOU WIN", logoFont, new Vector2(ScreenWidth / 2, 350)) { Fixed = true };
            loseText = new Text("YOU LOSE", logoFont, new Vector2(ScreenWidth / 2, 350)) { Fixed = true };
            
            gameObjectManager.AddWinObject(winText);            
            gameObjectManager.AddLoseObject(loseText);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameObjectManager.CurrentGameState == GameObjectManager.GameState.Won || gameObjectManager.CurrentGameState == GameObjectManager.GameState.Lost)
            {
                if (finishTimer > 0) finishTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                else Exit();
            }

            oldKeyState = newKeyState;
            newKeyState = Keyboard.GetState();
            if (newKeyState.IsKeyDown(Keys.Escape) && oldKeyState.IsKeyUp(Keys.Escape))
            {
                if (gameObjectManager.CurrentGameState == GameObjectManager.GameState.Playing)
                    OpenPauseMenu();

                else if (gameObjectManager.CurrentGameState == GameObjectManager.GameState.Paused)
                    ClosePauseMenu();
                    
            }
                

            gameObjectManager.UpdateGameObjects(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
           
            spriteBatch.Draw(bg, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            
            spriteBatch.End();


            gameObjectManager.DrawGameObjects(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        void QuitButton_Click(object sender, EventArgs e)
        {
            var sound = buttonClickSound.CreateInstance();
            sound.Volume = 0.01f;
            sound.Play();

            Exit();
        }

        void ContinueButton_Click(object sender, EventArgs e)
        {
            var sound = buttonClickSound.CreateInstance();
            sound.Volume = 0.01f;
            sound.Play();

            ClosePauseMenu();
        }

        void OpenPauseMenu()
        {
            gameObjectManager.CurrentGameState = GameObjectManager.GameState.Paused;
            IsMouseVisible = true;
        }

        void ClosePauseMenu()
        {
            gameObjectManager.CurrentGameState = GameObjectManager.GameState.Playing;
            IsMouseVisible = false;
        }

        #endregion


        #region Testing Methods

        private void ParticleEngineTesting()
        {
            var tempPEngine = new ParticleEngine(Content.Load<Texture2D>("Particles/diamond"), position: new Vector2(100, 100),
                            minVelocity: 200, maxVelocity: 500, direction: new Vector2(100, -17), lifetime: 0.3, delay: 0.09f)
            {
                EmitterWidth = 100,
                EmitterHeight = 100,
                ParticleAngularVelocity = -100,
                ParticleColour = Color.Red
            };

            var tempPEngine2 = new ParticleEngine(Content.Load<Texture2D>("Particles/star"), position: new Vector2(100, 100),
                minVelocity: 200, maxVelocity: 500, direction: new Vector2(100, -17), lifetime: 0.3, delay: 0.06f)
            {
                EmitterWidth = 100,
                EmitterHeight = 100,
                ParticleAngularVelocity = 100,
                ParticleColour = Color.Orange
            };

            var fireLeft = new RisingParticleEngine(Content.Load<Texture2D>("Particles/circle"), position: new Vector2(400, 400),
                minVelocity: 0, maxVelocity: 50, direction: new Vector2(100, -50), lifetime: 1, delay: 0.05f)
            {
                EmitterWidth = 20,
                EmitterHeight = 20,
                ParticleAngularVelocity = 100,
                ParticleColour = Color.Red,
                RiseRate = 1.0005f
            };
            var fireRight = new RisingParticleEngine(Content.Load<Texture2D>("Particles/circle"), position: new Vector2(400, 400),
                minVelocity: 0, maxVelocity: 50, direction: new Vector2(-100, -50), lifetime: 1, delay: 0.05f)
            {
                EmitterWidth = 20,
                EmitterHeight = 20,
                ParticleAngularVelocity = 100,
                ParticleColour = Color.Red,
                RiseRate = 1.0005f
            };

            var fireTop = new RisingParticleEngine(Content.Load<Texture2D>("Particles/circle"), position: new Vector2(400, 400),
                minVelocity: 0, maxVelocity: 50, direction: new Vector2(100, -50), lifetime: 1, delay: 0.05f)
            {
                EmitterWidth = 20,
                EmitterHeight = 20,
                ParticleAngularVelocity = 100,
                ParticleColour = Color.Orange,
                RiseRate = 1.0005f
            };
            var fireBot = new RisingParticleEngine(Content.Load<Texture2D>("Particles/circle"), position: new Vector2(400, 400),
                minVelocity: 0, maxVelocity: 50, direction: new Vector2(-100, -50), lifetime: 1, delay: 0.05f)
            {
                EmitterWidth = 20,
                EmitterHeight = 20,
                ParticleAngularVelocity = 100,
                ParticleColour = Color.Orange,
                RiseRate = 1.0005f
            };


            var sparkles = new GrowingParticleEngine(Content.Load<Texture2D>("Particles/diamond"), position: new Vector2(200, 400),
                minVelocity: 0, maxVelocity: 50, direction: new Vector2(-100, -50), lifetime: 1, delay: 0.4f)
            {
                EmitterWidth = 20,
                EmitterHeight = 20,
                ParticleColour = Color.Yellow
            };

            gameObjectManager.AddGameObject(tempPEngine);
            gameObjectManager.AddGameObject(tempPEngine2);
            gameObjectManager.AddGameObject(fireLeft);
            gameObjectManager.AddGameObject(fireRight);
            gameObjectManager.AddGameObject(fireTop);
            gameObjectManager.AddGameObject(fireBot);
            gameObjectManager.AddGameObject(sparkles);
        }

        

        #endregion
    }
}
