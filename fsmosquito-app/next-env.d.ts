/// <reference types="next" />
/// <reference types="next/types/global" />
declare module '*.css';
declare module '*.scss';

declare module '*.mp3';
declare module '*.wav';
declare module '*.ogg';

declare module '*.svg' {
  import React = require('react');
  export const ReactComponent: React.SFC<React.SVGProps<SVGSVGElement>>;
  const src: string;
  export default src;
}
