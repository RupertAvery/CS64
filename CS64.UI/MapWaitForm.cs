using System;
using System.Threading;
using System.Windows.Forms;
using CS64.Core.Interface.Input;
using SDL2;

namespace CS64.UI
{
    public partial class MapWaitForm : Form
    {
        private readonly InputProvider _inputProvider;
        private readonly InputKeyEnum _key;
        private System.Threading.Timer timer;
        private int countDown;
        public bool Success { get; set; }

        public MapWaitForm(InputProvider inputProvider, InputKeyEnum key)
        {
            _inputProvider = inputProvider;
            _key = key;
            Closing += (sender, args) =>
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            };

            InitializeComponent();
        }

        private void MapWaitForm_Load(object sender, EventArgs e)
        {
            timer = new System.Threading.Timer(Callback, null, 0, 1000);
            countDown = 5;
        }



        private void UpdateText()
        {
            label1.Text = $"Press a button  or key ({countDown})";
        }

        private void Callback(object? state)
        {
            if (countDown == 0)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                this.InvokeIfRequired(Close);
            }
            label1.InvokeIfRequired(UpdateText);
            countDown--;
        }

        private void MapWaitForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(!KeyCodeMappings.TryGetKeyCode(e.KeyCode, out var code))
            {
                code = (SDL.SDL_Keycode) e.KeyCode;
            }
            var keyEvent = new SDL.SDL_KeyboardEvent()
            {
                type = SDL.SDL_EventType.SDL_KEYDOWN,
                keysym = new SDL.SDL_Keysym()
                {
                    sym = code
                }
            };
            _inputProvider.HandleEvent(keyEvent);
            Success = true;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Close();
        }
    }
}
