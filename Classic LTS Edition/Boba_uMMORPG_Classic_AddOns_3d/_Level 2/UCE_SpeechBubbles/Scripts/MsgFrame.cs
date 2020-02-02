// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using UnityEngine;

//

public partial class MsgFrame : MonoBehaviour
{
    private Animator animator;
    private TextMesh frameText;
    private SpriteRenderer bubble;
    private float additionalWitdh;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    private void Awake()
    {
        animator = GetComponent<Animator>();
        frameText = GetComponentInChildren<TextMesh>();
        bubble = GetComponentInChildren<SpriteRenderer>();

        additionalWitdh = bubble.size.x;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void ShowMessage(string msg)
    {
        if (msg != "")
        {
            msg = msg.Replace(System.Environment.NewLine, " ");

            frameText.text = msg;
            bubble.size = new Vector2(GetTextMeshWidth(frameText, msg) + additionalWitdh, bubble.size.y);
            bubble.gameObject.SetActive(true);

            StartCoroutine("ShowMsgFrameSequence");
            animator.SetBool("SHOW_MSG", true);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    private IEnumerator ShowMsgFrameSequence()
    {
        yield return new WaitForSeconds((frameText.text.Length / 10) + 2.5f);
        animator.SetBool("SHOW_MSG", false);
        yield return new WaitForSeconds(0.3f);
        frameText.text = "";
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public float GetTextMeshWidth(TextMesh mesh, string txt)
    {
        float width = 0;
        foreach (char symbol in mesh.text)
        {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
            {
                width += info.advance;
            }
        }
        return width * mesh.characterSize * 0.1f;
    }

    // -----------------------------------------------------------------------------------
}
