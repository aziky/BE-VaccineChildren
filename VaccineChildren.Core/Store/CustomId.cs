namespace VaccineChildren.Core.Store;

[AttributeUsage(AttributeTargets.Field)]
public class CustomId : Attribute
{
    public int Id { get; }

    public CustomId(int id)
    {
        Id = id;
    }
}