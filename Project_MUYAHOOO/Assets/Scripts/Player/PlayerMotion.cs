using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    [SerializeField] GameObject body;
    Animator anim;
    public SpriteRenderer spriteRenderer { get; private set; }

    private void Start()
    {
        anim = body.GetComponent<Animator>();
        spriteRenderer = body.GetComponent<SpriteRenderer>();
    }

    public void Flip(float value)
    {
        if(value == 0) return;
        spriteRenderer.flipX = value<0;
    }
    public void PlayAnim(string parameter,int layer,float percent)
    {
        anim.Play(parameter,layer,percent);
    }
    public void ResetTriggerParameter(string parameter)
    {
        anim.ResetTrigger(parameter);
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

    public void RotateBody(float angle)
    {
        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 5f);
    }
}
