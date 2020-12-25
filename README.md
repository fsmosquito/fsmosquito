# FSMosquito

Provides real-time monitoring and interactivity to FS2020 via a locally running desktop application. Best on a second monitor, separate computer, or mobile device.

![FsMosquito Moving Map](https://raw.githubusercontent.com/fsmosquito/fsmosquito/master/docs/images/2020-12-25%2013_31_40-FSMosquito.png "FsMosquito Moving Map")

> FsMosquito is under development and currently is in alpha.

Features:
 - Moving map.

Need to reimplement:
 - Gauges.
 - Provide a customizable dashboard
 - View and control lights and various components of the aircraft.
 - Save previous flights and view breadcrumb trail.
 - Designate and forward messages to an upstream MQTT Server (bridge implementation)
 - other stuff...

Roadmap:
 - Virtual Pilot
 - Missions/Career mode.
 - Connect to a community server to allow for economy-based goals - leaderboards, competitions, virtual FBOs, etc... 
 - auto-updates

Currently built as an [Electron .Net](https://github.com/ElectronNET/Electron.NET) based app backed by .Net 5.0.

A [WebView2](https://docs.microsoft.com/en-us/microsoft-edge/webview2/) based app or conversion is planned. At least though about ;)
An annoyance right now is that since WebView2 isn't distributed as part of a release and folks will have to download it anyway and have a separate installation step, so,
other than .net jingoism, might as well stick with tried and true electron.

## FSMosquito MQTT Client

Also included in this repository is a small MQTT app that transmits SimConnect datum to any MQTT server. see /fsmosquito-client.

## Getting started

This is for Flight Simulator 2020 users, so you'll need to have FS2020 already. This desktop app will need to run on the same Win64-based machine that FS is running on.

- Download a release from the [releases page](https://github.com/fsmosquito/fsmosquito/releases)
- Install using the installer
- Launch FsMosquito Desktop (Best on a second monitor)
- Start Flight Simulator

Your coordinates will appear on the first screen (which will be the dashboard) click on the "Moving Map" in the title

## Accessing from Mobile/Other computers and devices

The Web-based interface runs on localhost:5272 by default, thus, if you open this port in Windows Firewall, other devices on your network can connect to the name/ip of the machine and view the same interface surfaced in FsMosquito Desktop.

TODO: Put the current computer name and port in the title bar for easy reference

### Development:

There are two general areas of development on FsMosquito:

 - The back end - which includes the SimConnect interface, backing data persistence APIs, the MQTT Implementation and so forth.
 - The front end - user interface, maps, gadgets, so forth.

Most of the development work is in the front end - providing functionality on top of data coming across the MQTT service bus and making it pretty and functional.
