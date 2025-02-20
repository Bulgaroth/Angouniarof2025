using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SerialPortAutobinder : MonoBehaviour
{
    private const string INVALID_PORT = "INVALID";
    private bool _controllerBinded = false;
    [SerializeField] private SerialController _serialController;
    [SerializeField, Tooltip("Looking for serial ports is costly, we can't do it everyframe")] private float _updateRate = 5f;
    [SerializeField] private UnityEvent OnControllerConnected;
    [SerializeField] private UnityEvent OnControllerInitialize;
    [SerializeField] private string ForcedComPortPlayerPrefKey="force_com_port";
    private string _forcedComPort;

    private Coroutine _slowUpdateCoroutine;
    private bool _outputInitialized = false;

    public void SetOutputInitialized(bool value)
    {
        _outputInitialized = value;
    }

    private void Start()
    {
        _slowUpdateCoroutine = StartCoroutine(SlowUpdate());
        _forcedComPort = PlayerPrefs.GetString(ForcedComPortPlayerPrefKey,"");
        TryToBindController();
    }

    IEnumerator SlowUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(_updateRate);
            Debug.Log("Trying to bind controller...");
            TryToBindController();
        }
    }

    bool TryToBindController()
    {
        if (_controllerBinded) return false;

        string port = AutodetectArduinoPort();
        if (!string.IsNullOrEmpty(_forcedComPort))
        {
            port = _forcedComPort;
        }

        if (port == INVALID_PORT) return false;

        _serialController.portName = port;
        _serialController.enabled = false;
        StartCoroutine(DelayedReEnable());
        StopCoroutine(_slowUpdateCoroutine);
        _slowUpdateCoroutine = null;
        _controllerBinded = true;
        OnControllerConnected?.Invoke();
        if (_outputInitialized) OnControllerInitialize?.Invoke();
        return true;
    }

    IEnumerator DelayedReEnable()
    {
        yield return null;
        _serialController.enabled = true;
    }

    void OnConnectionEvent(bool success)
    {
        _controllerBinded = success;
        if (success) return;
        if (_slowUpdateCoroutine != null) return;
        _slowUpdateCoroutine = StartCoroutine(SlowUpdate());
    }

    //Pasted and (very slightly) modified from this post: https://discussions.unity.com/t/auto-detect-arduino-com-port/151527/2
    public static string AutodetectArduinoPort()
    {
        List<string> comports = new List<string>();
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        string temp;
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            var subkeyNames = rk3.GetSubKeyNames();
            foreach (string s in subkeyNames)
            {
                if (s.Contains("VID") && s.Contains("PID"))
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        var friendlyName = (string)rk5.GetValue("FriendlyName");
                        var deviceDesc = (string)rk5.GetValue("DeviceDesc");
                        var Mfg = (string)rk5.GetValue("Mfg");
                        if ((friendlyName != null && friendlyName.Contains("Arduino")))
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");

                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                comports.Add(temp);
                            }
                        }
                    }
                }
            }
        }

        if (comports.Count > 0)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comports.Contains(s))
                    return s;
            }
        }
        //If we detected no com ports this way, it may be because the arduino is recognized as "Composite USB Device" instead of "Arduino"
        //If so, it's simpler to just pick a random open COM ports (outside of COM1, the one open by default by Windows), nobody outside of Arduino
        //is using com ports nowadays
        else
        {
            var validComports = SerialPort.GetPortNames().Where(x => x != "COM1" && !x.Contains("LPT"));
            if (validComports.Count() > 0)
            {
                int randomIndex = Random.Range(0, validComports.Count());
                return validComports.ToArray()[randomIndex];
            }
        }

        return INVALID_PORT;
    }
}
