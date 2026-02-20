using System;
using System.Collections.Generic;
using System.Text;

namespace PureGym.Domain.Entities
{
    public class MemberOrder
    {
        public Guid Id { get; private set; }
        public Guid MemberId { get; private set; }
        public Guid MembershipId { get; private set; }
        public DateTime OrderedAtUtc { get; private set; }
            public Guid MembershipTypeId { get; private set; }


        public Member Member { get; private set; } = null!;
        public Membership Membership { get; private set; } = null!;
        public MembershipType MembershipType { get; private set; } = null!;

        private MemberOrder() { }

        public static MemberOrder Create(Guid memberId, Guid membershipId, Guid membershipTypeId)
          => new MemberOrder
          {
              Id = Guid.NewGuid(),
              MemberId = memberId,
              MembershipId = membershipId,
              MembershipTypeId = membershipTypeId,
              OrderedAtUtc = DateTime.UtcNow
          };
    }
    
}
