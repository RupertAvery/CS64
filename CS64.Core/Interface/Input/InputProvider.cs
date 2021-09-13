using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace CS64.Core.Interface.Input
{
    public class InputProvider
    {
        private readonly Dictionary<int, Controller> _deviceControllerMapping = new Dictionary<int, Controller>();

        private readonly Keyboard _keyboard = new Keyboard();

        public InputEvent InputEvent { get; set; }

        public InputProvider(InputEvent inputEvent)
        {
            InputEvent = inputEvent;
        }

        public bool HandleEvent(SDL_MouseMotionEvent motionEvent)
        {
            // Get the position on screen where the Zapper is pointed at
            InputEvent?.Invoke(null, new InputEventArgs() { EventType = InputEventType.POINT, LightPenX = motionEvent.x, LightPenY = motionEvent.y });
            return true;
        }

        public bool HandleEvent(SDL_MouseButtonEvent buttonEvent)
        {
            InputEvent?.Invoke(null, new InputEventArgs() { EventType = InputEventType.TRIGGER });
            return true;
        }

        public bool HandleEvent(SDL_KeyboardEvent keyboardEvent)
        {
            var handled = false;

            switch (keyboardEvent.type)
            {
                case SDL_EventType.SDL_KEYUP:
                    {
                        if (_keyboard.TryMap(keyboardEvent.keysym.sym, out InputKeyEnum mappedInput))
                        {
                            InputEvent?.Invoke(null, new InputEventArgs() { EventType = InputEventType.BUTTON_UP, Player = 0, Key = mappedInput });
                            handled = true;
                        }

                        break;
                    }
                case SDL_EventType.SDL_KEYDOWN:
                    {
                        if (_mappingMode)
                        {
                            if (_keyboard.TrySetMap(keyboardEvent.keysym.sym, _mappingTarget))
                            {
                                _mappingMode = false;
                                handled = true;
                            }
                        }
                        else
                        {
                            if (_keyboard.TryMap(keyboardEvent.keysym.sym, out InputKeyEnum mappedInput))
                            {
                                InputEvent?.Invoke(null, new InputEventArgs() { EventType = InputEventType.BUTTON_DOWN, Player = 0, Key = mappedInput });
                                handled = true;
                            }
                        }

                        break;
                    }
            }
            return handled;
        }

        public bool HandleControllerEvent(SDL_ControllerButtonEvent buttonEvent)
        {
            var handled = false;
            switch (buttonEvent.type)
            {
                case SDL_EventType.SDL_CONTROLLERBUTTONUP:
                    Console.WriteLine($"{buttonEvent.type} {buttonEvent.button} {buttonEvent.which}");
                    {
                        if (_deviceControllerMapping.TryGetValue(buttonEvent.which, out var controller))
                        {
                            if (controller.TryMap(buttonEvent.button, out InputKeyEnum mappedInput))
                            {
                                InputEvent?.Invoke(null, new InputEventArgs() { EventType = InputEventType.BUTTON_UP, Player = controller.ControllerIndex, Key = mappedInput });
                                handled = true;
                            }
                        }
                    }
                    break;

                case SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                    Console.WriteLine($"{buttonEvent.type} {buttonEvent.button} {buttonEvent.which}");
                    {
                        if (_deviceControllerMapping.TryGetValue(buttonEvent.which, out var controller))
                        {
                            if (controller.TryMap(buttonEvent.button, out InputKeyEnum mappedInput))
                            {
                                InputEvent?.Invoke(null, new InputEventArgs() { EventType = InputEventType.BUTTON_DOWN, Player = controller.ControllerIndex, Key = mappedInput });
                                handled = true;
                            }
                        }
                    }
                    break;

            }

            return handled;
        }

        public void HandleDeviceEvent(SDL_ControllerDeviceEvent deviceEvent)
        {
            //Console.WriteLine($"{deviceEvent.type} {deviceEvent.which}");

            switch (deviceEvent.type)
            {
                case SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                    {
                        if (Controller.TryOpen(deviceEvent.which, out var controller))
                        {
                            _deviceControllerMapping.Add(deviceEvent.which, controller);
                        }

                    }
                    break;
                case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                    {
                        if (_deviceControllerMapping.TryGetValue(deviceEvent.which, out var controller))
                        {
                            controller.Close();
                            _deviceControllerMapping.Remove(deviceEvent.which);
                        }
                    }
                    break;
            }
        }


        private bool _mappingMode = false;
        private InputKeyEnum _mappingTarget;

        public void SetMapping(InputKeyEnum key)
        {
            _mappingMode = true;
            _mappingTarget = key;
        }

        public void ClearMapping(InputKeyEnum key)
        {
            //_mappingTarget = button;
        }

        public string GetMapping(InputKeyEnum key)
        {
            if (_keyboard.TryGetMap(key, out var mappedKey))
            {
                return mappedKey.ToString();
            }

            return "";
        }
    }
}