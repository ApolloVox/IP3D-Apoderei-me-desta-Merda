﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoFinal
{
    class Mapa
    {
        BasicEffect effect;
        Texture2D mapaImagem,texture;
        Matrix worldMatrix;
        Color[] pixeis;

        VertexPositionNormalTexture[] vertices;
        short[] verIndex;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        float maxHeight, maxWidht;

        public Mapa(GraphicsDevice device, ContentManager Content)
        {
            effect = new BasicEffect(device);
            worldMatrix = Matrix.Identity;
            mapaImagem = Content.Load<Texture2D>("lh3d1");
            texture = Content.Load<Texture2D>("ground1");

            ReadPixeis();

            float aspectRatio = (float)(device.Viewport.Width /
                device.Viewport.Height);

            float nearPlane = 1.0f, FarPlane = 40.0f;
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio, nearPlane, FarPlane);

            effect.VertexColorEnabled = true;

            effect.TextureEnabled = true;
            effect.Texture = texture;

            //LIGHTS
            //effect.EnableDefaultLighting();
            effect.LightingEnabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.78f,0.43f, 0f);
            effect.DirectionalLight0.Direction = new Vector3(1f, -1f, 0);
            effect.DirectionalLight0.SpecularColor = new Vector3(0, 0.025f, 0);
            effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
            effect.EmissiveColor = new Vector3(0f, 0f, 0f);

            //FOG
            effect.FogEnabled = true;
            effect.FogColor = new Color(180f, 180f, 180f).ToVector3(); // For best results, ake this color whatever your background is.
            effect.FogStart = 20f;
            effect.FogEnd = 40.0f;


            CreateMap();

            vertexBuffer = new VertexBuffer(device,
                typeof(VertexPositionNormalTexture),
                vertices.Length,
                BufferUsage.None);

            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            indexBuffer = new IndexBuffer(device,
                typeof(short),
                verIndex.Length,
                BufferUsage.None);

            indexBuffer.SetData(verIndex);
        }

        private void ReadPixeis()
        {
            pixeis = new Color[mapaImagem.Width * mapaImagem.Height];
            mapaImagem.GetData<Color>(pixeis);
        }

        //Cria o mapa atráves de indices e de triangle list
        private void CreateMap()
        {
            verIndex = new short[6 * (mapaImagem.Width - 1) * (mapaImagem.Height - 1)];
            vertices = new VertexPositionNormalTexture[mapaImagem.Width * mapaImagem.Height];

            maxHeight = mapaImagem.Height;
            maxWidht = mapaImagem.Width;

            for(int z = 0;z<mapaImagem.Width;z++)
            {
                for(int x = 0;x<mapaImagem.Height;x++)
                {
                    vertices[x+z*mapaImagem.Width] = new VertexPositionNormalTexture(new Vector3((float)x,(float)pixeis[x+z*mapaImagem.Width].R/255*10f, (float)z), Vector3.Up, new Vector2(x%2,z%2));
                }
            }

            GetNormals();

            int contador = 0;

            //Indices calculados 6 a 6 de modo que cada ciclo seja um "quadrado" da textura
            for(int y = 0;y<mapaImagem.Height-1;y++)
            {
                for(int x = 0;x<mapaImagem.Width-1;x++)
                {
                    verIndex[contador] = (short)(x + y * mapaImagem.Width);
                    verIndex[contador + 1] = (short)(x + y * mapaImagem.Width + 1);
                    verIndex[contador+2] = (short)(x + (y + 1) * mapaImagem.Width);
                    verIndex[contador + 3] = (short)(x + y * mapaImagem.Width + 1);
                    verIndex[contador + 4] = (short)(x + (y + 1) * mapaImagem.Width + 1);
                    verIndex[contador + 5] = (short)(x + (y + 1) * mapaImagem.Width);
                    contador += 6;
                }
            }
        }

        //Método que contém as normais
        public void GetNormals()
        {
            for(int z= 0;z < mapaImagem.Width;z++)
            {
                for(int x = 0;x<MapBoundariesHeight;x++)
                {
                    //Produto externo (V0 - p) * (V1-p)
                    Vector3[] normais = new Vector3[8];
                    int contador = 0;
                    
                    //Left Up
                    if (x - 1 >= 0 && z - 1 >= 0)
                    {
                        normais[contador] = vertices[x - 1 + (z - 1) * (int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }
                    //Center Up
                    if (z - 1 >= 0)
                    {
                        normais[contador] = vertices[x + (z - 1) * (int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }
                    //Right Up
                    if (x + 1 < maxHeight && z - 1 >= 0)
                    {
                        normais[contador] = vertices[x + 1 + (z - 1) * (int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }
                    //Left
                    if (x - 1 >= 0)
                    {
                        normais[contador] = vertices[x - 1 + z * (int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }
                    //Right
                    if (x + 1 < maxHeight)
                    {
                        normais[contador] = vertices[x + 1 + z*(int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }
                    //Down Left
                    if (x - 1 >= 0 && z + 1 < maxHeight)
                    {
                        normais[contador] = vertices[x - 1 + (z + 1) * (int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }
                    //Down Center
                    if (z + 1 < maxHeight)
                    {
                        normais[contador] = vertices[x + (z + 1) * (int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }
                    //Down Right
                    if (x + 1 < mapaImagem.Width && z + 1 < maxHeight)
                    {
                        normais[contador] = vertices[x + 1 + (z + 1) * (int)maxHeight].Position - vertices[x + z * (int)maxHeight].Position;
                        contador++;
                    }

                    //Cálculo final das normais
                    for(int i = contador-1;i>= 1;i--)
                    {
                        normais[i] = Vector3.Cross(normais[i], normais[i-1]);
                    }

                    //Obtenção da media das normais e atribuindo ao vertice central
                    // a normal correspondente
                    Vector3 media = Vector3.Zero;
                    for(int i = 0;i<contador;i++)
                    {
                        media += normais[i];
                    }

                    media /= contador;
                    media.Normalize();
                    vertices[x + z * (int)maxHeight].Normal = media;

                }
            }
        }

        //Metodo draw com triangle list
        public void Draw(GraphicsDevice device,ClsCamera camera)
        {
            effect.World = worldMatrix;
            effect.View = camera.ViewMatrixCamera;
            effect.Projection = camera.ProjectionMatrixCamera;

            effect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verIndex.Length / 3);
        }

        public float MapBoundariesHeight
        {
            get
            {
                return maxHeight;
            }
        }

        public float MapBoundariesWidth
        {
            get
            {
                return maxWidht;
            }
        }

        public VertexPositionNormalTexture[] mapVertices
        {
            get
            {
                return vertices;
            }
        }

        //Interpolação das normais
        public float InterpolyNormals(Vector3 position)
        {
            Vector3 verticeA, verticeB, verticeC, verticeD;

            //Obtem os vertices adjacentes a camera
            if ((int)(position.X) + (int)(position.Z + 1) * (int)MapBoundariesHeight < MapBoundariesHeight * MapBoundariesWidth && (int)(position.X) + (int)(position.Z + 1) * (int)MapBoundariesHeight > 0)
            {
                verticeA = mapVertices[(int)(position.X) + (int)position.Z * (int)MapBoundariesHeight].Normal;
                verticeB = mapVertices[(int)(position.X + 1) + (int)position.Z * (int)MapBoundariesHeight].Normal;
                verticeC = mapVertices[(int)(position.X) + (int)(position.Z + 1) * (int)MapBoundariesHeight].Normal;
                verticeD = mapVertices[(int)(position.X + 1) + (int)(position.Z + 1) * (int)MapBoundariesHeight].Normal;
            }
            else
            {
                verticeA = mapVertices[(int)MapBoundariesWidth * (int)MapBoundariesHeight - 1].Normal;
                verticeB = mapVertices[(int)MapBoundariesWidth * (int)MapBoundariesHeight - 1].Normal;
                verticeC = mapVertices[(int)MapBoundariesWidth * (int)MapBoundariesHeight - 1].Normal;
                verticeD = mapVertices[(int)MapBoundariesWidth * (int)MapBoundariesHeight - 1].Normal;
            }

            //interpolação das Normais para à medida que se anda com a camera o movimento ser fluido
            //interpolação feita com o peso que cada vertice da à camera
            float Ya, Yb, Yc, Yd;
            Ya = verticeA.Y;
            Yb = verticeB.Y;
            Yc = verticeC.Y;
            Yd = verticeD.Y;

            float Yab = (1 - (position.X - verticeA.X)) * Ya + (position.X - verticeA.X) * Yb;
            float Ycd = (1 - (position.X - verticeC.X)) * Yc + (position.X - verticeC.X) * Yd;
            float Y = (1 - (position.Z - verticeA.Z)) * Yab + (position.Z - verticeA.Z) * Ycd;

            return position.Y = Y + 2;
        }
    }
}
