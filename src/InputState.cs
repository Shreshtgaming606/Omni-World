using System.Collections.Generic;
using System.Windows.Forms;

namespace OmniWorld
{
    public sealed class InputState
    {
        private readonly HashSet<Keys> down;
        private readonly HashSet<Keys> pressed;

        public InputState()
        {
            down = new HashSet<Keys>();
            pressed = new HashSet<Keys>();
        }

        public void OnKeyDown(Keys key)
        {
            if (!down.Contains(key))
            {
                pressed.Add(key);
            }

            down.Add(key);
        }

        public void OnKeyUp(Keys key)
        {
            down.Remove(key);
        }

        public void EndFrame()
        {
            pressed.Clear();
        }

        public bool IsDown(Keys key)
        {
            return down.Contains(key);
        }

        public bool WasPressed(Keys key)
        {
            return pressed.Contains(key);
        }

        public bool Left
        {
            get { return IsDown(Keys.Left) || IsDown(Keys.A); }
        }

        public bool Right
        {
            get { return IsDown(Keys.Right) || IsDown(Keys.D); }
        }

        public bool UpPressed
        {
            get { return WasPressed(Keys.Up) || WasPressed(Keys.W); }
        }

        public bool DownPressed
        {
            get { return WasPressed(Keys.Down) || WasPressed(Keys.S); }
        }

        public bool SlamPressed
        {
            get { return WasPressed(Keys.Down) || WasPressed(Keys.S); }
        }

        public bool Run
        {
            get { return IsDown(Keys.ShiftKey) || IsDown(Keys.LShiftKey) || IsDown(Keys.RShiftKey); }
        }

        public bool JumpHeld
        {
            get { return IsDown(Keys.Space) || IsDown(Keys.Z) || IsDown(Keys.Up); }
        }

        public bool JumpPressed
        {
            get { return WasPressed(Keys.Space) || WasPressed(Keys.Z) || WasPressed(Keys.Up); }
        }

        public bool BurstPressed
        {
            get { return WasPressed(Keys.X) || WasPressed(Keys.C) || WasPressed(Keys.ControlKey) || WasPressed(Keys.LControlKey) || WasPressed(Keys.RControlKey); }
        }

        public bool ConfirmPressed
        {
            get { return WasPressed(Keys.Enter); }
        }

        public bool PausePressed
        {
            get { return WasPressed(Keys.Escape) || WasPressed(Keys.P); }
        }

        public bool RestartPressed
        {
            get { return WasPressed(Keys.R); }
        }

        public bool MenuPressed
        {
            get { return WasPressed(Keys.M); }
        }
    }
}
