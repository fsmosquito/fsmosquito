import React from 'react';
import Drift_Marker from './Drift_Marker';
import { Icon, DivIcon, LatLngExpression } from 'leaflet';
import { LeafletProvider, MapLayer, MapLayerProps, withLeaflet } from 'react-leaflet';

type LeafletElement = Drift_Marker;
export type DriftMarkerProps = {
  icon?: Icon | DivIcon;
  draggable?: boolean;
  opacity?: number;
  position: LatLngExpression;
  duration: number;
  keepAtCenter?: boolean;
  zIndexOffset?: number;
} & MapLayerProps;

class DriftMarker extends MapLayer<DriftMarkerProps, LeafletElement> {
  createLeafletElement(props: DriftMarkerProps): LeafletElement {
    const el = new Drift_Marker(props.position, this.getOptions(props));
    this.contextValue = { ...props.leaflet, popupContainer: el };
    return el;
  }

  updateLeafletElement(fromProps: DriftMarkerProps, toProps: DriftMarkerProps) {
    if (toProps.position !== fromProps.position && typeof toProps.duration == 'number') {
      this.leafletElement.slideTo(toProps.position, {
        duration: toProps.duration,
        keepAtCenter: toProps.keepAtCenter,
      });
    }
    if (toProps.icon !== fromProps.icon && toProps.icon) {
      this.leafletElement.setIcon(toProps.icon);
    }
    if (toProps.zIndexOffset !== fromProps.zIndexOffset && toProps.zIndexOffset !== undefined) {
      this.leafletElement.setZIndexOffset(toProps.zIndexOffset);
    }
    if (toProps.opacity !== fromProps.opacity && toProps.opacity !== undefined) {
      this.leafletElement.setOpacity(toProps.opacity);
    }
    if (toProps.draggable !== fromProps.draggable && this.leafletElement.dragging !== undefined) {
      if (toProps.draggable === true) {
        this.leafletElement.dragging.enable();
      } else {
        this.leafletElement.dragging.disable();
      }
    }
  }

  render() {
    const { children } = this.props;

    return children == null || this.contextValue == null ? null : (
      <LeafletProvider value={this.contextValue}>{children}</LeafletProvider>
    );
  }
}
export default withLeaflet<DriftMarkerProps>(DriftMarker);
