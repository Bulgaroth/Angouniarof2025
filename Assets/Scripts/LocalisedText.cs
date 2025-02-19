using System;
using UnityEngine;

[Serializable]
public struct LocalisedText
{
	[TextArea] public string textFR;
	[TextArea] public string textEN;

	public static LocalisedText operator +(LocalisedText a, LocalisedText b)
	{
		LocalisedText res;
		res.textFR = $"{a.textFR}\n{b.textFR}";
		res.textEN = $"{a.textEN}\n{b.textEN}";
		return res;
	}

	public override readonly string ToString()
		=> $"{textFR}<size=50%>\n\n</size><i><size=80%>{textEN}</size></i>";
}