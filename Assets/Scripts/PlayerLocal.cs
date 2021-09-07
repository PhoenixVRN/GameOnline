using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerLocal : MonoBehaviour
{
    public Transform gun;
    public Rigidbody2D rb;
    public float speed, jump;
    public GameObject bullet;
    public float cooldown;
    public float time;
    public Transform bulletPoint;
    public Camera camera;
    public FixedJoystick joystickMove;
    public FixedJoystick joystickGun;
    public Button button;

    Vector3 LocalScale = Vector3.one;
    Vector3 LocalScaleGun = Vector3.one;



    public bool onGround;

    private Vector3 diff;
    private float rot_z;
    private float SizeZoomCam = 5f;
    private bool stopPlayer;
    private bool facingRight = false;
    private GameObject _trPlayer;


    // Start is called before the first frame update
    void Start()
    {
        joystickMove = DataCont.instanse.fixedJoystickMove;
        joystickGun = DataCont.instanse.fixedJoystickGun;
        _trPlayer = GetComponent<GameObject>();
        transform.gameObject.tag = "LocalPlayer";
        transform.gameObject.layer = 6;

        //       camera = DataCont.instanse.camera;
    }

    // Update is called once per frame
    void Update()
    {
        diff = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
// jostik
//        rot_z = Mathf.Atan2(joystickGun.Vertical, joystickGun.Horizontal) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0f, 0f, rot_z);

//        rb.AddForce(Vector2.right * joystickMove.Horizontal * speed * Time.deltaTime, ForceMode2D.Force);
        rb.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, ForceMode2D.Force);

        if (Input.GetKey(KeyCode.Mouse0) && !FindObjectOfType<GameManager>().pause)
 //       if (joystickGun.Vertical != 0 || joystickGun.Horizontal != 0 && !FindObjectOfType<GameManager>().pause)
        {

            if (!stopPlayer)
            {
                fireGun();
            }
        }

     
        //       if (facingRight == false && joystickMove.Horizontal < 0)
        if (rot_z > 90 || rot_z < -90)
        {
            LocalScale.x = +1f;
            LocalScaleGun.y = -1f;
            LocalScaleGun.x = -1f;
        }
//        else if (facingRight == true && joystickMove.Horizontal > 0)
        else
        {
            LocalScale.x = -1f;
            LocalScaleGun.y = +1f;
            LocalScaleGun.x = +1f;
        }

        transform.localScale = LocalScale;
        gun.transform.localScale = LocalScaleGun;


Debug.DrawRay((Vector2)transform.position + Vector2.down * 1.0f, Vector2.down, Color.red);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down * 1.2f, Vector2.down, 0.1f);
        if (hit.collider != null && hit.collider.transform != transform)
        {
            onGround = true;
        }else
        {
            onGround = false;
        }

//        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        if (joystickMove.Vertical > 0.7)
        {
            if (onGround)
            {
                rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            }
        }
        time -= 1 * Time.deltaTime;

//        if (joystickMove.Vertical == 0 && joystickMove.Horizontal == 0 && onGround)
        if (rb.velocity == Vector2.zero)
        {
            stopPlayer = true;
            button.gameObject.SetActive(true);
            //           StartCoroutine(zoomUp());
        }
        
        else
        {
            //            StartCoroutine(zoomDown());
            button.gameObject.SetActive(false);
            stopPlayer = false;
        }

    }
    
    void flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
    IEnumerator zoomUp()
    {
        while (SizeZoomCam < 8)
        {
            camera.orthographicSize = SizeZoomCam;
            yield return new WaitForSeconds(0.1f);
            SizeZoomCam = SizeZoomCam + 0.1f;
        }
        button.gameObject.SetActive(true);
    } 
    IEnumerator zoomDown()
    {
        while (SizeZoomCam > 5)
        {
            button.gameObject.SetActive(false);
            camera.orthographicSize = SizeZoomCam;
            yield return new WaitForSeconds(0.1f);
            SizeZoomCam = SizeZoomCam - 0.1f;

        }
        button.gameObject.SetActive(false);
    }

    public void fireGun()
    {
        if (time <= 0)
        {
            var bl = PhotonNetwork.Instantiate(bullet.name, bulletPoint.position, gun.rotation);
            bl.GetPhotonView().RPC("Set", RpcTarget.AllBuffered, GetComponent<PhotonView>().Owner.NickName);
            time = cooldown;
        }
    }
}
