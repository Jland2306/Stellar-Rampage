                                                                                                                           using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.HelperClasses
{

    /// Note: Ease types are being referenced from: easings.net
    /// More curves can be found, along with a visual showing each case
    public enum EaseType
    {
        Linear,          // constant
        EaseIn,          // starts slow, speed to constant
        EaseOut,         // start constant, slow at end
        EaseInOutSine,   // slow start, constant middle, slow end
        EaseInQuad,      // slow, speed up
        EaseOutQuad,     // constant, slow down
        EaseInOutQuad,   // ease in out sine but greater
        EaseInCubic,     // really slow start, speeds up fast
        EaseOutCubic,    // fast start, really slow end
        EaseInOutCubic,  // slow in and out, fast middle
        EaseInBack,      // goes negative, then really fast forward
        EaseOutBack,     // really fast overshoot, then goes to final
        EaseInOutBack,   // negative start, overshoot end

    }

    /// <summary>
    /// Manages all tween instances. A tween controls animation "keyframes". This allows
    /// gradual transitions for float data. 
    /// </summary>
    public static class Tween
    {
        // Example:
        //
        // CreateTween(0, 1, 5f, EaseType.EaseIn);
        //
        // This tween would take a value from 0 => 1 over a time frame of 5s.
        // This will follow whatever curve type specified, otherwise it will be linear.
        // In a practical case, this could scale an object from nothing to its full size.
        // To use an actual reference, keep a reference of the tween and use the float tween.currValue;


        //Allows multiple tweens to exist at a time. Each will be updated each frame
        private static List<ObjectTween> activeTweens = new List<ObjectTween>();

        private static List<ObjectTween> deleteTweens = new List<ObjectTween>();

        private static List<ColorTween> activeColorTweens = new List<ColorTween>();

        private static List<ColorTween> deleteColorTweens = new List<ColorTween>();

        private static List<ObjectTween> finishedObjectTweens = new List<ObjectTween>();

        private static List<ColorTween> finishedTweens = new List<ColorTween>();

        private static List<Vector2Tween> activeVectorTweens = new List<Vector2Tween>();

        private static List<Vector2Tween> deleteVectorTweens = new List<Vector2Tween>();

        private static List<Vector2Tween> finishedVectorTweens = new List<Vector2Tween>();

        //Holds the data for a singular tween.
        public class ObjectTween
        {
            public float totalDuration;
            public float timeElapsed;
            public EaseType easing;
            public bool isLooping;
            public float startValue;
            public float endValue;
            public float currValue;
            public Action? OnComplete;

            /// <summary>
            /// tween is finished. Can be removed
            /// </summary>
            public bool IsCompleted
            {
                get
                {
                    //The tween is not set to loop, 
                    // and the time to finish the tween is greater than the specified time
                    return !isLooping && timeElapsed >= totalDuration;
                }

            }

            /// <summary>
            /// Create a new tween
            /// </summary>
            /// <param name="startValue">start float/vector</param>
            /// <param name="endValue">end float/vector</param>
            /// <param name="totalDuration">How long to tween</param>
            /// <param name="easing">curve type</param>
            /// <param name="isLooping">should it repeat</param>
            public ObjectTween(float startValue, float endValue, float totalDuration, EaseType easing = EaseType.Linear, bool isLooping = false)
            {
                this.startValue = startValue;
                this.endValue = endValue;
                this.totalDuration = totalDuration;
                this.easing = easing;
                this.isLooping = isLooping;
                currValue = startValue;
            }
        }

        /// <summary>
        /// A tween that transitions between Vector2 values
        /// </summary>
        public class Vector2Tween
        {
            public float totalDuration;
            public float timeElapsed;
            public EaseType easing;
            public bool isLooping;
            public Vector2 startVector;
            public Vector2 endVector;
            public Vector2 currVector;
            public Action? OnComplete;

            /// <summary>
            /// tween is finished. Can be removed
            /// </summary>
            public bool IsCompleted
            {
                get
                {
                    return !isLooping && timeElapsed >= totalDuration;
                }
            }

            /// <summary>
            ///  A tween that transitions from vector to vector
            /// </summary>
            /// <param name="startVector"></param>
            /// <param name="endVector"></param>
            /// <param name="totalDuration"></param>
            /// <param name="easing"></param>
            /// <param name="isLooping"></param>
            public Vector2Tween(Vector2 startVector, Vector2 endVector, float totalDuration, EaseType easing = EaseType.Linear, bool isLooping = false)
            {
                this.startVector = startVector;
                this.endVector = endVector;
                this.totalDuration = totalDuration;
                this.easing = easing;
                this.isLooping = isLooping;
                currVector = startVector;
            }
        }

        public static Vector2Tween CreateVectorTween(Vector2 startVector, Vector2 endVector, float duration, EaseType easing, bool loop = false)
        {
            Vector2Tween tween = new Vector2Tween(startVector, endVector, duration, easing, loop);
            activeVectorTweens.Add(tween);
            return tween;
        }

        /// <summary>
        /// Transitions colors from one to the next
        /// </summary>
        public class ColorTween
        {
            public float totalDuration;
            public float timeElapsed;
            public EaseType easing;
            public bool isLooping;
            public Color startColor;
            public Color endColor;
            public Color currColor;
            public Action? OnComplete;

            /// <summary>
            /// tween is finished. Can be removed
            /// </summary>
            public bool IsCompleted
            {
                get
                {
                    //The tween is not set to loop, 
                    // and the time to finish the tween is greater than the specified time
                    return !isLooping && timeElapsed >= totalDuration;
                }
            }

            /// <summary>
            ///  A tween that transitions from color to color
            /// </summary>
            /// <param name="startColor"></param>
            /// <param name="endColor"></param>
            /// <param name="totalDuration"></param>
            /// <param name="easing"></param>
            /// <param name="isLooping"></param>
            public ColorTween(Color startColor, Color endColor, float totalDuration, EaseType easing = EaseType.Linear, bool isLooping = false)
            {
                this.startColor = startColor;
                this.endColor = endColor;
                this.totalDuration = totalDuration;
                this.easing = easing;
                this.isLooping = isLooping;
                currColor = startColor;
            }
        }

        /// <summary>
        /// Creates and adds a new tween instance.
        /// </summary>
        public static ObjectTween CreateTween(float startValue, float endValue, float duration, EaseType easing, bool loop = false)
        {
            //Create a new tween
            ObjectTween tween = new ObjectTween(startValue, endValue, duration, easing, loop);
            //add the tween to the list
            activeTweens.Add(tween);

            return tween;
        }

        public static ColorTween CreateColorTween(Color startColor, Color endColor, float duration, EaseType easing, bool loop = false)
        {
            ColorTween tween = new ColorTween(startColor, endColor, duration, easing, loop);
            activeColorTweens.Add(tween);
            return tween;
        }

        /// <summary>
        /// Updates the tween values of every current tween in list
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            //add the number of seconds that have passed
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (ObjectTween tween in activeTweens)
            {
                //add the time thats passed to the current tween
                tween.timeElapsed += time;

                //progress needs to be clamped or tween will last too long.
                //For example, tween is set for 1 second, and current progress is 
                //59.99 seconds, the next frame update will take that above 1 minute. i.e 1 min, 2 sec
                //resulting in an inaccurate tween
                float progress = Math.Clamp(tween.timeElapsed / tween.totalDuration, 0f, 1f);

                //change the progress depending on the curve chosen
                progress = ApplyEase(progress, tween.easing);

                //updates the current float progress based on how far along it is
                tween.currValue = MathHelper.Lerp(tween.startValue, tween.endValue, progress);


                //The tween has exceeded the time allotted by the user
                if (tween.timeElapsed >= tween.totalDuration)
                {
                    if (tween.isLooping)
                    {
                        tween.timeElapsed = 0f;
                    }
                    else
                    {
                        deleteTweens.Add(tween);
                        finishedObjectTweens.Add(tween);
                    }
                }
            }
            //Deletes any tween that finished
            foreach (ObjectTween tween in deleteTweens)
            {
                activeTweens.Remove(tween);
            }
            deleteTweens.Clear();

            //invoke OnComplete after removal
            foreach (ObjectTween tween in finishedObjectTweens)
            {
                tween.OnComplete?.Invoke();
            }
            finishedObjectTweens.Clear();


            foreach (ColorTween tween in activeColorTweens)
            {
                //Increase color tween time
                tween.timeElapsed += time;

                //Get progress
                float progress = Math.Clamp(tween.timeElapsed / tween.totalDuration, 0f, 1f);
                progress = ApplyEase(progress, tween.easing);

                //Tween each color channel separately
                tween.currColor = new Color(
                    (int)MathHelper.Lerp(tween.startColor.R, tween.endColor.R, progress),
                    (int)MathHelper.Lerp(tween.startColor.G, tween.endColor.G, progress), 
                    (int)MathHelper.Lerp(tween.startColor.B, tween.endColor.B, progress),
                    (int)MathHelper.Lerp(tween.startColor.A, tween.endColor.A, progress)
                );

                //Delete if time has finished
                if (tween.timeElapsed >= tween.totalDuration)
                {
                    if (tween.isLooping)
                    {
                        tween.timeElapsed = 0f;
                    }
                    else
                    {
                        deleteColorTweens.Add(tween);
                        finishedTweens.Add(tween);
                    }
                }
            }
            //Remove completed tweens 
            foreach (ColorTween tween in deleteColorTweens)
            {
                activeColorTweens.Remove(tween);
            }
            deleteColorTweens.Clear();

            //invoke OnComplete after removal
            foreach (ColorTween tween in finishedTweens)
            {
                tween.OnComplete?.Invoke();
            }
            finishedTweens.Clear();

            foreach (Vector2Tween tween in activeVectorTweens)
            {
                tween.timeElapsed += time;

                float progress = Math.Clamp(tween.timeElapsed / tween.totalDuration, 0f, 1f);
                progress = ApplyEase(progress, tween.easing);

                tween.currVector = Vector2.Lerp(tween.startVector, tween.endVector, progress);

                if (tween.timeElapsed >= tween.totalDuration)
                {
                    if (tween.isLooping)
                    {
                        tween.timeElapsed = 0f;
                    }
                    else
                    {
                        deleteVectorTweens.Add(tween);
                        finishedVectorTweens.Add(tween);
                    }
                }
            }

            foreach (Vector2Tween tween in deleteVectorTweens)
            {
                activeVectorTweens.Remove(tween);
            }
            deleteVectorTweens.Clear();

            foreach (Vector2Tween tween in finishedVectorTweens)
            {
                tween.OnComplete?.Invoke();
            }
            finishedVectorTweens.Clear();

        }



        /// <summary>
        /// Applies a curve to the float value being modified
        /// </summary>
        private static float ApplyEase(float t, EaseType easing)
        {
            //View ease enum above
            //Functions from : easings.net
            switch (easing)
            {
                case EaseType.Linear:
                    return t;
                case EaseType.EaseIn:
                    return t * t;
                case EaseType.EaseOut:
                    return t * (2 - t);
                case EaseType.EaseInOutSine:
                    return -(float)(Math.Cos(Math.PI * t) - 1) / 2;
                case EaseType.EaseInQuad:
                    return t * t;
                case EaseType.EaseOutQuad:
                    return 1 - (1 - t) * (1 - t);
                case EaseType.EaseInOutQuad:
                    return t < 0.5 ? 2 * t * t : 1 - (float)Math.Pow(-2 * t + 2, 2) / 2;
                case EaseType.EaseInCubic:
                    return t * t * t;
                case EaseType.EaseOutCubic:
                    return 1 - (float)Math.Pow(1 - t, 3);
                case EaseType.EaseInOutCubic:
                    return t < 0.5 ? 4 * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 3) / 2;
                case EaseType.EaseInBack:
                    return 2.70158f * t * t * t - 1.70158f * t * t;
                case EaseType.EaseOutBack:
                    return 1 + 2.70158f * (float)Math.Pow(t - 1, 3) + 1.70158f * (float)Math.Pow(t - 1, 2);
                case EaseType.EaseInOutBack:
                    return t < 0.5 ? (float)Math.Pow(2 * t, 2) * ((1.70158f + 1) * 2 * t - 1.70158f) / 2 : ((float)Math.Pow(2 * t - 2, 2) * ((1.70158f + 1) * (t * 2 - 2) + 1.70158f) + 2) / 2;
                default:
                    return t;
            }
        }
    }
}
