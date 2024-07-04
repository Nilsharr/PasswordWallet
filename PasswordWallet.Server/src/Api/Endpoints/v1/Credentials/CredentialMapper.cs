using Api.Endpoints.v1.Credentials.AddCredential;
using Api.Endpoints.v1.Credentials.UpdateCredential;
using Core.Entities;
using Core.Models;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials;

public class CredentialMapper : ResponseMapper<CredentialResponse, Credential>
{
    public override CredentialResponse FromEntity(Credential credential) => new(credential.Id,
        credential.Username, credential.WebAddress, credential.Description, credential.Position);

    public PaginatedList<CredentialResponse> FromEntities(PaginatedList<Credential> paginatedList) => new(
        paginatedList.PageNumber, paginatedList.PageSize, paginatedList.TotalCount,
        FromEntities(paginatedList.Items).ToList());

    public Credential ToEntity(AddCredentialRequest req) => new()
    {
        Username = req.Username,
        Password = req.Password,
        WebAddress = req.WebAddress,
        Description = req.Description,
        FolderId = req.FolderId
    };

    public Credential ToEntity(UpdateCredentialRequest req) => new()
    {
        Id = req.CredentialId,
        Username = req.Username,
        Password = req.Password,
        WebAddress = req.WebAddress,
        Description = req.Description
    };

    private static IEnumerable<CredentialResponse> FromEntities(IEnumerable<Credential> credentials) =>
        credentials.Select(c => new CredentialResponse(c.Id, c.Username, c.WebAddress, c.Description, c.Position));
}