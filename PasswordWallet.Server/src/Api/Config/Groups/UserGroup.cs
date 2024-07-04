using FastEndpoints;

namespace Api.Config.Groups;

public sealed class UserGroup : Group
{
    public UserGroup()
    {
        Configure("users", ep => ep.Description(x => x.WithTags("Users")));
    }
}