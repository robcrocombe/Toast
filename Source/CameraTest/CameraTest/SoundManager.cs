using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CameraTest
{
    /// <summary>
    /// Class which controls all sound within the game, including SoundEffects and Background Songs
    /// </summary>

    public enum GameSongMode
    {
        intro,
        gameLoop
    }

    class SoundManager
    {
        // Songs
        private static Song introSong;
        private static Song gameLoopSong;

        // Game Sound Effects
        // Suck
        private static SoundEffect[] shootingSounds = new SoundEffect[1];
        private static SoundEffectInstance[] shootingSoundsInstances = new SoundEffectInstance[1];

        //Death
        private static SoundEffect toastorianDeathSound;
        private static SoundEffectInstance toastorianDeathSoundInstance;

        // Bool Determining if SplashScreen has played through
        private static bool introFinished = false;

        //Method used to load all the sound effects into memory
        public static void LoadContent(ContentManager Content)
        {
            //Load all background music
            //introSong = Content.Load<Song>("Sounds/Music/IntroMusic");
            gameLoopSong = Content.Load<Song>("Sounds/Music/GameLoopMusic");

            //Load all the suck up sounds (they have names based on their numbers)
            for (int i = 0; i < shootingSounds.Length; i++)
            {
                shootingSounds[i] = Content.Load<SoundEffect>("Sounds/SoundEffects/Shoot/Fire" + (i + 1));
                //Create an instance, saves on garbage collection
                shootingSoundsInstances[i] = shootingSounds[i].CreateInstance();
            }

            ////Load the vaccum cleaner death sound
            //toastorianDeathSound = Content.Load<SoundEffect>("Sounds/SoundEffects/DeathSound");
            ////Create an instance, saves on garbage collection
            //toastorianDeathSoundInstance = toastorianDeathSound.CreateInstance();
        }

        //Method used to Play background music based on the state of the game
        public static void PlaySong(GameSongMode gsm)
        {
            //We can only play music if the game has control, else we'll be stopping the users music for them! :(
            if (MediaPlayer.GameHasControl)
            {
                if (gsm == GameSongMode.intro)
                {
                    if (MediaPlayer.State == MediaState.Stopped && introFinished == false)
                    {
                        MediaPlayer.Play(introSong);
                        introFinished = true;
                    }
                }

                if (gsm == GameSongMode.gameLoop)
                {
                    //If the user has the setting turned on & the transition has finished play the music
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        //Revert Volume to max and play
                        MediaPlayer.Volume = 1;
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Play(gameLoopSong);
                    }
                }
            }
        }

        //Random used to randomize suck up sounds
        private static Random r = new Random();

        //Holds previous sound effect instance so we dont use the sound twice in a row
        private static SoundEffectInstance previousShootingSound;

        public static void PlayRandomShootingSound()
        {
            // This method plays a random Suck Up Sound Each time, but never the previous one
            SoundEffectInstance selectedSoundEffectInstance = shootingSoundsInstances[0];

            // Keep picking suck up sounds if theyre the same as the last
            //do
            //{
            //    selectedSoundEffectInstance = shootingSoundsInstances[r.Next(0, 4)];
            //}
            //while (selectedSoundEffectInstance == previousShootingSound);

            // Set "last used sound" to the one we're about to play to stop it being played immediately
            previousShootingSound = selectedSoundEffectInstance;           
            selectedSoundEffectInstance.Volume = 1f;
            selectedSoundEffectInstance.Play();
        }

        //Simple method used to play the vaccum death sound
        public static void PlayToastorianDeathSound()
        {
            toastorianDeathSound.Play();
        }
    }
}
