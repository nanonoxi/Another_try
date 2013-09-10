using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/* Author: Victor Soudien
 * Date: 18 July 2013
 */

namespace Gameception
{
    class GameObject
    {
        // The model for this object
        private Model objectModel;

        // The speed at which this object moves
        private float movementSpeed;

        // The starting health of this object
        private int health;

        // The postion of this object
        private Vector3 position;

        // The previous position of the object
        private Vector3 previousPosition;

        // The amount by which the object should be scaled when it is rendered
        private float scaleFactor;

        // A reference to the camera. Needed for rendering
        private Camera gameCamera;

        // Determines whether or not this object is active
        private bool active;

        #region Properties

        public Model ObjectModel
        {
            get { return objectModel; }
            set { objectModel = value; }
        }

        public float MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 PreviousPosition
        {
            get { return previousPosition; }
            set { previousPosition = value; }
        }

        public float ScaleFactor
        {
            get { return scaleFactor; }
            set { scaleFactor = value; }
        }

        public Camera GameCamera
        {
            get { return gameCamera; }
            set { gameCamera = value; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        #endregion

        public GameObject(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera)
        {
            ObjectModel = model;
            MovementSpeed = moveSpeed;
            Health = initialHealth;
            Position = startPosition;
            ScaleFactor = scale;
            GameCamera = camera;

            Active = true; // Active by default
        }

        public virtual void Update()
        {
            // do nothing
        }

        // Draw the model to the screen
        public virtual void Draw()
        {
            // Only draw a gameObject if it's active
            if (Active)
            {
                foreach (ModelMesh mesh in ObjectModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        effect.View = GameCamera.View;
                        effect.Projection = GameCamera.Projection;
                        effect.World = Matrix.CreateScale(ScaleFactor) * Matrix.CreateTranslation(Position);
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
