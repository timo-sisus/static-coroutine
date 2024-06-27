# StaticCoroutine
Class that can be used to start coroutines from anywhere without needing a reference to a mono behaviour instance.

# Usage Examples
```csharp
public class Example // <- class does not need to derive from MonoBehaviour
{
	public void SayHelloDelayed()
	{
		StaticCoroutine.Start(SayHelloAfterSeconds(1f));
	}

	static IEnumerator SayHelloAfterSeconds(float delay) // <- coroutine can be static
	{
		yield return new WaitForSeconds(delay);
		Debug.Log("Hello!");
	}
}
```
