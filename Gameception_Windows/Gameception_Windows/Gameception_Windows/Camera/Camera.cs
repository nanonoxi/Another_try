using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/* Author: Victor Soudien
 * Date: 18 July 2013
 */

namespace Gameception
{
    class Camera
    {
        // The position of the camera
        private Vector3 position;

        // The look at point of the camera
        private Vector3 lookAt;

        // The up vector of the camera
        private Vector3 upVector;

        // The view and projection matrices for the camera
        private Matrix view;
        private Matrix projection;

        #region Properties

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 LookAt
        {
            get { return lookAt; }
            set { lookAt = value; }
        }

        public Vector3 UpVector
        {
            get { return upVector; }
            set { upVector = value; }
        }

        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        #endregion

        public Camera(GraphicsDeviceManager graphics)
        {
            Position = new Vector3(0, 20, -20);
            LookAt = Vector3.Zero;
            UpVector = Vector3.UnitY;

            View = Matrix.CreateLookAt(Position, LookAt, UpVector);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
        }

        // Update the components of the camera based on the position of the players
        public void Update(Player player1, Player player2)
        {
            float midx = (player1.Position.X + player2.Position.X) / 2.0f;
            float midz = (player1.Position.Z + player2.Position.Z) / 2.0f;

            float distancex = Math.Abs(player1.Position.X - player2.Position.X);
            float distancez = Math.Abs(player1.Position.Z - player2.Position.Z);

            // Use the distance formula to get the distance between the two objects
            float distanceBetween = (float) Math.Sqrt(Math.Pow(player1.Position.X - player2.Position.X, 2) + Math.Pow(player1.Position.Z - player2.Position.Z, 2));

            if (distanceBetween > 45)
            {
                //player1.Position = player1.PreviousPosition;
                //player2.Position = player2.PreviousPosition;
            }

            distanceBetween = MathHelper.Clamp(distanceBetween, 20, 10000000);
            //Position = new Vector3(midx,20,midz - 30);
            float zPos = midz - 30;
            Position = new Vector3(midx,distanceBetween,zPos);
            LookAt = new Vector3(midx, 0, midz);
            View = Matrix.CreateLookAt(Position, LookAt, UpVector);
        }
    }
}
