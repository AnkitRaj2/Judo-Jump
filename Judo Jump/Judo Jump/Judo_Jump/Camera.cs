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
    class Camera
    {

        public Matrix Transform;
        Viewport view;
        Vector2 center;
        bool yLocked, xLocked;
        int width;
        int height;
        public enum CameraState
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT,
            UP_LEFT,
            UP_RIGHT,
            DOWN_LEFT,
            DOWN_RIGHT
        }
        public CameraState state = CameraState.NONE;
        public Vector2 Center
        {
            get { return center; }
        }
        public bool isYLocked
        {
            get { return yLocked; }
        }
        public bool isXLocked
        {
            get { return xLocked; }
        }

        public Camera(Viewport viewport)
        {
            view = viewport;
            Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                Matrix.CreateTranslation(new Vector3(0, 0, 0));

        }

        public void Update(GameTime gameTime, Player man, Level level)
        {
            height = level.Height;
            width = level.Width;
            center = man.getPos;
            Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                Matrix.CreateTranslation(new Vector3(-center.X, -center.Y + 0, 0));

            if (man.getPos.X <= 0)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(0, -center.Y, 0));
                state = CameraState.LEFT;

            }

            if (man.getPos.X >= width - 1200)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(-width + 1200, -center.Y, 0));
                state = CameraState.RIGHT;
            }

            if (man.Bottom.Y <= 300)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(-center.X, 0, 0));
                yLocked = true;
            }
            else
                yLocked = false;

            if (man.Rectangle.Y >= height - 600)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(-center.X, -height + 800, 0));
                yLocked = true;
            }

            if (man.getPos.X <= 0 && man.Bottom.Y <= 300)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(0, 0, 0));
                xLocked = true;
                yLocked = true;
            }

            if (man.getPos.X <= 0 && man.Rectangle.Y >= height - 600)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(0, -height + 800, 0));
                xLocked = true;
                yLocked = true;
            }

            if (man.getPos.X >= width - 1200 && man.Bottom.Y <= 300)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(-width + 1200, 0, 0));
                xLocked = true;
                yLocked = true;
            }

            if (man.getPos.X >= width - 1200 && man.Rectangle.Y >= height - 600)
            {
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(-width + 1200, -height + 800, 0));
                xLocked = true;
                yLocked = true;
            }

            if (Game1.gameState != GameState.PlayScreen)
                Transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(0, 0, 0));

        }
    }
}