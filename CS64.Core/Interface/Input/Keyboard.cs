using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace CS64.Core.Interface.Input
{
    public abstract class Keyboard
    {

        protected readonly Dictionary<KeyBinding, InputKeyEnum[]> _keyMapping = new Dictionary<KeyBinding, InputKeyEnum[]>();

        protected Keyboard()
        {
            SetupBinding();
        }

        public abstract void SetupBinding();

        public bool TryMap(SDL_Keycode keyCode, SDL_Keymod modifier, out InputKeyEnum[] mappedInputs)
        {
            return _keyMapping.TryGetValue(new KeyBinding(keyCode, modifier), out mappedInputs);
        }

        public bool TrySetMap(SDL_Keycode keyCode, SDL_Keymod modifier, InputKeyEnum[] target)
        {
            if (TryGetMap(target, out var oldkey))
            {
                _keyMapping.Remove(oldkey.Value);
            }
            _keyMapping[new KeyBinding(keyCode, modifier)] = target;
            return true;
        }

        public bool TryGetMap(InputKeyEnum[] key, out KeyBinding? mappedKey)
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

        protected void BindWithShift(SDL_Keycode keyCode, InputKeyEnum key)
        {
            _keyMapping.Add(new KeyBinding(keyCode), new[] { key });
            _keyMapping.Add(new KeyBinding(keyCode, SDL_Keymod.KMOD_LSHIFT), new[] { InputKeyEnum.LSHIFT, key });
            _keyMapping.Add(new KeyBinding(keyCode, SDL_Keymod.KMOD_RSHIFT), new[] { InputKeyEnum.RSHIFT, key });
            _keyMapping.Add(new KeyBinding(keyCode, SDL_Keymod.KMOD_ALT), new[] { InputKeyEnum.CTRL, key });
        }

        protected void Bind(SDL_Keycode keyCode, InputKeyEnum key)
        {
            _keyMapping.Add(new KeyBinding(keyCode), new[] { key });
        }

        protected void Bind(SDL_Keycode keyCode, InputKeyEnum[] keys)
        {
            _keyMapping.Add(new KeyBinding(keyCode), keys);
        }

        protected void Bind(SDL_Keycode keyCode, SDL_Keymod modifer, InputKeyEnum[] keys)
        {
            _keyMapping.Add(new KeyBinding(keyCode, modifer), keys);
        }

        protected void Bind(SDL_Keycode keyCode, SDL_Keymod modifer, InputKeyEnum key)
        {
            _keyMapping.Add(new KeyBinding(keyCode, modifer), new []{key});
        }

    }
}