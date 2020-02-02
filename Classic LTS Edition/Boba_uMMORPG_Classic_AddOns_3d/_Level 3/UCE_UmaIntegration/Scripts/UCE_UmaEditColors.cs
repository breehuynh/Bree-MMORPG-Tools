// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if _UMA

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UCE_UmaEditColors : MonoBehaviour, IPointerClickHandler
{
    public Color selectedColor;
    public UmaColorTypes Category;
    private UCE_UI_CharacterCreation creator;

    private void Start()
    {
        creator = FindObjectOfType<UCE_UI_CharacterCreation>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (creator.dca == null) return;

        selectedColor = GetColor(GetPointerUVPosition());
        switch (Category)
        {
            case UmaColorTypes.Skin:
                creator.ChangeSkinColor(selectedColor);
                break;

            case UmaColorTypes.Hair:
                creator.ChangeHairColor(selectedColor);
                break;

            case UmaColorTypes.Eyes:
                creator.ChangeEyesColor(selectedColor);
                break;

            case UmaColorTypes.Base_Clothing:
                creator.ChangeBaseColor(selectedColor);
                break;
        }
    }

    private Color GetColor(Vector2 pos)
    {
        Texture2D texture = GetComponent<Image>().sprite.texture;
        Color selected = texture.GetPixelBilinear(pos.x, pos.y);
        selected.a = 1; // force full alpha
        return selected;
    }

    private Vector2 GetPointerUVPosition()
    {
        Vector3[] imageCorners = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(imageCorners);
        float texWidth = imageCorners[2].x - imageCorners[0].x;
        float texHeight = imageCorners[2].y - imageCorners[0].y;
        float uvX = (Input.mousePosition.x - imageCorners[0].x) / texWidth;
        float uvY = (Input.mousePosition.y - imageCorners[0].y) / texHeight;
        return new Vector2(uvX, uvY);
    }
}

public enum UmaColorTypes
{
    Skin,
    Eyes,
    Hair,
    Base_Clothing
}

#endif