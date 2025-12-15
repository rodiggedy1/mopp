using Application.Common.Interfaces;
using Application.Identity;

namespace Infrastructure.MessageBroker
{
    public class ConsumerUserProvider : ICurrentUserService
    {
        private readonly IIdentityContextAccessor _identityContextAccessor;

        public ConsumerUserProvider(IIdentityContextAccessor identityContextAccessor)
        {
            _identityContextAccessor = identityContextAccessor;
        }
        public int? UserId
        {
            get
            {
                return _identityContextAccessor.IdentityContext?.CurrentUser?.Id;
            }
        }

        public bool? IsAdmin
        {
            get
            {
                return _identityContextAccessor.IdentityContext?.CurrentUser?.IsAdmin;
            }
        }
    }
}
