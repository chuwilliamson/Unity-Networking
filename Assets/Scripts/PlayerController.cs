using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{

    public GameObject _camera;
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 move = Vector3.zero;
    private CharacterController controller;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletPower = 15.0F;
    public float m_GroundCheckDistance = 0.1f;
    public Vector3 m_GroundNormal;
    void CheckGround()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
        }
    }
    public override void OnStartLocalPlayer()
    {
        controller = GetComponent<CharacterController>();
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void Start()
    {
        if(!isLocalPlayer)
        _camera.SetActive(false);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        CheckGround();        
        
        move = Camera.main.transform.TransformDirection(input);
        move.y = 0;

        if (move.magnitude > 0)
        {
            var target_rotation = Quaternion.LookRotation(move, transform.up);
            var new_rotation = Quaternion.Slerp(transform.rotation, target_rotation, 6.0f);
            transform.rotation = new_rotation;
        }

        if (controller.isGrounded)
        {
            move *= speed;
            if (Input.GetButton("Jump"))
                move.y = jumpSpeed;
        }

        move.y -= gravity * Time.deltaTime;
        controller.Move(move * Time.deltaTime);

        if (Input.GetButtonDown("Fire1"))
            CmdFire();
    }


    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletPower;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}


