# Tabbed MDI Window Manager For WinForms (.NET9)
**\*\*\*NOTE: This project is a .NET9 port of a legacy .NET Framework 3.5 project located [here](https://github.com/cmoya212/TabbedMDIWinMan). It contains compatibility fixes, improvements, and WPF support!\*\*\***
## Overview
MDIWindowManager is a control that can be added to your MDI Parent forms to achieve the Tabbed Document Interface (TDI) seen in many modern applications.
- No code changes required. Just plop the control on your MDI parent and everything is taken care at runtime. There is no complex "framework" locking you into a particular technology... windows do not need to derive from some base class and no code changes need to be done on either the child windows or in your MDI child loading code.
- Built-in support for various window layouts, Tile Vertically, Tile Horizontally, Pop out to desktop... all while keeping the aesthetic tabbed interface (no title bar on child windows, no overlapping windows, etc).
- Supports CTRL+TAB forward and reverse window switching.
- Supports advanced 2 and 3-pane views without forcing the user to implement their windows as UserControls.
- Supports minimizing the window manager (in multi-pane scenarios) in a novel way while still keeping tabbed aesthetics.
- Built-in "Windows" menu that can be cloned and added to your parent form. Furthermore, since MDI Children are still MDI Children under the scenes, there is full support for menu merging.
## Documentation
- **[Documentation](https://1drv.ms/w/s!AtUuwRmVvmd4g75JxWunqL6y38xnNQ?e=xM7Iac)**
- **[Object Model](http://riverfront.solutions/cmoya/cflashSoft/progs/mdiwinman/Doc/)**
### Source Code
[https://github.com/cmoya212/MDIWindowManager2](https://github.com/cmoya212/MDIWindowManager2)
