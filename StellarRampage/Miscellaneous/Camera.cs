using Microsoft.Xna.Framework;
using StellarRampage.GameObjects;
using System;


namespace StellarRampage.Miscellaneous
{
    /// <summary>
    /// Camera will center player so they never leave screen
    /// </summary>
    public class Camera
    {
        //Screen width
        private int width;
        //Screen height
        private int height;

        //Camera movement is updated frame to frame, depending
        //how far the player has moved.
        private Vector2 cameraMovement;

        //Camera position should be near world space of player position
        private Vector2 cameraPosition;

        //How fast it snaps to player. 1 = instant, 0 = not at all
        private float smooth;

        private Matrix transform;

        // Shake properties
        private float shakeDuration = 0;
        private float shakeTimer = 0;
        private float shakeStrength = 0;
        private Vector2 shakeOffset = Vector2.Zero;

        //Adds variation to the shake
        private Random randy = new Random();

        /// <summary>
        /// Matrix needed to offset _spritbatch draw
        /// </summary>
        public Matrix Transform
        {
            get { return transform; }
        }

        /// <summary>
        /// Get property for camera position
        /// </summary>
        public Vector2 CameraPosition
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }

        /// <summary>
        /// Creates a camera object that follows a target
        /// </summary>
        /// <param name="width">screen width</param>
        /// <param name="height">screen height</param>
        /// <param name="smooth">the speed at which the camera moves</param>
        public Camera(int width, int height, float smooth)
        {
            this.width = width;
            this.height = height;
            this.smooth = smooth;
        }

        /// <summary>
        /// This method updates the matrix attached to the _spritebatch draw method. It takes the product
        /// of two matrices, one being the players center position, the other being the cameras center position.
        /// The resulting transform matrix will keep the player centered at all times in Draw.
        /// </summary>
        /// <param name="player">Player to center</param>
        public void CameraFollow(GameObject player, GameTime gameTime)
        {
            CameraFollow(player.Position,gameTime);
        }

        /// <summary>
        /// This method updates the matrix attached to the _spritebatch draw method. It takes the product
        /// of two matrices, one being the players center position, the other being the cameras center position.
        /// The resulting transform matrix will keep the player centered at all times in Draw.
        /// </summary>
        /// <param name="playerPos">Player to center</param>
        public void CameraFollow(Vector2 playerPos, GameTime gameTime)
        {
            // Update shake
            if (shakeTimer < shakeDuration)
            {
                //Add time to the shake clock
                shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Create a new shake from -1 to 1 in each direction.
                //After this random is selcted, then strength can be applied
                shakeOffset = new Vector2(
                    (float)(randy.NextDouble() * 2 - 1) * shakeStrength,   //X
                    (float)(randy.NextDouble() * 2 - 1) * shakeStrength    //Y
                );

                // smooth strength over time for fade out
                float progress = shakeTimer / shakeDuration;

                //Reduce the strength by the progress completed
                //This will cut the time slighlty short, but will make it look more natural
                shakeStrength *= 1 - progress;
            }
            else
            {
                //No shake
                shakeOffset = Vector2.Zero;
            }

            //The camera movement is the distance the camera is from the player.
            //This will indicate how far it needs to move
            cameraMovement = new Vector2(playerPos.X + 16, playerPos.Y + 16) - cameraPosition;

            //The camera will move toward the player. If smooth is 1, it will be instant.
            //The lower the value, the slower it moves. Creating a delay.
            cameraPosition += cameraMovement * smooth;

            //Creates a translation matrix based on the position of the camera. The camera should
            //be near the player at all times, but may lag behind due to smooth. This is intentional
            //Add shake to the cam position.
            Matrix position = Matrix.CreateTranslation(
                -cameraPosition.X + shakeOffset.X,
                -cameraPosition.Y + shakeOffset.Y,
                0);

            //Offset matrix will be the dead center of the screen dependent on the size of the window
            Matrix offset = Matrix.CreateTranslation(
                width / 2,
                height / 2,
                0);

            //This final transformation matrix will be the product of the previous two. 
            //When used with the camera, the player will be centered, and will follow it throughout the level
            transform = position * offset;
        }

        /// <summary>
        /// Adds a shake to the screen. Useful for bosses
        /// </summary>
        /// <param name="duration">how long it lasts</param>
        /// <param name="strength">how large it is</param>
        public void TriggerShake(float duration, float strength)
        {
            //Add the new shake fields, will be applied in the follow method
            shakeDuration = duration;
            shakeStrength = strength;

            //Reset the timer
            shakeTimer = 0f;
        }

        public void UpdateShakeOnly(GameTime gameTime)
        {
            if (shakeTimer < shakeDuration)
            {
                //Add time to the shake clock
                shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Create a new shake from -1 to 1 in each direction.
                //After this random is selcted, then strength can be applied
                shakeOffset = new Vector2(
                    (float)(randy.NextDouble() * 2 - 1) * shakeStrength,   //X
                    (float)(randy.NextDouble() * 2 - 1) * shakeStrength    //Y
                );

                // smooth strength over time for fade out
                float progress = shakeTimer / shakeDuration;

                //Reduce the strength by the progress completed
                //This will cut the time slighlty short, but will make it look more natural
                shakeStrength *= 1 - progress;
            }
            else
            {
                //No shake
                shakeOffset = Vector2.Zero;
            }

            Matrix position = Matrix.CreateTranslation(
                -cameraPosition.X + shakeOffset.X,
                -cameraPosition.Y + shakeOffset.Y,
                0);


            //Offset matrix will be the dead center of the screen dependent on the size of the window
            Matrix offset = Matrix.CreateTranslation(
                width / 2,
                height / 2,
                0);

            transform = position * offset;
        }
    }
}
