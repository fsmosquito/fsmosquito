import { observable, action, makeAutoObservable } from 'mobx';
import { AppStore } from './AppStore';

export interface Locale {
  languageId: string;
  locale: string;
  name: string;
  icon: string;
}

class UIStore {
  appStore: AppStore;

  @observable searchOverlayOpen = false;
  @observable navCollapsed = false;
  @observable locale: Locale = {
    languageId: 'english',
    locale: 'en',
    name: 'English',
    icon: 'us',
  };

  constructor(appStore: AppStore) {
    makeAutoObservable(this);
    this.appStore = appStore;
  }

  @action setSearchOverlayOpen(value) {
    this.searchOverlayOpen = value;
  }

  @action toggleNavCollapsed() {
    this.navCollapsed = !this.navCollapsed;
  }
}

export default UIStore;
