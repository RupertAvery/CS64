﻿namespace CS64.Core.Interface.Input
{
    public enum InputKeyEnum
    {
        // Add offset for controller buttons to make things easier
        BACKSPACE,
        D1,
        D2, 
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        D0,
        PLUS,
        MINUS,
        POUND,
        HOME,
        DELETE,
        CTRL,
        AT,
        ASTERISK,
        CARET,
        RESTORE,
        STOP,
        COLON,
        SEMICOLON,
        EQUALS,
        RETURN,
        COMMODORE,
        LSHIFT,
        COMMA,
        PERIOD,
        SLASH,
        RSHIFT,
        CURSOR_DOWN,
        CURSOR_RIGHT,
        F1,
        F3,
        F5,
        F7,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        SPACE,
        // Standard buttons go here
        Rewind = 0x1001,
        FastForward = 0x1002,
        SaveState = 0x1003,
        LoadState = 0x1004,
        NextState = 0x1005,
        PreviousState = 0x1006,
    }
}