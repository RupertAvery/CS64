# CS64 - Commodore 64 Emulator

**CS64** is a Commodore 64 emulator written in C#. 

This is a work in progress and only the most basic functionality is working. Currently the emulator will boot into BASIC 
and you should be able to type in programs and run them.

# Prerequisites

* .NET 5 SDK
* Visual Studio 2019 v16+

# Todo

* Fix keyboard mappings and multiple-presses (LSHIFT)
* Fix video display (remove excessive borders)
* Graphics modes and sprites
* Fix interrupts
* Test timers
* States
* Rewind
* Light Pen
* Tape I/O
* Disk I/O
* SID (Probably not, might use another emulator's SID implementation, or reSID)

# Key mapping

|  Key    |  Function    |
|---------|--------------|
|  F10    |  Reset       |


# Known Issues

The keyboard interrupt seems to run a bit too fast, resulting in a fast blinking cursor and space bar moving too much when 
pressed. Delete also doesn't work, except for few times it does. Key mapping isn't complete and the Left Shift seems to 
block the first two rows of keys. 
