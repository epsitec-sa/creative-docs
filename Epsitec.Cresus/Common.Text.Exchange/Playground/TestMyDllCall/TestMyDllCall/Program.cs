using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TestMyDllCall
{
	class Program
	{

		[DllImport ("ClipboardDll.dll")]
		static extern int TestEntry(int value);

		static void Main(string[] args)
		{
			int b = TestEntry(12);
		}
	}
}
