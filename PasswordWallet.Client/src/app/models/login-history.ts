export interface LoginHistory {
  id: number;
  date: Date;
  correct: boolean;
  ipAddress?: string;
}
