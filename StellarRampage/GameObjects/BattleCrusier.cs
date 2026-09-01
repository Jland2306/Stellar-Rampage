
using StellarRampage.HelperClasses;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using StellarRampage.Miscellaneous;
using StellarRampage.GameObjects.Crusier_Projectiles;


namespace StellarRampage.GameObjects
{
    internal class BattleCrusier : Boss
    {
        private string contentName = "Bosses/BattleCrusier/";

        private Dictionary<AnimState, AnimatedSprite> animationHandler;

        private Vector2[] hitbox;

        private bool isMoving;

        private float rotation;

        private Random rng;

        private int shield;

        private List<BossProjectile> projectiles;

        public bool HasShield
        {
            get { return shield > 0; }
        }

        public float Rotation
        {
            get { return rotation; }
        }

        /// <summary>
        /// Load the Crusier's assets & initialize the object
        /// </summary>
        /// <param name="content"></param>
        public BattleCrusier(ContentManager content)
            : base(null, 50, Vector2.Zero)
        {
            //Load the animation states
            baseSheet = content.Load<Texture2D>(contentName + "Base");
            fireSheet = content.Load<Texture2D>(contentName + "Fire");
            destroySheet = content.Load<Texture2D>(contentName + "Destroy");
            shieldSheet = content.Load<Texture2D>(contentName + "Shield");
            trailSheet = content.Load<Texture2D>(contentName + "Trail");

            animationHandler = new Dictionary<AnimState, AnimatedSprite>();

            animationHandler.Add(AnimState.Base, new AnimatedSprite(baseSheet, 128, 128, 1, 0, new Rectangle((int)position.X, (int)position.Y, 128, 128), scale: 3));
            animationHandler.Add(AnimState.Destroy, new AnimatedSprite(destroySheet, 128, 128, 13, 0.1f, new Rectangle((int)position.X, (int)position.Y, 128, 128), scale: 3));
            animationHandler.Add(AnimState.Firing, new AnimatedSprite(fireSheet, 128, 128, 9, 0.1f, new Rectangle((int)position.X, (int)position.Y, 128, 128), scale: 3));
            animationHandler.Add(AnimState.Trail, new AnimatedSprite(trailSheet, 128, 128, 8, 0.1f, new Rectangle((int)position.X, (int)position.Y, 128, 128), scale: 3));
            animationHandler.Add(AnimState.Shield, new AnimatedSprite(shieldSheet, 72, 89, 1, 0, new Rectangle((int)position.X, (int)position.Y, 128, 128), scale: 3));

            position = new Vector2(500);

            state = AnimState.Firing;

            isMoving = true;

            hitbox = new Vector2[8];

            rotation = MathF.PI/2;

            rng = new Random();
        }



        public override void Update(Vector2 playerPosition, GameTime gameTime)
        {
            Vector2 direction = (playerPosition - position);
            direction.Normalize();
            rotation = MathF.Atan2(direction.Y, direction.X) + MathF.PI / 2;

            UpdateAnimationPosition();
            UpdatePosition();

            // Update the animation depending on the state

            switch (state)
            {
                case AnimState.Base:
                    animationHandler[AnimState.Base].Update(gameTime);
                    if (isMoving)
                    {
                        animationHandler[AnimState.Trail].Update(gameTime);
                    }

                    if (HasShield) { Teleport(playerPosition); }

                    break;
                case AnimState.Firing:
                    animationHandler[AnimState.Firing].Update(gameTime);

                    if (DistanceToPlayer(playerPosition) > 500) { Teleport(playerPosition); }



                    break;
                case AnimState.Destroy:
                    animationHandler[AnimState.Destroy].Update(gameTime);
                    break;
            }
            
        }

        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {

            switch (state)
            {
                case AnimState.Base:
                    animationHandler[AnimState.Base].DrawRotated(_spriteBatch);
                    if (isMoving)
                    {
                        animationHandler[AnimState.Trail].DrawRotated(_spriteBatch);
                    }
                    break;
                case AnimState.Firing:
                    animationHandler[AnimState.Firing].DrawRotated(_spriteBatch);
                    if (isMoving)
                    {
                        animationHandler[AnimState.Trail].DrawRotated(_spriteBatch);
                    }
                    break;
                case AnimState.Destroy:
                    animationHandler[AnimState.Destroy].DrawRotated(_spriteBatch);
                    break;
            }

            if (debugOn)
            {
                for (int i = 0; i < 8; i++)
                {
                    DebugLib.DrawCircleOutline(_spriteBatch, hitbox[i], 1, 10, 3, Color.Red);
                }
            }
            
        }

        public void UpdateAnimationPosition()
        {
            animationHandler[state].Position = new Rectangle((int)position.X, (int)position.Y, 128, 128);
            animationHandler[AnimState.Trail].Position = new Rectangle((int)position.X, (int)position.Y, 128, 128);

            animationHandler[state].Rotation = rotation;
            animationHandler[AnimState.Trail].Rotation = rotation;
        }

        public override void UpdatePosition()
        {
            hitbox[0] = position - (new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + (89 / 2 * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value

            hitbox[1] = position - ((23 * 3) * new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + ((89 / 2 - 10) * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value

            hitbox[2] = position - ((12 * 3) * new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + ((89 / 2 - 18) * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value

            hitbox[3] = position - ((72 / 2 * 3) * new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + (7 * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value

            hitbox[4] = position - (new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + ((-89 / 2) * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value

            hitbox[5] = position - ((-72 / 2 * 3) * new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + (7 * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value

            hitbox[6] = position - ((-12 * 3) * new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + ((89 / 2 - 18) * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value

            hitbox[7] = position - ((-23 * 3) * new Vector2(MathF.Cos(-rotation), -MathF.Sin(-rotation)) // X value
                                + ((89 / 2 - 10) * 3) * new Vector2(MathF.Sin(-rotation), MathF.Cos(-rotation))); // Y value
        }

        private void Teleport(Vector2 playerPosition)
        {
            float teleportRotation = (float)rng.NextDouble() * MathF.PI * 2;

            position = playerPosition +
                        (new Vector2(MathF.Cos(teleportRotation), -MathF.Sin(teleportRotation)) 
                        + new Vector2(MathF.Sin(teleportRotation), MathF.Cos(teleportRotation))) * 500;
        }

        

        private float DistanceToPlayer(Vector2 playerPosition)
        {
            Vector2 distance = playerPosition - position;

            return distance.Length();
        }
    }
}
