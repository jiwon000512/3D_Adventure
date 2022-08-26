using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;

    Transform CameraLockTarget;

    PlayerController playerController;

    Vector3 CameraPosition;

    public float CameraSpeed = 10f;

    public float TargetLookSpeed = 20f;


    void Start()
    {

        Player = GameObject.Find("Player");
        playerController = Player.GetComponent<PlayerController>();
        CameraPosition = new Vector3(0, 1f, 0);

    }


    void Update()
    {

        if (playerController.GetIsCameraLock() && playerController.GetCameraLockTarget() != null)
        {

            LockedCameraMove(playerController.GetCameraLockStandard().transform);

        }
        else
        {

            CameraMove(Player.transform);

        }

    }


    void CameraMove(Transform target)
    {

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = transform.rotation.eulerAngles;


        float x = camAngle.x - mouseDelta.y;
        float y = camAngle.y - mouseDelta.x;


        if (x < 180f)
        {

            x = Mathf.Clamp(x, -1f, 70f);

        }
        else
        {

            x = Mathf.Clamp(x, 335f, 361f);

        }


        transform.LookAt(target);
        transform.position = Vector3.Lerp(transform.position, target.position + CameraPosition, CameraSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    }

    void LockedCameraMove(Transform target)
    {

        Vector3 direction = target.transform.position - this.transform.position;

        transform.position = Vector3.Lerp(transform.position, Player.transform.position + CameraPosition, TargetLookSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction), TargetLookSpeed * Time.deltaTime);

    }

}
