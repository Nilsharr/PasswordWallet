import { FolderResponse } from '../contracts/responses/folder-response';
import { Folder } from '../models/folder';

export class FolderMapper {
  public static toModel = (source: FolderResponse): Folder => {
    return {
      id: source.id,
      name: source.name,
      position: source.position,
    };
  };
}
