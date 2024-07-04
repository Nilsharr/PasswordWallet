export interface AuthenticationResponse {
  username: string;
  lastSuccessfulLogin?: string;
  lastUnsuccessfulLogin?: string;
  accessToken: string;
  accessTokenExpiry: string;
  refreshToken: string;
  refreshTokenExpiry: string;
}
