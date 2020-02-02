// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

public class Spinner : MonoBehaviour
{
    public Image SpinnerImage;

    // Use this for initialization
    private void Start()
    {
        SpinnerImage.fillAmount = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (SpinnerImage.fillAmount == 1f)
        {
            SpinnerImage.fillAmount = 0f;
        }

        SpinnerImage.fillAmount += Time.deltaTime;
    }
}
