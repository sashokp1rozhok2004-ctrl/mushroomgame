using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayer : MonoBehaviour
{
    public Transform head;
    public MushroomGame game;
    public float speed = 4f;
    public float runMultiplier = 1.8f;
    public float jumpHeight = 1.2f;
    public float gravity = -22f;
    public float playableHalfSize = 72f;
    public float sensitivity = 2f;
    float pitch, vertical;
    CharacterController controller;
    Vector3 spawnPoint;
    void Start()
    {
        controller=GetComponent<CharacterController>();
        if (controller == null) controller = gameObject.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = Vector3.up * 0.9f;
        spawnPoint = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.position = spawnPoint;
        var ground = GameObject.Find("Ground");
        if (ground != null)
        {
            var meshCollider = ground.GetComponent<MeshCollider>();
            if (meshCollider != null) meshCollider.enabled = false;
        }
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
        if (transform.position.y < -10f)
        {
            controller.enabled = false;
            transform.position = spawnPoint;
            vertical = 0f;
            controller.enabled = true;
        }
        if(Input.GetMouseButtonDown(0)) LockCursor();
        // Movement must remain available while the Game view is acquiring mouse focus.
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            transform.Rotate(0,Input.GetAxis("Mouse X")*sensitivity,0);
            pitch=Mathf.Clamp(pitch-Input.GetAxis("Mouse Y")*sensitivity,-85,85);
            if(head != null) head.localRotation=Quaternion.Euler(pitch,0,0);
        }
        bool grounded = controller.isGrounded;
        if (grounded && vertical < 0f) vertical = -2f;
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.A)) input.x = -1f;
        if (Input.GetKey(KeyCode.D)) input.x = 1f;
        if (Input.GetKey(KeyCode.S)) input.y = -1f;
        if (Input.GetKey(KeyCode.W)) input.y = 1f;
        var move=Vector3.ClampMagnitude(transform.right*input.x+transform.forward*input.y,1)*speed;
        if(Input.GetKey(KeyCode.LeftShift)) move*=runMultiplier;
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            vertical = Mathf.Sqrt(jumpHeight * -2f * gravity);
        vertical += gravity * Time.deltaTime;
        controller.Move((move + Vector3.up * vertical) * Time.deltaTime);

        // Keep the player inside the designed forest clearing even if a tree mesh has a gap.
        Vector3 p = transform.position;
        Vector2 flat = Vector2.ClampMagnitude(new Vector2(p.x, p.z), playableHalfSize);
        if (!Mathf.Approximately(p.x, flat.x) || !Mathf.Approximately(p.z, flat.y))
        {
            controller.enabled = false;
            transform.position = new Vector3(flat.x, p.y, flat.y);
            controller.enabled = true;
        }
    }
}

