using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

/*** Code by Rajiv Puvvada and Ara Yessayan **/
/*** Modification code to work with MP3's by Sam Dannemiller **/

/*** Most of this code is from Music Lab 4, with some modifications **/


namespace CrisisAtSwissStation
{
    public class AudioManager
    {

        private Dictionary<MusicSelection, Song> songs;              //Dictionaries are useful for keying a selection enum to a particular audio element.
        private Dictionary<SFXSelection, SoundEffect> soundEffects;
        private float SFXVolume;     //Value between 0.0 and 1.0.

        //This is an enum that speeds up coding and reduces errors.
        //Specifying SFX by enum takes advantage of Intellisense and reduces 
        //the chance of a spelling mistake.
        public enum SFXSelection
        {
            VictoryTrumpets,
            LevelComplete

        }

        //This is an enum that speeds up coding and reduces errors.
        //Specifying songs by enum takes advantage of Intellisense and reduces 
        //the chance of a spelling mistake.
        public enum MusicSelection
        {
            EarlyLevelv2,
            Tension,
            Destruction
           
        }

        public void Initialize()
        {
            //Initializes the dictionaries.
            soundEffects = new Dictionary<SFXSelection, SoundEffect>();
            songs = new Dictionary<MusicSelection, Song>();
            SFXVolume = 0.5f;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
        }

        public void LoadContent(ContentManager content)
        {
            //Uses relative pathfiles from your project (i.e. your mp3 files 
            //should be located in the Music folder for this demo).


            //Music
            songs.Add(MusicSelection.EarlyLevelv2, content.Load<Song>("EarlyLevelv2"));
            songs.Add(MusicSelection.Tension, content.Load<Song>("Tension(Lab 3)"));
            songs.Add(MusicSelection.Destruction, content.Load<Song>("Destruction(v1)"));

            //Sound Effects
            soundEffects.Add(SFXSelection.VictoryTrumpets, content.Load<SoundEffect>("VictoryTrumpets"));
            soundEffects.Add(SFXSelection.LevelComplete, content.Load<SoundEffect>("LevelComplete"));

        }


        //Plays music.  This should be done through using the MediaPlayer static class.
        /* It also stops the previous song if it was playing or paused.
         * 
         * You typically don't want multiple songs to be playing at once, so make
         * sure to stop the previous song.
         *
         * Trying to play a song already playing may crash your game.
         * 
         */

        public void Play(MusicSelection selection)
        {
            Song sng;
            if (songs.TryGetValue(selection, out sng))
            {
                Stop();
                MediaPlayer.Play(sng);

            }
        }




        //Plays a sound effect.  I have done this one for you.
        public void Play(SFXSelection selection)
        {   
            /* Because you typically want to be able to play a sound effect again
             * before the first sound ends (i.e. gunfire, footsteps), 
             * and you typically won't need to pause or stop them, you probably
             * want to just call sound.Play() as this allows multiple
             * instances of a sound effect to play at the same time with no consequences.
             * 
             * Relatively straightforward, but you need to know the name of the sfx when called.
             * I've done this one for you.
             */

             SoundEffect sfx;
             if (soundEffects.TryGetValue(selection, out sfx))
             {
                 SoundEffectInstance sound = sfx.CreateInstance();
                 sound.Volume = SFXVolume;
                 sound.Play();
             }
             else
             {
                 Console.WriteLine("Could not find sound effect: " + selection.ToString());
             }
        }


        //This function will pause the currently playing song, and resume it if it is playing.
        //Be sure to check that it is valid to try and pause/resume the song at a given time.
        public void Pause()
        {   
            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
            else if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }

        }

        //Stops the song. Be sure to check that it is valid to stop the song here.
        public void Stop()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

        }

        
        //Make sure volume does not go above 1.0.  Hint: Look at MathHelper.Clamp for a quick function to use.
        public void IncreaseSFXVolume(float increment)
        {
            MathHelper.Clamp(SFXVolume+increment, 0.0f, 1.0f);
            SFXVolume = SFXVolume + increment;
        }

        //Make sure volume does not go below 0.0.
        public void DecreaseSFXVolume(float decrement)
        {
            MathHelper.Clamp(SFXVolume-decrement, 0.0f, 1.0f);
            SFXVolume = SFXVolume - decrement;
        }

        //Make sure volume does not go above 1.0.  
        public void IncreaseMusicVolume(float increment)
        {
            MathHelper.Clamp(MediaPlayer.Volume+increment, 0.0f, 1.0f);
            MediaPlayer.Volume= MediaPlayer.Volume + increment;
        }

        //Make sure volume does not go below 0.0.  
        public void DecreaseMusicVolume(float decrement)
        {
            MathHelper.Clamp(MediaPlayer.Volume-decrement, 0.0f, 1.0f);
            MediaPlayer.Volume = MediaPlayer.Volume - decrement;
        }

        //Set volume equal to 0.0 or 0.5
        public void Mute()
        {
            if (MediaPlayer.Volume == 0.0f)
                MediaPlayer.Volume = 0.5f;
            else if (MediaPlayer.Volume>0.0)
            MediaPlayer.Volume = 0.0f;
            
        }

        //Play next track
        public void PlayNext()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.MoveNext();
            }
        }


        //Don't touch these functions
        protected void Update(GameTime gameTime)
        {
            //Nothing to update.
        }

        protected  void Draw(GameTime gameTime)
        {
            //Nothing to draw..
        }

    }
}
