using UnityEngine;

public interface IInputGrab
{
    public void GrabEnd();
    public void GrabGone(bool removeFromList, Transform obj);
    public ThrowValues GetThrowValues();
}

public interface IInputInteract
{
    public void InteractFinish(bool removeFromList);
}
