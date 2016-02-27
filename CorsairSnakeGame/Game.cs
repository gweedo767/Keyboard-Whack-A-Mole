using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/**
 * This piece of code written by http://www.reddit.com/user/mythicmaniac
 * Original code for hooking to the lights written in C by http://www.reddit.com/user/chrisgzy
 * Original code ported to C# by http://www.reddit.com/user/billism
 */

namespace KeybaordAudio
{
    public class Game
    {

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public KeyboardHookProc KeyboardHookDelegate;

        public delegate void KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static int[] HealthMap = new int[10] {
            13, 25, 37, 49, 61, 73, 85, 97, 109, 121
        };

        public static int[,] ScreenMap = new int[10, 3] {
            {14, 15, 28},
            {26, 27, 40},
            {38, 39, 52},
            {50, 51, 64},
            {62, 63, 76},
            {74, 75, 88},
            {86, 87, 100},
            {98, 99, 112},
            {110, 111, 124},
            {122, 123, 136}
        };

        public static int[] ProgressMap = new int[6] {
            105,
            117,
            69,
            81,
            21,
            33
        };

        public KeyboardWriter Keyboard;

        public Random Random;
        public bool Running;

        public List<Mole> Moles;

        public int Score = 0;
        public int Health = 10;
        public int MoleSpawnTimer = 10;
        public int NextMole = 0;

        public IntPtr Hook;

        public int LoseTimer;
        public int WinTimer;
        public int Length;
        public int WinThreshold;

        public readonly int SleepTime = 150;

        public Game()
        {
            Keyboard = new KeyboardWriter();
        }

        public void HookInput()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                KeyboardHookDelegate = new KeyboardHookProc(KeyboardHook);
                Hook = SetWindowsHookEx(13, KeyboardHookDelegate, GetModuleHandle(curModule.ModuleName), 0);
            }

            Application.Run();
        }

        public void KeyboardHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >= 0 && wParam == (IntPtr)0x0100)
            {
                int code = Marshal.ReadInt32(lParam);

                Console.WriteLine("You hit: " + code);

                int X = 0;
                int Y = 0;

                if (code == (int)'Q')
                {
                    X = 0;
                    Y = 1;
                }
                else if (code == (int)'W')
                {
                    X = 1;
                    Y = 1;
                }
                else if (code == (int)'E')
                {
                    X = 2;
                    Y = 1;
                }
                else if (code == (int)'R')
                {
                    X = 3;
                    Y = 1;
                }
                else if (code == (int)'T')
                {
                    X = 4;
                    Y = 1;
                }
                else if (code == (int)'Y')
                {
                    X = 5;
                    Y = 1;
                }
                else if (code == (int)'U')
                {
                    X = 6;
                    Y = 1;
                }
                else if (code == (int)'I')
                {
                    X = 7;
                    Y = 1;
                }
                else if (code == (int)'O')
                {
                    X = 8;
                    Y = 1;
                }
                else if (code == (int)'P')
                {
                    X = 9;
                    Y = 1;
                }
                else if (code == (int)'A')
                {
                    X = 0;
                    Y = 2;
                }
                else if (code == (int)'S')
                {
                    X = 1;
                    Y = 2;
                }
                else if (code == (int)'D')
                {
                    X = 2;
                    Y = 2;
                }
                else if (code == (int)'F')
                {
                    X = 3;
                    Y = 2;
                }
                else if (code == (int)'G')
                {
                    X = 4;
                    Y = 2;
                }
                else if (code == (int)'H')
                {
                    X = 5;
                    Y = 2;
                }
                else if (code == (int)'J')
                {
                    X = 6;
                    Y = 2;
                }
                else if (code == (int)'K')
                {
                    X = 7;
                    Y = 2;
                }
                else if (code == (int)'L')
                {
                    X = 8;
                    Y = 2;
                }
                else if (code == 186) // ;
                {
                    X = 9;
                    Y = 2;
                }
                else if (code == (int)'Z')
                {
                    X = 0;
                    Y = 3;
                }
                else if (code == (int)'X')
                {
                    X = 1;
                    Y = 3;
                }
                else if (code == (int)'C')
                {
                    X = 2;
                    Y = 3;
                }
                else if (code == (int)'V')
                {
                    X = 3;
                    Y = 3;
                }
                else if (code == (int)'B')
                {
                    X = 4;
                    Y = 3;
                }
                else if (code == (int)'N')
                {
                    X = 5;
                    Y = 3;
                }
                else if (code == (int)'M')
                {
                    X = 6;
                    Y = 3;
                }
                else if (code == 188) // ,
                {
                    X = 7;
                    Y = 3;
                }
                else if (code == 190) // .
                {
                    X = 8;
                    Y = 3;
                }
                else if (code == 191) // /
                {
                    X = 9;
                    Y = 3;
                }

                // quick fix for removing the number rows as health
                Y--;

                Console.WriteLine("you pressed at: " + X + "," + Y);

                Boolean Hit = false;

                for (int i = 0; i < Moles.Count; i++)
                {
                    Mole mole = Moles[i];
                    if (X == mole.X && Y == mole.Y)
                    {
                        Moles.Remove(mole);
                        Console.WriteLine("You hit it!");
                        Score++;
                        Hit = true;
                    }
                }

                if (!Hit)
                {
                    Health--;
                    if (Health < 1)
                    {
                        GameLose();
                    }
                }

                Console.WriteLine("Score: " + Score);
                

                if (code == 27)
                {
                    Running = false;
                    Application.Exit();
                    Keyboard.Clear();
                }
            }
            CallNextHookEx(Hook, nCode, wParam, lParam);
        }

        public void Initialize()
        {
            LoseTimer = 0;
            WinTimer = 0;
            Length = 1;
            WinThreshold = 30;
            Random = new Random();
            Moles = new List<Mole>();
            SpawnMole();
            Score = 0;
            Health = 10;
            MoleSpawnTimer = 10;
            NextMole = MoleSpawnTimer;
        }

        public void Run()
        {
            Running = true;
            Initialize();
            var watch = new Stopwatch();
            var time = 0;
            while (Running)
            {
                watch.Restart();
                Update();
                Keyboard.UpdateKeyboard();
                watch.Stop();
                time = (int)(SleepTime - watch.Elapsed.TotalMilliseconds);
                if (time < 0)
                    time = 0;
                Thread.Sleep(time);
            }
        }

        public void Update()
        {
            if (LoseTimer > 0)
            {
                LoseTimer--;
                if (LoseTimer == 0)
                    Initialize();
                RefreshLoseScreen();
                return;
            }
            if (WinTimer > 0)
            {
                WinTimer--;
                if (WinTimer == 0)
                    Initialize();
                RefreshWinScreen();
                return;
            }

            NextMole--;
            if (NextMole < 1)
            {
                SpawnMole();
                NextMole = MoleSpawnTimer;
                MoleSpawnTimer--;
                if (MoleSpawnTimer < 2) MoleSpawnTimer = 2;
            }

            //decrement moles life
            for (int i = 0; i < Moles.Count; i++)
            {
                Mole mole = Moles[i];
                mole.Life--;
                if (mole.Life == 0)
                {
                    Moles.Remove(mole);
                    Health--;
                    Console.WriteLine("Score: " + Score);

                    if (Health < 1)
                    {
                        GameLose();
                    }
                    else
                    {
                        SpawnMole();
                    }
                }
            }

            RefreshScreen();
        }

        public void SpawnMole()
        {
            var occupied = new List<Mole>();
            for (int i = 0; i < Moles.Count; i++)
            {
                var piece = Moles[i];
                occupied.Add(new Mole(piece.X, piece.Y));
            }

            var unoccupied = new List<Mole>();
            var flag = false;
            for (int i = 0; i < ScreenMap.GetLength(0); i++)
            {
                for(int j = 0; j < ScreenMap.GetLength(1); j++)
                {
                    flag = false;
                    for(int k = 0; k < occupied.Count; k++)
                    {
                        if(occupied[k].X == i && occupied[k].Y == j)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                         unoccupied.Add(new Mole(i, j));
                }
            }

            Moles.Add(unoccupied[Random.Next(unoccupied.Count)]);
        }

        public void GameWin()
        {
            var inSecond = 1000f / SleepTime;
            WinTimer = (int)(inSecond * 3);
        }

        public void RefreshWinScreen()
        {
            var rand = new Random();
            var colors = new int[7][];
            colors[0] = new int[] { 255, 0, 0 };
            colors[1] = new int[] { 0, 255, 0 };
            colors[2] = new int[] { 0, 0, 255 };
            colors[3] = new int[] { 255, 255, 0 };
            colors[4] = new int[] { 0, 255, 255 };
            colors[5] = new int[] { 255, 0, 255 };
            colors[6] = new int[] { 255, 255, 255 };

            for (int i = 0; i < 144; i++)
            {
                var index = rand.Next(7);
                Keyboard.SetLed(i, colors[index][0], colors[index][1], colors[index][2]);
            }

        }

        public void GameLose()
        {
            var inSecond = 1000f / SleepTime;
            LoseTimer = (int)(inSecond * 3);
        }

        public void RefreshLoseScreen()
        {
            for (int i = 0; i < 144; i++)
            {
                Keyboard.SetLed(i, 255, 0, 0);
            }
        }

        public void RefreshHealthBar()
        {
            int index = 0;
            for (; index < Health; index++)
            {
                Keyboard.SetLed(HealthMap[index], 255, 0, 0);
            }
            for (; index < 10; index++)
            {
                Keyboard.SetLed(HealthMap[index], 255, 255, 255);
            }
        }

        public void RefreshScreen()
        {
            // Set everything to blue
            for (int i = 0; i < 144; i++)
            {
                Keyboard.SetLed(i, 0, 255, 255);
            }

            // Clear level section
            for (int i = 0; i < ScreenMap.GetLength(0); i++)
            {
                for (int j = 0; j < ScreenMap.GetLength(1); j++)
                {
                    Keyboard.SetLed(ScreenMap[i, j], 0, 0, 0);
                }
            }

            // Draw Moles
            for (int i = 0; i < Moles.Count; i++)
            {
                Mole mole = Moles[i];
                Keyboard.SetLed(ScreenMap[mole.X, mole.Y], 255, 255, 0);            
            }

            // Draw the health bar
            RefreshHealthBar();
        }
    }
    
    public class Mole
    {
        public int X;
        public int Y;
        public int Life;

        public Mole(int x, int y)
        {
            X = x;
            Y = y;
            Life = 20;
        }
    }
}
