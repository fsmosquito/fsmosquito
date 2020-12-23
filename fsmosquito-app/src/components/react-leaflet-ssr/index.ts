import dynamic from 'next/dynamic';
import L from 'react-leaflet';

import decorate from './decorate';
import { DriftMarkerProps } from './DriftMarker';

export type { Viewport } from 'react-leaflet';

export const AttributionControl = decorate<L.AttributionControlProps>('AttributionControl');
export const Circle = decorate<L.CircleProps>('Circle');
export const CircleMarker = decorate<L.CircleMarkerProps>('CircleMarker');
export const ControlledLayer = decorate('ControlledLayer');
export const DivOverlay = decorate<L.DivOverlayProps>('DivOverlay');
export const FeatureGroup = decorate('FeatureGroup');
export const GeoJSON = decorate<L.GeoJSONProps>('GeoJSON');
export const GridLayer = decorate<L.GridLayerProps>('GridLayer');
export const ImageOverlay = decorate<L.ImageOverlayProps>('ImageOverlay');
export const LayerGroup = decorate('LayerGroup');
export const LayersControl = decorate<L.LayersControlProps>('LayersControl');
export const Map = decorate<L.MapProps>('Map');
export const MapComponent = decorate<L.MapComponentProps>('MapComponent');
export const MapControl = decorate<L.MapControlProps>('MapControl');
export const MapEvented = decorate('MapEvented');
export const MapLayer = decorate<L.MapLayerProps>('MapLayer');
export const Marker = decorate<L.MarkerProps>('Marker');
export const Pane = decorate<L.PaneProps>('Pane');
export const Path = decorate<L.PathProps>('Path');
export const Polygon = decorate<L.PolygonProps>('Polygon');
export const Polyline = decorate<L.PolylineProps>('Polyline');
export const Popup = decorate<L.PopupProps>('Popup');
export const Rectangle = decorate<L.RectangleProps>('Rectangle');
export const ScaleControl = decorate<L.ScaleControlProps>('ScaleControl');
export const TileLayer = decorate<L.TileLayerProps>('TileLayer');
export const Tooltip = decorate<L.TooltipProps>('Tooltip');
export const VideoOverlay = decorate<L.VideoOverlayProps>('VideoOverlay');
export const WMSTileLayer = decorate<L.WMSTileLayerProps>('WMSTileLayer');
export const ZoomControl = decorate<L.ZoomControlProps>('ZoomControl');

export const DriftMarker = dynamic<DriftMarkerProps>(() => import('./DriftMarker'), {
  ssr: false,
});

export const LeafletControl: any = dynamic(() => import('./LeafletControl'), {
  ssr: false,
});
