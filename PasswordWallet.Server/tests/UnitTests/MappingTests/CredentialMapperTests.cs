using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.AddCredential;
using Api.Endpoints.v1.Credentials.UpdateCredential;
using Core.Entities;
using Core.Models;
using FluentAssertions;

namespace UnitTests.MappingTests;

public class CredentialMapperTests
{
    private readonly CredentialMapper _mapper = new();

    [Fact]
    public void MapCredentialToResponse_Always_ShouldMapProperties()
    {
        var credential = new Credential
        {
            Id = 1,
            Password = "pass123",
            Username = "test_user",
            WebAddress = "https://example.com",
            Description = "Test credential",
            Position = 1
        };

        var result = _mapper.FromEntity(credential);

        result.Should().NotBeNull();
        result.Id.Should().Be(credential.Id);
        result.Username.Should().Be(credential.Username);
        result.WebAddress.Should().Be(credential.WebAddress);
        result.Description.Should().Be(credential.Description);
        result.Position.Should().Be(credential.Position);
    }

    [Fact]
    public void MapCredentialPaginatedListToResponse_Always_ShouldMapProperties()
    {
        const int pageNumber = 1;
        const int pageSize = 20;
        const int totalCount = 50;
        List<Credential> credentials =
        [
            new Credential
            {
                Id = 1, Username = "user1", WebAddress = "https://example1.com", Description = "Cred1", Position = 1
            },
            new Credential
            {
                Id = 2, Username = "user2", WebAddress = "https://example2.com", Description = "Cred2", Position = 2
            },
            new Credential
                { Id = 3, Username = "user3", WebAddress = "https://example3.com", Description = "Cred3", Position = 3 }
        ];
        var paginatedList = new PaginatedList<Credential>(pageNumber, pageSize, totalCount, credentials);

        var result = _mapper.FromEntities(paginatedList);

        result.Should().NotBeNull();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(totalCount);
        result.HasNextPage.Should().BeTrue();
        result.Items.Should().HaveCount(credentials.Count);

        foreach (var (credentialResponse, credential) in result.Items.Zip(credentials))
        {
            credentialResponse.Id.Should().Be(credential.Id);
            credentialResponse.Username.Should().Be(credential.Username);
            credentialResponse.WebAddress.Should().Be(credential.WebAddress);
            credentialResponse.Description.Should().Be(credential.Description);
            credentialResponse.Position.Should().Be(credential.Position);
        }
    }

    [Fact]
    public void MapCredentialPaginatedListToResponse_EmptyList_ShouldReturnEmptyResponseList()
    {
        const int pageNumber = 1;
        const int pageSize = 20;
        const int totalCount = 0;
        var paginatedList = new PaginatedList<Credential>(pageNumber, pageSize, totalCount, Array.Empty<Credential>());

        var result = _mapper.FromEntities(paginatedList);

        result.Should().NotBeNull();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(totalCount);
        result.HasNextPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public void MapAddCredentialRequestToEntity_Always_ShouldMapProperties()
    {
        var credentialRequest = new AddCredentialRequest(Guid.NewGuid(), "test_user", "pass123", "https://example.com",
            "Test credential", 8);

        var result = _mapper.ToEntity(credentialRequest);

        result.Should().NotBeNull();
        result.Username.Should().Be(credentialRequest.Username);
        result.Password.Should().Be(credentialRequest.Password);
        result.WebAddress.Should().Be(credentialRequest.WebAddress);
        result.Description.Should().Be(credentialRequest.Description);
        result.FolderId.Should().Be(credentialRequest.FolderId);
    }

    [Fact]
    public void MapUpdateCredentialRequestToEntity_Always_ShouldMapProperties()
    {
        var credentialRequest =
            new UpdateCredentialRequest(34, "test_user", "pass123", "https://example.com", "Test credential", 9);

        var result = _mapper.ToEntity(credentialRequest);

        result.Should().NotBeNull();
        result.Id.Should().Be(credentialRequest.CredentialId);
        result.Username.Should().Be(credentialRequest.Username);
        result.Password.Should().Be(credentialRequest.Password);
        result.WebAddress.Should().Be(credentialRequest.WebAddress);
        result.Description.Should().Be(credentialRequest.Description);
    }
}