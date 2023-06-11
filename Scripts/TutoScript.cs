using System.Collections.Generic;
using UnityEngine;

public class TutoScript : MonoBehaviour
{
    public Camera cam;
    public GameObject player;
    bool running;
    private List<Vector3> posis;
    private GameObject fogLluvia;
    public GameObject particulas;
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("tutoCam").GetComponent<Camera>();
        fogLluvia = GameObject.FindGameObjectWithTag("fog");
    }

    void Update()
    {
        cam.transform.position = player.transform.position;
        particulas.transform.position = player.transform.position;
        fogLluvia.transform.position = player.transform.position;
    }
}
