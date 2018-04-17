using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectComponentAttribute : PropertyAttribute 
{
    public bool searchChildren { get; private set; }
    /// <summary>
    /// Property drawer that will inject the required component on to this field
    /// </summary>
    /// <param name="shouldSearchInChildren">should the component be searched in children of this object</param>
    public InjectComponentAttribute(bool shouldSearchInChildren = false)
    {
        this.searchChildren = shouldSearchInChildren;
    }
}
