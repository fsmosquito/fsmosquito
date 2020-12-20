import React from 'react';
import mqtt, { IMqttClient } from 'async-mqtt';
import FSMosquitoStore from '@stores/FSMosquitoStore';
import AirTrafficControlStore from '@stores/AirTrafficControlStore';
import { DefaultSimConnectTopics } from '@models/DefaultSimConnectTopics';

export class FSMosquitoClient {
  private client: IMqttClient;

  constructor(readonly fsMosquitoStore: FSMosquitoStore, readonly atcStore: AirTrafficControlStore) {}

  get mqtt(): IMqttClient {
    return this.client;
  }

  get connected(): boolean {
    if (!this.client) {
      return false;
    }

    return this.client.connected;
  }

  connect() {
    let host = window.location.host;

    if (process.env.NODE_ENV !== 'production') {
      host = 'localhost:5272';
    }

    this.client = mqtt.connect(`ws://${host}/mqtt`, {
      reconnectPeriod: 5000,
      keepalive: 15,
    });

    this.client.on('connect', () => {
      console.log(
        `%c connected!! (${this.fsMosquitoStore.hostName})`,
        'background: green; color: white; display: block;',
      );
      this.client.subscribe(`fsm/atc/${this.fsMosquitoStore.hostName}/ident`);
      this.pulse();

      this.client.subscribe(`fsm/atc/${this.fsMosquitoStore.hostName}/obj/+/+`);

      this.fsMosquitoStore.updateConnected(true);
      this.fsMosquitoStore.updateLastMessageRecieved();
    });

    this.client.on('message', (topic, message) => {
      const dataTopicRegex = new RegExp(`^fsm/atc/${this.fsMosquitoStore.hostName}/obj/1/(?<datumName>[a-z_]+)$`);
      let messageHandled = false;
      const strMessage = message.toString('utf8');

      // Match data topics.
      if (topic.match(dataTopicRegex)) {
        const match = dataTopicRegex.exec(topic);
        this.atcStore.setSelfValue(match.groups['datumName'], JSON.parse(strMessage));
        messageHandled = true;
      }

      switch (topic) {
        case `fsm/host`:
          const host = JSON.parse(strMessage);
          console.dir(host);
          break;
        case `fsm/c/${this.fsMosquitoStore.hostName}/atc/ident`:
          //const airplane = JSON.parse(strMessage);
          //this.fsMosquitoStore.setSelf(airplane);
          break;
      }

      if (!messageHandled) {
        console.log(`Message was not handled: ${topic} - ${strMessage}`);
      }
      this.fsMosquitoStore.updateLastMessageRecieved();
    });

    this.client.on('error', (err) => {
      this.fsMosquitoStore.updateConnected(false);
      console.log('Broker reported error: ' + err.message);
      console.log('Additional details: ' + err.stack);
    });
  }

  disconnect() {
    this.client?.end();
  }

  pulse() {
    if (!this.client || !this.fsMosquitoStore.hostName) {
      return;
    }
    this.client.publish(`fsm/c/${this.fsMosquitoStore.hostName}/atc/status/pulse`, null);
  }

  subscribe() {
    if (!this.client || !this.fsMosquitoStore.hostName) {
      return;
    }
    this.client.publish(
      `fsm/c/${this.fsMosquitoStore.hostName}/atc/subscribe/0`,
      JSON.stringify(DefaultSimConnectTopics),
    );
  }

  setSimVar(normalizedVarName: string, objectId: null | number, value: any) {
    if (!objectId) {
      objectId = 1;
    }

    this.client.publish(
      `fsm/u/${this.fsMosquitoStore.hostName}/atc/set_data/${objectId}/${normalizedVarName}`,
      JSON.stringify({
        value,
      }),
    );
  }
}

export const FSMosquitoClientContext = React.createContext<FSMosquitoClient>(null);
