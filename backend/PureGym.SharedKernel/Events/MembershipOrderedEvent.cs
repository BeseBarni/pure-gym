using System;
using System.Collections.Generic;
using System.Text;

namespace PureGym.SharedKernel.Events
{
    public record MembershipOrderedEvent
    (
        Guid MemberId,
        Guid MembershipId,
        DateTime OccurredOnUtc
    );
}
