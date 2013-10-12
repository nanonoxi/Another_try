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

        private Effect effect;
        private GraphicsDevice device;
        private Texture2D groundTexture;
        private VertexBuffer groundVertexBuffer;
        private int groundWidth = 4;
        private int groundLength = 4;

        #endregion

        #region Initialization

        public GroundObject(GraphicsDevice d, Effect e, Texture2D t)
        {
            effect = e;
            groundTexture = t;
            device = d;

            SetUpVertices();
        }

        private void SetUpVertices()
        {
            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();
            int X = 55; // set size of vertex increments
            int Z = 50;
            int i = 0;
            int j = 0;
            for (int x = 0; x < groundWidth; x++)
            {
                for (int z = 0; z < groundLength; z++)
                {
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(i + x * X, 0, j + (-z) * Z), new Vector3(0, 1, 0), new Vector2(0, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(i + x * X, 0, j + (-z - 1) * Z), new Vector3(0, 1, 0), new Vector2(0, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(i + (x + 1) * X, 0, j + (-z) * Z), new Vector3(0, 1, 0), new Vector2(1.0f, 1)));
                    
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(i + x * X, 0, j + (-z - 1) * Z), new Vector3(0, 1, 0), new Vector2(0, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(i + (x + 1) * X, 0, j + (-z - 1) * Z), new Vector3(0, 1, 0), new Vector2(1.0f, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(i + (x + 1) * X, 0, j + (-z) * Z), new Vector3(0, 1, 0), new Vector2(1.0f, 1)));
                }
            }

            groundVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            groundVertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }

        #endregion

        #region Draw

        public void Draw(Camera camera)
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(camera.View);//(viewMatrix);
            effect.Parameters["xProjection"].SetValue(camera.Projection);//(projectionMatrix);
            effect.Parameters["xTexture"].SetValue(groundTexture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(groundVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, groundVertexBuffer.VertexCount / 3);
            }
        }

        #endregion
    }
}
