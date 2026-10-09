using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IEventEmitter
    {
        Task EmitEvent(string eventName, string eventData);
    }
}
