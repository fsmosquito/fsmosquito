import React, { useEffect, useRef } from 'react';

import Head from 'next/head';

import { config as fontAwesomeConfig } from '@fortawesome/fontawesome';
import { enableStaticRendering } from 'mobx-react';
import { IntlProvider } from 'react-intl';

import { ThemeProvider } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';

import useInnerWindowSize from '@src/hooks/useInnerWindowSize';

import { AppStore, AppStoreContext } from '@stores/AppStore';
import defaultTheme from '@src/theme';
import { FSMosquitoClient, FSMosquitoClientContext } from '@services/FSMosquitoClient';
import { FSMosquitoRestClient, FSMosquitoRestClientContext } from '@services/FSMosquitoRestClient';

import AppLocale from '../localization';

// Styles
import '@fortawesome/fontawesome-svg-core/styles.css';
import 'bootswatch/dist/darkly/bootstrap.min.css';
import 'react-perfect-scrollbar/dist/css/styles.css';
import 'react-grid-layout/css/styles.css';
import 'react-resizable/css/styles.css';
import 'leaflet/dist/leaflet.css';
import '@styles/globals.css';

fontAwesomeConfig.autoAddCss = false;

function FSMosquitoApp({ Component, pageProps }) {
  const appStore = useRef(new AppStore());
  const fsMosquitoClient = useRef(new FSMosquitoClient(appStore.current.fsMosquitoStore, appStore.current.atcStore));
  const fsMosquitoRestClient = useRef(new FSMosquitoRestClient(appStore.current.fsMosquitoStore));
  const innerWindowSize = useInnerWindowSize();
  const currentAppLocale = AppLocale[appStore.current.uiStore.locale.locale];

  const isServer = typeof window === 'undefined';
  enableStaticRendering(isServer);

  useEffect(() => {
    // A kinda crazy way to detect if we're running under electron.
    // Note that this only works after the app has been built statically
    // and is hosted directly under electron - not using the dev server or the proxy.
    if (!window.require) {
      return;
    }
    try {
      const electron = window.require('electron');
      if (typeof electron !== 'object') {
        // Request the current hostname via MQTT rather than electron IPC
        fsMosquitoClient.current.pulse();
        return;
      }

      pageProps.electron = electron;
      // Perform some electron setup:

      const { ipcRenderer } = electron;

      ipcRenderer.on('hostName', (event: any, hostName: string) => {
        appStore.current.fsMosquitoStore.updateHostName(hostName);
        console.log(`Set hostName to ${hostName}`);
      });

      ipcRenderer.send('getHostName');

      // Block the window from unloading and make an IPC call
      // to instruct the .net side to hide the window.
      window.onbeforeunload = (e: { returnValue: boolean }) => {
        e.returnValue = true;
        ipcRenderer.send('hideToSystemTray');
      };
    } catch (ex) {
      console.log(`Unable to load electron: ${ex}`);
    }
  }, [pageProps]);

  useEffect(() => {
    if (!fsMosquitoClient.current.connected) {
      fsMosquitoClient.current.connect();
      setTimeout(fsMosquitoClient.current.pulse, 2500);
    }
  }, []);

  useEffect(() => {
    if (!fsMosquitoRestClient.current.baseUrl) {
      fsMosquitoRestClient.current.initialize();
    }
  }, []);

  useEffect(() => {
    document.documentElement.style.setProperty('--vh', `${window.innerHeight / 100}px`);
    document.documentElement.style.setProperty('--vw', `${window.innerWidth / 100}px`);
  }, [innerWindowSize]);

  useEffect(() => {
    // Remove the server-side injected CSS so that Material UI styles work.
    const jssStyles = document.querySelector('#jss-server-side');
    if (jssStyles) {
      jssStyles.parentElement?.removeChild(jssStyles);
    }
  }, []);

  return (
    <>
      <Head>
        <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
        <title>FSMosquito</title>
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <AppStoreContext.Provider value={appStore.current}>
        <ThemeProvider theme={defaultTheme}>
          <IntlProvider locale={currentAppLocale.locale} defaultLocale="en" messages={currentAppLocale.messages}>
            <FSMosquitoClientContext.Provider value={fsMosquitoClient.current}>
              <FSMosquitoRestClientContext.Provider value={fsMosquitoRestClient.current}>
                <CssBaseline />
                <Component {...pageProps} />
              </FSMosquitoRestClientContext.Provider>
            </FSMosquitoClientContext.Provider>
          </IntlProvider>
        </ThemeProvider>
      </AppStoreContext.Provider>
    </>
  );
}

export default FSMosquitoApp;
