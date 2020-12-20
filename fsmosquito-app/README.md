FSMosquito Client App
---

This is the NextJS application that is hosted by the Electron .Net application.

First, build FSMosquito Desktop by running

```
cd .\fsmosquito-desktop
electronize build /target win
```

at the root of the repository.

Then, start the NextJS Development server with ```yarn dev```

Now, to run the app in dev mode while pointing at the development server, create a new terminal window and run at the root of the repo

``` Powershell
$env:ASPNETCORE_ENVIRONMENT="Development" # Proxies the hosted site to localhost:3000 - but we lose the ability to interact with electron.
$env:FSMOSQUITO_ENVIRONMENT="Development" # Enables the tray icon option to show development tools
.\fsmosquito-desktop\bin\Desktop\win-unpacked\fsmosquito-desktop.exe
```