import React, { useEffect, useState, useContext } from 'react';

import Box from '@material-ui/core/Box';
import Typography from '@material-ui/core/Typography';
import Fade from '@material-ui/core/Fade';

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
    <Box display="flex" justifyContent="center" alignItems="center" css={{ height: '100%', width: '100%' }}>
      <FontAwesomeIcon icon={faCircleNotch} size="4x" spin={true} />
      <Box
        display="flex"
        flexDirection="column"
        justifyContent="center"
        alignItems="center"
        css={{ paddingLeft: '10px' }}
      >
        <Typography variant="h1">Waiting for FSMosquito client data...</Typography>
        <Fade in={fadeIn} timeout={2000} style={{ height: '1pt' }}>
          <Typography variant="h6" color="textSecondary">
            {messages[messageIndex]}
          </Typography>
        </Fade>
      </Box>
    </Box>
  );
};

export default WaitingForFSMosquito;
