using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace StellarRampage.Managers
{

    /// <summary>
    /// The sound manager controls all audio in the game.
    /// From music, to sound effects
    /// </summary>
    public static class SoundManager
    {
        //All audio in the game.
        private static Dictionary<string, SoundEffect> soundEffects;
        private static Dictionary<string, Song> songs;

        //How loud everything is played. These will need to be changed in the settings class.
        private static float masterVolume = 0.3f;
        private static float musicVolume = 0.3f;
        private static float soundVolume = 0.3f;

        //What music is playing
        private static string currSong;

        //The folders to load the audio from
        private static string fileName = "Audio/";
        private static string effectName = "Sounds/";
        private static string musicName = "Songs/";

        //Pitch
        private static Random randy = new Random();

        //Environment
        private static float maxDistance = 2000;

        //Music fade
        private static float fadeDuration = 3f;
        private static float fadeTimer = 0f;
        private static bool isFading;

        //---------------------------------------------------------------------
        //                          PROPERTIES
        //---------------------------------------------------------------------


        public static float MasterVolume
        {
            get { return masterVolume; }
            set { masterVolume = value; }
        }
        public static float MusicVolume
        { 
            get { return musicVolume; } 
            set { musicVolume = value; }
        }

        public static float SoundVolume
        {
            get { return soundVolume; }
            set { soundVolume = value; }
        }

        /// <summary>
        /// Get and setter Property for current song
        /// </summary>
        public static string CurrentSongName { get; private set; } = "";

        //---------------------------------------------------------------------
        //                          LOAD AUDIO
        //---------------------------------------------------------------------

        /// <summary>
        /// Loads all the audio from the folder
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            //Initialize the dictionaries
            soundEffects = new Dictionary<string, SoundEffect>();
            songs = new Dictionary<string, Song>();

            //Get the full relative path in which the audio clips are stored
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "/" + fileName + effectName);

            //Store each of the audio clips into an array
            FileInfo[] clips = dir.GetFiles("*.xnb");

            //Add each of the clips into the dictionary
            foreach (FileInfo c in clips)
            {
                //removes the .wav or .mp3 from the audio clip
                string key = Path.GetFileNameWithoutExtension(c.Name);

                //add the clip to the dictionary
                soundEffects[key] = content.Load<SoundEffect>(fileName + effectName + key);
            }

            //switch the directory to music
            dir = new DirectoryInfo(content.RootDirectory + "/" + fileName + musicName);

            //Add the music clips
            clips = dir.GetFiles("*.xnb");

            //Add each of the music clips into the proper dictionary
            foreach (FileInfo c in clips)
            {
                //removes the .wav or .mp3 from the audio clip
                string key = Path.GetFileNameWithoutExtension(c.Name);

                //add the clip to the dictionary
                songs[key] = content.Load<Song>(fileName + musicName + key);
            }

        }

        //---------------------------------------------------------------------
        //                          SOUND EFFECTS
        //---------------------------------------------------------------------

        /// <summary>
        /// Play a sound effect 
        /// </summary>
        public static void PlaySound(string name, float volume = 1f, float pitch = 0f, float pan = 0f)
        {
            //Try the sound effect name. Sometimes people will enter the wrong name
            //which would cause an error.
            if(soundEffects.TryGetValue(name, out SoundEffect soundEffect))
            {
                //Play the sound
                soundEffect.Play(volume * soundVolume * masterVolume, pitch, pan);
            }
        }

        /// <summary>
        /// Play a sound effect using distance to control pan and volume
        /// </summary>
        public static void PlayEnvironmentalSound(string name, Vector2 position, float maxVolume = 1, float pitch = 0f)
        {
            //Try the sound effect name. Sometimes people will enter the wrong name
            //which would cause an error.
            if (soundEffects.TryGetValue(name, out SoundEffect soundEffect))
            {
                //How far away is the object from the center screen
                Vector2 distance = position - Game1.Cam.CameraPosition;

                //Length of the vector
                float length = distance.Length();

                //If length is greater than the max distance. The volume is clamped to 0.
                //The audio is a max of 1 whenever the audio is right on top of the camera.
                float volume = Math.Clamp(maxVolume - (length / maxDistance), 0, maxVolume);

                //Pan is the left/right audio. 
                // -1 means only left ear, 1 means only right
                float pan = Math.Clamp(distance.X / maxDistance, -1, 1);

                //play the sound with calculated volume
                soundEffect.Play(volume * soundVolume * masterVolume, RandomPitch(pitch), pan);

                //play a small stereo version
                soundEffect.Play(0.1f * soundVolume * masterVolume, 0, 0);
            }
        }
        /// <summary>
        /// Play a sound effect with the pitch randomized
        /// </summary>
        public static void PlaySoundRandomPitch(string name, float volume = 1f, float pitch = 0f, float pan = 0f)
        {
            //Try the sound effect name. Sometimes people will enter the wrong name
            //which would cause an error.
            if (soundEffects.TryGetValue(name, out SoundEffect soundEffect))
            {
                //Play the sound
                soundEffect.Play(volume * soundVolume * masterVolume, RandomPitch(pitch), pan);
            }
        }

        /// <summary>
        /// Picks a random pitch slightly lower and higher than the pitch provided
        /// </summary>
        /// <param name="pitch">The base pitch to randomize</param>
        /// <returns></returns>
        private static float RandomPitch(float pitch)
        {
            int min = (int)((pitch - 0.1f) * 1000);
            int max = (int)((pitch + 0.1f) * 1000);
            return randy.Next(min, max) / 1000f;
        }

        //---------------------------------------------------------------------
        //                          MUSIC
        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the current song 
        /// </summary>
        /// <param name="songName">Song to play</param>
        public static void PlaySong(string songName, bool fade = false)
        {
            if (CurrentSongName == songName && MediaPlayer.State == MediaState.Playing)
            {
                return;
            }

            if (songs.TryGetValue(songName, out Song song))
            {
                //Stop the current song
                MediaPlayer.Stop();

                //Play the next song
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
                CurrentSongName = songName;

                //Start the song at 0, fade over time
                if (fade)
                {
                    MediaPlayer.Volume = 0f;
                    fadeTimer = 0f;
                    isFading = true;
                }
                else
                {
                    MediaPlayer.Volume = musicVolume * masterVolume;
                    isFading = false;
                }
            }
        }

        /// <summary>
        /// Stops the current song
        /// </summary>
        public static void StopMusic()
        {
            //Stop the song
            MediaPlayer.Stop();

            // Make current song empty
            CurrentSongName = "";

            //Remove song from currSong
            currSong = null;    
        }


        public static void Update(GameTime gameTime)
        {
            //Only update if theres a fad
            if (isFading)
            {
                //Increase the timer
                fadeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Get the percent complete
                float time = Math.Clamp(fadeTimer / fadeDuration, 0, 1);

                //Increase volume slowly
                MediaPlayer.Volume = time * musicVolume * masterVolume;

                //If the fade is over, turn it off
                if (time >= 1f)
                {
                    isFading = false;
                }
            }
        }



        //---------------------------------------------------------------------
        //                          SETTINGS
        //---------------------------------------------------------------------

        /// <summary>
        /// Change the volume of the media player
        /// </summary>
        /// <param name="volume">volume to set to</param>
        public static void SetVolume(float volume)
        {
            //Volume should not go above 1
            masterVolume = Math.Clamp(volume, 0f, 1f);

            //Set the music volume
            MediaPlayer.Volume = musicVolume * masterVolume;
        }

        /// <summary>
        /// Change the volume of the music
        /// </summary>
        /// <param name="volume">volume to set to</param>
        public static void SetMusicVolume(float volume)
        {
            //Volume should not go above 1
            musicVolume = Math.Clamp(volume, 0f, 1f);

            //Set the music volume
            MediaPlayer.Volume = musicVolume * masterVolume;
        }

        /// <summary>
        /// Change the volume of the media player
        /// </summary>
        /// <param name="volume"></param>
        public static void SetEffectsVolume(float volume)
        {
            //Volume should not go above 1
            soundVolume = Math.Clamp(volume, 0f, 1f);
        }
    }
}
