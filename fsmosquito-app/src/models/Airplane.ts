import { DatumValue } from './DatumValue';

export type Airplane = {
  id: string;
  /** Indicates the kind of airplane */
  title: string;
  atc_id: string;
  callsign: string;
  plane_latitude: DatumValue;
  plane_longitude: DatumValue;
  plane_heading_degrees_true: DatumValue;
  indicated_altitude: DatumValue;
  kohlsman_setting_mb: DatumValue;
  kohlsman_setting_hg: DatumValue;
  attitide_indicator_pitch_degrees: number;
  attitide_indicator_bank_degrees: number;
  altitude: number;
  ground_altitude: number;
  airspeed_indicated: number;
  vertical_speed: number;
  ambient_pressure: number;
  sim_on_ground: boolean;
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
