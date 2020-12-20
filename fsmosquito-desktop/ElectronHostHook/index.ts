// @ts-ignore
import * as Electron from "electron";
import { Connector } from "./connector";

export class HookService extends Connector {
    constructor(socket: SocketIO.Socket, public app: Electron.App) {
        super(socket, app);
    }

    onHostReady(): void {
        this.on("get-window-handle", async (data, done) => {
            const result = 42 + data;
            done(result);
        });
    }
}

