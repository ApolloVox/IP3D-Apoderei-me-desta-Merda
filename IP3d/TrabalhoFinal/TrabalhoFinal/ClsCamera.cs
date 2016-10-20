﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoFinal
{
    class ClsCamera
    {
        private Matrix viewMatrix, projectionMatrix;
        private BasicEffect effect;
        Vector3 position, dir;
        float yaw, pitch;
        float width, height;
        float scale = MathHelper.ToRadians(10) /500;
        MouseState oldState,mouse;
        Mapa map;

        public ClsCamera(GraphicsDevice device, Vector3 startPos,Mapa map)
        {
            width = device.Viewport.Width;
            height = device.Viewport.Height;
            yaw = 0;
            pitch = 0;
            position = new Vector3(8.0f,5.0f, 8.0f);
            dir = Vector3.Zero - position;
            dir.Normalize();
            effect = new BasicEffect(device);
            float aspectRatio = (float)(width /
                height);
            viewMatrix = Matrix.CreateLookAt(
                position,
                dir,
                Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio, 0.5f, 50.0f);

            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            oldState = Mouse.GetState();

            this.map = map;
        }


        public void Update(GameTime gametime)
        {
            KeyboardState keys = Keyboard.GetState();
            mouse = Mouse.GetState();
            Vector3 oldPos = position;
            Vector2 mousePos;
            float speed = 0.5f;

            mousePos.X = mouse.X;
            mousePos.Y = mouse.Y;

            mousePos.X -= width / 2;
            mousePos.Y -= height / 2;

            yaw -= mousePos.X * scale;

            pitch = MathHelper.Clamp(pitch + mousePos.Y * scale, -1.5f, 1.5f);

            HeightY();

            if (keys.IsKeyDown(Keys.NumPad5))
            {
                position.X -= (dir.X - position.X) * speed;
                position.Z -= (dir.Z - position.Z) * speed;
                
            }
            if (keys.IsKeyDown(Keys.NumPad8))
            {
                position.X += (dir.X - position.X) * speed;
                position.Z += (dir.Z - position.Z) * speed;
            }
            if (keys.IsKeyDown(Keys.NumPad4))
            {
                position -= speed * Vector3.Cross(dir - position, Vector3.Up);
            }
            if (keys.IsKeyDown(Keys.NumPad6))
            {
                position += speed * Vector3.Cross(dir - position, Vector3.Up);
            }

            if ((position.X < 0 || position.Z < 0))
                position = oldPos;
            if ((position.X > 127 || position.Z > 127))
                position = oldPos;

            Console.WriteLine(oldPos);
            dir.X = (float)Math.Cos(yaw) * (float)Math.Cos(pitch) + position.X;
            dir.Z = -(float)Math.Sin(yaw) * (float)Math.Cos(pitch) + position.Z;
            dir.Y = (float)Math.Sin(pitch) + position.Y;

            viewMatrix = Matrix.CreateLookAt(position, dir, Vector3.Up);
            Mouse.SetPosition((int)(width / 2), (int)(height / 2));
            oldState = mouse;
        }

        public Matrix ViewMatrixCamera
        {
            get
            {
                return viewMatrix;
            }
        }

        public Matrix ProjectionMatrixCamera
        {
            get
            {
                return projectionMatrix;
            }
        }

        public void HeightY()
        {
            Vector3 verticeA,verticeB,verticeC,verticeD;

            if ((int)(position.X) + (int)(position.Z + 1) * (int)map.MapBoundariesHeight < map.MapBoundariesHeight * map.MapBoundariesWidth && (int)(position.X) + (int)(position.Z + 1) * (int)map.MapBoundariesHeight >0)
            {
                verticeA = map.mapVertices[(int)(position.X) + (int)position.Z * (int)map.MapBoundariesHeight].Position;
                verticeB = map.mapVertices[(int)(position.X + 1) + (int)position.Z * (int)map.MapBoundariesHeight].Position;
                verticeC = map.mapVertices[(int)(position.X) + (int)(position.Z + 1) * (int)map.MapBoundariesHeight].Position;
                verticeD = map.mapVertices[(int)(position.X+1) + (int)(position.Z + 1) * (int)map.MapBoundariesHeight].Position;
            }
            else
            {
                verticeA = map.mapVertices[(int)map.MapBoundariesWidth * (int)map.MapBoundariesHeight-1].Position;
                verticeB = map.mapVertices[(int)map.MapBoundariesWidth * (int)map.MapBoundariesHeight - 1].Position;
                verticeC = map.mapVertices[(int)map.MapBoundariesWidth * (int)map.MapBoundariesHeight - 1].Position;
                verticeD = map.mapVertices[(int)map.MapBoundariesWidth * (int)map.MapBoundariesHeight - 1].Position;
            }

            float Ya, Yb, Yc, Yd;
            Ya = verticeA.Y;
            Yb = verticeB.Y;
            Yc = verticeC.Y;
            Yd = verticeD.Y;

            float Yab = (1 - (position.X - verticeA.X)) * Ya + (position.X - verticeA.X) * Yb;
            float Ycd = (1 - (position.X - verticeC.X)) * Yc + (position.X - verticeC.X) * Yd;
            float Y = (1 - (position.Z - verticeA.Z)) * Yab + (position.Z - verticeA.Z) * Ycd;

            position.Y = Y + 2;
        }
    }
}
