using RAIDAChatNode.DTO;

namespace RAIDAChatNode.Reflections
{
    interface IReflectionActions
    {
        OutputSocketMessageWithUsers Execute(object val, string myLogin);
    }
}
