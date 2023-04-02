using System;
using System.Management.Automation.Host;
using System.Runtime.InteropServices;

namespace doob.Scripter.Engine.Powershell
{
    internal class CustomPSHostRawUserInterface : PSHostRawUserInterface
    {

        public override ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        public override Size BufferSize
        {
            get => new Size(300, 3000);
            set
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.SetBufferSize(value.Width, value.Height);
                }
            }
            
        }

        public override int CursorSize
        {
            get => Console.CursorSize;
            set
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.CursorSize = value;
                }
            }
           
        }

        public override ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public override bool KeyAvailable => Console.KeyAvailable;

        public override Size MaxPhysicalWindowSize => new Size(Console.LargestWindowWidth, Console.LargestWindowHeight);

        public override Size MaxWindowSize => new Size(Console.LargestWindowWidth, Console.LargestWindowHeight);

        public override Coordinates WindowPosition
        {
            get => new Coordinates(Console.WindowLeft, Console.WindowTop);
            set
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.SetWindowPosition(value.X, value.Y);
                }
            }
            
        }

        public override Size WindowSize
        {
            get => new Size(Console.WindowWidth, Console.WindowHeight);
            set
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.SetWindowSize(value.Width, value.Height);
                }
            }
            
        }

        public override string WindowTitle
        {
            get => "CustomPS";
            set => Console.Title = value;
        }

        public override void FlushInputBuffer()
        {
            Console.In.ReadToEnd();
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            //throw new NotImplementedException("TODO: Verify my ReadKey code works");
            //var keyInfo = Console.ReadKey((options & ReadKeyOptions.NoEcho) == ReadKeyOptions.NoEcho);
            //ControlKeyStates ctrlKeyState;
            //switch (keyInfo.Modifiers)
            //{
            //    case ConsoleModifiers.Control:
            //        ctrlKeyState = ControlKeyStates.LeftCtrlPressed;
            //        break;
            //    case ConsoleModifiers.Shift:
            //        ctrlKeyState = ControlKeyStates.ShiftPressed;
            //        break;
            //    case ConsoleModifiers.Alt:
            //        ctrlKeyState = ControlKeyStates.LeftAltPressed;
            //        break;
            //    case ConsoleModifiers.Control | ConsoleModifiers.Alt:
            //        ctrlKeyState = ControlKeyStates.LeftCtrlPressed | ControlKeyStates.LeftAltPressed;
            //        break;
            //    case ConsoleModifiers.Control | ConsoleModifiers.Shift:
            //        ctrlKeyState = ControlKeyStates.LeftCtrlPressed | ControlKeyStates.ShiftPressed;
            //        break;
            //    case ConsoleModifiers.Alt | ConsoleModifiers.Shift:
            //        ctrlKeyState = ControlKeyStates.LeftAltPressed | ControlKeyStates.ShiftPressed;
            //        break;
            //    default:
            //        ctrlKeyState = 0;
            //        break;
            //}
            //return new KeyInfo
            //{
            //    Character = keyInfo.KeyChar,
            //    ControlKeyState = ctrlKeyState,
            //    KeyDown = false,
            
            //    VirtualKeyCode = (int)keyInfo.Key
            //};
            throw new NotImplementedException();
        }

        public override Coordinates CursorPosition
        {
            get => new Coordinates(0, 0);
            set => throw new NotImplementedException();
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException();
        }
    }
}