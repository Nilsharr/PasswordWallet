import { TokenResponse } from '../contracts/responses/token-response';
import { TokenInfo } from '../models/token-info';

export class TokenMapper {
  public static toModel = (source: TokenResponse): TokenInfo => {
    return {
      token: source.accessToken,
      expiry: new Date(source.accessTokenExpiry),
    };
  };
}
