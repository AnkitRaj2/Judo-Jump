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
    class Player : Actor
    {
        enum PlayerState
        {
            Idle, Walking, Jumping, Punching, Kicking, Fireball, Teleportation
        }

        int speed;
        float dash;
        int dashTimer, idleTimer, walkTimer, jumpAndFallTimer, actionTimer, FireballTimer, teleporttimer,
            soundeffectTimer;
        int buffer;
        int jumpSpeed;
        //Special ability 0 represents fireball, 1 represents airdash, and 2 represents teleportation
        int specialAbilityNumber;
        public static int coinsCollected, totalScore = 0, kickRebound, playerTimer;
        string FireballDirection;
        KeyboardState oldKB;
        public static SoundEffect currentSound, walkingSound, coinCollectSound,
            dashsound, jumpsound, punchsound, kicksound, hadukensound, spikesound, teleportsound;
        Rectangle fallCollision;
        Rectangle[] idleAnimation, walkAnimation, jumpAnimation, 
                    punchAnimation, kickAnimation, fireBallLaunchAnimation, FireballProjectileAnimation;
        Vector2 FireballPosition;
        List<Fireball1> fireballs;
        PlayerState state;
        Keys[] airdashDirections;
        Boolean IsTeleported;

        public List<Fireball1> FireBalls { get { return fireballs; } }
        public Level Level { get { return level; } }
        public Rectangle FallCollision { get { return fallCollision; } }
        public bool IsFalling { get { return isFalling; } }

        public Player(Level l, Texture2D t, List<String> key, List<Keys> input)
        {
            level = l;
            playerTimer = 0;
            tex = t;
            base.Setup();
            actRect = new Rectangle(100, 668, 49, 93);
            idleAnimation = new Rectangle[4];
            walkAnimation = new Rectangle[5];
            jumpAnimation = new Rectangle[7];
            punchAnimation = new Rectangle[5];
            kickAnimation = new Rectangle[5];
            fireBallLaunchAnimation = new Rectangle[3];
            FireballProjectileAnimation = new Rectangle[6];
            idleAnimation[0] = new Rectangle(1, 1, 47, 84);
            idleAnimation[1] = new Rectangle(48, 1, 47, 84);
            idleAnimation[2] = new Rectangle(97, 1, 47, 84);
            idleAnimation[3] = new Rectangle(141, 1, 47, 84);
            walkAnimation[0] = new Rectangle(1, 85, 47, 85);
            walkAnimation[1] = new Rectangle(47, 85, 47, 85);
            walkAnimation[2] = new Rectangle(94, 85, 47, 85);
            walkAnimation[3] = new Rectangle(141, 85, 47, 85);
            walkAnimation[4] = new Rectangle(188, 85, 47, 85);
            jumpAnimation[0] = jumpAnimation[6] = new Rectangle(235, 508, 35, 96);
            punchAnimation[0] = punchAnimation[4] = new Rectangle(440, 44, 47, 83);
            punchAnimation[1] = punchAnimation[3] = new Rectangle(490, 40, 47, 85);
            punchAnimation[2] = new Rectangle(547, 40, 65, 85);
            kickAnimation[0] = kickAnimation[4] = new Rectangle(285, 173, 45, 83);
            kickAnimation[1] = kickAnimation[3] = new Rectangle(328, 168, 46, 85);
            kickAnimation[2] = new Rectangle(372, 168, 72, 85);
            fireBallLaunchAnimation[0] = new Rectangle(7, 375, 53, 85);
            fireBallLaunchAnimation[1] = new Rectangle(65, 375, 55, 85);
            fireBallLaunchAnimation[2] = new Rectangle(142, 375, 64, 85);
            fireballs = new List<Fireball1>();
            for (int i = 1; i <= 5; i++)
            {
                jumpAnimation[i] = new Rectangle(50 + 35 * (i - 1), 508, 35, 96);
            }
            tex = Level.content.Load<Texture2D>("PlayerSpriteSheet");
            walkingSound = Level.content.Load<SoundEffect>("Sound Effects/Walk");
            dashsound = Level.content.Load<SoundEffect>("Sound Effects/teleport");
            punchsound = Level.content.Load<SoundEffect>("Sound Effects/punch");
            kicksound = Level.content.Load<SoundEffect>("Sound Effects/kick");
            hadukensound = Level.content.Load<SoundEffect>("Sound Effects/fireball");
            spikesound = Level.content.Load<SoundEffect>("Sound Effects/tikispike");
            teleportsound = Level.content.Load<SoundEffect>("Sound Effects/teleport");
            coinCollectSound = Level.content.Load<SoundEffect>("Sound Effects/Coins");
            Console.WriteLine(walkingSound.Duration.Milliseconds);
            speed = 5;
            dash = 1;
            keyNames = key;
            keyInputs = input;
            jumpSpeed = 90;
            numJump = coinsCollected = 0;
            specialAbilityNumber = 0;
            maxJumps = 1;
            health = 100;
            kickRebound = 0;
            idleTimer = walkTimer = jumpAndFallTimer = actionTimer = soundeffectTimer = 0;
            direction = "r";
        }

        public void Update(GameTime gameTime, KeyboardState kb)
        {
            Console.WriteLine(state + " " + actionTimer);
            Level.player = this;
            fallCollision = new Rectangle(actRect.X, actRect.Bottom, actRect.Width, 15);
            base.Update(gameTime);
            playerTimer++;
            //timer for dash
            if (direction.Equals("r"))
            {
                hitbox = new Rectangle(actRect.Right, actRect.Y + actRect.Height / 2, 50, 10);
            }
            else
            {
                hitbox = new Rectangle(actRect.Left - 50, actRect.Y + actRect.Height / 2, 50, 10);
            }
            if (dashTimer >= 120)
                dash = 2;
            else
                dash = 1;
            if (kickRebound > 0)
            {
                actRect.X++;
                kickRebound--;
            }
            if (kickRebound < 0)
            {
                actRect.X--;
                kickRebound++;
            }
            for (int i = fireballs.Count - 1; i >= 0; i--)
            {
                fireballs[i].Update();
                if (fireballs[i].timer == 0)
                {
                    fireballs.Remove(fireballs[i]);
                }
            }
            bool beingAttacked = false;

            for (int i = 0; i < Level.bossList.Count; i++)
            {
                if (actRect.Intersects(Level.bossList[i].Rectangle))
                {
                    beingAttacked = true;
                    switch (state)
                    {
                        case PlayerState.Punching:
                            {
                                if (actionTimer == 15)
                                {
                                    if (Level.bossList[i].state != Boss.BossSate.stunned)
                                    {
                                        if (Level.bossList[i].health >= 2)
                                        {
                                            Level.bossList[i].health -= 2;
                                        }
                                        else
                                        {
                                            Level.bossList[i].health = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (Level.bossList[i].health >= 6)
                                        {
                                            Level.bossList[i].health -= 6;
                                        }
                                        else
                                        {
                                            Level.bossList[i].health = 0;
                                        }
                                    }
                                }
                                break;
                            }
                        case PlayerState.Kicking:
                            {
                                if (actionTimer % 30 == 0)
                                {
                                    if (Level.bossList[i].state != Boss.BossSate.stunned)
                                    {
                                        if (Level.bossList[i].health >= 4)
                                        {
                                            Level.bossList[i].health -= 4;
                                        }
                                        else
                                        {
                                            Level.bossList[i].health = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (Level.bossList[i].health >= 10)
                                        {
                                            Level.bossList[i].health -= 10;
                                        }
                                        else
                                        {
                                            Level.bossList[i].health = 0;
                                        }

                                    }
                                }
                                break;
                            }
                    }
                }
            }

                for (int i = 0; i < Level.enemies.Count; i++)
                {
                    if (actRect.Intersects(Level.enemies[i].Rectangle))
                    {
                        beingAttacked = true;
                        switch (state)
                        {
                            case PlayerState.Punching:
                                {
                                    if (actionTimer == 15)
                                    {
                                        if (Level.enemies[i].health >= 5)
                                        {
                                            Level.enemies[i].health -= 5;
                                        }
                                        else
                                        {
                                            Level.enemies[i].health = 0;
                                        }
                                    }
                                    break;
                                }
                            case PlayerState.Kicking:
                                {
                                    if (actionTimer % 30 == 0)
                                    {
                                        if (Level.enemies[i].health >= 10)
                                        {
                                            Level.enemies[i].health -= 10;
                                        }
                                        else
                                        {
                                            Level.enemies[i].health = 0;
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                if (Level.enemies[i].Rectangle.Contains(new Point((int)FireballPosition.X, (int)FireballPosition.Y)))
                {
                    if (FireballDirection.Equals("r"))
                    {
                        FireballPosition.X--;
                    }
                    else
                    {
                        FireballPosition.X++;
                    }
                    if (FireballTimer == 1)
                    {
                        Level.enemies[i].health = 0;
                    }
                }
            }
            if (!kb.IsKeyDown(keyInputs[keyNames.IndexOf("moveLeft")]) &&
                !kb.IsKeyDown(keyInputs[keyNames.IndexOf("moveRight")]) &&
                actionTimer == 0)
            {
                state = PlayerState.Idle;
            }
            if (beingAttacked && state == PlayerState.Jumping)
            {
                actionTimer = 0;
            }
            if (actionTimer == 0)
            {
                if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("moveLeft")]))
                {
                    if (soundeffectTimer == 0)
                    {
                        currentSound = walkingSound;
                    }                    
                    soundeffectTimer++;
                    direction = "l";
                    Move(1);
                    dashTimer++;
                    if (onPlatform)
                    {
                        if (soundeffectTimer % 20 == 0)
                        {
                            currentSound.Play();
                        }
                        state = PlayerState.Walking;
                    }
                }
                else if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("moveRight")]))
                {
                    if (soundeffectTimer == 0)
                    {
                        currentSound = walkingSound;
                    }
                    soundeffectTimer++;
                    direction = "r";
                    Move(2);
                    dashTimer++;
                    if (onPlatform)
                    {
                        if (soundeffectTimer % 20 == 0)
                        {
                            currentSound.Play();
                        }
                        state = PlayerState.Walking;
                    }
                }
            }
            if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("jump")]) && onPlatform && numJump > 0 &&
                actionTimer == 0)
            {
                jumpTimer = 28;
                numJump--;
            }
            if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("punch")]) &&
                state != PlayerState.Walking &&
                onPlatform &&
                state != PlayerState.Kicking &&
                state != PlayerState.Fireball)
            {                
                actionTimer = 29;
                state = PlayerState.Punching;
            }
            if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("kick")]) &&
                state != PlayerState.Walking &&
                onPlatform &&
                state != PlayerState.Punching &&
                state != PlayerState.Fireball)
            {
                actionTimer = 44;
                state = PlayerState.Kicking;
            }
            if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("useSpecialAbility")]) &&
               !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("useSpecialAbility")]))
            {
                if (specialAbilityNumber == 0 &&
                    state != PlayerState.Walking &&
                    state != PlayerState.Punching &&
                    state != PlayerState.Kicking &&
                    fireballs.Count == 0)
                {
                    hadukensound.Play();
                    if (onPlatform)
                    {
                        actionTimer = 29;
                        state = PlayerState.Fireball;
                    }
                    if (direction.Equals("r"))
                    {
                        fireballs.Add(new Fireball1(actRect.Right + 16, actRect.Center.Y - 20, direction, false, 29));
                    }
                    else
                    {
                        fireballs.Add(new Fireball1(actRect.Left - 16, actRect.Center.Y - 20, direction, false, 29));
                    }
                }
                if (specialAbilityNumber == 1 &&
                    state != PlayerState.Punching &&
                    state != PlayerState.Kicking &&
                    onPlatform &&
                    state != PlayerState.Fireball &&
                    kb.GetPressedKeys().Count() > 1)
                {
                    airdashDirections = kb.GetPressedKeys();
                    if (airdashDirections.Contains(Keys.Up))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            actRect.Y--;
                        }
                    }
                    if (airdashDirections.Contains(Keys.Down))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            actRect.Y++;
                        }
                    }
                    if (airdashDirections.Contains(Keys.Right))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            actRect.X++;
                        }
                    }
                    if (airdashDirections.Contains(Keys.Left))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            actRect.X--;
                        }
                    }
                }
            }
            if (kb.IsKeyDown(keyInputs[keyNames.IndexOf("cycleThroughSpecialAbilities")]) &&
                !oldKB.IsKeyDown(keyInputs[keyNames.IndexOf("cycleThroughSpecialAbilities")]))
            {
                if (specialAbilityNumber == 2)
                {
                    specialAbilityNumber = 0;
                }
                else
                {
                    specialAbilityNumber++;
                }
            }
            if (!kb.IsKeyDown(keyInputs[keyNames.IndexOf("moveLeft")]) && !kb.IsKeyDown(keyInputs[keyNames.IndexOf("moveRight")]))
            {
                dashTimer = 0;
            }
            bool inAir = true;
            foreach (List<Platform> set in Level.platforms)
            {
                foreach (Platform platform in set)
                {
                    if (fallCollision.Intersects(platform.rectangle))
                    {
                        inAir = false;
                        break;
                    }
                }
            }
            if (inAir)
            {
                state = PlayerState.Jumping;
            }
            if (kb.IsKeyDown(Keys.Space))
                Setup();
            if (kb.IsKeyDown(Keys.RightAlt) && !oldKB.IsKeyDown(Keys.RightAlt))
                Level.bossList.Add(new Boss(level, tex, new Vector2(actRect.X + 40, actRect.Y), false));
            if (kb.IsKeyDown(Keys.D0) && !oldKB.IsKeyDown(Keys.D0))
                Level.bossList.Add(new Boss(level, tex, new Vector2(actRect.X + 40, actRect.Y), true));
            if (state != PlayerState.Idle)
            {
                idleTimer = 0;
            }
            if (state != PlayerState.Walking)
            {
                walkTimer = 0;
            }
            if (state != PlayerState.Jumping || jumpAndFallTimer == 44)
            {
                jumpAndFallTimer = 0;
            }
            if (kb.IsKeyDown(Keys.C) && !oldKB.IsKeyDown(Keys.C))
            {
                //teleporttimer = 0;
                //IsTeleported = true;
                //state = PlayerState.Teleportation;
                Teleport();
            }
            switch (state)
            {
                case PlayerState.Idle:
                    {
                        idleTimer++;
                        srcRect = idleAnimation[(idleTimer / 8) % 4];
                        break;
                    }
                case PlayerState.Walking:
                    {
                        walkTimer++;
                        srcRect = walkAnimation[(walkTimer / 8) % 5];
                        break;
                    }
                case PlayerState.Jumping:
                    {
                        jumpAndFallTimer++;
                        if (jumpAndFallTimer < 42)
                        {
                            srcRect = jumpAnimation[jumpAndFallTimer / 6];
                        }
                        else
                        {
                            srcRect = jumpAnimation[0];
                        }
                        break;
                    }
                case PlayerState.Punching:
                    {
                        actionTimer--;
                        srcRect = punchAnimation[actionTimer / 6];
                        break;
                    }
                case PlayerState.Kicking:
                    {
                        actionTimer--;
                        srcRect = kickAnimation[actionTimer / 9];
                        break;
                    }
                case PlayerState.Fireball:
                    {
                        actionTimer--;
                        srcRect = fireBallLaunchAnimation[actionTimer / 10];
                        break;
                    }
            }
            actRect.Width = srcRect.Width;
            oldKB = kb;
        }

        public void Move(int num)
        {
            switch (num)
            {
                case 1:
                    actRect.X -= (int)(speed);
                    break;
                case 2:
                    actRect.X += (int)(speed);
                    break;
            }
        }

        public void Teleport()
        {
            if (direction.Equals("r"))
            {
                actRect.X += 165;
            }
            if (direction.Equals("l"))
            {
                actRect.X -= 165;
            }
            teleportsound.Play();
        }

        public static int getTotalScore()
        {
            return totalScore;
        }

        public static int getCoinsCollected()
        {
            return coinsCollected;
        }

        public static int getPlayerTimer()
        {
            return playerTimer;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (direction.Equals("r"))
            {
                spritebatch.Draw(tex, actRect, srcRect, Color.White);
            }
            else if (direction.Equals("l"))
            {
                spritebatch.Draw(tex, actRect, srcRect, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 1);
            }
            foreach (Fireball1 fireball in fireballs)
            {
                fireball.Draw(spritebatch);
            }
        }

        public void decrementPlayerHealth(int amount)
        {

            health = health - amount;

        }
    }
}