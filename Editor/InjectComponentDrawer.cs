using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(InjectComponentAttribute),true)]
public class InjectComponentDrawer : PropertyDrawer 
{
	// Property Drawer gets instantiated everytime a major change happens to the inspector
	// including adding / removing components
	// this flag will help us not slow down the editor with tons of GetComponent calls
	private bool m_didSearch;
	// easy accessor to the casted version
	private InjectComponentAttribute m_castedAttribute;
	// cache the component type
	private static readonly Type ComponentType = typeof(Component);
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Draw the property itself
		EditorGUI.PropertyField(position, property, label);
		if (m_didSearch)
		{
			return;
		}
		// Make sure this is a component field
		if (!ComponentType.IsAssignableFrom(fieldInfo.FieldType))
		{
			Debug.LogWarningFormat("InjectComponent attribute assigned to non component field\n{0}->{1}",
				property.serializedObject.targetObject.name , fieldInfo.Name);
			// Stop further processing and spamming of warnings
			m_didSearch = true;
			return;
		}
		// Check if we should try and edit the value
		// but only if there isn't a component already assigned
		if (!property.editable || property.objectReferenceValue != null)
		{
			return;
		}
		// Make sure we don't constantly use GetComponent
		m_didSearch = true;
		m_castedAttribute = ((InjectComponentAttribute) attribute);
		// Grab the serialized object
		SerializedObject serObject = property.serializedObject;
		if (serObject.isEditingMultipleObjects)
		{
			// To add support for multi edit , we have to do some work
			// as we have one property object for all targets
			foreach (Object serObjectTarget in serObject.targetObjects)
			{
				// So we will reconstruct the serializedObject/Property
				// for each type and apply the correct component to all
				Component target = (Component) serObjectTarget;
				SerializedObject serObjectInner = new SerializedObject(target);
				SerializedProperty singleProperty = serObjectInner.FindProperty(property.propertyPath);
				// assing the correct component based on the field and attribute
				HandleTarget(singleProperty, target, fieldInfo.FieldType);
				serObjectInner.ApplyModifiedProperties();
			}
			serObject.SetIsDifferentCacheDirty();
		}
		else
		{
			// handle just the one target
			HandleTarget(property, ((Component) serObject.targetObject), fieldInfo.FieldType);
		}
	}

	private void HandleTarget(SerializedProperty property,
		Component targetObject, Type injectType)
	{
		//first search the game object itself.
		Component component = targetObject.GetComponent(injectType);
		if (component != null)
		{
			property.objectReferenceValue = component;
			return;
		}
		// next if we search all, do it by order
		if (m_castedAttribute.SearchAll)
		{
			switch (m_castedAttribute.SearchOrder)
			{
				case SearchOrder.ChildrenFirst:
				{
					if (HandleChildren(property, targetObject, injectType))
					{
						return;
					}
					HandleParents(property, targetObject, injectType);
					return;
				}
				case SearchOrder.ParentsFirst:
				{
					if (HandleParents(property, targetObject, injectType))
					{
						return;
					}
					HandleChildren(property, targetObject, injectType);
					return;
				}
			}
		}
		else // otherwise just search the wanted direction
		{
			if (m_castedAttribute.SearchChildren)
			{
				HandleChildren(property, targetObject, injectType);
				return;
			}

			if (!m_castedAttribute.SearchParents)
			{
				return;
			}
			HandleParents(property, targetObject, injectType);
		}
	}

	private bool HandleChildren(SerializedProperty property,
		Component targetObject, Type injectType)
	{
		property.objectReferenceValue = targetObject.GetComponentInChildren(injectType, 
																			m_castedAttribute.AllowDisabled);
		return property.objectReferenceValue != null;
	}

	private bool HandleParents(SerializedProperty property,
		Component targetObject, Type injectType)
	{
		// Get component in parent doesn't allow disabled search
		if (m_castedAttribute.AllowDisabled)
		{
			var components = targetObject.GetComponentsInParent(injectType, true);
			if (components.Length <= 0)
				return false;
			property.objectReferenceValue = components[0];
			return true;
		}

		property.objectReferenceValue = targetObject.GetComponentInParent(injectType);
		return property.objectReferenceValue != null;
	}
	
}
