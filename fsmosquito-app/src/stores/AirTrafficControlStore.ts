import { observable, action, computed, set, get, makeAutoObservable } from 'mobx';

import { Airplane } from '@models/Airplane';
import { AppStore } from './AppStore';
import { merge } from 'lodash';
import { DatumValue } from '@models/DatumValue';

class AirTrafficControlStore {
  appStore: AppStore;

  @observable self: Partial<Airplane> = {};
  @observable traffic: Record<string, Airplane> = {};

  constructor(appStore: AppStore) {
    makeAutoObservable(this);
    this.appStore = appStore;
  }

  @action setSelfValue(datumName: string, datumValue: DatumValue) {
    set(this.self, datumName, observable(datumValue));
  }

  @action addTraffic(airplane: Airplane) {
    const existingAirplane = this.traffic[airplane.id];
    if (!existingAirplane) {
      this.traffic[airplane.id] = observable(airplane);
      return;
    }

    merge(this.traffic[airplane.id], airplane);
  }

  @computed get selfPosition(): [number, number] {
    const position: [number, number] = [0, 0];
    const plane_latitude = get(this.self, 'plane_latitude');
    const plane_logitude = get(this.self, 'plane_longitude');
    if (!this.self || !plane_latitude || !plane_logitude) {
      return position;
    }

    position[0] = (plane_latitude * 180) / Math.PI;
    position[1] = (plane_logitude * 180) / Math.PI;
    return position;
  }

  @computed get heading() {
    return (get(this.self, 'plane_heading_degrees_true.value') * 180) / Math.PI;
  }

  @computed get latitude() {
    return (this.self.plane_latitude.value * 180) / Math.PI;
  }

  @computed get longitude() {
    return (this.self.plane_longitude.value * 180) / Math.PI;
  }
}

export default AirTrafficControlStore;
