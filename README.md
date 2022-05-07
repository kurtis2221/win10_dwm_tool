# Windows 10 DWM tool
## Info
User friendly and oversimplified tool to enable and disable DWM.

Tested with Windows 10 21H2.
## Warning
- Using this tool may lock up your system and need to hard restart!
- Save everyting important before using!
- Use this tool at your own risk!
- Has experimental settings for Windows 11. Working, but not tested on Windows 11!
## Usage
### Normal usage:
1. Before running it's recommended to open an alternative file manager or a game
2. Run the program, click Disable DWM
3. If you need to start something (taskmanager, filemanager, etc.) type in the program's name and click on Run
4. If you can click the Yes button then you have successfully disabled DWM
5. If you get a black screen, the program will try to restore everything in 5 seconds
6. Click Enable DWM when you are finished
### Hotkey usage:
1. To disable DWM press: CTRL + ALT + SHIFT + PAGE DOWN
2. If you have the dwm_off.wav file it should make a sound
3. To enable DWM press: CTRL + ALT + SHIFT + PAGE UP
4. If you have the dwm_on.wav file it should make a sound
## Useful info
- Do not try to close anything other than this program while DWM is disabled, it will remain in the process list and may cause a black screen.
- You might need to add more processes to the win10_dwm_tool.ini file if you get a black screen after disabling DWM.
- Close UWP apps before disabling DWM! Otherwise you will experience graphics glitches.
- If somehow get a black screen while playing without DWM just try pressing ALT+TAB and ALT+F4 these keys still work!
- If you get lucky and close this program everything will be restored to normal.
- Some games may not work while DWM is disabled. UWP apps can't work without it!
## Files
- win10_dwm_tool.ini - Contains process names that need to be disabled. New entries can be added if you get a black screen.
- win10_dwm_tool_hk.ini - Contains ON and OFF hotkeys. If it can't be loaded, it will disable hotkeys.
- win11_dwm_tool.ini - Contains 2 checkbox settings for Windows 11.
- dwm_off.wav - DWM hotkey sound for tuning it off. If it can't load the file, no sound will be made when pressing the hotkey.
- dwm_on.wav - DWM hotkey sound for tuning it on. If it can't load the file, no sound will be made when pressing the hotkey.

## Sources
https://github.com/Ingan121/DWMKiller

https://www.youtube.com/watch?v=cutsuVbvork

https://www.codeproject.com/Articles/19004/A-Simple-C-Global-Low-Level-Keyboard-Hook

https://github.com/Ingan121/DWMSwitch

https://www.nuget.org/packages/TaskScheduler/
