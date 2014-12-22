using UnityEngine;

public class createTopBar : MonoBehaviour
{
    public GameObject panel = null;
    public Transform topbar = null;
    // Use this for initialization
    private void Start()
    {
        var topBaar = Instantiate(topbar, new Vector3(0, 0, 0), Quaternion.identity) as Transform;
        topBaar.transform.parent = panel.transform;
        topBaar.localScale = new Vector3(240, 240, 240);
        topBaar.localPosition = new Vector3(0, 64, 0);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}