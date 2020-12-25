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
 - Publish as a windows store app/auto-updates

Currently built as an [Electron .Net](https://github.com/ElectronNET/Electron.NET) based app backed by .Net 5.0 built as a ready-to-run app and released through Github Actions.

A [WebView2](https://docs.microsoft.com/en-us/microsoft-edge/webview2/) based app or conversion is planned. At least though about ;)
An annoyance right now is that since WebView2 isn't distributed as part of a release and folks will have to download it anyway and have a separate installation step, so, other than .net jingoism, might as well stick with tried and true electron.

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


## FSMosquito MQTT Client

Also included in this repository is a small application that transmits SimConnect datum to [any MQTT Broker](https://en.wikipedia.org/wiki/Comparison_of_MQTT_implementations) including [Azure IoT Hub](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-mqtt-support) and is the evolution of the FsMosquitoClient repository located [here](https://github.com/fsmosquito/fsmosquitoclient)

This client is also built and installed and used along with the FsMosquito Desktop App. It's located within ```C:\Users\<username>\AppData\Local\Programs\fsmosquito-desktop\resources\bin```

If you wish to just transmit SimConnect variables to any MQTT broker, you can run this app independently of FsMosquito Desktop.

Configuration:

edit ```C:\Users\<username>\AppData\Local\Programs\fsmosquito-desktop\resources\bin\appsettings.json``` in a text editor and add any additional SimConnect variables to subscribe to and transmit. By default, only the variables used by the FsMosquito desktop app are included and may change over time.

A full listing of the available variables is installed as a [help file](file:///C:/MSFS%20SDK/Documentation/04-Developer_Tools/SimConnect/SimConnect_Status_of_Simulation_Variables.html) along with the MS2020 SDK (```C:/MSFS SDK/Documentation/04-Developer_Tools/SimConnect/SimConnect_Status_of_Simulation_Variables.html```)

### Development:

There are two general areas of development on FsMosquito:

 - The back end - MQTT service, data persistence, gathering data from FS2020, so forth.
 - The front end - user interface, maps, gadgets, so forth.

Most of the development work is in the front end - providing functionality on top of data coming across the MQTT service bus and making it pretty and functional. The frontend is built using NextJS.


The backend is build using .Net 5 based components. It self-hosts a MQTT Broker, using MQTTNet, an ASP.Net 5 API layer described by swashbuckle, LiteDB for data peristence and a custom-built wrapper around the Microsoft-provided SimConnect API that provides enhanced reliability and publishes information coming from FS2020, such as Lat/Long/Alt/Heading/Airspeed and so forth, to MQTT subscribers.

What brings the Frontend and Backend together into the desktop app is Electron.Net. This wraps the Frontend code and hoists the backend code to provide the full experience. A Github Action builds the electron app and publishes it as a ready-to-run app.

### Getting Started

As the usual use-case is to improve the frontend, the steps to do so are the following:

1. Clone the repository.
2. Start the FsMosquito Desktop using an existing release, or build it using the steps in ./fsmosquito-desktop/README.md. The backend can be run directly through visual studio by opening the solution and starting the FsMosquito desktop app - this will only launch the ASP.Net backend.
3. Start the front end through ./fsmosquito-app, running yarn and yarn dev in that folder. It will connect to the running backend.
4. Make changes and commit.

### Why is it called FsMosquito?

Like a mosquito, it siphons data from Flight Simulator 2020 via SimConnect. Also, there's a famous MQTT broker called [Mosquitto](https://mosquitto.org/) and the origination of this project was to play around with MQTT a bit so it fits.
