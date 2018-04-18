using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectComponentAttribute : PropertyAttribute 
{
    public bool SearchChildren { get; private set; }
    public bool SearchParents { get; private set; }
    public bool SearchAll { get; private set; }
    public bool AllowDisabled { get; private set; }
    public SearchOrder SearchOrder { get; private set; }

    /// <summary>
    /// Property drawer that will inject the required component on to this field
    /// </summary>
    /// <param name="searchOptions">Object hierarchy to search</param>
    /// <param name="searchOrder">Order of search</param>
    public InjectComponentAttribute(SearchOptions searchOptions = SearchOptions.GameObjectOnly ,
        SearchOrder searchOrder = SearchOrder.ChildrenFirst)
    {
        SearchOrder = searchOrder;
        SearchChildren = (searchOptions & SearchOptions.GameObjectAndChildren) != 0;
        SearchParents = (searchOptions & SearchOptions.GameObjectAndParents) != 0;
        AllowDisabled = (searchOptions & SearchOptions.AllowDisabled) != 0;
        SearchAll = (searchOptions & SearchOptions.SearchAll) == SearchOptions.SearchAll;
    }
}
