using System.Collections.Generic;
using CS64.Core.Interface.Input;

namespace CS64.Core.Interface
{
    public class KeyboardMatrix
    {
        private static InputKeyEnum[,] _lookup = new InputKeyEnum[,]
        {
            {
                InputKeyEnum.STOP, InputKeyEnum.Q, InputKeyEnum.COMMODORE, InputKeyEnum.SPACE, InputKeyEnum.D2,
                InputKeyEnum.CTRL, InputKeyEnum.BACKSPACE, InputKeyEnum.D1
            },
            {
                InputKeyEnum.SLASH, InputKeyEnum.CARET, InputKeyEnum.EQUALS, InputKeyEnum.RSHIFT, InputKeyEnum.HOME,
                InputKeyEnum.SEMICOLON, InputKeyEnum.ASTERISK, InputKeyEnum.POUND
            },
            {
                InputKeyEnum.COMMA, InputKeyEnum.AT, InputKeyEnum.COLON, InputKeyEnum.DECIMAL, InputKeyEnum.MINUS,
                InputKeyEnum.L, InputKeyEnum.P, InputKeyEnum.PLUS
            },
            {
                InputKeyEnum.N, InputKeyEnum.O, InputKeyEnum.K, InputKeyEnum.M, InputKeyEnum.D0, InputKeyEnum.J,
                InputKeyEnum.I, InputKeyEnum.D9
            },
            {
                InputKeyEnum.V, InputKeyEnum.U, InputKeyEnum.H, InputKeyEnum.B, InputKeyEnum.D8,
                InputKeyEnum.G, InputKeyEnum.Y, InputKeyEnum.D7
            },
            {
                InputKeyEnum.X, InputKeyEnum.T, InputKeyEnum.F, InputKeyEnum.C, InputKeyEnum.D6,
                InputKeyEnum.D, InputKeyEnum.R, InputKeyEnum.D5
            },
            {
                InputKeyEnum.LSHIFT, InputKeyEnum.E, InputKeyEnum.S, InputKeyEnum.Z, InputKeyEnum.D4,
                InputKeyEnum.A, InputKeyEnum.W, InputKeyEnum.D3
            },
            {
                InputKeyEnum.CURSOR_DOWN, InputKeyEnum.F5, InputKeyEnum.F3, InputKeyEnum.F1, InputKeyEnum.F7,
                InputKeyEnum.CURSOR_RIGHT, InputKeyEnum.RETURN, InputKeyEnum.DELETE
            },
        };

        public Dictionary<InputKeyEnum, RowCol> _matrix = new Dictionary<InputKeyEnum, RowCol>();
        public uint[] ColumnIndex = new uint[129];

        public KeyboardMatrix()
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    var key = _lookup[col, row];

                    _matrix.Add(key, new RowCol((uint)(1 << (7 - col)), (uint)(1 << (7 - row))));
                }
            }

            for (var i = 0; i < 8; i++)
            {
                ColumnIndex[1 << i] = (uint)i;
            }
        }

        public bool TryEncode(InputKeyEnum key, out RowCol rowCol)
        {
            return _matrix.TryGetValue(key, out rowCol);
        }

    }
}