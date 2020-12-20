import React from 'react';

import axios, { AxiosInstance } from 'axios';
import FSMosquitoStore from '@stores/FSMosquitoStore';

export class FSMosquitoRestClient {
  private baseUrlInternal: string;
  private ax: AxiosInstance;

  constructor(readonly fsMosquitoStore: FSMosquitoStore) {}

  get baseUrl() {
    return this.baseUrlInternal;
  }

  initialize() {
    this.baseUrlInternal = `//${window?.location.host}`;

    if (process.env.NODE_ENV !== 'production') {
      this.baseUrlInternal = `//localhost:5272`;
    }
    this.ax = axios.create({
      baseURL: this.baseUrlInternal,
    });
  }
}

export const FSMosquitoRestClientContext = React.createContext<FSMosquitoRestClient>(null);
