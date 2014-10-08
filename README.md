Space-Tapper
============

A learning experiment centered around SFML and C#. Only tested on Arch Linux and Windows 7.
On Linux, you will need to install the latest version of CSFML.

At the moment the rewrite is still in its early stages.


There are various commands you can pass to the executable:
```
-w|width :      Specifies the width of the window.
-h|height:      Specifies the height of the window.
-v|vsync:       Specifies if vsync should be enabled.
-f|fullscreen:  Specifies if fullscreen should be enabled.
-a|autosize:    When called, it will set the window size to match the desktop. Must be placed after any -w|width or -h|height calls.
-file|parse:    Specifies a file to parse for commands.
-log:           Specifies a log file path. By default, no log is written.
```

Example usage:
```
./SpaceTapper.exe -w 1024 -h 768 -fullscreen -autosize -vsync true -log ./log.txt
```
