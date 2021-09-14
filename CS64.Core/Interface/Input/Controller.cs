using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace CS64.Core.Interface.Input
{
    public class Controller
    {
        private IntPtr gamepad;
        private IntPtr joystick;
        private int instanceId;

        private readonly Dictionary<int, InputKeyEnum[]> _buttonMapping = new Dictionary<int, InputKeyEnum[]>();
        
        public int ControllerIndex { get; set; }

        public Controller()
        {
            //_buttonMapping.Add(11, InputKeyEnum.Up);
            //_buttonMapping.Add(12, InputKeyEnum.Down);
            //_buttonMapping.Add(13, InputKeyEnum.Left);
            //_buttonMapping.Add(14, InputKeyEnum.Right);
            //_buttonMapping.Add(3, InputKeyEnum.B);
            //_buttonMapping.Add(2, InputKeyEnum.A);
            //_buttonMapping.Add(1, InputKeyEnum.B);
            //_buttonMapping.Add(0, InputKeyEnum.A);
            //_buttonMapping.Add(4, InputKeyEnum.Select);
            //_buttonMapping.Add(6, InputKeyEnum.Start);
        }

        public bool TryMap(int button, out InputKeyEnum[] mappedInput)
        {
            return _buttonMapping.TryGetValue(button, out mappedInput);
        }

        public static bool TryOpen(int deviceId, out Controller controller)
        {
            controller = new Controller();
            controller.gamepad = SDL_GameControllerOpen(deviceId);
            controller.joystick = SDL_GameControllerGetJoystick(controller.gamepad);
            controller.instanceId = SDL_JoystickInstanceID(controller.joystick);
            return true;
        }

        public void Close()
        {
            SDL_GameControllerClose(gamepad);
        }
    }
}