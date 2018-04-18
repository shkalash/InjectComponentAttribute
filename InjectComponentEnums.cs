using System;
/// <summary>
/// Defines the GameObject hierarchy to search
/// </summary>
[Flags]
public enum SearchOptions
{
    GameObjectOnly = 1,
    GameObjectAndChildren = 2,
    GameObjectAndParents = 4,
    SearchAll = 6,
    AllowDisabled = 8
}
/// <summary>
/// Defines the hierarchy seacrh order
/// When combining SearchOptions this will dictate the order of search
/// </summary>
public enum SearchOrder
{
    ChildrenFirst = 1,
    ParentsFirst = 2
}