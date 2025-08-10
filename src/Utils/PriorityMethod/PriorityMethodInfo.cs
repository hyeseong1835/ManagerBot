using System.Reflection;

namespace ManagerBot.Utils.PriorityMethod;

public struct PriorityMethodInfo
{
    public MethodInfo info;
    public PriorityMethodAttribute attribute;

    public PriorityMethodInfo(MethodInfo method, PriorityMethodAttribute attribute)
    {
        this.info = method;
        this.attribute = attribute;
    }
}


public struct PriorityMethodInfo<TParam>
{
    public MethodInfo info;
    public PriorityMethodAttribute<TParam> attribute;

    public PriorityMethodInfo(MethodInfo method, PriorityMethodAttribute<TParam> attribute)
    {
        this.info = method;
        this.attribute = attribute;
    }
}