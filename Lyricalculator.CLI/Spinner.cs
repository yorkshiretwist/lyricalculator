using System;
using System.Threading;

namespace Lyricalculator.CLI
{
    /// <summary>
    /// Shamelessly stolen from https://stackoverflow.com/questions/1923323/console-animations then amended a bit
    /// </summary>
    public class Spinner : IDisposable
    {
        private const string Sequence = @"/-\|";
        private int counter = 0;
        private int left;
        private int top;
        private readonly int delay;
        private bool active;
        private Thread thread;

        public Spinner(int delay = 100)
        {
            this.delay = delay;
        }

        public void Start()
        {
            thread = new Thread(Spin);
            left = 0;
            top = Console.CursorTop;

            active = true;
            if (!thread.IsAlive)
            {
                thread.Start();
            }
        }

        public void Stop()
        {
            active = false;
            Draw(' ');
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.White;
            thread = null;
        }

        private void Spin()
        {
            while (active)
            {
                Turn();
                Thread.Sleep(delay);
            }
        }

        private void Draw(char c)
        {
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(c);
        }

        private void Turn()
        {
            Draw(Sequence[++counter % Sequence.Length]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
