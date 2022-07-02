using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Judo_Jump
{
    class Spike
    {
        public Rectangle rectangle;
        public Texture2D texture;

        public Spike(Rectangle r, Texture2D t)
        {
            rectangle = r;
            texture = t;
        }

        public void Update(Player player)
        {
            if (player.FallCollision.Intersects(rectangle) && player.IsFalling && player.getJumpTimer == 0)
                player.health = 0;
        }
    }
}
