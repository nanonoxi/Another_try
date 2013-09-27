// Jose Strachan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Gameception
{
    class SoundObject
    {
        Song song;
        SoundEffect soundEffect;
        double songDuration;

        public double playTime;
        public string soundName;
        public int songOrSoundEffect;
        public double startTime, timeNow;


        public SoundObject(Song s, string name)
        {
            song = s;
            songDuration = song.Duration.TotalSeconds;
            soundName = name;
            playTime = 0.0;
            startTime = 0.0;
            songOrSoundEffect = 1;
        }

        public SoundObject(SoundEffect s, string name)
        {
            soundEffect = s;
            soundName = name;
            songOrSoundEffect = 2;
        }

        public Song getSong()
        {
            return song;
        }

        public SoundEffect getSound()
        {
            
            return soundEffect;
        }

        public void stopTrack()
        {
            playTime = 0.0;
        }

        public void update()
        {
            timeNow = DateTime.Now.TimeOfDay.TotalSeconds;
            if ((timeNow - startTime) >= songDuration)
                playTime = 0.0;

        }
    }
}
