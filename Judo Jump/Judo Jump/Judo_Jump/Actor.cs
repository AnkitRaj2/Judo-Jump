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
    class Actor
    {
        //gravity value. Int for now but might change to a float later.
        protected int gravity = 10;

        //Player input why isn't this a dictionary????
        protected List<String> keyNames;
        protected List<Keys> keyInputs;

        public string direction;

        //values for jumping
        protected bool jump;
        protected int numJump;
        protected int maxJumps;
        protected bool isFalling;
        protected float yPos;
        protected int jumpTimer;
        protected bool onPlatform;

        public int health;

        //platforms in the current level
        protected Level level;

        //Camera stuff
        protected Vector2 Position;
        public Vector2 getPos
        {
            get
            {
                return Position;
            }
        }

        //collision detection rectangles
        protected Rectangle bottom;
        protected Rectangle top;
        protected Rectangle right;
        protected Rectangle left;

        protected Rectangle hitbox;

        //stuff needed to draw the actor. I would reccomend using the collision detection rectangles instead of the rectangle for drawing.
        protected Rectangle srcRect, actRect;
        protected Texture2D tex;

        public Actor()
        {
            actRect = new Rectangle(100, 675, 100, 100);
            isFalling = true;
            bottom = new Rectangle(actRect.X, actRect.Y + (actRect.Height - 10), actRect.Width, 10);
            jumpTimer = 0;
            Position = new Vector2(actRect.Center.X - 375, actRect.Center.Y - 250);
        }

        public Actor(Level l, Texture2D t, List<String> key, List<Keys> input)
        {
            actRect = new Rectangle(100, 100, 100, 100);
            tex = t;
            bottom = new Rectangle(actRect.X, actRect.Y + (actRect.Height - 10), actRect.Width, 10);
            isFalling = true;
            keyInputs = input;
            keyNames = key;
            jumpTimer = 0;
            numJump = 1;
            maxJumps = 2;
            Position = new Vector2(actRect.Center.X - 375, actRect.Center.Y - 250);
        }

        public Rectangle Rectangle {get { return actRect; } }
        public int getJumpTimer { get { return jumpTimer; } }

        public Rectangle Bottom
        {
            get { return bottom; }
        }

        public void SetX(int x)
        {
            actRect.X = x;
        }

        public void SetY(int y)
        {
            actRect.Y = y;
        }

        //sets certain values so that the child class doesn't have to repeat code.
        public void Setup()
        {
            isFalling = true;
            Position = new Vector2(actRect.Center.X - 375, actRect.Center.Y - 250);
            bottom = new Rectangle(actRect.X, actRect.Y + (actRect.Height - 10), actRect.Width, 10);
            actRect = new Rectangle(100, 100, 100, 100);
            //keyInputs = input;
            //keyNames = key;
            jumpTimer = 0;
        }

        //in this update method it will handle gravity
        public void Update(GameTime gameTime)
        {
            Position = new Vector2(actRect.Center.X - 375, actRect.Center.Y - 250);
            top = new Rectangle(actRect.X, actRect.Top, actRect.Width, 15);
            bottom = new Rectangle(actRect.X, actRect.Bottom, actRect.Width, 15);
            onPlatform = false;
            if (jumpTimer == 0)
            {
                foreach (List<Platform> set in level.Platforms)
                {
                    foreach (Platform p in set)
                    {

                        if (bottom.Intersects(p.rectangle))
                        {
                            if (p.dx != 0 || p.dy != 0)
                            {
                                actRect.X += p.dx;
                                actRect.Y += p.dy;
                            }
                            actRect.Y = p.rectangle.Y - actRect.Height - 6;
                            bottom = new Rectangle(actRect.X, actRect.Bottom - 15, actRect.Width, 15);
                            // isFalling = false;
                            onPlatform = true;
                            numJump = maxJumps;
                        }
                        else
                        {
                            isFalling = true;
                        }
                    }
                }
                if (isFalling)
                {

                    actRect.Y += gravity;
                    bottom.Y += gravity;
                }
            }
            if (jumpTimer > 0)
            {
                actRect.Y -= jumpTimer / 2;
                bottom.Y -= jumpTimer / 2;
                jumpTimer--;
                foreach (List<Platform> set in level.Platforms)
                {
                    foreach (Platform p in set)
                    {

                        if (top.Intersects(p.rectangle))
                        {
                            jumpTimer = 0;
                        }
                    }
                }
            }
        }

        //draws actor to screen....duh
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(tex, actRect, srcRect, Color.White);
        }


    }
}
