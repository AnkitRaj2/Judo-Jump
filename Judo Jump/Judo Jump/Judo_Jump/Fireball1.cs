using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Judo_Jump
{
    class Fireball1
    {
        Texture2D texture;
        Vector2 position;
        Rectangle[] animation;
        public int timer, velocity, animationFrame;
        Boolean isEnemy;

        public Vector2 Position { get { return position; } }


        public Fireball1(int x, int y, string direction, bool enem, int fireBallLifeSpan)
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
            velocity = direction.Equals("r") ? 18 : -18;
            isEnemy = enem;
        }

        public void Update()
        {
            position.X += velocity;
            if (!isEnemy)
            {
                for (int i = 0; i < Level.enemies.Count; i++)
                {
                    if (Level.enemies[i].Rectangle.Contains(new Point((int)position.X, (int)position.Y)))
                    {
                        position.X -= velocity;
                        if (timer % 2 == 0)
                        {
                            Level.enemies[i].health -= 2;
                        }
                        Console.WriteLine(Level.enemies[i].health);
                    }
                }
                timer--;
            }
            else
            {
                if (position.X >= 1170 || position.X <= 10)
                {
                    position.X -= velocity;
                    animationFrame++;
                }
                if (Level.player.Rectangle.Contains(new Point((int)position.X, (int)position.Y)))
                {
                    position.X -= velocity;
                    animationFrame++;
                    if (animationFrame == 5)
                    {
                        if (Level.player.health >= 25)
                        {
                            Level.player.health -= 25;
                        }
                        else
                        {
                            Level.player.health = 0;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isEnemy)
            {
                if (velocity > 0)
                {
                    spriteBatch.Draw(texture, position, animation[timer / 5], Color.White);
                }
                else
                {
                    spriteBatch.Draw(texture, position, animation[timer / 5], Color.White,
                           0, new Vector2(), 1, SpriteEffects.FlipHorizontally, 0);
                }
            }
            else
            {
                if (velocity > 0)
                {
                    spriteBatch.Draw(texture, position, animation[animationFrame], Color.White);
                }
                else
                {
                    spriteBatch.Draw(texture, position, animation[animationFrame], Color.White,
                           0, new Vector2(), 1, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }
    }
}
