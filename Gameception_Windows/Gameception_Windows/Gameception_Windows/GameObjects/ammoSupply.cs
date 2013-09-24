using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    class ammoSupply : GameObject   
    {
        Vector3 position;
        bool used;

        public ammoSupply(Model model, float moveSpeed, int initialHealth, float scale, Camera camera)
            : base(model, moveSpeed, initialHealth, scale, camera)
        {
            used = false;
        }

        public void setPosition(Creep c)
        {
            base.Position = c.Position;
        }

       
        public void pickedUp()
        {
            used = true;
        }

        public bool isPickedUp()
        {
            return used;
        }
    }
}
