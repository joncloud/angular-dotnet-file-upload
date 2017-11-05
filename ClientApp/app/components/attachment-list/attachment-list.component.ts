import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';

import { Attachment } from '../../core/attachment.model';
import { AttachmentService } from '../../core/attachment.service';

@Component({
    selector: 'attachment-list',
    styleUrls: ['./attachment-list.component.css'],
    templateUrl: './attachment-list.component.html'
})
export class AttachmentListComponent implements OnInit {
    constructor(readonly attachmentService: AttachmentService) { }

    get attachments() {
        return this._attachments;
    }
    _attachments: Attachment[] = [];

    delete(attachment: Attachment) {
        attachment.delete().subscribe(
            () => {
                const index = this._attachments.indexOf(attachment);
                if (index > -1) {
                    this._attachments.splice(index, 1);
                }
            },
            error => console.log(error));
    }

    download(attachment: Attachment) {
        attachment.download().subscribe(
            blob => {
                const a = document.createElement('a');
                a.style.display = 'none';
                a.href = window.URL.createObjectURL(blob);
                a.download = attachment.name;

                // Ensure the <a> element exists in the document 
                // to allow the download for Firefox compatibility.
                document.body.appendChild(a);
                a.click();
                a.remove();
            },
            error => console.log(error));
    }

    ngOnInit() {
        this.attachmentService.list()
            .subscribe(
                attachments => this._attachments = attachments,
                error => console.log(error));
    }

    upload(event: Event) {
        const inputElement = <HTMLInputElement>(event.srcElement || event.target);
        const files = inputElement.files;

        if (!files) return;

        for (var index = 0; index < files.length; index++) {
            const file = files[index];
            this.attachmentService.upload(file.name, file)
                .subscribe(
                    attachments => this._attachments = this._attachments.concat(attachments),
                    error => console.log(error));
        }

        // Clear the value to allow uploading the same file again.
        inputElement.value = '';
    }
}
