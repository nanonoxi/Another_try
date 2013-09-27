// Jose Strachan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Gameception
{
    public class SoundManager
    {
        SoundObject sound;
        List<SoundObject> sounds;
        int debouncer = 0, pauseDebouncer = 0;
        bool playing = false;

        private float volumeLevel;

        public SoundManager()
        { 
            sounds = new List<SoundObject>();
            volumeLevel = .5f;

            MediaPlayer.Volume = volumeLevel;
            SoundEffect.MasterVolume = volumeLevel;
        }

        /* 2 add methods catering for both songs(mp3 files)
         * and soundEffect files(wav files)
         * nameReference => how to reference sound
         */
        public void add(SoundEffect s, string nameReference)
        {
            sound = new SoundObject(s, nameReference);
            sounds.Add(sound);
        }
        public void add(Song s, string nameReference)
        {
            sound = new SoundObject(s, nameReference);
            sounds.Add(sound);
        }

        /*
         * play method, use nameReference to play sound
         */
        public void play(String s)
        {
            foreach (SoundObject so in sounds)
            {

                if (so.soundName.Equals(s))
                {
                    //if song
                    if (so.songOrSoundEffect == 1)
                    {
                        if (so.playTime <= 0.0)
                        {
                            MediaPlayer.Play(so.getSong());
                            so.startTime = DateTime.Now.TimeOfDay.TotalSeconds;
                            so.playTime = so.getSong().Duration.TotalSeconds;
                        }
                    }
                    //if soundEffect
                    else if (so.songOrSoundEffect == 2)
                    {
                        if (debouncer == 0)
                            debouncer = 60;

                        so.getSound().Play();
        
                    }


                    break;
                }

            }
        }

        public void increaseMasterVolume()
        {
            if (MediaPlayer.Volume < 1)
                MediaPlayer.Volume += 0.1f;
            if (SoundEffect.MasterVolume < 1)
                SoundEffect.MasterVolume += 0.1f;
        }
        public void decreaseMasterVolume()
        {
            if (MediaPlayer.Volume > 0)
                MediaPlayer.Volume -= 0.1f;
            if (SoundEffect.MasterVolume > 0)
                SoundEffect.MasterVolume -= 0.1f;
        }

        public void increaseSFXVoume()
        {
            if (SoundEffect.MasterVolume < 1)
                SoundEffect.MasterVolume += 0.1f;
        }
        public void decreseSFXVolume()
        {
            if (SoundEffect.MasterVolume > 0)
                SoundEffect.MasterVolume -= 0.1f;
        }

        public void increaseBGM()
        {
            if (MediaPlayer.Volume < 1)
                MediaPlayer.Volume += 0.1f;
        }
        public void decreaseBGM()
        {
            if (MediaPlayer.Volume > 0)
                MediaPlayer.Volume -= 0.1f;
        }

        public void pause()
        {
            if (pauseDebouncer <= 0)
            {
                pauseDebouncer = 40;

                playing = !playing;

                if (playing)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();
            }
        }

        public void stop()
        {
            foreach (SoundObject s in sounds)
                s.stopTrack();
        }

        public void Update()
        {
            debouncer--;
            pauseDebouncer--;

            foreach (SoundObject so in sounds)
                if (so.songOrSoundEffect == 1)
                    so.update();
        }
    }
}
