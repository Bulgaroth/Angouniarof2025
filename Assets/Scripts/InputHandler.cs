using System;
using System.IO.Ports;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	private readonly SerialPort sp = new("COM3", 9600);

	private void Start()
	{
		sp.Open();
		sp.ReadTimeout = 1;
	}

	private void Update()
	{
		try { print(sp.ReadLine()); }
		catch(Exception e) { print(e.StackTrace); }

		/*try
		{
			if (sp.ReadBufferSize > 0)
			{
				int input = sp.ReadByte();
				GameManager.Instance.AnswerSelected(input);
			}
		}
		catch(Exception e) {print(e.StackTrace); }*/
	}
}
