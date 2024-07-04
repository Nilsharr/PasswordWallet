import { LoginHistoryResponse } from '../contracts/responses/login-history-response';
import { LoginHistory } from '../models/login-history';
import { PaginatedList } from '../models/paginated-list';

export class LoginHistoryMapper {
  private static dtoToModel = (source: LoginHistoryResponse): LoginHistory => {
    return {
      id: source.id,
      date: new Date(source.date),
      correct: source.correct,
      ipAddress: source.ipAddress,
    };
  };

  public static toModel = (
    source: PaginatedList<LoginHistoryResponse>
  ): PaginatedList<LoginHistory> => {
    return {
      ...source,
      items: source.items.map(this.dtoToModel),
    };
  };
}
