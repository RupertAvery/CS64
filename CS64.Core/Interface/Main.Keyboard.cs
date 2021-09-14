using System;
using CS64.Core.Interface.Input;

namespace CS64.Core.Interface
{
    public partial class Main
    {
        private readonly KeyboardMatrix _keyboardMatrix = new KeyboardMatrix();
        private bool DeletePressed = false;

        public uint key_col;
        private uint[] rowCols = new uint[8];

        private void SetupKeyboardScanRoutines()
        {
            _c64.Cia1.PortAWrite = PortAWrite;
            _c64.Cia1.PortBRead = PortBRead;
        }

        private uint PortBRead()
        {
            // Initially scan routine pulls all outputs low 
            // (now high since we flipped it)
            if (key_col == 0xFF &&
                // check if anything pressed
                ((rowCols[0] |
                  rowCols[1] |
                  rowCols[2] |
                  rowCols[3] |
                  rowCols[4] |
                  rowCols[5] |
                  rowCols[6] |
                  rowCols[7]) > 0)

                | DeletePressed
            )
            {
                // send any non-zero value to trigger scan routine
                return 0x01;
            }

            if (DeletePressed & key_col == 1)
            {
                return 0xFE;
            }

            // The scan routine will now have a shifting 1 bit (never 0xFF)
            // So we can compare each column test
            if (key_col != 0xFF)
            {
                // convert the column bit into an index
                var index = _keyboardMatrix.ColumnIndex[key_col];
                // invert the row bit because CIA expects the line to be pulled to 0
                return rowCols[index] ^ 0xFF;
            }

            return 0xFF;
        }

        private void PortAWrite(uint value)
        {
            // flip the output so we get a shifting 1 bit instead of 0
            key_col = value ^ 0xFF;
        }

        private void InputEvent(object sender, InputEventArgs args)
        {
            switch (args.EventType)
            {
                case InputEventType.BUTTON_UP:
                    foreach (var key in args.Keys)
                    {
                        Console.Write(key);
                        if (((uint)key & 0x1000) == 0x1000)
                        {
                            switch (key)
                            {
                                case InputKeyEnum.Rewind:
                                    _rewind = false;
                                    break;
                                case InputKeyEnum.FastForward:
                                    _fastForward = false;
                                    break;
                            }
                        }
                        else
                        {
                            if (key == InputKeyEnum.DELETE)
                            {
                                DeletePressed = false;
                            }
                            else
                            {
                                _keyboardMatrix.TryEncode(key, out var rowCol);
                                var index = _keyboardMatrix.ColumnIndex[rowCol.Col];
                                // Clear specific bit only
                                rowCols[index] &= rowCol.Row ^ 0xFF;
                            }
                        }
                    }
                    Console.WriteLine();

                    break;
                case InputEventType.BUTTON_DOWN:

                    foreach (var key in args.Keys)
                    {
                        if (((uint)key & 0x1000) == 0x1000)
                        {
                            switch (key)
                            {
                                case InputKeyEnum.Rewind:
                                    _rewind = true;
                                    break;
                                case InputKeyEnum.FastForward:
                                    _fastForward = true;
                                    break;
                                case InputKeyEnum.SaveState:
                                    _saveStatePending = true;
                                    break;
                                case InputKeyEnum.LoadState:
                                    _loadStatePending = true;
                                    break;
                                case InputKeyEnum.NextState:
                                    break;
                                case InputKeyEnum.PreviousState:
                                    break;
                            }
                        }
                        else
                        {
                            // For some reason DELETE key doesn't work properly
                            // so, force it to work
                            if (key == InputKeyEnum.DELETE)
                            {
                                DeletePressed = true;
                            }
                            else
                            {
                                _keyboardMatrix.TryEncode(key, out var rowCol);
                                var index = _keyboardMatrix.ColumnIndex[rowCol.Col];
                                // Setr specific bit without erasing any other bits
                                rowCols[index] |= rowCol.Row;
                            }
                        }
                    }

                    break;
            }
        }
    }
}