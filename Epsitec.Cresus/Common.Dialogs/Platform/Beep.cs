using System;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Dialogs.Platform
{
	/// <summary>
	/// Summary description for Beep.
	/// </summary>
	public class Beep
	{
		public enum MessageType
		{
			Default		= -1,
			Ok			= 0x00000000,
			Error		= 0x00000010,
			Question	= 0x00000020,
			Warning		= 0x00000030,
			Information	= 0x00000040,
		}
		
		[DllImport("User32.dll", SetLastError=true, EntryPoint="MessageBeep")]	private static extern bool Win32MessageBeep(int beepType);
		[DllImport("Kernel32.dll", EntryPoint="Beep")]							private static extern void Win32Beep(int f, int d);
		
		public static void MessageBeep(MessageType type)
		{
			Beep.Win32MessageBeep ((int)type);
		}
 	}
}
