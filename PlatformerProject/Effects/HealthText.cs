using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerProject.Effects
{
    class HealthText : Text
    {
        Player Player;

        public HealthText(Player player, SpriteFont font, Vector2 position, int lifeTime = 1000, bool fadingOut = false, bool fadingIn = false, float fadePercent = 0.01F) : 
            base("Health: " + player.Health.ToString(), font, position, lifeTime, fadingOut, fadingIn, fadePercent)
        {
            Player = player;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Content = "Health: " + Player.Health.ToString();
        }
    }
}
