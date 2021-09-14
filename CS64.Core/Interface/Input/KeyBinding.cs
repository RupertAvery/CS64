using static SDL2.SDL;

namespace CS64.Core.Interface.Input
{
    public struct KeyBinding
    {
        public KeyBinding(SDL_Keycode keyCode)
        {
            KeyCode = keyCode;
            Modifier = SDL_Keymod.KMOD_NONE;
        }

        public KeyBinding(SDL_Keycode keyCode, SDL_Keymod modifier)
        {
            KeyCode = keyCode;
            // My keyboard is stuck on NUM
            modifier &= (SDL_Keymod)((int)(SDL_Keymod.KMOD_NUM) ^ 0xFFFF);
            modifier &= (SDL_Keymod)((int)(SDL_Keymod.KMOD_CAPS) ^ 0xFFFF);
            Modifier = modifier;
        }

        public SDL_Keycode KeyCode { get; set; }
        public SDL_Keymod Modifier { get; set; }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   KeyCode == ((KeyBinding)obj).KeyCode &&
                   Modifier == ((KeyBinding)obj).Modifier;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 7199369;
                // Suitable nullity checks etc, of course :)
                hash = hash * 486187739 + KeyCode.GetHashCode();
                hash = hash * 486187739 + Modifier.GetHashCode();
                return hash;
            }

            //return HashCode.Combine(KeyCode, Modifier);
        }
    }
}