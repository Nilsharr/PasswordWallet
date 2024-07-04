using Api.Config.Groups;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.User.UsernameAvailability;

public class UsernameAvailabilityEndpoint(IUnitOfWork unitOfWork)
    : Endpoint<UsernameAvailabilityRequest, UsernameAvailabilityResponse>
{
    public override void Configure()
    {
        Get("/availability/{username}");
        AllowAnonymous();
        Group<UserGroup>();
    }

    public override async Task HandleAsync(UsernameAvailabilityRequest req, CancellationToken ct)
    {
        var userExists = await unitOfWork.UserRepository.ExistsWithUsername(req.UserName, ct);
        var response = new UsernameAvailabilityResponse(!userExists);
        await SendOkAsync(response, ct);
    }
}