import { observable, action, autorun, makeAutoObservable, runInAction } from 'mobx';

import { AppStore } from './AppStore';
import moment from 'moment';

class FSMosquitoStore {
  appStore: AppStore;

  @observable hostName: string;
  @observable isConnected = false;
  @observable lastMessageReceived: string = null;
  @observable secondsSinceLastMessageRecieved = null;

  constructor(appStore: AppStore) {
    makeAutoObservable(this);
    this.appStore = appStore;

    if (process.env.NODE_ENV !== 'production') {
      this.hostName = 'DESKTOP-K4T6J4J';
    }

    autorun(
      () => {
        runInAction(() => {
          this.updateSecondsSinceLastMessageRecieved();
        });
      },
      {
        scheduler: (run) => setInterval(run, 1000),
      },
    );
  }

  @action updateHostName(newHostName: string) {
    this.hostName = newHostName;
  }

  @action updateConnected(isConnected: boolean) {
    this.isConnected = isConnected;
  }

  @action updateLastMessageRecieved() {
    this.lastMessageReceived = new Date().toISOString();
  }

  @action updateSecondsSinceLastMessageRecieved() {
    const lastMessageReceived = moment(this.lastMessageReceived);
    this.secondsSinceLastMessageRecieved = moment().diff(lastMessageReceived, 'seconds');
  }
}

export default FSMosquitoStore;
