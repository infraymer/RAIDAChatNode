using System;
using RAIDAChatNode.DTO;

namespace RAIDAChatNode.Reflections
{
    interface IReflectionActions
    {
        OutputSocketMessageWithUsers Execute(object val, string myLogin, Guid actId);
    }
}
