# InjectAttribute
Simple Attribute and PropertyDrawer to replace RequireComponent in Unity.
This allows you to define a component field that can automatically be set from the game object it is on, it's children, or be manually assigned with a component from a different object in scene.

# Usage
Add [InjectComponent] to any serialized component field in a MonoBehaviour to auto assign this field with a component on the same GameObject.

You can use overloads to denote searching parents and children, disabled components / GameObjects, and search order.

# Examples 
```csharp
public class TestBehaviour : MonoBehaviour
{
	[InjectComponent][SerializeField]
	private AudioSource m_audioSource;
	[InjectComponent(SearchOptions.GameObjectAndChildren)] 
	public Collider myCollider;
	[InjectComponent(SearchOptions.GameObjectAndParents)] 
	public Collider myOtherCollider;
	[InjectComponent(SearchOptions.SearchAll | SearchOptions.AllowDisabled, SearchOrder.ParentsFirst)]
	public Rigidbody myBody;
	[InjectComponent]
	public int wrongType;
}
```
