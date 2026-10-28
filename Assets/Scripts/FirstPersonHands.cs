using System.Collections;
using UnityEngine;

public class FirstPersonHands : MonoBehaviour
{
    public Transform leftHand, rightHand;
    Vector3 leftHome, rightHome;
    Quaternion leftRot, rightRot;
    void Start()
    {
        leftHome=leftHand.localPosition; rightHome=rightHand.localPosition;
        leftRot=leftHand.localRotation; rightRot=rightHand.localRotation;
    }
    public IEnumerator PlayPickup()
    {
        float t=0;
        while(t<1f)
        {
            t+=Time.deltaTime/.22f; float k=Mathf.SmoothStep(0,1,t);
            leftHand.localPosition=Vector3.Lerp(leftHome,leftHome+new Vector3(.05f,.16f,-.22f),k);
            rightHand.localPosition=Vector3.Lerp(rightHome,rightHome+new Vector3(-.05f,.16f,-.22f),k);
            leftHand.localRotation=Quaternion.Slerp(leftRot,Quaternion.Euler(-38,12,-18),k);
            rightHand.localRotation=Quaternion.Slerp(rightRot,Quaternion.Euler(-38,-12,18),k);
            yield return null;
        }
        t=0;
        while(t<1f)
        {
            t+=Time.deltaTime/.35f; float k=Mathf.SmoothStep(0,1,t);
            leftHand.localPosition=Vector3.Lerp(leftHome+new Vector3(.05f,.16f,-.22f),leftHome,k);
            rightHand.localPosition=Vector3.Lerp(rightHome+new Vector3(-.05f,.16f,-.22f),rightHome,k);
            leftHand.localRotation=Quaternion.Slerp(Quaternion.Euler(-38,12,-18),leftRot,k);
            rightHand.localRotation=Quaternion.Slerp(Quaternion.Euler(-38,-12,18),rightRot,k);
            yield return null;
        }
    }
}
