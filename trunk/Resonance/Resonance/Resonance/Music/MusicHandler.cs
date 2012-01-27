using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Resonance
{
    class MusicHandler
    {
        public static string GREEN = "TomLow";
        public static string BLUE = "TomMiddle";
        public static string YELLOW = "TomHigh";
        public static string RED = "Snare";
        public static string CYMBAL = "Crash";
        public static string CHINK = "chink";
        public static string DING = "ding";
        public static string SHIMMER = "shimmer";

        MusicTrack bgMusic;
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;

        bool autoPlayMusic = false;

        public MusicHandler(ContentManager newContent)
        {
            bgMusic = new MusicTrack(newContent);
            audioEngine = new AudioEngine("Content/SoundProject.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Sound Bank.xsb");

            if (autoPlayMusic == true) bgMusic.playTrack();
        }

        /// <summary>
        /// Gets access to the background music track
        /// </summary>
        /// <returns>bgMusic the background music track</returns>
        public MusicTrack getTrack()
        {
            return bgMusic;
        }

        public void playSound(string sound)
        {
            soundBank.PlayCue(sound);
        }

        public void Update()
        {
            audioEngine.Update();
            bgMusic.update();
        }
    }
}
