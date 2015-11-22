using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingSend : MonoBehaviour {
	
	public const string kCommandIpAddressPrefix = "192.168.10.";
	public const string kDefaultCommandPort = "7000";
	public const string kDefaultComBaud = "9600";

	public InputField IF_ipadr;
	public InputField IF_port;
	public InputField IF_combaud;

	void Start () {
		IF_ipadr.text = kCommandIpAddressPrefix;
		IF_port.text = kDefaultCommandPort;
		IF_combaud.text = kDefaultComBaud;
	}
	
	public void SendButtonClick() {
		Debug.Log ("SendButton");
	}
}

