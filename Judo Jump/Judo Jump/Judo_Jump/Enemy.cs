using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Judo_Jump
{
    
    class Enemy : Actor
    {
        enum EnemyState
        {
            move, punch, regularkick, idle, backkick, lowkick
        }
        int punchtimer = 0;
        int regularkicktimer = 0;
        int lowkicktimer = 0;
        int sidekicktimer = 0;
        int c = 0;
        int u = 0;
        Rectangle idlerect;
        EnemyState currentstate = EnemyState.move;
        List<Platform> platformset;
        ContentManager content;
        Rectangle sprite;
        Texture2D enemySpriteSheet;
        int timer = 0;
        int timer2 = 0;
        int timer3 = 0;
        Rectangle[] movementAnimation = new Rectangle[4];
        Rectangle[] punchAnimation = new Rectangle[1];
        Rectangle[] regularKickAnimation = new Rectangle[2];
        Rectangle[] backKickAnimation = new Rectangle[4];
        Rectangle[] lowKickAnimation = new Rectangle[2];
        public int r = 0;
        KeyboardState oldKB = Keyboard.GetState();
        bool reachedtop, jump, regularMovement, isAttacking;
        int leftEdge, rightEdge;
        int xvelocity = -1;
        int t = 0;
        Platform currentplatform;
        // Rectangle movementAnimation2 = new Rectangle(128, 8, 64, 90);
        Random random;
        int actionNumber, actionTimer;

        public Enemy(Rectangle dest, int edge)
        {
            actRect = dest;
            // this.sprite = sprite;
            leftEdge = actRect.X;
            rightEdge = edge;
            //  actRect = actRect;
            movementAnimation[0] = new Rectangle(64, 8, 64, 93);//leg movement sprite
            movementAnimation[1] = new Rectangle(128, 8, 64, 93);//leg movement sprite      
            punchAnimation[0] = new Rectangle(170, 110, 75, 96);//punch 
            idlerect = new Rectangle(0, 5,70,95);
            regularKickAnimation[0] = new Rectangle(7, 212, 52, 96);//kick punch 1
            regularKickAnimation[1] = new Rectangle(61,212,86,96);//kick punch 2
            backKickAnimation[0] = new Rectangle(10, 321, 54, 90);
            backKickAnimation[1] = new Rectangle(70, 318, 59, 95);
            backKickAnimation[2] = new Rectangle(134, 317, 47, 95);
            backKickAnimation[3] = new Rectangle(155, 215, 98, 95);
            lowKickAnimation[0] = new Rectangle(261, 215, 56, 93);
            lowKickAnimation[1] = new Rectangle(319, 216, 77, 93);
            //   movementAnimation[3] = new Rectangle(0, 0, 64, 94);
            content = Level.content;
            enemySpriteSheet = content.Load<Texture2D>("EnemySpriteSheet");
            random = new Random();
            actionNumber = random.Next(2);
            actionTimer = 0;
            health = 50;
            regularMovement = true;
            isAttacking = false;
            direction = "r";
        }

        public void Update()
        {
            bottom = new Rectangle(actRect.X, actRect.Bottom, actRect.Width, 50);
            foreach (List<Platform> set in Level.platforms)
            {
                foreach (Platform p in set)
                {
                    if (bottom.Intersects(p.rectangle))
                    {
                        //currentplatform = p;
                        platformset = set;
                        break;
                    }
                }
            }
            bool flag = false;
            foreach (Platform p in platformset)
            {
                if (Level.player.Bottom.Intersects(p.rectangle))
                {
                    flag = true;
                    regularMovement = false;
                    break;
                }
            }
            if (!flag)
            {
                currentstate = EnemyState.move;
                regularMovement = true;
            }
            string previousdirection = direction;
            if (direction.Equals("r"))
            {
                hitbox = new Rectangle(actRect.Right, actRect.Y + actRect.Height / 2, 10, 10);
            }
            else
            {
                hitbox = new Rectangle(actRect.Left - 10, actRect.Y + actRect.Height / 2, 10, 10);
            }
            if (!regularMovement)
            {
                if (hitbox.Intersects(Level.player.Rectangle))
                {
                    isAttacking = true;
                }
                if (!isAttacking)
                {
                    currentstate = EnemyState.move;
                }
                else
                {
                    if (currentstate == EnemyState.move)
                    {
                        GenerateAttack();
                    }
                }
                switch (currentstate)
                {
                    case EnemyState.move:
                        {
                            if (actRect.Right < Level.player.Rectangle.X)
                            {
                                actRect.X++;
                                if (!direction.Equals("r"))
                                {
                                    xvelocity *= -1;
                                }
                                direction = "r";
                                Moveright();
                            }
                            if (actRect.X > Level.player.Rectangle.Right)
                            {
                                actRect.X--;
                                if (!direction.Equals("l"))
                                {
                                    xvelocity *= -1;
                                }
                                direction = "l";
                                Moveleft();
                            }
                            break;
                        }
                    case EnemyState.punch:
                        {
                            actRect.Width = punchAnimation[0].Width;
                            if (punchtimer == 30)
                            {
                                if (hitbox.Intersects(Level.player.Rectangle))
                                {
                                    if (Level.player.health >= 5)
                                    {
                                        Level.player.health -= 5;
                                    }
                                    else
                                    {
                                        Level.player.health = 0;
                                    }
                                }
                            }
                            if (punchtimer >= 30)
                            {
                                isAttacking = false;
                                GenerateAttack();
                            }
                            punchtimer++;
                            break;
                        }
                    case EnemyState.regularkick:
                        {
                            actRect.Width = regularKickAnimation[t].Width;
                            if (regularkicktimer == 10 && hitbox.Intersects(Level.player.Rectangle))
                            {
                                if (Level.player.health >= 10)
                                {
                                    Level.player.health -= 10;
                                }
                                else
                                {
                                    Level.player.health = 0;
                                }
                            }
                            if (regularkicktimer < 10)
                            {
                                t = 0;
                            }
                            else if (regularkicktimer < 40)
                            {
                                t = 1;
                            }
                            else
                            {
                                isAttacking = false;
                                GenerateAttack();
                            }
                            regularkicktimer++;
                            break;
                        }
                    case EnemyState.backkick:
                        {
                            actRect.Width = backKickAnimation[c].Width;
                            if (sidekicktimer == 33 && hitbox.Intersects(Level.player.Rectangle))
                            {
                                if (Level.player.health >= 15)
                                {
                                    Level.player.health -= 15;
                                }
                                else
                                {
                                    Level.player.health = 0;
                                }
                                if (direction.Equals("l"))
                                {
                                    Player.kickRebound = -30;
                                }
                                else
                                {
                                    Player.kickRebound = 30;
                                }
                            }
                            if (sidekicktimer < 10)
                            {
                                c = 0;
                            }
                            else if (sidekicktimer < 20)
                            {
                                c = 1;
                            }
                            else if (sidekicktimer < 33)
                            {
                                c = 2;
                            }
                            else if (sidekicktimer < 50)
                            {
                                c = 3;
                            }
                            else
                            {
                                isAttacking = false;
                                GenerateAttack();
                            }
                            sidekicktimer++;
                            break;
                        }
                    case EnemyState.lowkick:
                        {
                            actRect.Width = lowKickAnimation[u].Width;
                            if (lowkicktimer == 10 && hitbox.Intersects(Level.player.Rectangle))
                            {
                                if (Level.player.health >= 5)
                                {
                                    Level.player.health -= 5;
                                }
                                else
                                {
                                    Level.player.health = 0;
                                }
                            }
                            if (lowkicktimer < 10)
                            {
                                u = 0;
                            }
                            else if (lowkicktimer < 40)
                            {
                                u = 1;
                            }
                            else
                            {
                                isAttacking = false;
                                GenerateAttack();
                            }
                            lowkicktimer++;
                            break;
                        }
                    case EnemyState.idle:
                        {
                            actRect.Width = idlerect.Width;
                            break;
                        }
                }
            }
            KeyboardState kb = Keyboard.GetState();
            if (regularMovement)
            {
                if (actRect.X == rightEdge)
                {
                    xvelocity *= -1;
                    direction = "l";
                    timer = 0;
                    r = 0;
                }
                if (actRect.X == leftEdge)
                {
                    direction = "r";
                    timer = 0;
                    r = 0;
                    xvelocity *= -1;
                }
                if (direction.Equals("r"))
                {
                    Moveright();
                }
                if (direction.Equals("l"))
                {
                    Moveleft();
                }
                actRect.X += xvelocity;
            }

        }

        public void Moveright()
        {
            actRect.Width = movementAnimation[r].Width;
            if (timer == 20)
            {
                r++;
            }
            if (timer == 40)
            {
                r = 0; timer = 0;
            }

            timer++;
        }

        public void Moveleft()
        {
            actRect.Width = movementAnimation[r].Width;
            if (timer == 20)
            {
                r++;
            }
            if (timer == 40)
            {
                r = 0; timer = 0;
            }

            timer++;
        }

        public void GenerateAttack()
        {

            actionNumber = random.Next(4);
            switch (actionNumber)
            {
                case 0:
                    {
                        currentstate = EnemyState.punch;
                        punchtimer = 0;
                        break;
                    }
                case 1:
                    {
                        currentstate = EnemyState.regularkick;
                        regularkicktimer = 0;
                        break;
                    }
                case 2:
                    {
                        currentstate = EnemyState.backkick;
                        sidekicktimer = 0;
                        break;
                    }
                case 3:
                    {
                        currentstate = EnemyState.lowkick;
                        lowkicktimer = 0;
                        break;
                    }
            }
        }

        //public void Jump()
        //{

        //    if(timer==30)
        //    {
        //        r++;
        //    }
        //   if(r==4 && timer3<90)
        //    {
        //        actRect.Y--;
        //    }
        //   if(timer3==90)
        //    {
        //        reachedtop = true;
        //        r++;
        //    }
        //   if(reachedtop == true && r==5)
        //    {
        //        actRect.Y++;
        //    }
        //    if(actRect.Y==670 && reachedtop == true)
        //    {
        //        reachedtop = false;
        //        r = 0;
        //        timer3 = 0;
        //    }

        //    timer3++;
        //}
        public void Draw(SpriteBatch spritebatch)
        {
            Rectangle srcRect = new Rectangle();
            SpriteEffects orientation = SpriteEffects.None;
            switch (currentstate)
            {
                case EnemyState.move:
                    {
                        srcRect = movementAnimation[r];
                        break;
                    }
                case EnemyState.punch:
                    {
                        srcRect = punchAnimation[0];
                        break;
                    }
                case EnemyState.regularkick:
                    {
                        srcRect = regularKickAnimation[t];
                        break;
                    }
                case EnemyState.backkick:
                    {
                        srcRect = backKickAnimation[c];
                        break;
                    }
                case EnemyState.lowkick:
                    {
                        srcRect = lowKickAnimation[u];
                        break;
                    }
                case EnemyState.idle:
                    {
                        srcRect = idlerect;
                        break;
                    }
            }
            if (direction.Equals("l"))
            {
                orientation = SpriteEffects.FlipHorizontally;
            }
            spritebatch.Draw(enemySpriteSheet, actRect, srcRect, Color.White, 0, new Vector2(0, 0), orientation, 1);
        }
    }
}