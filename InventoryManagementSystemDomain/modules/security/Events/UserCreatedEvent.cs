using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationCore.Events
{
    public record UserCreatedEvent(Guid UserId, string Email, string? Username): IDomainEvent;
}
