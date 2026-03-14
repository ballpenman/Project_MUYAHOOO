using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void SetTriggerParameter(string parameter)
    {
        anim.SetTrigger(parameter);
    }
    public void SetBoolParameter(string parameter, bool value)
    {
        anim.SetBool(parameter,value);
    }
    public void SetFloatParameter(string parameter, float value)
    {
        anim.SetFloat(parameter,value);
    }
}
