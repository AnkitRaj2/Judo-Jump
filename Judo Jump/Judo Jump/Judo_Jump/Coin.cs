using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Judo_Jump
{
    public class Coin
    {
        public Rectangle[] sprite = new Rectangle[10];

        public Rectangle dest;
        public Texture2D coin;
        int timer = 0;
        public int r = 0;
        public Coin(Rectangle dest)
        {
            this.dest = dest;
            coin = Level.content.Load<Texture2D>("coinssprite");
            sprite[0] = new Rectangle(900, 0, 100, 100);
            sprite[1] = new Rectangle(800,0, 100, 100);
            sprite[2] = new Rectangle(700, 0, 100, 100);
            sprite[3] = new Rectangle(600, 0, 100, 100);
            sprite[4] = new Rectangle(500, 0, 100, 100);
            sprite[5] = new Rectangle(400, 0, 100, 100);
            sprite[6] = new Rectangle(300, 0, 100, 100);
            sprite[7] = new Rectangle(200, 0, 100, 100);
            sprite[8] = new Rectangle(100, 0, 100, 100);
            sprite[9] = new Rectangle(0, 0, 100, 100);
        }
        public void Update()
        {

            if (timer%6==0 && timer!=60)
            {
                r++;
            }
            else if(r==9)
            {
                r =timer = 0;
            }
            timer++;
        }
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(coin, dest, sprite[r], Color.White);
        }
    }
}
