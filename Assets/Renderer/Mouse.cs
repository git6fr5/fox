using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Screen.MousePosition;
        transform.eulerAngles -= Vector3.forward * 1f;

    }
}
