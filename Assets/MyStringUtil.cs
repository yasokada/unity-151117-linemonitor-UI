using UnityEngine;
using System.Linq;

/*
 * v0.2 2015/11/21
 *   - add replaceNonAsciiToAscii()
 */

namespace NS_MyStringUtil
{
	public static class MyStringUtil {
		public static string removeLine(string src, int numRemove)
		{
			string work = src;
			for(int loop=0; loop < numRemove; loop++) {
				int pos = work.IndexOf('\n');
				work = work.Substring(pos+1);
			}
			return work;
		}
		public static string addToRingBuffer(string src, string addStr, int maxline) 
		{
			string work = src + addStr;
			int numline = work.Count( c => c == '\n') + 1;
			int numRemove = numline - maxline;
			work = removeLine(work, numRemove);     
			return work;
		}

		public static string replaceNonAsciiToAscii(string srcstr)
		{
			string work = srcstr;
			bool hasNewLine = false;

			if (srcstr.IndexOf ("\r") >= 0) {
				hasNewLine = true;
			}
			if (srcstr.IndexOf ("\n") >= 0) {
				hasNewLine = true;
			}

			work = work.Replace ("\r", "<CR>");
			work = work.Replace ("\n", "<LF>");

			if (hasNewLine) {
				work = work + System.Environment.NewLine;
			}
			return work;
		}
	}
}