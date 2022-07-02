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
    class Boss : Actor
    {
        public enum BossSate
        {
            idle, run, dashAttack, stunned, follow, fireball1, fireball2
        }
        protected Texture2D bossspritesheet;
        protected SpriteEffects oriontation;
        public BossSate state;
        Rectangle bossdest;
        protected Rectangle[] motionsprite = new Rectangle[4];
        Rectangle[] BossSuper = new Rectangle[7];
        String direction = "r";
        float endRun, leftEnd, rightEnd;
        protected int spriteChange;
        int movingPosition, xVelocity, idleTime, bossDistance, stunTime, dashStartUp, vertDistance, followTimer;
        int dashSpriteChange, DashTimer;
        Boolean regularmovement;
        bool start, isOnLeft, hasHit, isChaseBoss, above;
        Player player;
        List<Fireball1> horizontalFireballs;
        List<Fireball2> diagonalFireballs;

        public int BossDistance { get { return bossDistance; } }

        public Boss(Level l, Texture2D t, Vector2 location, bool chase)
        {
            level = l;
            player = level.Player;
            actRect = new Rectangle((int)location.X - 55, (int)location.Y - 105, 111, 200);
            bossspritesheet = Level.content.Load<Texture2D>("bossspritesheet");
            motionsprite[0] = new Rectangle(218, 19, 48, 85);
            motionsprite[1] = new Rectangle(288, 14, 44, 94);
            motionsprite[2] = new Rectangle(358, 15, 54, 92);
            motionsprite[3] = new Rectangle(417, 24, 73, 85);
            BossSuper[0] = new Rectangle(10, 762, 58, 83);
            BossSuper[1] = new Rectangle(73, 768, 64, 79);
            BossSuper[2] = new Rectangle(257, 783, 100, 50);
            BossSuper[3] = new Rectangle(342, 783, 100, 50);
            BossSuper[4] = new Rectangle(447, 783, 100, 50);
            BossSuper[5] = new Rectangle(549, 783, 100, 50);
            BossSuper[6] = new Rectangle(546, 791, 95, 34);
            horizontalFireballs = new List<Fireball1>();
            diagonalFireballs = new List<Fireball2>();
            dashSpriteChange = 0;
            health = 200;
            spriteChange = 1;
            xVelocity = -1;
            movingPosition = 0;
            isChaseBoss = chase;
            if (isChaseBoss)
            {
                state = BossSate.follow;
                followTimer = 360;
            }
            else
                state = 0;
            idleTime = 300;
            regularmovement = true;

        }
        public void Update(GameTime gameTime, Player p)
        {
            player = p;
            if (collideWithFireBall())
            {
                if (state != BossSate.stunned)
                    health -= 2;
                else
                    health -= 10;
            }
            if (!isChaseBoss && state != BossSate.dashAttack)
                base.Update(gameTime);
            if (state != BossSate.dashAttack)
                actRect = new Rectangle(actRect.X, actRect.Y, 111, 200);
            if (isOnLeft)
            {
                direction = "l";
            }
            else
            {
                direction = "r";
            }
            if (state == BossSate.idle)
            {
                checkDistance();
                idleTime--;
                if (idleTime <= 0)
                {
                    endRun = player.Rectangle.X;
                    int random = (new Random()).Next(10);
                    
                    if (bossDistance >= 650 || BossDistance <= - 650)
                    {
                        if (random <= 3)
                        {
                            actRect.Y += 20;
                            state = BossSate.dashAttack;
                        }
                        else
                        {
                            state = BossSate.fireball1;
                        }
                    }
                    else
                    {
                        if (random < 5)
                        {
                            state = BossSate.run;
                        }
                        else
                        {
                            state = BossSate.fireball2;
                        }
                    }
                }
            }

            //if by random the boss decides to run it will go to the last known player position and it ends when it gets "close enough"
            if (state == BossSate.run)
            {
                checkDistance();
                leftEnd = endRun - 50;
                rightEnd = endRun + player.Rectangle.Width + 10;
                if (leftEnd <= actRect.X)
                {
                    actRect.X -= 6;
                }

                if (leftEnd >= actRect.Right)
                {
                    actRect.X += 6;
                }

                if (collideWithFireBall())
                {
                    health -= 10;
                }

                if (actRect.Right >= leftEnd && actRect.X <= rightEnd)
                {
                    idleTime = 180;
                    state = BossSate.idle;
                }

            }

            if (state == BossSate.stunned)
            {
                checkDistance();

                if (stunTime > 0)
                {
                    stunTime--;
                }
                if (stunTime <= 0)
                {
                    idleTime = 180;
                    state = BossSate.idle;
                }
            }

            if (state == BossSate.dashAttack)
            {
                actRect = new Rectangle(actRect.X, actRect.Y, 200, 100);

                //Animation
                if (DashTimer % 4 == 0)
                {
                    dashSpriteChange++;
                    if (dashSpriteChange >= 6)
                        dashSpriteChange = 2;
                    actRect.Width = BossSuper[dashSpriteChange].Width;
                }
                if (!isChaseBoss)
                {
                    //movement
                    if (isOnLeft)
                        actRect.X -= 13;
                    else
                        actRect.X += 13;
                    //stun
                    if ((!isOnLeft && actRect.Right >= 1200) || (isOnLeft && actRect.Left <= 0))
                    {
                        stunTime = 240;
                        actRect.Y -= 0;
                        state = BossSate.stunned;
                    }
                    if (collideWithFireBall())
                    {
                        stunTime = 240;
                        actRect.Y -= 0;
                        state = BossSate.stunned;
                    }



                    if (actRect.Intersects(player.Rectangle) && !hasHit)
                    {
                        player.decrementPlayerHealth(25);
                        hasHit = true;
                    }
                    else
                    {

                        hasHit = false;

                    }
                }
                else
                {
                    if (isOnLeft)
                        actRect.X -= 13;
                    else
                        actRect.X += 13;

                    if (actRect.Right >= player.Rectangle.X + 1000 && !isOnLeft || actRect.Left <= player.Rectangle.X - 200 && isOnLeft)
                    {
                        followTimer = 240;
                        state = BossSate.follow;
                    }

                    if (actRect.Intersects(player.Rectangle) && !hasHit)
                    {
                        player.decrementPlayerHealth(25);
                        hasHit = true;
                    }
                    else
                    {

                        hasHit = false;

                    }

                }

                DashTimer++;

            }
            if (state == BossSate.fireball1)
            {
                if (actRect.X < 1200 - actRect.Right && actRect.X >= 4)
                {
                    direction = "l";
                    actRect.X -= 4;
                }
                else if (actRect.X >= 1200 - actRect.Right && actRect.Right <= 1196)
                {
                    direction = "r";
                    actRect.X += 4;
                }
                else
                {
                    if (actRect.X <= 4)
                    {
                        direction = "r";
                    }
                    else
                    {
                        direction = "l";
                    }
                    if (horizontalFireballs.Count == 0)
                    {
                        horizontalFireballs.Add(new Fireball1(actRect.Center.X, actRect.Center.Y + 20, direction, true, 29));
                    }
                }
            }
            if (state == BossSate.fireball2)
            {
                if (diagonalFireballs.Count == 0)
                {
                    diagonalFireballs.Add(new Fireball2(actRect.Center.X, actRect.Center.Y, direction));
                }
            }
            if (state == BossSate.follow)
            {
                checkDistance();
                checkYDistance();

                actRect.Y += (int)MathHelper.Clamp((float)(vertDistance * .1), -10, 10);
                if (actRect.Right <= player.Rectangle.X - 250)
                {
                    actRect.X += (int)(bossDistance * .015);
                }
                if (actRect.Left >= player.Rectangle.Right + 600)
                {
                    actRect.X += (int)(bossDistance * .009);
                }
                followTimer--;
                if (followTimer <= 0)
                {
                    state = BossSate.dashAttack;
                }

            }
            for (int i = horizontalFireballs.Count - 1; i >= 0; i--)
            {
                horizontalFireballs[i].Update();
                if ((Level.player.Rectangle.Contains(new Point((int)horizontalFireballs[i].Position.X, (int)horizontalFireballs[i].Position.Y)) &&
                    horizontalFireballs[i].animationFrame == 1) || horizontalFireballs[i].animationFrame == 5)
                {
                    horizontalFireballs.Remove(horizontalFireballs[i]);
                    idleTime = 60;
                    state = BossSate.idle;
                }
            }
            for (int i = diagonalFireballs.Count - 1; i >= 0; i--)
            {
                diagonalFireballs[i].Update();
                if (diagonalFireballs[i].timer == 0)
                {
                    diagonalFireballs.Remove(diagonalFireballs[i]);
                    idleTime = 60;
                    state = BossSate.idle;
                }
            }
        }

        public void checkDistance()
        {
            //check if boss is on either side, and find boss distance from player
            if (player.Rectangle.X > actRect.X)
            {
                bossDistance = player.Rectangle.X - actRect.X;
                isOnLeft = false;
            }
            else if (player.Rectangle.Right < actRect.X)
            {
                bossDistance = player.Rectangle.X - actRect.X;
                isOnLeft = true;
            }
        }

        public void checkYDistance()
        {
            if (!isChaseBoss)
            vertDistance = player.Rectangle.Y - actRect.Y;
            else
            {
                vertDistance = (player.Rectangle.Y - actRect.Y) - 50;

            }

        }

        public Boolean collideWithFireBall()
        {
            foreach (Fireball1 f in player.FireBalls)
            {
                if ((f.Position.X >= actRect.X && f.Position.X <= actRect.Right) && (f.Position.Y >= actRect.Y && f.Position.Y <= actRect.Bottom))
                {
                    player.FireBalls.Remove(player.FireBalls[0]);
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (direction.Equals("r"))
            {
                oriontation = SpriteEffects.None;
            }



            if (direction.Equals("l"))
            {
                oriontation = SpriteEffects.FlipHorizontally;
            }
            if (state != BossSate.dashAttack)
                spriteBatch.Draw(bossspritesheet, actRect, motionsprite[spriteChange], Color.White, 0, new Vector2(0, 0), oriontation, 1);
            if (state == BossSate.dashAttack)
                spriteBatch.Draw(bossspritesheet, actRect, BossSuper[dashSpriteChange], Color.White, 0, new Vector2(0, 0), oriontation, 1);
            foreach (Fireball1 fireball in horizontalFireballs)
            {
                fireball.Draw(spriteBatch);
            }
            foreach (Fireball2 fireball in diagonalFireballs)
            {
                fireball.Draw(spriteBatch);
            }
        }
    }
}





//    class Boss : Actor
//    {
//        Rectangle[] idleAnimation;
//        public Boss(Level l, Texture2D t, Vector2 location)
//        {
//            level = l;
//            tex = t;
//            actRect = new Rectangle(200, 200, (int)location.X, (int)location.Y);
//            health = 300;
//            direction = "l";

//            idleAnimation = new Rectangle[4];
//            idleAnimation[0] = new Rectangle(1, 1, 47, 84);
//            idleAnimation[1] = new Rectangle(47, 1, 47, 84);
//            idleAnimation[2] = new Rectangle(94, 1, 47, 84);
//            idleAnimation[3] = new Rectangle(141, 1, 47, 84);
//        }

//        public void Update(GameTime gameTime)
//        {
//            base.Update(gameTime);
//        }
//    }
//}
