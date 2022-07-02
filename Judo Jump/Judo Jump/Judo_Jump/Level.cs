using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Judo_Jump
{
    class Level : Microsoft.Xna.Framework.Game
    {

        public static List<List<Platform>> platforms;
        public static List<Enemy> enemies;
        public static List<Boss> bossList;
        public static List<Fire> fireList;
        public static int width, height;
        int lTimer = 0;
        public static List<Spike> spikes;
        List<Coin> coins;
        public Rectangle exitRect;
        public static ContentManager content;
        Texture2D whiteTexture;
        IServiceProvider serviceProvider;
        public static Player player;
        Coin coinCountIcon;
        Camera cam;

        public Player Player { get { return player; } }
        public Camera Camera { get { return cam; } }

        public Level(IServiceProvider serviceProvider, Camera c, Player p)
        {
            platforms = new List<List<Platform>>();
            enemies = new List<Enemy>();
            spikes = new List<Spike>();
            coins = new List<Coin>();
            fireList = new List<Fire>();
            bossList = new List<Boss>();
            this.serviceProvider = serviceProvider;
            content = new ContentManager(serviceProvider, "Content");
            whiteTexture = content.Load<Texture2D>("White Pixel");
            coinCountIcon = new Coin(new Rectangle(4, 5, 25, 25));
            cam = c;
            player = p;
        }

        public List<List<Platform>> Platforms
        {
            get { return platforms; }
        }

        public int Height
        { get { return height; } }

        public int Width
        { get { return width; } }

        /* Each line in the text file has either of the following formats:-
         * 1. Platform_FileName X Y Width Height X-Velocity Y-Velocity MovementDuration
         * 2. "Exit" X Y Width Height
         * 3. "Spikes" X-Coordinate_of_leftmost_spike Y-Coordinate Number_of_spikes_in_the_particular_set_of_spikes Damage 
         * 4. "Enemy" Leftmost_X-Position Y-Position Width Height Rightmost_X-Position
         * The line that begins with "Exit" specifies the rectangle coordinates and dimensions for the exit door in the level.
         * The line that begins with a file name specifies the name of the image file, the rectangle parameters, and the movement
         * duration of a platform in the level. */
        public void LoadLevel(string fileName, Player p)
        {
            platforms.Clear();
            enemies.Clear();
            spikes.Clear();
            coins.Clear();
            player = p;
            p.direction = "r";
            Player.coinsCollected = 0;
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string[] dimensions = reader.ReadLine().Split(' ');
                    width = Convert.ToInt32(dimensions[0]);
                    height = Convert.ToInt32(dimensions[1]);
                    while (!reader.EndOfStream)
                    {
                        string[] parts = reader.ReadLine().Split(' ');
                        switch (parts[0])
                        {
                            case "Exit":
                                exitRect = new Rectangle(Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]),
                                                         Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4]));
                                break;
                            case "Enemy":
                                enemies.Add(new Enemy(new Rectangle(Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]),
                                                                    Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4])),
                                                                    Convert.ToInt32(parts[5])));
                                break;
                            case "Spikes":
                                for (int x = 0; x < Convert.ToInt32(parts[3]); x++)
                                {
                                    spikes.Add(new Spike(new Rectangle(Convert.ToInt32(parts[1]) + x * 20, Convert.ToInt32(parts[2]), 20, 25),
                                                         content.Load<Texture2D>("Objects/Spike")));
                                }
                                break;
                            case "Coins":
                                for (int x = 0; x < Convert.ToInt32(parts[5]); x++)
                                {
                                    coins.Add(new Coin(new Rectangle(Convert.ToInt32(parts[1]) + x * 50, Convert.ToInt32(parts[2]),
                                                                     Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4]))));
                                }
                                break;
                            default:
                                while (!reader.EndOfStream)
                                {
                                    parts = reader.ReadLine().Split(' ');
                                    int x = Convert.ToInt32(parts[0]);
                                    int y = Convert.ToInt32(parts[1]);
                                    List<Platform> platformSet = new List<Platform>();
                                    for (int i = 0; i < Convert.ToInt32(parts[2]); i++)
                                    {
                                        string[] tempParts = reader.ReadLine().Split(' ');
                                        Texture2D platformTexture = content.Load<Texture2D>("Platforms/Platform" + tempParts[0]);
                                        if (tempParts[0].Equals("3"))
                                        {
                                            Rectangle platformRectangle = new Rectangle(x, y, 90, 40);
                                            platformSet.Add(new Platform(platformRectangle, platformTexture,
                                                Convert.ToInt32(tempParts[1]), Convert.ToInt32(tempParts[2]), Convert.ToInt32(tempParts[3])));
                                            x += 90;
                                        }
                                        else
                                        {
                                            Rectangle platformRectangle = new Rectangle(x, y, 150, 40);
                                            platformSet.Add(new Platform(platformRectangle, platformTexture,
                                                Convert.ToInt32(tempParts[1]), Convert.ToInt32(tempParts[2]), Convert.ToInt32(tempParts[3])));
                                            x += 150;
                                        }
                                    }
                                    platforms.Add(platformSet);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read.");
            }
        }

        public void Update(Camera c, GameTime gameTime)
        {
            for (int i = bossList.Count - 1; i >= 0; i--)
            {
                bossList[i].Update(gameTime, player);
            }
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                if (player.Rectangle.Intersects(coins[i].dest))
                {
                    Player.coinCollectSound.Play();
                    Player.coinsCollected++;
                    Player.totalScore += 5;
                    coins.Remove(coins[i]);
                }
            }
            for (int i = 0; i < platforms.Count; i++)
            {
                for (int j = 0; j < platforms[i].Count; j++)
                {
                    platforms[i][j].Update();
                }
            }
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].health == 0)
                {
                    enemies.Remove(enemies[i]);
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update();
            }
            for (int x = 0; x < coins.Count; x++)
            {
                coins[x].Update();
            }
            for (int x = 0; x < spikes.Count; x++)
            {
                spikes[x].Update(player);
            }
            for (int x = 0; x < fireList.Count; x++)
            {
                fireList[x].Update(player);
                if (fireList[x].LifeSpan <= 0)
                    fireList.Remove(fireList[x]);
            }
            coinCountIcon.Update();
        }

        public void Draw(SpriteBatch spriteBatch, Camera c)
        {
            cam = c;
            foreach (List<Platform> platformSet in platforms)
            {
                foreach (Platform platform in platformSet)
                {
                    spriteBatch.Draw(platform.texture, platform.rectangle, Color.White);
                }
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            foreach (Spike spike in spikes)
            {
                spriteBatch.Draw(spike.texture, spike.rectangle, Color.White);
            }
            foreach (Coin coin in coins)
            {
                coin.Draw(spriteBatch);
            }
            foreach (Boss b in bossList)
            {
                b.Draw(spriteBatch);
            }
            foreach (Fire f in fireList)
            {
                f.Draw(spriteBatch);
            }
            spriteBatch.Draw(whiteTexture, exitRect, Color.Black);


            spriteBatch.DrawString(Game1.tempFont, ": " + Player.coinsCollected, new Vector2(-cam.Transform.Translation.X + 30, -cam.Transform.Translation.Y), Color.Black);
            if (bossList.Capacity > 1)
            {


                spriteBatch.DrawString(Game1.tempFont, "Boss", new Vector2(-cam.Transform.Translation.X + 240, -cam.Transform.Translation.Y), Color.Black);
                spriteBatch.Draw(Game1.whiteTexture, new Rectangle((int)-cam.Transform.Translation.X + 320, (int)-cam.Transform.Translation.Y + 5, 410, 25), Color.Black);
                spriteBatch.Draw(Game1.whiteTexture, new Rectangle((int)-cam.Transform.Translation.X + 325, (int)-cam.Transform.Translation.Y + 10,
                    bossList[0].health * 2, 15), Color.Red);
            }


            spriteBatch.Draw(coinCountIcon.coin,
                new Rectangle((int)-cam.Transform.Translation.X + coinCountIcon.dest.X, (int)-cam.Transform.Translation.Y + coinCountIcon.dest.Y,
                coinCountIcon.dest.Width, coinCountIcon.dest.Height),
                coinCountIcon.sprite[coinCountIcon.r], Color.White);
                spriteBatch.DrawString(Game1.tempFont, "Health:", new Vector2(-cam.Transform.Translation.X + 880, -cam.Transform.Translation.Y), Color.Black);
                spriteBatch.Draw(Game1.whiteTexture, new Rectangle((int)-cam.Transform.Translation.X + 990, (int)-cam.Transform.Translation.Y + 5, 210, 25), Color.Black);
                spriteBatch.Draw(Game1.whiteTexture, new Rectangle((int)-cam.Transform.Translation.X + 995, (int)-cam.Transform.Translation.Y + 10,
                    player.health * 2, 15), Color.Red);

        }
    }
}