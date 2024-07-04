import { HttpContextToken } from '@angular/common/http';

export class AuthenticationConstants {
  public static readonly IS_PUBLIC_API = new HttpContextToken<boolean>(
    () => false
  );
  public static readonly OMIT_DEFAULT_UNAUTHORIZED_ERROR_HANDLING =
    new HttpContextToken<boolean>(() => false);
}
