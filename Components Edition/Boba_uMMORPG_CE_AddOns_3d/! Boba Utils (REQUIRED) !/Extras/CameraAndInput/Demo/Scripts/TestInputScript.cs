using UnityEngine;

public class TestInputScript : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        InputManager.useMobileInputOnNonMobile = true;
    }

    // Update is called once per frame
    private void Update()
    {
        float hAxis = InputManager.GetAxis("Horizontal", false);
        float vAxis = InputManager.GetAxis("Vertical", false);
    }
}