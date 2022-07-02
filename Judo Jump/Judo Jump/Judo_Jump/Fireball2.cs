using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Judo_Jump
{
    //This class represents player and horizontal boss fireballs.
    class Fireball2
    {
        Texture2D texture;
        public Vector2 position;
        Rectangle[] animation;
        public float rotation;
        public int timer;
        public double decimalXCoordinate, decimalYCoordinate, dx, dy;

        public Fireball2(int x, int y, string direction)
        {
            texture = Level.content.Load<Texture2D>("PlayerSpriteSheet");
            position = new Vector2(x, y);
            animation = new Rectangle[6];
            animation[0] = new Rectangle(242, 390, 33, 24);
            animation[1] = new Rectangle(364, 392, 26, 28);
            animation[2] = new Rectangle(292, 452, 16, 16);
            animation[3] = new Rectangle(314, 448, 8, 28);
            animation[4] = new Rectangle(326, 448, 18, 28);
            animation[5] = new Rectangle(348, 448, 22, 28);
            timer = 29;
            dx = (Level.player.Rectangle.X - x) / 29.0;
            dy = (Level.player.Rectangle.Bottom - y) / 29.0;
            rotation = (float)Math.Atan((double)dx / (double)dy);
            rotation += direction.Equals("r") ? MathHelper.PiOver2 : -MathHelper.PiOver2;
            decimalXCoordinate = x;
            decimalYCoordinate = y;
        }

        public void Update()
        {
            decimalXCoordinate += dx;
            decimalYCoordinate += dy;
            position = new Vector2((int)decimalXCoordinate, (int)decimalYCoordinate);
            timer--;
            if (timer == 1)
            {
                Platform closest = Level.platforms.First().First();
                double xDistance = closest.Rectangle.Center.X - position.X;
                double yDistance = closest.Rectangle.Center.Y - position.Y;
                double totalDistance = Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
                foreach (List<Platform> set in Level.platforms)
                {
                    foreach (Platform platform in set)
                    {
                        double tempXDistance = platform.Rectangle.Center.X - position.X;
                        double tempYDistance = platform.Rectangle.Center.Y - position.Y;
                        double tempTotalDistance = Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
                        if (tempTotalDistance < totalDistance)
                        {
                            closest = platform;
                        }
                    }
                }
                Level.fireList.Add(new Fire(new Rectangle((int)position.X, closest.Rectangle.Y - 25, 100, 25),
                                                         Level.content.Load<Texture2D>("Objects/Spike")));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (dx > 0)
            {
                spriteBatch.Draw(texture, position, animation[timer / 5], Color.White, rotation,
                    new Vector2(animation[timer / 5].Width / 2, animation[timer / 5].Height / 2), 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(texture, position, animation[timer / 5], Color.White, rotation,
                    new Vector2(animation[timer / 5].Width / 2, animation[timer / 5].Height / 2), 1, SpriteEffects.FlipHorizontally, 0);
            }
        }
    }
}