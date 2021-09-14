using static SDL2.SDL;

namespace CS64.Core.Interface.Input
{
    public class C64Keyboard : Keyboard
    {
        public override void SetupBinding()
        {
            BindWithShift(SDL_Keycode.SDLK_UP, InputKeyEnum.CURSOR_DOWN);
            BindWithShift(SDL_Keycode.SDLK_DOWN, InputKeyEnum.CURSOR_DOWN);
            BindWithShift(SDL_Keycode.SDLK_LEFT, InputKeyEnum.CURSOR_RIGHT);
            BindWithShift(SDL_Keycode.SDLK_RIGHT, InputKeyEnum.CURSOR_RIGHT);

            BindWithShift(SDL_Keycode.SDLK_DELETE, InputKeyEnum.DELETE);
            BindWithShift(SDL_Keycode.SDLK_BACKSPACE, InputKeyEnum.BACKSPACE);
            BindWithShift(SDL_Keycode.SDLK_SPACE, InputKeyEnum.SPACE);
            BindWithShift(SDL_Keycode.SDLK_RETURN, InputKeyEnum.RETURN);

            Bind(SDL_Keycode.SDLK_LSHIFT, SDL_Keymod.KMOD_LSHIFT, InputKeyEnum.LSHIFT);
            // When releasing shift, the modifer is gone!
            // Need to bind to this as well or we won't detect when SHIFT was released
            Bind(SDL_Keycode.SDLK_LSHIFT, InputKeyEnum.LSHIFT);
            
            Bind(SDL_Keycode.SDLK_RSHIFT, SDL_Keymod.KMOD_RSHIFT, InputKeyEnum.RSHIFT);
            Bind(SDL_Keycode.SDLK_RSHIFT, InputKeyEnum.RSHIFT);

            BindWithShift(SDL_Keycode.SDLK_SEMICOLON, InputKeyEnum.COLON);
            BindWithShift(SDL_Keycode.SDLK_QUOTE, InputKeyEnum.SEMICOLON);

            BindWithShift(SDL_Keycode.SDLK_COMMA, InputKeyEnum.COMMA);
            BindWithShift(SDL_Keycode.SDLK_PERIOD, InputKeyEnum.PERIOD);

            BindWithShift(SDL_Keycode.SDLK_HOME, InputKeyEnum.HOME);
            BindWithShift(SDL_Keycode.SDLK_END, InputKeyEnum.STOP);
            BindWithShift(SDL_Keycode.SDLK_ESCAPE, InputKeyEnum.RESTORE);
            BindWithShift(SDL_Keycode.SDLK_LALT, InputKeyEnum.CTRL);
            BindWithShift(SDL_Keycode.SDLK_RALT, InputKeyEnum.CTRL);

            BindWithShift(SDL_Keycode.SDLK_SLASH, InputKeyEnum.SLASH);
            BindWithShift(SDL_Keycode.SDLK_LEFTBRACKET, InputKeyEnum.AT);
            BindWithShift(SDL_Keycode.SDLK_RIGHTBRACKET, InputKeyEnum.ASTERISK);

            //TODO: CARET (uparrow)
            //_keyMapping.Add(SDL_Keycode.SDLK_BACKSLASH, InputKeyEnum.CARET);
            BindWithShift(SDL_Keycode.SDLK_EQUALS, InputKeyEnum.PLUS);
            BindWithShift(SDL_Keycode.SDLK_MINUS, InputKeyEnum.MINUS);
            BindWithShift(SDL_Keycode.SDLK_BACKSLASH, InputKeyEnum.EQUALS);

            BindWithShift(SDL_Keycode.SDLK_0, InputKeyEnum.D0);
            BindWithShift(SDL_Keycode.SDLK_1, InputKeyEnum.D1);
            BindWithShift(SDL_Keycode.SDLK_2, InputKeyEnum.D2);
            BindWithShift(SDL_Keycode.SDLK_3, InputKeyEnum.D3);
            BindWithShift(SDL_Keycode.SDLK_4, InputKeyEnum.D4);
            BindWithShift(SDL_Keycode.SDLK_5, InputKeyEnum.D5);
            BindWithShift(SDL_Keycode.SDLK_6, InputKeyEnum.D6);
            BindWithShift(SDL_Keycode.SDLK_7, InputKeyEnum.D7);
            BindWithShift(SDL_Keycode.SDLK_8, InputKeyEnum.D8);
            BindWithShift(SDL_Keycode.SDLK_9, InputKeyEnum.D9);

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