import React, { useEffect, useState, useRef } from 'react';

export default function decorate<T>(componentName: string) {
  const ForwardedRefComponent = React.forwardRef<T, any>((props, ref) => {
    const ClientComponent = useRef(null);
    const [isLoaded, setIsLoaded] = useState(false);

    useEffect(() => {
      setIsLoaded(true);
      // eslint-disable-next-line @typescript-eslint/no-var-requires
      ClientComponent.current = require('react-leaflet')[componentName];
    }, []);

    if (!isLoaded) {
      return null;
    }

    return <ClientComponent.current {...props} ref={ref} />;
  });
  ForwardedRefComponent.displayName = `${componentName}Ssr`;
  return ForwardedRefComponent;
}
