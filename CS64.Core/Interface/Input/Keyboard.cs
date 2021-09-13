using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace CS64.Core.Interface.Input
{
    public class Keyboard
    {
        private readonly Dictionary<SDL_Keycode, InputKeyEnum> _keyMapping = new Dictionary<SDL_Keycode, InputKeyEnum>();

        public Keyboard()
        {
            _keyMapping.Add(SDL_Keycode.SDLK_UP, InputKeyEnum.CURSOR_DOWN);
            _keyMapping.Add(SDL_Keycode.SDLK_DOWN, InputKeyEnum.CURSOR_DOWN);
            _keyMapping.Add(SDL_Keycode.SDLK_LEFT, InputKeyEnum.CURSOR_RIGHT);
            _keyMapping.Add(SDL_Keycode.SDLK_RIGHT, InputKeyEnum.CURSOR_RIGHT);

            _keyMapping.Add(SDL_Keycode.SDLK_DELETE, InputKeyEnum.DELETE);
            _keyMapping.Add(SDL_Keycode.SDLK_BACKSPACE, InputKeyEnum.BACKSPACE);
            _keyMapping.Add(SDL_Keycode.SDLK_SPACE, InputKeyEnum.SPACE);
            _keyMapping.Add(SDL_Keycode.SDLK_RETURN, InputKeyEnum.RETURN);

            _keyMapping.Add(SDL_Keycode.SDLK_LSHIFT, InputKeyEnum.LSHIFT);
            _keyMapping.Add(SDL_Keycode.SDLK_RSHIFT, InputKeyEnum.RSHIFT);

            _keyMapping.Add(SDL_Keycode.SDLK_SEMICOLON, InputKeyEnum.COLON);
            _keyMapping.Add(SDL_Keycode.SDLK_QUOTE, InputKeyEnum.SEMICOLON);
            
            _keyMapping.Add(SDL_Keycode.SDLK_COMMA, InputKeyEnum.COMMA);
            _keyMapping.Add(SDL_Keycode.SDLK_PERIOD, InputKeyEnum.PERIOD);

            _keyMapping.Add(SDL_Keycode.SDLK_HOME, InputKeyEnum.HOME);
            _keyMapping.Add(SDL_Keycode.SDLK_END, InputKeyEnum.STOP);
            _keyMapping.Add(SDL_Keycode.SDLK_ESCAPE, InputKeyEnum.RESTORE);
            _keyMapping.Add(SDL_Keycode.SDLK_TAB, InputKeyEnum.CTRL);

            _keyMapping.Add(SDL_Keycode.SDLK_0, InputKeyEnum.D0);
            _keyMapping.Add(SDL_Keycode.SDLK_1, InputKeyEnum.D1);
            _keyMapping.Add(SDL_Keycode.SDLK_2, InputKeyEnum.D2);
            _keyMapping.Add(SDL_Keycode.SDLK_3, InputKeyEnum.D3);
            _keyMapping.Add(SDL_Keycode.SDLK_4, InputKeyEnum.D4);
            _keyMapping.Add(SDL_Keycode.SDLK_5, InputKeyEnum.D5);
            _keyMapping.Add(SDL_Keycode.SDLK_6, InputKeyEnum.D6);
            _keyMapping.Add(SDL_Keycode.SDLK_7, InputKeyEnum.D7);
            _keyMapping.Add(SDL_Keycode.SDLK_8, InputKeyEnum.D8);
            _keyMapping.Add(SDL_Keycode.SDLK_9, InputKeyEnum.D9);

            _keyMapping.Add(SDL_Keycode.SDLK_KP_COLON, InputKeyEnum.COLON);
            _keyMapping.Add(SDL_Keycode.SDLK_KP_COMMA, InputKeyEnum.COMMA);
            _keyMapping.Add(SDL_Keycode.SDLK_KP_EQUALS, InputKeyEnum.EQUALS);

            _keyMapping.Add(SDL_Keycode.SDLK_a, InputKeyEnum.A);
            _keyMapping.Add(SDL_Keycode.SDLK_b, InputKeyEnum.B);
            _keyMapping.Add(SDL_Keycode.SDLK_c, InputKeyEnum.C);
            _keyMapping.Add(SDL_Keycode.SDLK_d, InputKeyEnum.D);
            _keyMapping.Add(SDL_Keycode.SDLK_e, InputKeyEnum.E);
            _keyMapping.Add(SDL_Keycode.SDLK_f, InputKeyEnum.F);
            _keyMapping.Add(SDL_Keycode.SDLK_g, InputKeyEnum.G);
            _keyMapping.Add(SDL_Keycode.SDLK_h, InputKeyEnum.H);
            _keyMapping.Add(SDL_Keycode.SDLK_i, InputKeyEnum.I);
            _keyMapping.Add(SDL_Keycode.SDLK_j, InputKeyEnum.J);
            _keyMapping.Add(SDL_Keycode.SDLK_k, InputKeyEnum.K);
            _keyMapping.Add(SDL_Keycode.SDLK_l, InputKeyEnum.L);
            _keyMapping.Add(SDL_Keycode.SDLK_m, InputKeyEnum.M);
            _keyMapping.Add(SDL_Keycode.SDLK_n, InputKeyEnum.N);
            _keyMapping.Add(SDL_Keycode.SDLK_o, InputKeyEnum.O);
            _keyMapping.Add(SDL_Keycode.SDLK_p, InputKeyEnum.P);
            _keyMapping.Add(SDL_Keycode.SDLK_q, InputKeyEnum.Q);
            _keyMapping.Add(SDL_Keycode.SDLK_r, InputKeyEnum.R);
            _keyMapping.Add(SDL_Keycode.SDLK_s, InputKeyEnum.S);
            _keyMapping.Add(SDL_Keycode.SDLK_t, InputKeyEnum.T);
            _keyMapping.Add(SDL_Keycode.SDLK_u, InputKeyEnum.U);
            _keyMapping.Add(SDL_Keycode.SDLK_v, InputKeyEnum.V);
            _keyMapping.Add(SDL_Keycode.SDLK_w, InputKeyEnum.W);
            _keyMapping.Add(SDL_Keycode.SDLK_x, InputKeyEnum.X);
            _keyMapping.Add(SDL_Keycode.SDLK_y, InputKeyEnum.Y);
            _keyMapping.Add(SDL_Keycode.SDLK_z, InputKeyEnum.Z);
        }

        public bool TryMap(SDL_Keycode keyCode, out InputKeyEnum mappedInput)
        {
            return _keyMapping.TryGetValue(keyCode, out mappedInput);
        }

        public bool TrySetMap(SDL_Keycode key, InputKeyEnum target)
        {
            if (TryGetMap(target, out var oldkey))
            {
                _keyMapping.Remove(oldkey.Value);
            }
            _keyMapping[key] = target;
            return true;
        }

        public IEnumerable<KeyValuePair<SDL_Keycode, InputKeyEnum>> GetMappings()
        {
            return _keyMapping;
        }

        public bool TryGetMap(InputKeyEnum key, out SDL_Keycode? mappedKey)
        {
            mappedKey = null;

            if (_keyMapping.ContainsValue(key))
            {
                var mapping = _keyMapping.First(m => m.Value == key);
                mappedKey = mapping.Key;
                return true;
            }

            return false;
        }

    }
}