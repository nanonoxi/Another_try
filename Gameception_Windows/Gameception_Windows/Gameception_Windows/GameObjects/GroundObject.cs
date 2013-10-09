using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gameception
{
    class GroundObject
    {
        #region Attributes

        protected Model groundModel;
        public Model GroundModel
        {
            get { return groundModel; }
            set { groundModel = value; }
        }

        protected Texture2D groundTexture;
        public Texture2D GroundTexture
        {
            get { return groundTexture; }
            set { groundTexture = value; }
        }

        protected Vector3 position = Vector3.Zero;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        private Camera gameCamera;
        public Camera GameCamera
        {
            get { return gameCamera; }
            set { gameCamera = value; }
        }

        private BasicEffect effect;
        public BasicEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        private GraphicsDevice device;
        public GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }

        #endregion

        #region Fields
        public const int worldWidth = 200;
        public const int worldHeight = 200;
        public const int numCellsWide = 100;
        public const int numCellsHigh = 100;
        int cellXsize = worldWidth / numCellsWide;
        int cellZsize = worldHeight / numCellsHigh;
        
        int numVerticesWide;
        int numVerticesHigh;
        int numVertices;

        int numTrianglesWide;
        int numTrianglesHigh;
        int numTriangles;

        VertexPositionNormalTexture[] vertices;
        int[] indices;

        public GroundObject(GraphicsDevice graphicsDevice, Camera camera)
        {
            device = graphicsDevice;
            effect = new BasicEffect(device);
            gameCamera = camera;

            numVerticesWide = numCellsWide + 1;
            numVerticesHigh = numVerticesWide + 1;
            numVertices = numVerticesWide * numVerticesHigh;

            numTrianglesWide = numCellsWide * 2;
            numTrianglesHigh = numCellsHigh;
            numTriangles = numTrianglesWide * numTrianglesHigh;

            indices = new int[numTriangles * 3];
            vertices = new VertexPositionNormalTexture[numVertices];

            int startZPosition = -worldHeight / 2;
            int startXPosition = -worldHeight / 2;

            // Fill in the vertices
            int count = 0;
            int worldZPosition = startZPosition;
            for (int y = 0; y < numVerticesHigh; y++)
            {
                int worldXPosition = startXPosition;
                for (int x = 0; x < numVerticesWide; x++)
                {
                    vertices[count].Position = new Vector3(worldXPosition, 0, worldZPosition);
                    vertices[count].Normal = Vector3.Up;
                    vertices[count].TextureCoordinate.X = (float)x / (numVerticesWide - 1);
                    vertices[count].TextureCoordinate.Y = (float)y / (numVerticesHigh - 1);

                    count++;

                    // Advance in x
                    worldXPosition += cellXsize;
                }
                // Advance in z
                worldZPosition += cellZsize;
            }

            // create triangles
            int index = 0;
            int startVertex = 0;
            for (int cellY = 0; cellY < numCellsHigh; cellY++)
            {
                for (int cellX = 0; cellX < numCellsWide; cellX++)
                {
                    indices[index] = startVertex + 0;
                    indices[index + 1] = startVertex + 1;
                    indices[index + 2] = startVertex + numVerticesWide;

                    index += 3;

                    indices[index] = startVertex + 1;
                    indices[index + 1] = startVertex + numVerticesWide + 1;
                    indices[index + 2] = startVertex + numVerticesWide;

                    index += 3;

                    startVertex++;
                }
                startVertex++;
            }
        }

        #endregion

        #region Draw

        public void Draw(GraphicsDevice g)
        {
            g.SamplerStates[0] = SamplerState.LinearWrap;

            effect.World = Matrix.Identity;
            effect.View = GameCamera.View;
            effect.Projection = GameCamera.Projection;
            effect.TextureEnabled = true;
            effect.Texture = GroundTexture;
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices,
                    0, vertices.Length, indices, 0, indices.Length / 3);
            }
        }

        #endregion
    }
}
