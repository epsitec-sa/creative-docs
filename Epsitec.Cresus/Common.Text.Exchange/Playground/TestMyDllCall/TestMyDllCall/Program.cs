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

		[DllImport ("ClipboardDll.dll")]
		static unsafe extern byte* ReadHtmlFromClipboard();

		[DllImport ("ClipboardDll.dll")]
		static extern int GetClipboardSize();

		[DllImport ("ClipboardDll.dll")]
		static extern void FreeClipboard();


		static unsafe byte[] ReadClipBoard()
		{
			int size;
			byte* pBuffer;

			pBuffer = ReadHtmlFromClipboard ();

			if (pBuffer != null)
			{

				size = GetClipboardSize ();

				byte[] managedBuffer = new byte[size];

				for (int i = 0; i < size; i++)
				{
					managedBuffer[i] = pBuffer[i];
				}

				return managedBuffer;
			}

			return null;

		}

		static void Main(string[] args)
		{
			int b = TestEntry(12);

			byte[] p = ReadClipBoard ();


		}
	}
}
