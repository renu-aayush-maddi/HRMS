import { Component, NgZone } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { SessionService } from './core/services/session.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {

  constructor(
  private sessionService: SessionService,
  private zone: NgZone
) {
  console.log('App Zone:', this.zone.constructor.name);
  console.log('Is in Angular Zone:', NgZone.isInAngularZone());

  this.sessionService.restoreSession();
}
}