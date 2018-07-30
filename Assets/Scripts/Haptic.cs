using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Haptic {
	[DllImport("__Internal")]
	static extern void _haptic();

	public static void HapticMid() {
		if(Application.platform==RuntimePlatform.IPhonePlayer)
		_haptic();
	}

}
