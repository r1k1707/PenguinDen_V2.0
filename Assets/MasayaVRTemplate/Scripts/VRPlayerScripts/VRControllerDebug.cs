using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using TMPro;

public class VRControllerDebug : MonoBehaviour
{
    [SerializeField] VRController vrController;
    [SerializeField] GameObject debugTextPrefab;
    [SerializeField] Transform contentGrp;
    List<TextMeshProUGUI> debugTexts = new List<TextMeshProUGUI>();
    List<DebugType> variables = new List<DebugType>();

    [Header("Animation Objects")]
    [SerializeField] Transform trigger;
    [SerializeField] Transform grip;
    [SerializeField] Transform primaryButton;
    [SerializeField] Transform secondaryButton;
    [SerializeField] Transform analog;

    private void Start()
    {
        CreateDebugs();
    }

    void CreateDebugs()
    {
        Type script = typeof(ControllerValues);
        FieldInfo[] fields = script.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            GameObject obj = Instantiate(debugTextPrefab, contentGrp);
            obj.name = field.Name;
            TextMeshProUGUI objText = obj.GetComponent<TextMeshProUGUI>();
            if(vrController.GetHand() == VRController.ControllerHand.right)
            {
                objText.alignment = TextAlignmentOptions.Left;
            }
            debugTexts.Add(objText);
            DebugType debugType = new DebugType()
            {
                name = field.Name,
                type = field.FieldType,
            };
            variables.Add(debugType);
        }
    }

    void Update()
    {
        for (int i = 0; i < variables.Count; i++)
        {
            UpdateVisuals(i, variables[i]);
        }

        UpdateAnimations();
    }

    void UpdateVisuals(int index, DebugType value)
    {
        string valueText = vrController.controllerValues.GetType().GetField(value.name).GetValue(vrController.controllerValues).ToString();
        debugTexts[index].text = $"{value.name}: {vrController.controllerValues.GetType().GetField(value.name).GetValue(vrController.controllerValues)}";
    }

    void UpdateAnimations()
    {
        ControllerValues controllerValues = vrController.controllerValues;
        //trigger
        float triggerValue = Mathf.Lerp(0, -25f, controllerValues.triggerValue);
        trigger.localEulerAngles = new Vector3(triggerValue, 0, 0);

        //grip
        float gripValue = Mathf.Lerp(0, 0.005f, controllerValues.gripValue);
        grip.localPosition = new Vector3(gripValue, 0, 0);

        //analog
        float yRot = controllerValues.analogValue.y * 25f;
        float xRot = controllerValues.analogValue.x * 25f;
        analog.localRotation = Quaternion.Euler(yRot, 0, -xRot);

        //PrimaryButton : -0.00015
        if (controllerValues.primaryButtonPressed)
        {
            primaryButton.localPosition = new Vector3(0, -0.0015f, 0);
        }
        else
        {
            primaryButton.localPosition = Vector3.zero;
        }

        //SecondaryButton
        if (controllerValues.secondaryButtonPressed)
        {
            secondaryButton.localPosition = new Vector3(0, -0.0015f, 0);
        }
        else
        {
            secondaryButton.localPosition = Vector3.zero;
        }
    }
}

public struct DebugType
{
    public string name;
    public Type type;
}
