using UnityEngine;

public class Activable : MonoBehaviour
{
	public virtual void BaseInteraction(float value) { }
	public virtual void LeftInteraction(bool click) { }
	public virtual void LeftInteraction(bool click, float value) { }
	public virtual void RightInteraction(bool click) { }
	public virtual void RightInteraction(bool click, float value) { }
}
