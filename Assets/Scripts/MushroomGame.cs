using UnityEngine;
using UnityEngine.SceneManagement;

public class MushroomGame : MonoBehaviour
{
    public int target = 10;
    public int collected;
    public bool Won => collected >= target;
    public Camera view;
    public FirstPersonHands hands;
    bool collecting;
    readonly RaycastHit[] collectHits = new RaycastHit[32];
    MushroomPickup targetedMushroom;
    bool canCollect;
    GUIStyle labelStyle;
    GUIStyle instructionStyle;
    GUIStyle centerStyle;
    public bool CanCollect(out MushroomPickup mushroom)
    {
        mushroom = null;
        if (!view) return false;
        int count = Physics.SphereCastNonAlloc(view.transform.position, .22f, view.transform.forward, collectHits, 3f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        float nearest = float.PositiveInfinity;
        for (int i = 0; i < count; i++)
        {
            var hit = collectHits[i];
            if (!hit.collider || !hit.collider.enabled) continue;
            var candidate = hit.collider.GetComponentInParent<MushroomPickup>();
            if (candidate && hit.distance < nearest)
            {
                mushroom = candidate;
                nearest = hit.distance;
            }
        }
        return mushroom != null;
    }
    void Update()
    {
        if (!Won)
        {
            canCollect = CanCollect(out targetedMushroom);
            if (!collecting && Input.GetKeyDown(KeyCode.E) && canCollect)
                StartCoroutine(CollectMushroom(targetedMushroom));
        }
        if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if (!Won && Input.GetMouseButtonDown(0)) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    }
    System.Collections.IEnumerator CollectMushroom(MushroomPickup mushroom)
    {
        collecting = true;
        if (mushroom)
        {
            mushroom.DisableForCollection();
            MushroomCollectEffect.PlayAt(mushroom.transform.position + Vector3.up * 0.5f);
        }
        if (hands) yield return hands.PlayPickup();
        if (mushroom) mushroom.gameObject.SetActive(false);
        collected++;
        collecting = false;
        if (Won) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }
    void OnGUI()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 23, normal = { textColor = Color.white } };
            instructionStyle = new GUIStyle(labelStyle) { fontSize = 13, wordWrap = true };
            centerStyle = new GUIStyle(labelStyle) { alignment = TextAnchor.MiddleCenter, fontSize = 34 };
        }
        GUI.Box(new Rect(18,18,440,112), "");
        GUI.Label(new Rect(32,26,360,40), $"Грибы: {collected} / {target}", labelStyle);
        GUI.Label(new Rect(32,65,410,45), "WASD — ходить · Shift — бег · Space — прыжок · E — собрать", instructionStyle);
        if (!Won)
        {
            GUI.Label(new Rect(Screen.width/2-5,Screen.height/2-15,30,30), "+", labelStyle);
            if(canCollect) GUI.Label(new Rect(Screen.width/2-125,Screen.height/2+35,280,40), "E — собрать гриб", instructionStyle);
        }
        else
        {
            var box = new Rect(Screen.width/2-230,Screen.height/2-125,460,250);
            GUI.Box(box, "");
            GUI.Label(new Rect(box.x,box.y+25,460,70), "ПОБЕДА!", centerStyle);
            GUI.Label(new Rect(box.x,box.y+95,460,40), "Все грибы собраны", instructionStyle);
            if(GUI.Button(new Rect(box.x+120,box.y+165,220,50), "Играть снова")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
