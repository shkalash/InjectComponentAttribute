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
		bool shouldSearchChildren = ((InjectComponentAttribute) attribute).searchChildren;
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
				singleProperty.objectReferenceValue = shouldSearchChildren ? 
					target.GetComponentInChildren(fieldInfo.FieldType) : 
					target.GetComponent(fieldInfo.FieldType);
				serObjectInner.ApplyModifiedProperties();
			}
			// TODO:
			// here we would like to update the serialized object with the new changes.
			// but we don't know if there are other changes pending.
			// so for now, when multi editing, the inspector will be wrong and show just the 
			// initial property value
		}
		else
		{
			// handle just the one target
			property.objectReferenceValue = shouldSearchChildren ? 
				((Component) serObject.targetObject).GetComponentInChildren(fieldInfo.FieldType) : 
				((Component) serObject.targetObject).GetComponent(fieldInfo.FieldType);
		}
	}

	
}
