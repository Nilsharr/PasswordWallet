export interface Credential {
  id: number;
  username?: string;
  password?: string;
  hiddenPassword: string;
  webAddress?: string;
  description?: string;
  position: number;
}
