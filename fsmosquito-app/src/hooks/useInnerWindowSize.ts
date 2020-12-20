import { useState, useEffect } from 'react';

// Hook
export default function useInnerWindowSize() {
  // Initialize state with undefined width/height so server and client renders match
  // Learn more here: https://joshwcomeau.com/react/the-perils-of-rehydration/
  const [windowSize, setWindowSize] = useState({
    width: undefined,
    height: undefined,
  });

  useEffect(() => {
    // Handler to call on window resize
    function handleInnerWindowResize() {
      // Set window width/height to state
      setWindowSize({
        width: window.innerHeight / 100,
        height: window.innerHeight / 100,
      });
    }

    // Add event listener
    window.addEventListener('resize', handleInnerWindowResize);
    window.addEventListener('orientationchange', handleInnerWindowResize);

    // Call handler right away so state gets updated with initial window size
    handleInnerWindowResize();

    // Remove event listener on cleanup
    return () => {
      window.removeEventListener('resize', handleInnerWindowResize);
      window.removeEventListener('orientationchange', handleInnerWindowResize);
    };
  }, []); // Empty array ensures that effect is only run on mount

  return windowSize;
}
