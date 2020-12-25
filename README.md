# FSMosquito

Desktop App that provides real-time interactivity to FS2020 to view live data from Flight Sim via the app on a second monitor, separate computer or mobile device.

![FsMosquito Moving Map](/fsmosquito/docs/images/2020-12-25 13_31_40-FSMosquito.png?raw=true "FsMosquito Moving Map")

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

- Download a release
- Install
- launch FsMosquito Desktop (Best on a second monitor)
- Start Flight Simulator

## Accessing from Mobile/Other computers and devices

The Web-based interface runs on localhost:5272 by default, thus, if you open this port in Windows Firewall, other devices on your network can connect to the name/ip of the machine and view the same interface surfaced in FsMosquito Desktop.

### Development:

There are two general areas of development on FsMosquito:

 - The back end - which includes the SimConnect interface, backing data persistence APIs, the MQTT Implementation and so forth.
 - The front end - user interface, maps, gadgets, so forth.

Most of the development work is in the front end - providing functionality on top of data coming across the MQTT service bus and making it pretty and functional.
