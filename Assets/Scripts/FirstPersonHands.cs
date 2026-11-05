using System.Collections;
using UnityEngine;

public class FirstPersonHands : MonoBehaviour
{
    public Transform leftHand, rightHand;
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
            foreach (var renderer in handsRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                renderer.enabled = true;
                renderer.updateWhenOffscreen = true;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            leftHand = FindDescendant(handsRoot, "forearm.L") ?? FindDescendant(handsRoot, "upper_arm.L") ?? FindDescendant(handsRoot, "hand.L") ?? leftHand;
            rightHand = FindDescendant(handsRoot, "forearm.R") ?? FindDescendant(handsRoot, "upper_arm.R") ?? FindDescendant(handsRoot, "hand.R") ?? rightHand;
        }
        if (!leftHand || !rightHand) return;
        leftRot=leftHand.localRotation; rightRot=rightHand.localRotation;
    }
    public IEnumerator PlayPickup()
    {
        if (!leftHand || !rightHand) yield break;
        float t=0;
        while(t<1f)
        {
            t+=Time.deltaTime/.22f; float k=Mathf.SmoothStep(0,1,t);
            leftHand.localRotation=Quaternion.Slerp(leftRot,leftRot*Quaternion.Euler(-45f,10f,-15f),k);
            rightHand.localRotation=Quaternion.Slerp(rightRot,rightRot*Quaternion.Euler(-45f,-10f,15f),k);
            yield return null;
        }
        t=0;
        while(t<1f)
        {
            t+=Time.deltaTime/.35f; float k=Mathf.SmoothStep(0,1,t);
            leftHand.localRotation=Quaternion.Slerp(leftRot*Quaternion.Euler(-45f,10f,-15f),leftRot,k);
            rightHand.localRotation=Quaternion.Slerp(rightRot*Quaternion.Euler(-45f,-10f,15f),rightRot,k);
            yield return null;
        }
    }
}
