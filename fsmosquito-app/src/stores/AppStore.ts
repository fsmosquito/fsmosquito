import React from 'react';
import { makeAutoObservable } from 'mobx';

import FSMosquitoStore from './FSMosquitoStore';
import AirTrafficControlStore from './AirTrafficControlStore';
import UIStore from './UIStore';

export class AppStore {
  fsMosquitoStore: FSMosquitoStore;
  atcStore: AirTrafficControlStore;
  uiStore: UIStore;

  constructor() {
    makeAutoObservable(this);
    this.fsMosquitoStore = new FSMosquitoStore(this);
    this.atcStore = new AirTrafficControlStore(this);
    this.uiStore = new UIStore(this);
  }
}

export const AppStoreContext = React.createContext<AppStore>(null);
