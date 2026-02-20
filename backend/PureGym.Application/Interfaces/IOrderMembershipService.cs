using PureGym.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PureGym.Application.Interfaces
{
    public interface IOrderMembershipService
    {
        Task<Result<Guid>> OrderAsync(Guid memberId, Guid membershipTypeId, CancellationToken ct);
    }
}
