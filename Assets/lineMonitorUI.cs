using UnityEngine;
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
 * v0.6 2015 Dec. 12
 * 	 - change locations of each component downward for Windows 1024 x 768 screen
 * v0.5 2015 Dec. 12
 *   - add TG_oneline to show monitor strings in one line (by trimming the exceeded character strings)
 * v0.4 2015/11/23
 *   - add Panel for setting combaud and monitor(ip,port)[Send Me]
 *   - rename port to m_port
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
	public int m_port = 9000;

	public const string kAppName = "line monitor UI";
	public const string kVersion = "v0.5";
	public const int kMaxLine = 32;
	public const int kOneLineLength = 50; // for vertical display on 304SH

	public string lastRcvd;
	private string bufferText;

	public Text myipText; // to show my IP address(port)
	public Text recvdText;
	public Text versionText;
	public Toggle TG_update; // to turn on/off text update
	public Toggle TG_ctrl; // to convert non-ASCII char to ASCII char (e.g. <CR>
	public Toggle TG_time; // to add current time (sec+msec) or not
	public Toggle TG_oneline; // to trimming for one-line display

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
		myipText.text = MyNetUtil.getMyIPAddress() + " (" + m_port.ToString () + ")";
		startTread ();
	}
	
	void Update() {
		if (lastRcvd.Length > 0) {
			bool addNewline = false;
			if (TG_time.isOn) {
				lastRcvd = get_sec_msec_string() + "," + lastRcvd;
			}
			if (TG_oneline.isOn) {
				if (lastRcvd.Length > (kOneLineLength - 3)) { // 3: for "..."
					lastRcvd = lastRcvd.Substring(0, kOneLineLength - 3); // 3: for "..."
					lastRcvd = lastRcvd + "...";
					addNewline = true;
				}
			}
			if (TG_ctrl.isOn) {
				lastRcvd = MyStringUtil.replaceNonAsciiToAscii(lastRcvd);
			}
			if (TG_update.isOn) {
				bufferText = MyStringUtil.addToRingBuffer(bufferText, lastRcvd, kMaxLine);
				if (addNewline) {
					bufferText = bufferText + System.Environment.NewLine;
				}
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
		client = new UdpClient (m_port);
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

// TODO: 0m > bug > oneline > following lines are diplayed > <LF><LF><LF><LF><LF><LF><LF><LF><LF><LF><LF><LF><LF>...

