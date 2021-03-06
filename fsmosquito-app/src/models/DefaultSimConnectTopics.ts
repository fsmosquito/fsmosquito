import { SimConnectTopic } from './SimConnectTopic';

export const DefaultSimConnectTopics: SimConnectTopic[] = [
  {
    datumName: 'TITLE',
    units: 'SIMCONNECT_DATATYPE::SIMCONNECT_DATATYPE_STRINGV',
  },
  //{ datumName: "CATEGORY", units: "SIMCONNECT_DATATYPE::SIMCONNECT_DATATYPE_STRINGV"}, // Reports "Airplane" ... thanks.
  {
    datumName: 'ATC ID',
    units: 'SIMCONNECT_DATATYPE::SIMCONNECT_DATATYPE_STRINGV',
  },
  //{ datumName: "ATC MODEL", units: "SIMCONNECT_DATATYPE::SIMCONNECT_DATATYPE_STRINGV"}, // Reports "TT:ATCCOM.AC_MODEL_C700.0.text"
  { datumName: 'PLANE LATITUDE', units: 'radians' },
  { datumName: 'PLANE LONGITUDE', units: 'radians' },
  { datumName: 'PLANE ALTITUDE', units: 'feet' },
  { datumName: 'PLANE PITCH DEGREES', units: 'radians' },
  { datumName: 'PLANE BANK DEGREES', units: 'radians' },
  { datumName: 'PLANE HEADING DEGREES TRUE', units: 'radians' },
  { datumName: 'INDICATED ALTITUDE', units: 'feet' },
  { datumName: 'KOHLSMAN SETTING MB', units: 'millibars' },
  { datumName: 'KOHLSMAN SETTING HG', units: 'inHg' },
  { datumName: 'ATTITUDE INDICATOR PITCH DEGREES', units: 'radians' },
  { datumName: 'ATTITUDE INDICATOR BANK DEGREES', units: 'radians' },
  { datumName: 'GROUND ALTITUDE', units: 'feet' },
  { datumName: 'AIRSPEED INDICATED', units: 'knots' },
  { datumName: 'AMBIENT PRESSURE', units: 'inHg' },
  { datumName: 'VERTICAL SPEED', units: 'feet per second' },
  { datumName: 'SIM ON GROUND', units: 'bool' },
  // Aircraft Lights
  { datumName: 'LIGHT STROBE', units: 'bool' },
  { datumName: 'LIGHT PANEL', units: 'bool' },
  { datumName: 'LIGHT LANDING', units: 'bool' },
  { datumName: 'LIGHT TAXI', units: 'bool' },
  { datumName: 'LIGHT BEACON', units: 'bool' },
  { datumName: 'LIGHT NAV', units: 'bool' },
  { datumName: 'LIGHT LOGO', units: 'bool' },
  { datumName: 'LIGHT WING', units: 'bool' },
  { datumName: 'LIGHT RECOGNITION', units: 'bool' },
  { datumName: 'LIGHT CABIN', units: 'bool' },
  // Aircraft Controls
  { datumName: 'YOKE X POSITION', units: 'position' },
];
