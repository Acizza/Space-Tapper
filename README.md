Space-Tapper
============

Currently a learning experiment centered around SFML and C#. Only tested on Arch Linux and Windows 7.

A and D currently move the player (the green square) left and right, respectively.
Holding down space will increase the player's vertical velocity.

Various upgrades are currently implemented as yellow squares scattered near blocks that can be activated by moving into them. Getting the "Slow Down" upgrade will allow you to use S to decrease your vertical velocity for a short period of time to try and get out of uncomfortable encounters with red blocks.

You can create a file named settings.cfg in the output directory to use persistent settings, instead of launching the executable with them every time. A typical settings.cfg file will look like this:

```
-vsync = true
-width = 1024
-height = 768
-fullscreen = true
-autosize
```

In the example above, the -autosize setting will make sure the window is the size of the desktop.
To get a list of available settings, launch the executable with -help in a terminal.
