using System;
using System.Windows.Forms;
using CS64.Core.Interface;
using CS64.Core.Interface.Input;

namespace CS64.UI
{
    public partial class MappingForm : Form
    {
        private readonly Main _main;

        public MappingForm(Main main)
        {
            _main = main;
            InitializeComponent();
        }
        
        private void ShowMapping(InputKeyEnum key)
        {
            _main.SetMapping(key);
            var mapWait = new MapWaitForm(_main.InputProvider, key);
            mapWait.Top = Top + (Height - mapWait.Height) / 2;
            mapWait.Left = Left + (Width - mapWait.Width) / 2;
            mapWait.ShowDialog(this);
        }

        //private void GetMapping(InputKeyEnum key, TextBox textbox)
        //{
        //    textbox.Text = _main.InputProvider.GetMapping(key);
        //}

        private void MappingForm_Load(object sender, EventArgs e)
        {
            //GetMapping(InputKeyEnum.Up, textBoxUp);
            //GetMapping(InputKeyEnum.Down, textBoxDown);
            //GetMapping(InputKeyEnum.Left, textBoxLeft);
            //GetMapping(InputKeyEnum.Right, textBoxRight);
            //GetMapping(InputKeyEnum.B, textBoxB);
            //GetMapping(InputKeyEnum.A, textBoxA);
            //GetMapping(InputKeyEnum.Select, textBoxSelect);
            //GetMapping(InputKeyEnum.Start, textBoxStart);


            //buttonUp.Click += (o, e) => { ShowMapping(InputKeyEnum.Up); GetMapping(InputKeyEnum.Up, textBoxUp); };
            //buttonDown.Click += (o, e) => { ShowMapping(InputKeyEnum.Down); GetMapping(InputKeyEnum.Down, textBoxDown); };
            //buttonLeft.Click += (o, e) => { ShowMapping(InputKeyEnum.Left); GetMapping(InputKeyEnum.Left, textBoxLeft); };
            //buttonRight.Click += (o, e) => { ShowMapping(InputKeyEnum.Right); GetMapping(InputKeyEnum.Right, textBoxRight); };
            //buttonB.Click += (o, e) => { ShowMapping(InputKeyEnum.B); GetMapping(InputKeyEnum.B, textBoxB); };
            //buttonA.Click += (o, e) => { ShowMapping(InputKeyEnum.A); GetMapping(InputKeyEnum.A, textBoxA); };
            //buttonSelect.Click += (o, e) => { ShowMapping(InputKeyEnum.Select); GetMapping(InputKeyEnum.Select, textBoxSelect); };
            //buttonStart.Click += (o, e) => { ShowMapping(InputKeyEnum.Start); GetMapping(InputKeyEnum.Start, textBoxStart); };
        }
    }
}
