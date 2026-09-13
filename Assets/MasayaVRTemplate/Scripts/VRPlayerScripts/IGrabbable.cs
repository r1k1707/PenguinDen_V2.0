using UnityEngine;

public interface IGrabbable
{
    public void GrabStart(IInputGrab controller);
    public void GrabEnd();
}
