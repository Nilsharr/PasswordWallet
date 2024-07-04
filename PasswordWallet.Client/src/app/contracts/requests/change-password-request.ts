export interface ChangePasswordRequest {
  currentPassword: string;
  password: string;
  confirmPassword: string;
}
