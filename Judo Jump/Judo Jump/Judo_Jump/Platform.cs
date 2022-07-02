using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Judo_Jump
{
    class Platform
    {
        public Rectangle rectangle;
        public Texture2D texture;
        // If dx or dy is 0, the platform is stationary. Otherwise, it moves.
        public int dx, dy, time, timeIteration, movementDuration;

        /// <param name="x">X-Velocity</param>
        /// <param name="y">Y-Velocity</param>
        /// <param name="duration">The number of Update() method calls before reversing the velocity values</param>
        public Platform(Rectangle r, Texture2D t, int x, int y, int duration)
        {
            rectangle = r;
            texture = t;
            dx = x;
            dy = y;
            movementDuration = duration;
            timeIteration = -1;
            time = 0;
        }

        public Rectangle Rectangle
        {
            get { return rectangle; }
        }

        public void Update()
        {
            if (time == 0 || time == movementDuration)
            {
                timeIteration *= -1;
                dx *= -1;
                dy *= -1;
            }
            time += timeIteration;
            rectangle.X += dx;
            rectangle.Y += dy;
        }
    }
}
