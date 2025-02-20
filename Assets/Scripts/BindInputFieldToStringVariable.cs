using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindInputFieldToStringVariable : MonoBehaviour
{
    [SerializeField] private string ForcedComPortPlayerPrefKey = "force_com_port";
    public TMPro.TMP_InputField _inputField;

    private void Start()
    {
        _inputField.text = PlayerPrefs.GetString(ForcedComPortPlayerPrefKey, "");
        _inputField.onValueChanged.AddListener(OnInputFieldChanged);
    }

    private void OnEnable()
    {
        _inputField.text = PlayerPrefs.GetString(ForcedComPortPlayerPrefKey, "");
    }

    private void OnDestroy()
    {
        _inputField.onValueChanged.RemoveListener(OnInputFieldChanged);
    }

    public void OnInputFieldChanged(string value)
    {
        PlayerPrefs.SetString(ForcedComPortPlayerPrefKey,value);
    }
}
