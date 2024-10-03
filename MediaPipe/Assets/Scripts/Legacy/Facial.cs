using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facial : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public GameObject[] landMarks;
    public float distance;
    public GameObject[] spinningCubes;

    float rotSpeed;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        string[] points = data.Split(',');
        print(points[0]);

        //0     1*2    2*2
        //x1,y1,x2,y2,x3,y3

        for (int i = 0; i < 468; i++)
        {
            float x = 7 - float.Parse(points[i * 3]) / 100;
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            landMarks[i].transform.localPosition = new Vector3(x, y, z);
        }
        Vector3 i_11 = new Vector3(7 - float.Parse(points[11 * 3]) / 100, float.Parse(points[11 * 3 + 1]) / 100, float.Parse(points[11 * 3 + 2]) / 100);
        Vector3 i_14 = new Vector3(7 - float.Parse(points[14 * 3]) / 100, float.Parse(points[14 * 3 + 1]) / 100, float.Parse(points[14 * 3 + 2]) / 100);

        distance = Vector3.Distance(i_11, i_14);
        rotSpeed += Time.deltaTime * 100f;

        foreach (GameObject cube in spinningCubes)
        {
            cube.transform.rotation = Quaternion.Euler(0f, rotSpeed * distance, 0f);
        }
    }
}