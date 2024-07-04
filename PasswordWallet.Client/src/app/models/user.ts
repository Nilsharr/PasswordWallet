export interface User {
  username: string;
  lastValidLoginDate: Date | null;
  lastInvalidLoginDate: Date | null;
}
