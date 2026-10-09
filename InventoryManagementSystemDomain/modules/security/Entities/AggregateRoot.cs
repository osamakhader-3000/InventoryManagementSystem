using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationCore.Models
{
    public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
    {

        protected AggregateRoot(TId id) : base(id) { }

    }
}
