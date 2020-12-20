import React from 'react';

import { FiBox, FiCircle, AltitudePressure, AltitudeTicks, FiNeedleSmall, FiNeedle } from './images';
import { IInstrumentProps } from './config';
interface IAltimeterProps extends IInstrumentProps {
  pressure: number;
  altitude: number;
  style?: React.CSSProperties;
}

const Altimeter: React.FunctionComponent<IAltimeterProps> = (props) => {
  const needle = 90 + ((props.altitude % 1000) * 360) / 1000;
  const needleSmall = (props.altitude / 10000) * 360;
  const pressure = 2 * props.pressure - 1980;
  return (
    <span id="altimeter">
      <div className="instrument altimeter" style={{ height: '200px', width: '200px', ...props.style }}>
        <img src={FiBox} className="background box" style={{ display: props.showBox ? '' : 'none' }} />
        <div className="pressure box" style={{ transform: `rotate(${pressure}deg)` }}>
          <img src={AltitudePressure} className="box" />
        </div>
        <img src={AltitudeTicks} className="box" />
        <div className="needleSmall box" style={{ transform: `rotate(${needleSmall}deg)` }}>
          <img src={FiNeedleSmall} className="box" />
        </div>
        <div className="needle box" style={{ transform: `rotate(${needle}deg)` }}>
          <img src={FiNeedle} className="box" />
        </div>
        <div className="mechanics box">
          <img src={FiCircle} className="box" />
        </div>
      </div>
    </span>
  );
};

export default Altimeter;
