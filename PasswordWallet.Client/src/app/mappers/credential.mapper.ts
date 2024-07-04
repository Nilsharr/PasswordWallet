import { CredentialRequest } from '../contracts/requests/credential-request';
import { CredentialResponse } from '../contracts/responses/credential-response';
import { Credential } from '../models/credential';
import { PaginatedList } from '../models/paginated-list';

export class CredentialMapper {
  public static dtoToModel = (source: CredentialResponse): Credential => {
    return {
      id: source.id,
      username: source.username,
      hiddenPassword: '*********',
      webAddress: source.webAddress,
      description: source.description,
      position: source.position,
    };
  };

  public static toModel = (
    source: PaginatedList<CredentialResponse>
  ): PaginatedList<Credential> => {
    return {
      pageNumber: source.pageNumber,
      pageSize: source.pageSize,
      totalCount: source.totalCount,
      items: source.items.map(this.dtoToModel),
    };
  };

  public static toDto = (source: Credential): CredentialRequest => {
    return {
      username: source.username,
      password: source.password,
      webAddress: source.webAddress,
      description: source.description,
    };
  };
}
