using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandButton : MonoBehaviour
{
    MeshRenderer mesh;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mat = mesh.material;
        transform.localPosition = new Vector3(-4.84f, 0, -1.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finger")
        {
            mat.color = new Color(0.6f, 0, 0);
            transform.localPosition = new Vector3(-4.84f, -0.3f, -1.3f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Finger")
        {
            mat.color = new Color(1, 0, 0);
            transform.localPosition = new Vector3(-4.84f, 0, -1.3f);
        }
    }
}
