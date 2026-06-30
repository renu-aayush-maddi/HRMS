import { SessionService } from '../services/session.service';

export function initializeApp(
  sessionService: SessionService
) {
  return () => {
    sessionService.restoreSession();
  };
}