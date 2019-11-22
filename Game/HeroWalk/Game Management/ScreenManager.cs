using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

// TODO: Refactor the whole XY system for levels...

namespace HeroWalk
{
    public interface IGameScreen
    {
        // Used to determine whether to change songs, and if so which song to play
        void OnLoad();
        void Update(GameTime gt, KeyboardState ks);
        void Draw(SpriteBatch sb);
        void Draw(SpriteBatch sb, Color c);
    }

    public enum Direction { North, East, South, West }
    public enum FilterState { Black, White, Darkening, Lightening };
    public enum TransitionType { Instant, Fade, Scroll, None };

    public static class ScreenManager
    {
        public static IGameScreen TitleScreen;

        // TODO: This is pretty cheap...
        public static IGameScreen IntroScreen1;
        public static IGameScreen IntroScreen2;
        public static IGameScreen IntroScreen3;
        public static IGameScreen IntroScreen4;

        public static IGameScreen DefeatScreen;

        public static IGameScreen VictoryScreen1;
        public static IGameScreen VictoryScreenFinal;
        public static SpriteFont Vinque20Font;

        public static IGameScreen HighScoreScreen;
        public static int HighScoreMoney;
        public static int HighScoreMonsters;
        public static int HighScoreTime;

        public static IGameScreen HelpScreen;
        public static SoundEffect HelpStartSound;

        public static Sprite DialogBox;
        public static Texture2D FontAtlas;
        public static SpriteFont Consolas12Font;

        public static IGameScreen CurrentState { get; set; }

        public static Dictionary<string, Song> Songs { get; set; }
        public static string CurrentSongKey { get; set; }
        public static Song CurrentSong { get; set; }
        //private static float Volume = 0;
        private static TimeSpan playPosition;

        public static void Init(IGameScreen titleScreen, IGameScreen introScreen1, 
            IGameScreen introScreen2, IGameScreen introScreen3, IGameScreen introScreen4,
            IGameScreen defeatScreen, IGameScreen victoryScreen1,
            IGameScreen victoryScreenFinal, IGameScreen highScoreScreen, IGameScreen helpScreen,
            IGameScreen currentState, Sprite dialogBox, Texture2D fontAtlas,
            SpriteFont dialogFont, Dictionary<string, Song> songs,
            SoundEffect helpStart, SpriteFont vinque20font)
        {
            TitleScreen = titleScreen;

            IntroScreen1 = introScreen1;
            IntroScreen2 = introScreen2;
            IntroScreen3 = introScreen3;
            IntroScreen4 = introScreen4;

            DefeatScreen = defeatScreen;

            VictoryScreen1 = victoryScreen1;
            VictoryScreenFinal = victoryScreenFinal;
            HighScoreScreen = highScoreScreen;
            Vinque20Font = vinque20font;

            HelpScreen = helpScreen;
            HelpStartSound = helpStart;

            CurrentState = currentState;

            DialogBox = dialogBox;
            FontAtlas = fontAtlas;
            Consolas12Font = dialogFont;

            Songs = songs;
            CurrentState.OnLoad();
        }

        public static void Update(GameTime gt, KeyboardState ks)
        {
            if (LevelManager.CurrentLevel.Hero.TimerStarted)
            {
                LevelManager.CurrentLevel.Hero.MSCounter +=
                    gt.ElapsedGameTime.Milliseconds;
            }

            if (MediaPlayer.State == MediaState.Stopped)
            {
                PlaySong(CurrentSongKey, playPosition);
            }
            CurrentState.Update(gt, ks);
        }

        public static void Draw(SpriteBatch sb)
        {
            CurrentState.Draw(sb);
        }

        public static void PlayFanfareThenResume(string key)
        {
            MediaPlayer.IsRepeating = false;
            playPosition = MediaPlayer.PlayPosition;
            MediaPlayer.Play(Songs[key]);
        }

        public static void PlaySong(string key)
        {
            if (key != CurrentSongKey)
            {
                MediaPlayer.Volume = 0.25f;
                MediaPlayer.Stop();
                MediaPlayer.Play(Songs[key]);
                MediaPlayer.IsRepeating = true;
                CurrentSongKey = key;
            }
        }

        public static void PlaySong(string key, TimeSpan playPosition)
        {
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.Stop();
            MediaPlayer.Play(Songs[key], playPosition);
            MediaPlayer.IsRepeating = true;
        }

        public static void ChangeVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        /* Given a string, a font and a line width, returns a formatted string so that none of
           the characters go out of bounds. */
        public static string WrapText(string text, SpriteFont font, int lineWidth)
        {
            var sb = new StringBuilder(text);
            int beginIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\n')
                {
                    string substring = text.Substring(beginIndex, i - beginIndex + 1);
                    if ((int)font.MeasureString(substring).X > lineWidth)
                    {
                        // go back, insert newline
                        for (int j = i; j >= 0; j--)
                        {
                            if (text[j] == ' ')
                            {
                                sb[j] = '\n';
                                i -= i - j;
                                beginIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    beginIndex = i;
                }
            }
            return sb.ToString();
        }

        // Multiply everything by 8 to get the top left corner
        // All chars are 8*12
        public static Dictionary<string, Vector2> FontDictionary =
            new Dictionary<string, Vector2>
        {
            { "A", new Vector2(0, 0) },
            { "a", new Vector2(1, 0) },
            { "B", new Vector2(2, 0) },
            { "b", new Vector2(3, 0) },
            { "C", new Vector2(4, 0) },
            { "c", new Vector2(5, 0) },
            { "D", new Vector2(6, 0) },
            { "d", new Vector2(7, 0) },
            { "E", new Vector2(8, 0) },
            { "e", new Vector2(9, 0) },
            { "F", new Vector2(10, 0) },
            { "f", new Vector2(11, 0) },
            { "G", new Vector2(12, 0) },
            { "g", new Vector2(13, 0) },
            { "H", new Vector2(14, 0) },
            { "h", new Vector2(15, 0) },
            { "I", new Vector2(16, 0) },
            { "i", new Vector2(17, 0) },
            { "J", new Vector2(18, 0) },
            { "j", new Vector2(19, 0) },
            { "K", new Vector2(20, 0) },
            { "k", new Vector2(21, 0) },
            { "L", new Vector2(22, 0) },
            { "l", new Vector2(23, 0) },
            { "M", new Vector2(24, 0) },
            { "m", new Vector2(25, 0) },

            { "N", new Vector2(26, 0) },
            { "n", new Vector2(27, 0) },
            { "O", new Vector2(28, 0) },
            { "o", new Vector2(29, 0) },
            { "P", new Vector2(30, 0) },
            { "p", new Vector2(31, 0) },
            { "Q", new Vector2(32, 0) },
            { "q", new Vector2(33, 0) },
            { "R", new Vector2(34, 0) },
            { "r", new Vector2(35, 0) },
            { "S", new Vector2(36, 0) },
            { "s", new Vector2(37, 0) },
            { "T", new Vector2(38, 0) },
            { "t", new Vector2(39, 0) },
            { "U", new Vector2(40, 0) },
            { "u", new Vector2(41, 0) },
            { "V", new Vector2(42, 0) },
            { "v", new Vector2(43, 0) },
            { "W", new Vector2(44, 0) },
            { "w", new Vector2(45, 0) },
            { "X", new Vector2(46, 0) },
            { "x", new Vector2(47, 0) },
            { "Y", new Vector2(48, 0) },
            { "y", new Vector2(49, 0) },
            { "Z", new Vector2(50, 0) },
            { "z", new Vector2(51, 0) },

            { "0", new Vector2(52, 0) },
            { "1", new Vector2(53, 0) },
            { "2", new Vector2(54, 0) },
            { "3", new Vector2(55, 0) },
            { "4", new Vector2(56, 0) },
            { "5", new Vector2(57, 0) },
            { "6", new Vector2(58, 0) },
            { "7", new Vector2(59, 0) },
            { "8", new Vector2(60, 0) },
            { "9", new Vector2(61, 0) },

            { ".", new Vector2(62, 4) },
            { ",", new Vector2(63, 4) },
            { "!", new Vector2(64, 4) },
            { "?", new Vector2(66, 4) },
            { "#", new Vector2(68, 4) },
            { "_", new Vector2(69, 4) },
            { "-", new Vector2(70, 4) },
            { ":", new Vector2(72, 4) },
            { ";", new Vector2(73, 4) },
            { "\'", new Vector2(74, 4) },
            { "\"", new Vector2(75, 4) },
            { " ", new Vector2(76, 4) }
        };
    }
}
