using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPro.Application.Features.Auth.Query.Login
{
    public record LoginQuery(string email, string password) : IRequest<bool>;
}
