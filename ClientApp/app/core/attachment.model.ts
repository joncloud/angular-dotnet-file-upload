import { Observable } from 'rxjs/Observable';

export class Attachment {
    constructor(readonly id: string, readonly name: string, readonly contentType: string, private readonly downloader: () => Observable<Blob>, private readonly deleter: () => Observable<{}>) {
    }

    delete() {
        return this.deleter();
    }

    download() {
        return this.downloader();
    }
}
