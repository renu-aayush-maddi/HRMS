export interface CurrentUser {
  userId: string;
  employeeId?: string;
  email: string;
  role: string;
  firstName?: string;
  lastName?: string;
  profilePhotoUrl?: string | null;
}