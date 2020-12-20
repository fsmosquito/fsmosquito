import delay from 'delay';

export const timeout = (ms: number) => {
  return new Promise((resolve: (value?: unknown) => void) => setTimeout(resolve, ms));
};

export function rand(min?: number, max?: number) {
  let ms: number;

  if (Number.isInteger(max) && Number.isInteger(min)) {
    ms = Math.floor(Math.random() * Math.abs(max - min)) + Math.abs(min);
  } else {
    ms = Math.floor(Math.random() * 250) + 130;
  }

  return ms;
}

export function getDelayMs(options: { delay?: number; max?: number; min?: number } = {}) {
  let ms: number;

  if (Number.isInteger(options.delay)) {
    ms = Math.abs(options.delay);
  } else if (Number.isInteger(options.max) && Number.isInteger(options.min)) {
    ms = Math.floor(Math.random() * Math.abs(options.max - options.min)) + Math.abs(options.min);
  } else {
    ms = Math.floor(Math.random() * 700) + 300;
  }

  return ms;
}

export async function hesitate(options: { delay?: number; max?: number; min?: number } = {}) {
  const ms = getDelayMs(options);
  await delay(ms);
}
