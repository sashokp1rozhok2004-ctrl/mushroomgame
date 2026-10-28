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
    public bool CanCollect(out MushroomPickup mushroom)
    {
        mushroom = null;
        return view && Physics.Raycast(view.transform.position, view.transform.forward, out RaycastHit hit, 3f)
            && (mushroom = hit.collider.GetComponentInParent<MushroomPickup>()) != null;
    }
    void Update()
    {
        if (!Won && !collecting && Input.GetKeyDown(KeyCode.E) && CanCollect(out var mushroom))
        {
            StartCoroutine(CollectMushroom(mushroom));
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
        var label = new GUIStyle(GUI.skin.label) { fontSize = 23, normal = { textColor = Color.white } };
        GUI.Box(new Rect(18,18,390,105), "");
        GUI.Label(new Rect(32,26,360,40), $"Грибы: {collected} / {target}", label);
        GUI.Label(new Rect(32,65,365,45), "WASD — ходить · E — собрать", new GUIStyle(label) { fontSize = 18 });
        if (!Won)
        {
            GUI.Label(new Rect(Screen.width/2-5,Screen.height/2-15,30,30), "+", label);
            if(CanCollect(out _)) GUI.Label(new Rect(Screen.width/2-125,Screen.height/2+35,280,40), "E — собрать гриб", label);
        }
        else
        {
            var box = new Rect(Screen.width/2-230,Screen.height/2-125,460,250);
            GUI.Box(box, "");
            var center = new GUIStyle(label) { alignment=TextAnchor.MiddleCenter, fontSize=34 };
            GUI.Label(new Rect(box.x,box.y+25,460,70), "ПОБЕДА!", center);
            GUI.Label(new Rect(box.x,box.y+95,460,40), "Все грибы собраны", new GUIStyle(center){fontSize=22});
            if(GUI.Button(new Rect(box.x+120,box.y+165,220,50), "Играть снова")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
