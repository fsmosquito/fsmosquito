import React, { useEffect, useState, useContext } from 'react';

import Typography from '@material-ui/core/Typography';
import Fade from '@material-ui/core/Fade';

import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleNotch } from '@fortawesome/pro-duotone-svg-icons';
import { FSMosquitoClientContext } from '@services/FSMosquitoClient';

const messages = [
  `Please ensure the FSMosquito client is running...`,
  `Reticulating Splines`,
  `Inserting Sublimated Messages`,
  `Two FSMosquitos walk into a bar...`,
  `Normalizing Power`,
  `Searching for Llamas`,
];

const WaitingForFSMosquito = () => {
  const fsMosquitoClient = useContext(FSMosquitoClientContext);
  const [fadeIn, setFadeIn] = useState(false);
  const [messageIndex, setMessageIndex] = useState(-1);

  useEffect(() => {
    if (messageIndex === -1) {
      fsMosquitoClient.pulse();
    }

    // Move on to the next message every `n` milliseconds
    const messageTimer = setTimeout(() => {
      if (fadeIn) {
        setFadeIn(false);
        return;
      }

      setFadeIn(true);
      setMessageIndex(messageIndex >= messages.length ? -1 : messageIndex + 1);
    }, 5000);

    return () => {
      clearTimeout(messageTimer);
    };
  }, [fsMosquitoClient, fadeIn, messageIndex]);

  return (
    <Container className="d-flex align-items-center" style={{ height: '100%' }}>
      <Col>
        <Row className="justify-content-md-center align-middle">
          <FontAwesomeIcon icon={faCircleNotch} size="3x" spin={true} />
          &nbsp;
          <Typography variant="h4">Waiting for FSMosquito client data...</Typography>
        </Row>
        <Row className="justify-content-md-center">
          <Fade in={fadeIn} timeout={2000} style={{ height: '1pt' }}>
            <Typography variant="h6" color="textSecondary">
              {messages[messageIndex]}
            </Typography>
          </Fade>
        </Row>
      </Col>
    </Container>
  );
};

export default WaitingForFSMosquito;
