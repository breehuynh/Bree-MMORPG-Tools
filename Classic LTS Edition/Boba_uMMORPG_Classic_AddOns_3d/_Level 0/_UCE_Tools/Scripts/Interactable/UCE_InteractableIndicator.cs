// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE INTERACTABLE INDICATOR

public partial class UCE_InteractableIndicator : MonoBehaviour
{
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.25f;
    public float frequency = 0.5f;

    private Vector3 posOffset;
    private Vector3 tempPos;

    private void Start()
    {
        posOffset = transform.localPosition;
    }

    private void Update()
    {
        if (degreesPerSecond != 0)
            transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        if (amplitude != 0 && frequency != 0)
        {
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        }

        if (transform.localPosition != tempPos)
            transform.localPosition = tempPos;
    }
}
