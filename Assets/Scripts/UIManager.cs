using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    GameObject canvas;
    public Image crosshairImage;
    public TMP_Text statusText;

    public float crosshairAnimSpeed = 5;

    [Header("Default Crosshair Settings")]
    public float originalScaleRaw = 0.3f;
    Vector3 originalScale;
    public float originalRotation = 0; //euler angle degrees, rotated around the Z axis
    public Color originalColor;

    [Header("Looking at Item Crosshair Settings")]
    public float itemScaleRaw = 0.5f;
    Vector3 itemScale;
    public float itemRotation = 45; //euler angle degrees, rotated around the Z axis
    public Color itemColor;

    [Header("Status Text Settings")]
    public Color hiddenText;
    public Color visibleText;
    public float textAnimSpeed = 5;

    [Header("Dialogue Text Settings")]
    public TMP_Text dialogueText;
    public Image dialogueBoxImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = new Vector3(originalScaleRaw, originalScaleRaw, originalScaleRaw);
        itemScale = new Vector3(itemScaleRaw, itemScaleRaw, itemScaleRaw);

        canvas = GameObject.Find("Canvas");
        statusText.enabled = false;

        dialogueBoxImage.enabled = false;
        dialogueText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CrosshairDefault()
    {
        CrosshairAnimation(originalScale, originalRotation, originalColor, crosshairAnimSpeed);
    }

    public void CrosshairItem()
    {
        CrosshairAnimation(itemScale, itemRotation, itemColor, crosshairAnimSpeed);
    }

    void CrosshairAnimation(Vector3 targetScale, float targetRotation, Color targetColor, float animationSpeed)
    {
        var step = animationSpeed * Time.deltaTime;

        crosshairImage.color = Color.Lerp(crosshairImage.color, targetColor, step);

        crosshairImage.transform.localScale = Vector3.Lerp(crosshairImage.transform.localScale, targetScale, step);
        crosshairImage.transform.localRotation = Quaternion.Lerp(crosshairImage.transform.localRotation, Quaternion.Euler(0, 0, targetRotation), step);
    }

    public void InteractableTextShow(String message)
    {
        TextAnimation(true, message, visibleText, textAnimSpeed);
    }

    public void InteractableTextHide()
    {
        TextAnimation(false, "", hiddenText, textAnimSpeed);
    }

    /* Not high priority atm
    public void PickupNotification(GameObject pickup)
    {
        statusText.text = "Picked up " + pickup.name;

        statusText.enabled = true;
    }
    */

    void TextAnimation(bool visible, String message, Color targetColor, float animationSpeed)
    {
        var step = animationSpeed * Time.deltaTime;

        statusText.text = message;
        statusText.enabled = visible;
        statusText.color = Color.Lerp(statusText.color, targetColor, step);
    }
    
    public void DisplayDialogue(string dialogue)
    {
        dialogueText.text = dialogue;

        dialogueText.enabled = true;
        dialogueBoxImage.enabled = true;

        Invoke("HideDialogue", 5);
    }

    public void HideDialogue()
    {
        dialogueText.text = "";
        dialogueText.enabled = false;
        dialogueBoxImage.enabled = false;
    }
}
