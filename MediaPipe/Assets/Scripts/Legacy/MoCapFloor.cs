using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoCapFloor : MonoBehaviour
{
    MeshRenderer mesh;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mat = mesh.material;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Leg")
        {
            mat.color = new Color(1, 0, 0);
        }
    }
}
