public interface IHover
{
    HoverType GetHoverType { get; }
}

public enum HoverType
{
    None = 0,
    Type1 = 1,
    Type2 = 2
}