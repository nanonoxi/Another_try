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

        public SoundManager()
        { sounds = new List<SoundObject>(); }


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
