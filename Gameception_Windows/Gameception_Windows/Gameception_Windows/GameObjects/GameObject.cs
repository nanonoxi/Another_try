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

        // Used to determine whether or not to use the alternate texture
        private bool useAlternateTexture;

        // An alternate texture for this gameObject
        private Texture2D alternateTexture;

        // Determines whether this object is in the view frustrum
        private bool inFrustrum;

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

        public bool UseAlternateTexture
        {
            get { return useAlternateTexture; }
            set { useAlternateTexture = value; }
        }

        public Texture2D AlternateTexture
        {
            get { return alternateTexture; }
            set { alternateTexture = value; }
        }

        public bool InFrustrum
        {
            get { return inFrustrum; }
            set { inFrustrum = value; }
        }

        #endregion

        public GameObject(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera)
        {
            ObjectModel = model;
            MovementSpeed = moveSpeed;
            Health = initialHealth;
            Position = startPosition;
            PreviousPosition = Position;
            ScaleFactor = scale;
            GameCamera = camera;

            Active = true; // Active by default
        }


        //positionless contructor
        public GameObject(Model model, float moveSpeed, int initialHealth, float scale, Camera camera)
        {
            ObjectModel = model;
            MovementSpeed = moveSpeed;
            Health = initialHealth;
            ScaleFactor = scale;
            GameCamera = camera;

            Active = true; // Active by default
        }


        // Create and return a bounding sphere for this game object
        public BoundingSphere getBoundingSphere()
        {
            BoundingSphere objectBoundingShere = new BoundingSphere();

            foreach (ModelMesh mesh in ObjectModel.Meshes)
            {
                if (objectBoundingShere.Radius == 0) // If this is the first mesh
                {
                    objectBoundingShere = mesh.BoundingSphere;
                }
                else
                {
                    objectBoundingShere = BoundingSphere.CreateMerged(objectBoundingShere, mesh.BoundingSphere);
                }
            }

            objectBoundingShere.Center = Position;
            objectBoundingShere.Radius *= ScaleFactor;

            return objectBoundingShere;
        }

        Matrix world = new Matrix();
        int counter = 0;

        public virtual void Update()
        {
            inFrustrum = gameCamera.inView(this.getBoundingSphere());
        }

        // Draw the model to the screen
        public virtual void Draw()
        {
            Matrix [] transforms = new Matrix[ObjectModel.Bones.Count];
            ObjectModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Only draw a gameObject if it's active
            if (Active)
            {
                if (inFrustrum)
                {
                    foreach (ModelMesh mesh in ObjectModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();

                            // Three Directional Lights for the scene
                            effect.DirectionalLight0.Enabled = true;
                            effect.DirectionalLight0.Direction = new Vector3(0, 0, 1);
                            effect.DirectionalLight0.DiffuseColor = Color.Azure.ToVector3();
                            effect.DirectionalLight0.SpecularColor = Color.Blue.ToVector3();

                            effect.DirectionalLight1.Enabled = true;
                            effect.DirectionalLight1.Direction = new Vector3(0, -1, 0);
                            effect.DirectionalLight1.DiffuseColor = Color.Azure.ToVector3();
                            effect.DirectionalLight1.SpecularColor = Color.Gold.ToVector3();

                            effect.DirectionalLight2.Enabled = true;
                            effect.DirectionalLight2.Direction = new Vector3(-1, 0, 0);
                            effect.DirectionalLight2.DiffuseColor = Color.BurlyWood.ToVector3();
                            effect.DirectionalLight2.SpecularColor = Color.BurlyWood.ToVector3();

                            if (UseAlternateTexture && AlternateTexture != null)
                            {
                                effect.TextureEnabled = true;
                                effect.Texture = AlternateTexture;
                            }

                            effect.View = GameCamera.View;
                            effect.Projection = GameCamera.Projection;
                            effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(ScaleFactor) * Matrix.CreateTranslation(Position);
                        }

                        mesh.Draw();
                    }
                }
            }
        }

        #region Public Methods

        // Change the position of the game object to it's position in the previous cycle
        public void revertPosition()
        {
            this.Position = this.PreviousPosition;
        }

        // Allows for the incrementing and decrementing of health
        public void adjustHealth(int amount)
        {
            this.Health += amount;

            // Ensures that health is always a value between 0 and 100
            this.Health = (int) MathHelper.Clamp(this.Health, 0, 100);
        }

        #endregion
    }
}
