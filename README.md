# InjectAttribute
Simple Attribute and PropertyDrawer to replace RequireComponent in Unity.
This allows you to define a component field that can automatically be set from the game object it is on, it's children, or be manually assigned with a component from a different object in scene.

# Usage
Add [InjectComponent] to any serialized component field in a MonoBehaviour to auto assign this field with a component on the same GameObject.

Add [InjectComponent(true)] to also search in children of the game object.

# Example 
```csharp
public class TestBehaviour : MonoBehaviour
{
	[InjectComponent][SerializeField]
	private AudioSource m_audioSource;
	[InjectComponent(true)]
	public Collider myCollider;
	[InjectComponent] // Will produce a warning in the console
	public int wrongType;
}
```
