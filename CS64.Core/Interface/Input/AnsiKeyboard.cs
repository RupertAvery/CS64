using static SDL2.SDL;

namespace CS64.Core.Interface.Input
{
    public class AnsiKeyboard : Keyboard
    {
        public override void SetupBinding()
        {
            //TODO: Bug - when holding SHIFT and alternating between characters rapidly, 
            // you may end up with unshifted characters
            Bind(SDL_Keycode.SDLK_UP, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.CURSOR_DOWN });
            Bind(SDL_Keycode.SDLK_DOWN, new[] { InputKeyEnum.CURSOR_DOWN });
            Bind(SDL_Keycode.SDLK_LEFT, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.CURSOR_RIGHT });
            Bind(SDL_Keycode.SDLK_RIGHT, new[] { InputKeyEnum.CURSOR_RIGHT });

            BindWithShift(SDL_Keycode.SDLK_BACKSPACE, InputKeyEnum.DELETE);
            Bind(SDL_Keycode.SDLK_SPACE, new[] { InputKeyEnum.SPACE });
            Bind(SDL_Keycode.SDLK_RETURN, new[] { InputKeyEnum.RETURN });

            Bind(SDL_Keycode.SDLK_LSHIFT, SDL_Keymod.KMOD_LSHIFT, InputKeyEnum.LSHIFT);
            // When releasing shift, the modifer is gone!
            // Need to bind to this as well or we won't detect when SHIFT was released
            Bind(SDL_Keycode.SDLK_LSHIFT, InputKeyEnum.LSHIFT);
            Bind(SDL_Keycode.SDLK_RSHIFT, SDL_Keymod.KMOD_RSHIFT, InputKeyEnum.RSHIFT);
            Bind(SDL_Keycode.SDLK_RSHIFT, InputKeyEnum.RSHIFT);

            Bind(SDL_Keycode.SDLK_COLON, new[] { InputKeyEnum.COLON });
            Bind(SDL_Keycode.SDLK_SEMICOLON, new[] { InputKeyEnum.SEMICOLON });
            Bind(SDL_Keycode.SDLK_QUOTE, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.D7 });
            Bind(SDL_Keycode.SDLK_QUOTE, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.D2 });
            Bind(SDL_Keycode.SDLK_QUOTE, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.RSHIFT, InputKeyEnum.D2 });

            Bind(SDL_Keycode.SDLK_COMMA, new[] { InputKeyEnum.COMMA });
            Bind(SDL_Keycode.SDLK_PERIOD, new[] { InputKeyEnum.PERIOD });

            BindWithShift(SDL_Keycode.SDLK_HOME, InputKeyEnum.HOME);
            BindWithShift(SDL_Keycode.SDLK_ESCAPE, InputKeyEnum.STOP);
            //_keyMapping.Add(SDL_Keycode.SDLK_ESCAPE, new[] { InputKeyEnum.RESTORE });
            Bind(SDL_Keycode.SDLK_LALT, new[] { InputKeyEnum.CTRL });
            Bind(SDL_Keycode.SDLK_RALT, new[] { InputKeyEnum.CTRL });

            Bind(SDL_Keycode.SDLK_SLASH, new[] { InputKeyEnum.SLASH });
            Bind(SDL_Keycode.SDLK_LEFTBRACKET, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.COLON });
            Bind(SDL_Keycode.SDLK_RIGHTBRACKET, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.SEMICOLON });

            Bind(SDL_Keycode.SDLK_EQUALS, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.PLUS });
            Bind(SDL_Keycode.SDLK_EQUALS, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.PLUS });

            Bind(SDL_Keycode.SDLK_2, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.AT });
            Bind(SDL_Keycode.SDLK_2, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.AT });
            Bind(SDL_Keycode.SDLK_6, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.CARET });
            Bind(SDL_Keycode.SDLK_6, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.CARET });
            Bind(SDL_Keycode.SDLK_7, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.POUND });
            Bind(SDL_Keycode.SDLK_7, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.POUND });
            Bind(SDL_Keycode.SDLK_8, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.ASTERISK });
            Bind(SDL_Keycode.SDLK_8, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.ASTERISK });
            Bind(SDL_Keycode.SDLK_9, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.D8 });
            Bind(SDL_Keycode.SDLK_9, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.RSHIFT, InputKeyEnum.D8 });
            Bind(SDL_Keycode.SDLK_0, SDL_Keymod.KMOD_LSHIFT, new[] { InputKeyEnum.LSHIFT, InputKeyEnum.D9 });
            Bind(SDL_Keycode.SDLK_0, SDL_Keymod.KMOD_RSHIFT, new[] { InputKeyEnum.RSHIFT, InputKeyEnum.D9 });

            Bind(SDL_Keycode.SDLK_EQUALS, new[] { InputKeyEnum.EQUALS });
            Bind(SDL_Keycode.SDLK_MINUS, new[] { InputKeyEnum.MINUS });
            Bind(SDL_Keycode.SDLK_BACKSLASH, new[] { InputKeyEnum.EQUALS });

            BindWithShift(SDL_Keycode.SDLK_1, InputKeyEnum.D1);
            Bind(SDL_Keycode.SDLK_2, InputKeyEnum.D2);
            BindWithShift(SDL_Keycode.SDLK_3, InputKeyEnum.D3);
            BindWithShift(SDL_Keycode.SDLK_4, InputKeyEnum.D4);
            BindWithShift(SDL_Keycode.SDLK_5, InputKeyEnum.D5);
            Bind(SDL_Keycode.SDLK_6, InputKeyEnum.D6);
            Bind(SDL_Keycode.SDLK_7, InputKeyEnum.D7);
            Bind(SDL_Keycode.SDLK_8, InputKeyEnum.D8);
            Bind(SDL_Keycode.SDLK_9, InputKeyEnum.D9);
            Bind(SDL_Keycode.SDLK_0, InputKeyEnum.D0);

            BindWithShift(SDL_Keycode.SDLK_a, InputKeyEnum.A);
            BindWithShift(SDL_Keycode.SDLK_b, InputKeyEnum.B);
            BindWithShift(SDL_Keycode.SDLK_c, InputKeyEnum.C);
            BindWithShift(SDL_Keycode.SDLK_d, InputKeyEnum.D);
            BindWithShift(SDL_Keycode.SDLK_e, InputKeyEnum.E);
            BindWithShift(SDL_Keycode.SDLK_f, InputKeyEnum.F);
            BindWithShift(SDL_Keycode.SDLK_g, InputKeyEnum.G);
            BindWithShift(SDL_Keycode.SDLK_h, InputKeyEnum.H);
            BindWithShift(SDL_Keycode.SDLK_i, InputKeyEnum.I);
            BindWithShift(SDL_Keycode.SDLK_j, InputKeyEnum.J);
            BindWithShift(SDL_Keycode.SDLK_k, InputKeyEnum.K);
            BindWithShift(SDL_Keycode.SDLK_l, InputKeyEnum.L);
            BindWithShift(SDL_Keycode.SDLK_m, InputKeyEnum.M);
            BindWithShift(SDL_Keycode.SDLK_n, InputKeyEnum.N);
            BindWithShift(SDL_Keycode.SDLK_o, InputKeyEnum.O);
            BindWithShift(SDL_Keycode.SDLK_p, InputKeyEnum.P);
            BindWithShift(SDL_Keycode.SDLK_q, InputKeyEnum.Q);
            BindWithShift(SDL_Keycode.SDLK_r, InputKeyEnum.R);
            BindWithShift(SDL_Keycode.SDLK_s, InputKeyEnum.S);
            BindWithShift(SDL_Keycode.SDLK_t, InputKeyEnum.T);
            BindWithShift(SDL_Keycode.SDLK_u, InputKeyEnum.U);
            BindWithShift(SDL_Keycode.SDLK_v, InputKeyEnum.V);
            BindWithShift(SDL_Keycode.SDLK_w, InputKeyEnum.W);
            BindWithShift(SDL_Keycode.SDLK_x, InputKeyEnum.X);
            BindWithShift(SDL_Keycode.SDLK_y, InputKeyEnum.Y);
            BindWithShift(SDL_Keycode.SDLK_z, InputKeyEnum.Z);
        }
    }
}