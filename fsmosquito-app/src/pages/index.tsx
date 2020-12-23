import React, { useContext, useEffect } from 'react';
import AppLayout from '@layouts/AppLayout';

import { observer } from 'mobx-react';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';

import { AppStoreContext } from '@stores/AppStore';

import { FSMosquitoClientContext } from '@services/FSMosquitoClient';

const Home = () => {
  const { fsMosquitoStore, atcStore } = useContext(AppStoreContext);
  const fsMosquitoClient = useContext(FSMosquitoClientContext);
  useEffect(() => {
    fsMosquitoClient.pulse();
  }, [fsMosquitoClient]);

  return (
    <AppLayout>
      <Container fluid>
        <Row>Hi {fsMosquitoStore.hostName}</Row>
        <Row>
          {atcStore.self.plane_longitude?.value}, {atcStore.self.plane_latitude?.value}
        </Row>
        <Row>
          {atcStore.self.plane_heading_degrees_true?.value}
          {atcStore.self.indicated_altitude?.value}
        </Row>
      </Container>
    </AppLayout>
  );
};

export default observer(Home);
