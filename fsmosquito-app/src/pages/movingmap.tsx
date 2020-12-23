import React, { useEffect, useState, useContext } from 'react';
import ReactDOMServer from 'react-dom/server';

import { useRouter } from 'next/router';

import { observer } from 'mobx-react';
import { AppStoreContext } from '@stores/AppStore';

import AppLayout from '@layouts/AppLayout';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCompass, faQuestion, faTachometerAlt } from '@fortawesome/pro-duotone-svg-icons';
import { faPlaneAlt } from '@fortawesome/pro-solid-svg-icons';

import IconButton from '@material-ui/core/IconButton';
import Tooltip from '@material-ui/core/Tooltip';
import { Altimeter, Heading, Airspeed, Variometer } from '@components/FlightIndicators';

import WaitingForFSMosquito from '@components/WaitingForFSMosquito';

import {
  Map,
  TileLayer,
  Popup,
  Tooltip as LeafletTooltip,
  DriftMarker,
  Viewport,
  LeafletControl,
} from '@components/react-leaflet-ssr';
import useWindowSize from '@src/hooks/useWindowSize';
import { FSMosquitoClientContext } from '@services/FSMosquitoClient';
import { Airplane } from '@models/Airplane';

const updateRate = 750;
const defaultZoom = 13;
const instrumentScale = '125px';
const options = {
  doubleClickZoom: true,
  closePopupOnClick: true,
  dragging: true,
  zoomSnap: 0,
  zoomDelta: 1,
  trackResize: false,
  touchZoom: true,
  scrollWheelZoom: true,
  boxZoom: false,
};

const MovingMap = () => {
  const { atcStore } = useContext(AppStoreContext);
  const fsMosquitoClient = useContext(FSMosquitoClientContext);
  useEffect(() => {
    fsMosquitoClient.pulse();
  }, [fsMosquitoClient]);
  const leafletMapRef = React.useRef<any>();
  const airplaneDivIconRef = React.useRef();
  const router = useRouter();
  // if we attempt to view the map after a page navigation, without this,
  // We'll get a partial render of the map
  const [isReady, setIsReady] = useState(false);
  const [windowSize, setWindowSize] = useState<{ width: number; height: number }>();
  const [showInstruments, setShowInstruments] = useState(false);
  const [viewport, setViewport] = useState<Viewport>({
    center: undefined,
    zoom: defaultZoom,
  });
  const [keepCentered, setKeepCentered] = useState(true);

  const initialWindowSize = useWindowSize();
  const airplane: Partial<Airplane> = atcStore.self;

  const requestPositionUpdate = () => {
    //atcClient.requestClientStatusUpdate();
    console.dir('Requested!');
  };

  const toggleInstruments = () => {
    setShowInstruments(!showInstruments);
  };

  const reCenter = () => {
    setViewport({ ...viewport, center: atcStore.currentPosition });
    setKeepCentered(true);
  };

  const handleDragStart = () => {
    setKeepCentered(false);
  };

  useEffect(() => {
    if (keepCentered && !viewport.center) {
      setViewport({ zoom: viewport.zoom, center: atcStore.currentPosition });
    }
  }, [atcStore.currentPosition, keepCentered, viewport.center, viewport.zoom]);

  useEffect(() => {
    // If there isn't a current airplane, pulse for one.
    if (!airplane) {
      // if (atcClient.connected) {
      //   atcClient.pulse();
      // }
    }

    if (!isReady) {
      setIsReady(true);
    }
  }, [airplane, isReady]);

  // If the window size changes, invalidate the map size, after a delay.
  useEffect(() => {
    if (!initialWindowSize.width || !initialWindowSize.height) {
      return;
    }

    let navigateTimer: NodeJS.Timer;
    if (!windowSize || initialWindowSize.width !== windowSize.width || initialWindowSize.height !== windowSize.height) {
      navigateTimer = setTimeout(() => {
        setWindowSize(initialWindowSize);
        if (windowSize && leafletMapRef.current) {
          leafletMapRef.current.leafletElement.invalidateSize(true);
        }
      }, 250);
      return () => {
        clearTimeout(navigateTimer);
      };
    }
  }, [initialWindowSize, router, windowSize]);

  let airplaneRotation: any = 0;
  if (airplane) {
    airplaneRotation = atcStore.heading - 90;
  }

  const airplaneIcon = (
    <FontAwesomeIcon
      icon={faPlaneAlt}
      size="2x"
      color="#00d4ff"
      style={{
        stroke: 'black',
        strokeWidth: 25,
        background: 'transparent',
        transform: `rotate(${airplaneRotation}deg)`,
      }}
    />
  );
  const airplaneIconHtml = ReactDOMServer.renderToString(airplaneIcon);
  useEffect(() => {
    const L = window.L ? window.L : require('leaflet');
    airplaneDivIconRef.current = L.divIcon({
      className: 'dummy',
      html: airplaneIconHtml,
      iconSize: [16, 16],
      iconAnchor: [8, 8],
      popupAnchor: [4, -8],
      shadowUrl: null,
      shadowSize: null,
      shadowAnchor: null,
    });
  }, [airplaneIconHtml]);

  const position = atcStore.currentPosition;
  const hasPosition = position && !isNaN(position[0]) && !isNaN(position[1]) && position[0] !== 0 && position[1] !== 0;
  return (
    <AppLayout>
      {hasPosition && isReady && (
        <Map
          ref={leafletMapRef}
          id="FSMosquito_Moving_Map"
          ondragstart={handleDragStart}
          ondrag={handleDragStart}
          ondragend={handleDragStart}
          center={viewport.center}
          zoom={viewport.zoom}
          animate={true}
          style={{ height: '100%' }}
          //whenReady={wolfman}
          {...options}
        >
          <TileLayer
            attribution='&amp;copy <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.osm.org/{z}/{x}/{y}.png"
          />
          <LeafletControl position="topleft">
            <Tooltip title="Show Instruments">
              <IconButton color="primary" onClick={toggleInstruments} style={{ padding: '5px' }}>
                <FontAwesomeIcon icon={faTachometerAlt} size="1x" color={showInstruments ? 'green' : 'grey'} />
              </IconButton>
            </Tooltip>
          </LeafletControl>
          <LeafletControl position="bottomleft">
            <Tooltip title="Re-center map">
              <IconButton color="primary" onClick={reCenter}>
                <FontAwesomeIcon icon={faCompass} size="2x" color={keepCentered ? 'green' : 'grey'} />
              </IconButton>
            </Tooltip>
          </LeafletControl>
          <LeafletControl position="bottomleft">
            <Tooltip title="(Dev) Request Position Update">
              <IconButton color="default" onClick={requestPositionUpdate}>
                <FontAwesomeIcon icon={faQuestion} size="1x" />
              </IconButton>
            </Tooltip>
          </LeafletControl>
          {showInstruments && (
            <LeafletControl position="topright">
              <div>
                <Heading heading={atcStore.heading} style={{ height: instrumentScale, width: instrumentScale }} />
                <Altimeter
                  altitude={airplane.indicated_altitude?.value}
                  pressure={airplane.kohlsman_setting_mb?.value}
                  style={{ height: instrumentScale, width: instrumentScale }}
                />
                <Airspeed
                  speed={airplane.airspeed_indicated?.value}
                  style={{ height: instrumentScale, width: instrumentScale }}
                />
                <Variometer
                  verticalSpeed={airplane.vertical_speed?.value / 60}
                  style={{ height: instrumentScale, width: instrumentScale }}
                />
              </div>
            </LeafletControl>
          )}
          {airplaneDivIconRef.current && (
            <DriftMarker
              position={position}
              duration={updateRate}
              keepAtCenter={keepCentered}
              icon={airplaneDivIconRef.current}
            >
              <Popup>
                <div key={airplane.id}>
                  <b>{airplane.atc_id}</b>
                  <div style={{ paddingLeft: '5px' }}>
                    <div>Kind: {airplane.title}</div>
                    <div>Latitude: {atcStore.latitude}</div>
                    <div>Longitude: {atcStore.longitude}</div>
                    <div>Heading: {atcStore.heading}</div>
                    <div>Altitude: {airplane.indicated_altitude?.value}</div>
                    <div>Ground Altitude: {airplane.ground_altitude?.value}</div>
                    <div>Is On Ground: {airplane.sim_on_ground?.value ? 'Yes' : 'No'}</div>
                  </div>
                </div>
              </Popup>
              <LeafletTooltip>{airplane.atc_id?.value}</LeafletTooltip>
            </DriftMarker>
          )}
        </Map>
      )}
      {!hasPosition && <WaitingForFSMosquito />}
    </AppLayout>
  );
};

export default observer(MovingMap);
