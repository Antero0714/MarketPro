using MediatR;
using MarketPro.Application.Interfaces.Services;

namespace MarketPro.Application.Features.Auth.Command.Register
{
    public class RegisterCommandHandler(IAuthService _authService) : IRequestHandler<RegisterCommand, bool>
    {
        public async Task<bool> Handle(RegisterCommand command, CancellationToken cancellationToken) =>
            await _authService.RegisterAsync(command.FirstName, command.LastName, command.Email, command.Password);
    }
}
