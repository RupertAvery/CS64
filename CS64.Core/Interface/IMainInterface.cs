using System;
using CS64.Core.Interface.Input;

namespace CS64.Core.Interface
{
    public interface IMainInterface
    {

        IntPtr Handle { get;  }
        Action OnHostResize { get; set; }
        Func<string, bool> LoadRom { get; set; }
        Action<int> ChangeSlot { get; set; }
        Action LoadState { get; set; }
        Action SaveState { get; set; }
        Action<int, int> ResizeWindow { get; set; }
        Action<InputKeyEnum> SetMapping { get; set; }
    }
}