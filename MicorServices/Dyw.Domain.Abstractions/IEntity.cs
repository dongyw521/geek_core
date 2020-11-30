using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Domain.Abstractions
{
    public interface IEntity
    {
        object[] GetKeys();
    }

    public interface IEntity<TPrimaryKey> : IEntity
    {
        TPrimaryKey Id { get; }
    }
}
