Space-Tapper
============

Currently a learning experiment centered around SFML and C#. Only tested on Windows and Linux.
Slightly unstable.

A and D currently move your player (the green square) left and right, respectively.
Holding down space will increase your vertical velocity.

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
