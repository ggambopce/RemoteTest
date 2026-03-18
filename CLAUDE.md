# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build

This is a C# Windows Forms application targeting .NET Framework 4.7.2. Open `remotetest.sln` in Visual Studio 2017+ and build with **Ctrl+Shift+B**, or from the command line:

```bash
msbuild remotetest.sln /p:Configuration=Debug
msbuild remotetest.sln /p:Configuration=Release
```

Output lands in `remotetest/bin/Debug/` or `remotetest/bin/Release/`. There are no tests and no linter configuration.

## Architecture

Remote desktop control application (Windows-only, WinExe). The project is currently a skeleton — most business logic classes are stubs awaiting implementation.

**Communication model:** Event-driven client-server split.
- Server side: captures screen (`ImageServer`), receives keyboard/mouse events (`RecvEventServer`), applies them via `Remote`/`Controller`
- Client side: receives and displays screen images (`ImageClient`, `RemoteClientForm`), captures and sends input events (`SendEventClient`)
- Setup: `SetupServer` / `SetupClient` handle initialization on each side

**UI Forms:**
- `MainForm` — application entry point UI (480×217), root form launched by `Program.cs`
- `RemoteClientForm` — displays remote screen (800×450)
- `VirtualCursorForm` — overlays virtual cursor on remote screen (800×450)

**Event system:** Custom event args and delegates in `RecvImageEventArgs`, `RecvKMEEventArgs`, `RecvRCInfEventArgs` carry image frames, keyboard/mouse events, and remote control info respectively.

**`Meta.cs`** defines shared enums: `KeyFlag`, `MouseFlag`, `MsgType` — used for encoding protocol messages between client and server.

Comments in the source are written in Korean.
