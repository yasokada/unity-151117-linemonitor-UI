using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Text.RegularExpressions; // for Regex

// for UDP send
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class SettingSend : MonoBehaviour {
	
	public const string kCommandIpAddressPrefix = "192.168.10.";
	public const string kDefaultCommandPort = "7000";
	public const string kDefaultComBaud = "9600";

	public InputField IF_ipadr;
	public InputField IF_port;
	public InputField IF_combaud;

	public bool s_udp_isNotInit = true; // TODO: 1> rename to positive variable
	UdpClient s_udp_client; 

	void Start () {
		IF_ipadr.text = kCommandIpAddressPrefix;
		IF_port.text = kDefaultCommandPort;
		IF_combaud.text = kDefaultComBaud;
	}

	void UDP_init() {
		s_udp_client = new UdpClient ();
		s_udp_client.Client.ReceiveTimeout = 300; // msec
		s_udp_client.Client.Blocking = false;
	}

	void UDP_send(string ipadr, string portStr, string text) {
		int portInt = int.Parse(new Regex("[0-9]+").Match(portStr).Value); // from string to integer
		byte[] data = System.Text.Encoding.ASCII.GetBytes(text); // from string to byte[]
		s_udp_client.Send (data, data.Length, ipadr, portInt);
	}

	public void SendButtonClick() {
		Debug.Log ("SendButton");

		if (s_udp_isNotInit == true) {
			s_udp_isNotInit = false;
			UDP_init ();
		}
	}
}


// client.Send (data, data.Length, ipadr, port);

