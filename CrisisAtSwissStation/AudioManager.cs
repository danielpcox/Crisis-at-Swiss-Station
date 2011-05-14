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

/*** Most of this code is from Music Lab 4 from Cornell's CIS 3000 course, 
 * with some modifications by Zachary Davis to adapt it to Crisis at Swiss Station**/


namespace CrisisAtSwissStation
{
    public class AudioManager
    {

        private Dictionary<MusicSelection, Song> songs;              //Dictionaries are useful for keying a selection enum to a particular audio element.
        private Dictionary<string, Song> songsdict;              //Dictionaries are useful for keying a selection enum to a particular audio element.
        private Dictionary<SFXSelection, SoundEffect> soundEffects;
        private float SFXVolume;     //Value between 0.0 and 1.0.
        private SoundEffectInstance SFXplaying = null;
        private bool muted; //Is audio muted?

        


        //This is an enum that speeds up coding and reduces errors.
        //Specifying SFX by enum takes advantage of Intellisense and reduces 
        //the chance of a spelling mistake.
        public enum SFXSelection
        {
            LevelComplete,
            Construct,
            Charge,
            Deconstruct

        }

        //This is an enum that speeds up coding and reduces errors.
        //Specifying songs by enum takes advantage of Intellisense and reduces 
        //the chance of a spelling mistake.
        public enum MusicSelection
        {
            EarlyLevelv2,
            Basement,
            Happy,
            Robots,
            Menu,
            MidBasement,
            MidCore,
            Ballroom
           
        }

        public void Initialize()
        {
            //Initializes the dictionaries.
            soundEffects = new Dictionary<SFXSelection, SoundEffect>();
            songs = new Dictionary<MusicSelection, Song>();
            songsdict = new Dictionary<string, Song>();
            SFXVolume = 0.6f;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            muted = false;
            
        }

        public void LoadContent(ContentManager content)
        {
            //Uses relative pathfiles from your project (i.e. your mp3 files 
            //should be located in the Music folder for this demo).


            //Music
            songs.Add(MusicSelection.EarlyLevelv2, content.Load<Song>("Music/EarlyLevelv2"));
            songs.Add(MusicSelection.Basement, content.Load<Song>("Music/Basement"));
            songs.Add(MusicSelection.Happy, content.Load<Song>("Music/SpaceThemeHappy"));
            songs.Add(MusicSelection.Robots, content.Load<Song>("Music/LaterLevelv1"));
            songs.Add(MusicSelection.Menu, content.Load<Song>("Music/MainMenuv1"));
            songs.Add(MusicSelection.MidCore, content.Load<Song>("Music/Middle Core"));
            songs.Add(MusicSelection.MidBasement, content.Load<Song>("Music/Upper Basement"));
            songs.Add(MusicSelection.Ballroom, content.Load<Song>("Music/Ballroom"));

            // MAPS BACKGROUND FILENAMES TO MUSIC FILES
            songsdict.Add("background", content.Load<Song>("Music/Basement"));
            songsdict.Add("tutorial", content.Load<Song>("Music/EarlyLevelv2"));
            songsdict.Add("ballroom", content.Load<Song>("Music/Ballroom"));
            songsdict.Add("RonniesBestWork", content.Load<Song>("Music/SpaceThemeHappy"));
            songsdict.Add("basement2", content.Load<Song>("Music/Upper Basement"));
            songsdict.Add("bathroom", content.Load<Song>("Music/Basement"));
            songsdict.Add("plinko", content.Load<Song>("Music/EarlyLevelv2"));
            songsdict.Add("hallway2", content.Load<Song>("Music/Ballroom"));
            songsdict.Add("credits", content.Load<Song>("Music/SpaceThemeHappy"));
            songsdict.Add("space", content.Load<Song>("Music/Ballroom"));
            songsdict.Add("corelevelbg1", content.Load<Song>("Music/LaterLevelv1"));
            songsdict.Add("corelevelbg2", content.Load<Song>("Music/Middle Core"));
            songsdict.Add("corelevelbg3", content.Load<Song>("Music/LaterLevelv1"));
            songsdict.Add("corelevelbg4", content.Load<Song>("Music/Middle Core"));


            //Sound Effects
            soundEffects.Add(SFXSelection.LevelComplete, content.Load<SoundEffect>("Music\\LevelComplete"));
            soundEffects.Add(SFXSelection.Construct, content.Load<SoundEffect>("Music\\Construct"));
            soundEffects.Add(SFXSelection.Charge, content.Load<SoundEffect>("Music\\Constructing(charge)"));
            soundEffects.Add(SFXSelection.Deconstruct, content.Load<SoundEffect>("Music\\Deconstruct"));

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

        public void Play(string songname)
        {
            Song sng;
            if (songsdict.TryGetValue(songname, out sng))
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
                 if (SFXplaying != sound)
                 {
                     sound.Play();
                     SFXplaying = sound;
                 }
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
            if (muted)
            {
                MediaPlayer.Volume = 0.5f;
                SFXVolume = 0.5f;
            }
            else
            {
                MediaPlayer.Volume = 0.0f;
                SFXVolume = 0.0f;
            }
            muted = !muted;
        }

        //= true if the game is muted
        public bool isMuted()
        {
            return muted;
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
