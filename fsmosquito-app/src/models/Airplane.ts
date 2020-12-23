import { DatumValue } from './DatumValue';

export type Airplane = {
  id: string;
  /** Indicates the kind of airplane */
  title: DatumValue;
  atc_id: DatumValue;
  callsign: DatumValue;
  plane_latitude: DatumValue;
  plane_longitude: DatumValue;
  plane_heading_degrees_true: DatumValue;
  indicated_altitude: DatumValue;
  kohlsman_setting_mb: DatumValue;
  kohlsman_setting_hg: DatumValue;
  attitide_indicator_pitch_degrees: DatumValue;
  attitide_indicator_bank_degrees: DatumValue;
  altitude: DatumValue;
  ground_altitude: DatumValue;
  airspeed_indicated: DatumValue;
  vertical_speed: DatumValue;
  ambient_pressure: DatumValue;
  sim_on_ground: DatumValue;
  // Lights
  light_strobe: boolean;
  light_panel: boolean;
  light_landing: boolean;
  light_taxi: boolean;
  light_beacon: boolean;
  light_nav: boolean;
  light_logo: boolean;
  light_wing: boolean;
  light_recognition: boolean;
  light_cabin: boolean;
} & {
  [prop: string]: DatumValue;
};
