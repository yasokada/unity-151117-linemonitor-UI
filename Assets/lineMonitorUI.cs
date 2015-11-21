﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI; // for Text

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NS_MyNetUtil; // for MyNetUtil.getMyIPAddress()
using NS_MyStringUtil; // for addToRingBuffer()

/*
 * v0.3 2015/11/21
 *   - can add time info (sec.msec) to the received text
 *   - can convert non-ASCII char to ASCII char (e.g. <CR>)
 * 	 - add toggle Upd to turn on/off text update
 * v0.2 2015/11/21
 *   - can keep 32-1 lines of UDP packet
 *   - add setting for android (304SH)
 * v0.1 2015/11/19
 *   - receive text with ring buffer
 * --------- converted from UdpEchoServer to line monitor ----------
 * v0.4 2015/08/30
 *   - separate IP address get method to MyNetUtil.cs
 * v0.3 2015/08/30
 *   - show version info
 *   - correct .gitignore file
 * v0.2 2015/08/29
 *   - fix for negative value for delay_msec
 *   - fix for string to int
 *   - fix for android (splash freeze)
 * v0.1 2015/08/29
 *   following features have been implemented.
 *   - delay before echo back
 *   - echo back
 */

public class lineMonitorUI : MonoBehaviour {
	Thread rcvThr;
	UdpClient client;
	public int port = 9000;

	public const string kAppName = "line monitor UI";
	public const string kVersion = "v0.3";
	public const int kMaxLine = 32;

	public string lastRcvd;
	private string bufferText;

	public Text myipText; // to show my IP address(port)
	public Text recvdText;
	public Text versionText;
	public Toggle TG_update; // to turn on/off text update
	public Toggle TG_ctrl; // to convert non-ASCII char to ASCII char (e.g. <CR>
	public Toggle TG_time; // to add current time (sec+msec) or not

	private bool stopThr = false;

	int getDelay() { 
		return 0;
	}

	long get_msec() {
		System.DateTime now = System.DateTime.Now;
		System.DateTime nowMsec0 = new DateTime (now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
		long TotalMsec = (long)((DateTime.Now - nowMsec0).TotalMilliseconds);
		
		return TotalMsec;
	}

	string get_sec_msec_string() {
		int sec = System.DateTime.Now.Second;
		long msec = get_msec ();

		string res = string.Format ("{0:00}", sec) + "." + string.Format ("{0:000}", msec);
		return res;
	}

	void Start () {
		bufferText = "";
		versionText.text = kAppName + " " + kVersion;
		myipText.text = MyNetUtil.getMyIPAddress() + " (" + port.ToString () + ")";
		startTread ();
	}
	
	void Update() {
		if (lastRcvd.Length > 0) {
			if (TG_time.isOn) {
				lastRcvd = get_sec_msec_string() + "," + lastRcvd;
			}
			if (TG_ctrl.isOn) {
				lastRcvd = MyStringUtil.replaceNonAsciiToAscii(lastRcvd);
			}
			if (TG_update.isOn) {
				bufferText = MyStringUtil.addToRingBuffer(bufferText, lastRcvd, kMaxLine);
				lastRcvd = "";
				recvdText.text = bufferText;
			} else {
				lastRcvd = "";
			}
		}
	}
	
	void startTread() {
		Debug.Log ("init");
		rcvThr = new Thread( new ThreadStart(FuncRcvData));
		rcvThr.Start ();
	}
	
	void OnApplicationQuit() {
		stopThr = true;
		rcvThr.Abort ();
	}
	
	private void FuncRcvData()
	{
		client = new UdpClient (port);
		client.Client.ReceiveTimeout = 300; // msec
		client.Client.Blocking = false;
		while (stopThr == false) {
			try {
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.ASCII.GetString(data);
				lastRcvd = text;

//				if (lastRcvd.Length > 0) {
//					client.Send(data, data.Length, anyIP); // echo
//				}
			}
			catch (Exception err)
			{
				//              print(err.ToString());
			}

			// without this sleep, on adnroid, the app will not start (freeze at Unity splash)
			Thread.Sleep(20); // 200
		}
		client.Close ();
	}
}


