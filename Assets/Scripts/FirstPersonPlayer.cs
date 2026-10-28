using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayer : MonoBehaviour
{
    public Transform head;
    public MushroomGame game;
    public float speed = 4f;
    public float runMultiplier = 1.8f;
    public float jumpHeight = 1.2f; // Reduced from 2.2f - more realistic jump, won't fly above trees
    public float sensitivity = 2f;
    float pitch, vertical;
    CharacterController controller;
    Vector3 spawnPoint;
    const float FloorHeight = 1.0f;
    void Start()
    {
        controller=GetComponent<CharacterController>();
        if (controller == null) controller = gameObject.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = Vector3.up * 0.9f;
        spawnPoint = new Vector3(transform.position.x, 1.1f, transform.position.z);
        transform.position = spawnPoint;
        var ground = GameObject.Find("Ground");
        if (ground != null)
        {
            var meshCollider = ground.GetComponent<MeshCollider>();
            if (meshCollider != null) meshCollider.enabled = false;
        }
        var oldFloors = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var oldFloor in oldFloors)
            if (oldFloor.name == "RuntimeGroundCollider") Destroy(oldFloor);
        var floor = new GameObject("RuntimeGroundCollider");
        var floorCollider = floor.AddComponent<BoxCollider>();
        floorCollider.center = new Vector3(0f, -1f, 0f);
        floorCollider.size = new Vector3(500f, 2f, 500f);
        if (game == null) game = GetComponent<MushroomGame>();
        if (head == null)
        {
            var camera = GetComponentInChildren<Camera>();
            if (camera != null) head = camera.transform.parent != null ? camera.transform.parent : camera.transform;
        }
        LockCursor();
    }
    void LockCursor() { Cursor.lockState=CursorLockMode.Locked; Cursor.visible=false; }
    void Update()
    {
        if(game != null && game.Won) return;
        if (transform.position.y < FloorHeight)
        {
            controller.enabled = false;
            transform.position = new Vector3(transform.position.x, FloorHeight, transform.position.z);
            vertical = 0f;
            controller.enabled = true;
        }
        if(Input.GetMouseButtonDown(0)) LockCursor();
        // Movement must remain available while the Game view is acquiring mouse focus.
        transform.Rotate(0,Input.GetAxis("Mouse X")*sensitivity,0);
        pitch=Mathf.Clamp(pitch-Input.GetAxis("Mouse Y")*sensitivity,-85,85);
        if(head != null) head.localRotation=Quaternion.Euler(pitch,0,0);
        bool grounded=true;
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.A)) input.x = -1f;
        if (Input.GetKey(KeyCode.D)) input.x = 1f;
        if (Input.GetKey(KeyCode.S)) input.y = -1f;
        if (Input.GetKey(KeyCode.W)) input.y = 1f;
        var move=Vector3.ClampMagnitude(transform.right*input.x+transform.forward*input.y,1)*speed;
        if(Input.GetKey(KeyCode.LeftShift)) move*=runMultiplier;
        if(Input.GetKeyDown(KeyCode.Space)) vertical=Mathf.Sqrt(jumpHeight*-2f*Physics.gravity.y);
        vertical+=Physics.gravity.y*Time.deltaTime;
        var position = transform.position + move * Time.deltaTime;
        position.y += vertical * Time.deltaTime;
        if (position.y < FloorHeight) { position.y = FloorHeight; vertical = 0f; }
        transform.position = position;
    }
}

