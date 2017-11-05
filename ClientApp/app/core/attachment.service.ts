import 'rxjs/add/operator/map';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";

import { Attachment } from './attachment.model';

interface AttachmentResponse {
    attachments: Array<{
        id: string,
        name: string,
        contentType: string
    }>
}

@Injectable()
export class AttachmentService {
    constructor(private readonly httpClient: HttpClient) { }

    delete(id: string) {
        return this.httpClient.delete(`api/attachments/${id}`);
    }

    download(id: string) {
        return this.httpClient.get(`api/attachments/${id}`, {
            responseType: 'blob'
        });
    }

    private mapAttachments(o: Observable<AttachmentResponse>) {
        return o.map(resp => resp.attachments
            .map(a => {
                return new Attachment(a.id, a.name, a.contentType,
                    () => this.download(a.id),
                    () => this.delete(a.id)
                );
            }));
    }

    list() {
        const o = this.httpClient.get<AttachmentResponse>('api/attachments');
        return this.mapAttachments(o);
    }
    
    upload(name: string, contents: Blob) {
        const formData = new FormData();
        formData.set(name, contents, name);

        const o = this.httpClient.post<AttachmentResponse>('api/attachments', formData);
        return this.mapAttachments(o);
    }
}
