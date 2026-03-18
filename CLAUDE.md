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

**Two sides of the connection:**
- **Remote (host being controlled):** `Remote` singleton starts `SetupServer` (port 20002) to accept incoming control requests, then starts `RecvEventServer` (port 20010) to receive keyboard/mouse events and apply them via `WrapNative` (Win32 input injection, not in this repo).
- **Controller (client doing the controlling):** `Controller` singleton starts `ImageServer` (port 20004) to receive screen frames, then calls `SetupClient.Setup()` to notify the remote. After setup, `SendEventClient` sends input events to port 20010.

**Counterintuitive naming:** "ImageServer" is the *receiver* of images (listens on the controller side); "ImageClient" is the *sender* of images (runs on the remote side). The remote captures the screen and pushes JPEG frames to the controller's ImageServer.

**Wire protocol — event messages:** Fixed 9-byte binary format.
- Byte 0: `MsgType` enum (key down/up, mouse buttons, mouse move)
- Bytes 1–4: key code as little-endian int32 (key events), or X coordinate (mouse move)
- Bytes 5–8: Y coordinate (mouse move only)
- Decoded by `Meta` constructor from `byte[]`

**Wire protocol — image messages:** 4-byte little-endian int32 length prefix followed by raw JPEG bytes. Each image uses a fresh TCP connection (connect → send → close).

**Ports** (defined in `NetworkInfo`): Setup=20002, Image=20004, Event=20010.

**Namespace split:** `SetupClient` is in the `원격제어기` namespace; everything else is in `remotetest`. `Controller.cs` references it with `using 원격제어기;`.

**Unresolved dependencies in `Remote.cs`:** References `AutomationElement` (from `UIAutomationClient.dll` / `System.Windows.Automation`) to get the desktop bounding rect, and `WrapNative` (a Win32 `SendInput` wrapper not present in this repo).

**UI Forms:**
- `MainForm` — application entry point UI (480×217), root form launched by `Program.cs`
- `RemoteClientForm` — displays remote screen (800×450)
- `VirtualCursorForm` — overlays virtual cursor on remote screen (800×450)

**`MsgType` enum** is defined in `SendEventClient.cs` (not `Meta.cs`). `Meta.cs` contains the `Meta` class that decodes raw bytes into typed event data.

Comments in the source are written in Korean.
