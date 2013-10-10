/// Merada Richter, 2013.07.27
/// Based on GSMSample for Windows

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gameception
{

    /// <summary>
    /// TO-DO: change configuration options here
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Attributes

        MenuEntry crawlMenuEntry;
        MenuEntry soundMenuEntry;
        MenuEntry languageMenuEntry;

        static bool crawl = true;

        static int[] soundVolume = { 50, 75, 100, 0, 25 };
        static int currentSoundVolume = 0;

        static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        static int currentLanguage = 0;

        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("optionsmenu")
        {
            // Create our menu entries.
            crawlMenuEntry = new MenuEntry(string.Empty);
            soundMenuEntry = new MenuEntry(string.Empty);
            languageMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            crawlMenuEntry.Selected += CrawlMenuEntrySelected;
            soundMenuEntry.Selected += SoundMenuEntrySelected;
            languageMenuEntry.Selected += LanguageMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(crawlMenuEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(languageMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            crawlMenuEntry.Text = "Display Crawl: " + (crawl ? "on" : "off");
            soundMenuEntry.Text = "Sound Volume: " + soundVolume[currentSoundVolume];
            languageMenuEntry.Text = "Language: " + languages[currentLanguage];
        }


        #endregion


        #region Handle Input

        /// <summary>
        /// Event handler for when the Crawl menu entry is selected.
        /// </summary>
        void CrawlMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            crawl = !crawl;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Sound menu entry is selected.
        /// </summary>
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            currentSoundVolume = (currentSoundVolume + 1) % soundVolume.Length;

            float setVol = 0; ;
            if (currentSoundVolume == 0)
            {
                setVol = .5f;
            }
            if (currentSoundVolume == 1)
            {
                setVol = .75f;
            }
            if (currentSoundVolume == 2)
            {
                setVol = 1f;
            }
            if (currentSoundVolume == 3)
            {
                setVol = 0f;
            }
            if (currentSoundVolume == 4)
            {
                setVol = .25f;
            }

            ScreenManager.SoundManager.setVolume(setVol);

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;

            SetMenuEntryText();
        }


        #endregion
    }
}
