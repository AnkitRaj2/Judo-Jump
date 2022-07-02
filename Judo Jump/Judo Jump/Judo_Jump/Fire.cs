using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Judo_Jump
{
    class Fire : Spike 
    {
        int timer,lifeTimer, damage, lifeSpan;
        Random rng = new Random();
        public int LifeSpan { get { return lifeSpan; } }
        public Fire(Rectangle rect, Texture2D tex) : base(rect,tex)
        {
            lifeSpan = rng.Next(30, 60);
            damage = 2;
        }

        public void Update(Player player)
        {
            lifeTimer++;
            if (player.Rectangle.Intersects(rectangle))
            {
                timer++;
                if (timer % 10 == 0)
                    player.health -= damage;
            }
            else
                timer = 0;

            if (lifeTimer % 20 == 0)
            {
                lifeSpan--;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.Red);
        }
    }
}
