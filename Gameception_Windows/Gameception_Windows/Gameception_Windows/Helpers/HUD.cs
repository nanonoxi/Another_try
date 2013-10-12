using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    class HUD
    {
        #region Attributes
        
        // Allows access information general to all screens in the game
        TetrisLevel level;
        ScreenManager screenManager;
        SpriteFont font;

        // Allows access to the players so that all the proper information can be draw
        Player player1;
        Player npc;

        // Textures
        private Texture2D[] hud_player1;
        public Texture2D[] Hud_player1
        {
            get { return hud_player1; }
            set { hud_player1 = value; }
        }

        private Texture2D[] hud_npc;
        public Texture2D[] Hud_npc
        {
            get { return hud_npc; }
            set { hud_npc = value; }
        }

        private Texture2D hud_score;
        public Texture2D Hud_score
        {
            get { return hud_score; }
            set { hud_score = value; }
        }

        private Texture2D health_green;
        public Texture2D Health_green
        {
            get { return health_green; }
            set { health_green = value; }
        }


        private Texture2D health_yellow;
        public Texture2D Health_yellow
        {
            get { return health_yellow; }
            set { health_yellow = value; }
        }


        private Texture2D health_red;
        public Texture2D Health_red
        {
            get { return health_red; }
            set { health_red = value; }
        }

        /// <summary>
        /// HUD element positions
        /// </summary>
        private Vector2 hud_player1_position;
        private Vector2 hud_npc_position;
        private Vector2 player1_ammo_position;
        private Vector2 wumpa_remaining_position;
        private Vector2 player1_score_position;
        private Vector2 npc_score_position;

        /// <summary>
        /// Health
        /// </summary>
        private float player1_health;
        private float npc_health;
        private float player1_score;
        private float npc_score;
        private int ammo_remaining;
        private int wumpa_remaining;
        private Texture2D player1_health_texture;
        private Texture2D npc_health_texture;
        private Vector2 player1_health_position;
        private Vector2 npc_health_position;

        /// <summary>
        /// Ammo and charge count
        /// </summary>
        private int player1_ammo = 0;
        private int npc_charge = 0;

        #endregion

        #region Initialization

        public HUD(TetrisLevel lev, ScreenManager screenMan, Player p1, Player n)
        {
            level = lev;
            screenManager = screenMan;
            font = screenManager.Gamefont;

            player1 = p1;
            npc = n;
        }

        public void SetElements(Viewport viewport)
        {
            int x = 10;
            int y = -5;
            hud_player1_position = new Vector2(viewport.X + x, viewport.Y + y);
            hud_npc_position = new Vector2(viewport.Width - hud_npc[0].Width - x, viewport.Y + y);
            //hud_player1_position = new Vector2(viewport.X + i, viewport.Height - hud_player1[0].Height - i);
            //hud_npc_position = new Vector2(viewport.Width - hud_npc[0].Width - i, viewport.Height - hud_npc[0].Height - i);
            
            y = 18;
            player1_ammo_position = new Vector2(viewport.X + 217, viewport.Y + y);
            wumpa_remaining_position = new Vector2(viewport.Width - 180, viewport.Y + y);

            player1_score_position = new Vector2(viewport.X + 462, viewport.Y + y);
            npc_score_position = new Vector2(viewport.Width - 310, viewport.Y + y);
            
            x = 248;
            y = 100; // exploits symmetry
            player1_health_position = new Vector2(viewport.X + x, viewport.Y + y);
            npc_health_position = new Vector2(viewport.Width - x, viewport.Y + y);

            player1_health_texture = health_green;
            npc_health_texture = health_green;

            player1_health = player1.Health * 2;
            npc_health = npc.Health * 2;

            player1_ammo = player1.Ammo;
            wumpa_remaining = 10;

            player1_score = player1.Score;
            npc_score = npc.Score;
        }

        #endregion

        #region Update

        public void Update()
        {
            // update ammo and charge
            player1_ammo = player1.Ammo;
            npc_charge = 5 - player1.Ammo / 2;

            player1_health = player1.Health * 2;
            npc_health = npc.Health * 2;

            if (player1_health < 40)
                player1_health_texture = health_red;
            else if (player1_health < 100)
                player1_health_texture = health_yellow;
            else
                player1_health_texture = health_green;

            if (npc_health < 40)
                npc_health_texture = health_red;
            else if (npc_health < 100)
                npc_health_texture = health_yellow;
            else
                npc_health_texture = health_green;

            player1_score = player1.Score;
            npc_score = npc.Score;
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();

            // Draw HUDS
            spriteBatch.Draw(hud_player1[player1_ammo / 2], hud_player1_position, Color.White); // player1.totalAmmo / 5
            spriteBatch.Draw(hud_npc[npc_charge], hud_npc_position, Color.White);

            // Draw ammo & fruit
            spriteBatch.DrawString(font, (player1_ammo % 2 + 1) + "", player1_ammo_position, Color.White);
            spriteBatch.DrawString(font, wumpa_remaining + "", wumpa_remaining_position, Color.White);

            // Draw Score
            spriteBatch.DrawString(font, player1_score + "", player1_score_position - new Vector2 (font.MeasureString(player1_score + "").X, 0), Color.White);
            spriteBatch.DrawString(font, npc_score + "", npc_score_position - new Vector2(font.MeasureString(npc_score + "").X, 0), Color.White);

            // Draw health, scaled to current health of players, health texture color set in Update
            int threshold = 150; // threshold to begin drawing diagonal on
            int threshold_inverse = 200 - threshold;
            if (player1_health > threshold)
            {
                spriteBatch.Draw(player1_health_texture, player1_health_position, null, Color.White, 0f, Vector2.Zero, new Vector2(threshold, 1), 0, 0);

                // diagonal
                float j = 1;
                float remaining_health = player1_health - threshold;
                for (int i = 0; i < threshold_inverse && i < remaining_health; i++)
                {
                    spriteBatch.Draw(player1_health_texture, player1_health_position + new Vector2(threshold + i, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(2, j), 0, 0);
                    j -= 0.018f;
                }
            }
            else
            {
                spriteBatch.Draw(player1_health_texture, player1_health_position, null, Color.White, 0f, Vector2.Zero, new Vector2(player1_health, 1), 0, 0);
            }
            
            if (npc_health > threshold)
            {
                spriteBatch.Draw(npc_health_texture, npc_health_position - new Vector2(threshold, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(threshold, 1), 0, 0);
                
                // diagonal
                float j = 1;
                float remaining_health = npc_health - threshold;
                for (int i = 0; i < threshold_inverse && i < remaining_health; i++)
                {
                    spriteBatch.Draw(npc_health_texture, npc_health_position - new Vector2( threshold + i, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(2, j), 0, 0);
                    j -= 0.018f;
                }
            }
            else
            {
                spriteBatch.Draw(npc_health_texture, npc_health_position - new Vector2(npc_health, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(npc_health, 1), 0, 0);
            }

            spriteBatch.End();
        }

        #endregion
    }
}
