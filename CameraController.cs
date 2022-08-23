using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;

    Vector3 CameraPosition;

    public float CameraSpeed = 10;


    void Start()
    {

        CameraPosition = new Vector3(0, 1.2f, 0);

    }


    void Update()
    {

        CameraMove();

    }


    void CameraMove()
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

        transform.LookAt(Target);
        transform.position = Vector3.Lerp(transform.position, Target.position + CameraPosition, CameraSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
    
}
