using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialInputHandler : MonoBehaviour
{
    [SerializeField] private int MaxAnswerIndex = 2;
    [SerializeField] private float InputCooldown = 0.5f;
    private float LastAnswerOn = -1000;

    private void OnMessageArrived(string msg)
    {
        int MessageAsInt = 0;
        if (!Int32.TryParse(msg, out MessageAsInt)) return;
        if (MessageAsInt > MaxAnswerIndex) return;
        if (Time.time - LastAnswerOn < InputCooldown) return;
        LastAnswerOn = Time.time;
		GameManager.Instance.AnswerSelected(MessageAsInt);
    }
}
