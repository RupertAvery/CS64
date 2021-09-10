using System;
using CS64.Core.Interface;
using CS64.UI;

namespace CS64
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var main = new Main())
            {

                using (var form = new MainForm(main))
                {
                    form.Show();
                    
                    main.Initialize(form);
                    main.LoadROM("rom\\characters.bin", 0xD000, 0x1000);
                    main.LoadROM("rom\\basic.bin", 0xA000, 0x2000);
                    main.LoadROM("rom\\kernal.bin", 0xE000, 0x2000);

                    string rom;
                    //main.Test();
                    ////main.Load(args[0]);
                    //if (args.Length > 0)
                    //{
                    //    main.Load(args[0]);
                    //}
                    main.Reset();
                    main.Run();
                }

            }


        }
    }
}