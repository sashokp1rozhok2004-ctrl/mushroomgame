using System.Collections;
using UnityEngine;

public class FirstPersonHands : MonoBehaviour
{
    public Transform leftHand, rightHand;
    Vector3 leftHome, rightHome;
    Quaternion leftRot, rightRot;
    Transform handsRoot;

    Transform FindDescendant(Transform root, string wantedName)
    {
        if (!root) return null;
        if (root.name == wantedName) return root;
        for (int i = 0; i < root.childCount; i++)
        {
            var found = FindDescendant(root.GetChild(i), wantedName);
            if (found) return found;
        }
        return null;
    }

    void Start()
    {
        if (leftHand != null)
        {
            var root = leftHand;
            while (root.parent && root.name != "Hands") root = root.parent;
            if (root.name == "Hands") handsRoot = root;
        }
        if (!handsRoot && rightHand != null)
        {
            var root = rightHand;
            while (root.parent && root.name != "Hands") root = root.parent;
            if (root.name == "Hands") handsRoot = root;
        }
        if (handsRoot)
        {
            handsRoot.localPosition = new Vector3(0f, -0.18f, 0.48f);
            handsRoot.localScale = Vector3.one * 0.48f;
            foreach (var renderer in handsRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                renderer.enabled = true;
                renderer.updateWhenOffscreen = true;
            }
            leftHand = FindDescendant(handsRoot, "hand.L") ?? FindDescendant(handsRoot, "forearm.L") ?? leftHand;
            rightHand = FindDescendant(handsRoot, "hand.R") ?? FindDescendant(handsRoot, "forearm.R") ?? rightHand;
        }
        if (!leftHand || !rightHand) return;
        leftHome=leftHand.localPosition; rightHome=rightHand.localPosition;
        leftRot=leftHand.localRotation; rightRot=rightHand.localRotation;
    }
    public IEnumerator PlayPickup()
    {
        if (!leftHand || !rightHand) yield break;
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
