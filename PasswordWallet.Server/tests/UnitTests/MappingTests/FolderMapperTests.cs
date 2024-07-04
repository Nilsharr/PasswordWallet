using Api.Endpoints.v1.Folders;
using Api.Endpoints.v1.Folders.AddFolder;
using Core.Entities;
using FluentAssertions;

namespace UnitTests.MappingTests;

public class FolderMapperTests
{
    private readonly FolderMapper _mapper = new();

    [Fact]
    public void MapFolderToResponse_Always_ShouldMapProperties()
    {
        var folder = new Folder
        {
            Id = Guid.NewGuid(),
            Name = "General",
            Position = 1
        };

        var result = _mapper.FromEntity(folder);

        result.Should().NotBeNull();
        result.Id.Should().Be(folder.Id);
        result.Name.Should().Be(folder.Name);
        result.Position.Should().Be(folder.Position);
    }

    [Fact]
    public void MapFolderCollectionToResponse_Always_ShouldMapProperties()
    {
        List<Folder> folders =
        [
            new Folder { Id = Guid.NewGuid(), Name = "General", Position = 1 },
            new Folder { Id = Guid.NewGuid(), Name = "Misc", Position = 2 },
            new Folder { Id = Guid.NewGuid(), Name = "Other", Position = 3 }
        ];

        var result = _mapper.FromEntities(folders).ToList();

        result.Should().NotBeNull();
        result.Should().HaveCount(folders.Count);

        foreach (var (folderResponse, folder) in result.Zip(folders))
        {
            folderResponse.Id.Should().Be(folder.Id);
            folderResponse.Name.Should().Be(folder.Name);
            folderResponse.Position.Should().Be(folder.Position);
        }
    }

    [Fact]
    public void MapAddFolderRequestToEntity_Always_ShouldMapProperties()
    {
        var folderRequest = new AddFolderRequest("General", 5);

        var result = _mapper.ToEntity(folderRequest);

        result.Should().NotBeNull();
        result.UserId.Should().Be(folderRequest.UserId);
        result.Name.Should().Be(folderRequest.Name);
    }
}