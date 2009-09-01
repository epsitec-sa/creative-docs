//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Epsitec.Common.Drawing.Platform
{
	static class FontHelper
	{
		#region Decompiled from System.Drawing
		[StructLayout (LayoutKind.Sequential, CharSet=CharSet.Auto), ComVisible (false)]
		public class LOGFONT
		{
			public LOGFONT()
			{
			}

			public override string ToString()
			{
				object[] objArray1 = new object[28];
				objArray1[0] = "lfHeight=";
				objArray1[1] = this.lfHeight;
				objArray1[2] = ", lfWidth=";
				objArray1[3] = this.lfWidth;
				objArray1[4] = ", lfEscapement=";
				objArray1[5] = this.lfEscapement;
				objArray1[6] = ", lfOrientation=";
				objArray1[7] = this.lfOrientation;
				objArray1[8] = ", lfWeight=";
				objArray1[9] = this.lfWeight;
				objArray1[10] = ", lfItalic=";
				objArray1[11] = this.lfItalic;
				objArray1[12] = ", lfUnderline=";
				objArray1[13] = this.lfUnderline;
				objArray1[14] = ", lfStrikeOut=";
				objArray1[15] = this.lfStrikeOut;
				objArray1[16] = ", lfCharSet=";
				objArray1[17] = this.lfCharSet;
				objArray1[18] = ", lfOutPrecision=";
				objArray1[19] = this.lfOutPrecision;
				objArray1[20] = ", lfClipPrecision=";
				objArray1[21] = this.lfClipPrecision;
				objArray1[22] = ", lfQuality=";
				objArray1[23] = this.lfQuality;
				objArray1[24] = ", lfPitchAndFamily=";
				objArray1[25] = this.lfPitchAndFamily;
				objArray1[26] = ", lfFaceName=";
				objArray1[27] = this.lfFaceName;
				return string.Concat (objArray1);
			}

			public byte lfCharSet;
			public byte lfClipPrecision;
			public int lfEscapement;
			[MarshalAs (UnmanagedType.ByValTStr, SizeConst=32)]
			public string lfFaceName;
			public int lfHeight;
			public byte lfItalic;
			public int lfOrientation;
			public byte lfOutPrecision;
			public byte lfPitchAndFamily;
			public byte lfQuality;
			public byte lfStrikeOut;
			public byte lfUnderline;
			public int lfWeight;
			public int lfWidth;
		}

		public struct HandleRef
		{
			public HandleRef(object wrapper, System.IntPtr handle)
			{
				//	Methods
				this.mWrapper = wrapper;
				this.mHandle = handle;
			}
			public static explicit operator System.IntPtr(HandleRef value)
			{
				return value.mHandle;
			}

			public System.IntPtr Handle
			{
				get
				{
					return this.mHandle;
				}
			}
			public object Wrapper
			{
				get
				{
					return this.mWrapper;
				}
			}
			//	Properties

			//	Fields
			internal System.IntPtr mHandle;
			internal object mWrapper;
		}
		[DllImport ("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetObject(System.IntPtr hObject, int nSize, [In, Out] LOGFONT lf);
		[DllImport ("user32.dll", EntryPoint="GetDC", CharSet=CharSet.Auto, ExactSpelling=true)]
		private static extern System.IntPtr IntGetDC(System.IntPtr hWnd);
		[DllImport ("user32.dll", EntryPoint="ReleaseDC", CharSet=CharSet.Auto, ExactSpelling=true)]
		private static extern int IntReleaseDC(System.IntPtr hWnd, System.IntPtr hDC);


		public static int GetObject(System.IntPtr hObject, ref LOGFONT lp)
		{
			return FontHelper.GetObject (hObject, Marshal.SizeOf (typeof (LOGFONT)), lp);
		}
		public static System.Drawing.Font FromHfont(System.IntPtr hfont)
		{
			LOGFONT logfont1 = new LOGFONT ();
			FontHelper.GetObject (hfont, ref logfont1);
			System.IntPtr ptr1 = FontHelper.IntGetDC (System.IntPtr.Zero);
			try
			{
				return FontHelper.FromLogFont (logfont1, ptr1);
			}
			finally
			{
				FontHelper.IntReleaseDC (System.IntPtr.Zero, ptr1);
			}
		}
		[DllImport ("gdiplus.dll", CharSet=CharSet.Ansi, ExactSpelling=true)]
		internal static extern int GdipCreateFontFromLogfontA(System.IntPtr hdc, [In, Out, MarshalAs (UnmanagedType.AsAny)] object lf, out System.IntPtr font);
		[DllImport ("gdiplus.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
		internal static extern int GdipCreateFontFromLogfontW(System.IntPtr hdc, [In, Out, MarshalAs (UnmanagedType.AsAny)] object lf, out System.IntPtr font);

		public static System.Drawing.Font FromLogFont(object lf, System.IntPtr hdc)
		{
			int num1;
			bool flag1;
			System.IntPtr ptr1 = System.IntPtr.Zero;
			if (Marshal.SystemDefaultCharSize == 1)
			{
				num1 = FontHelper.GdipCreateFontFromLogfontA (hdc, lf, out ptr1);
			}
			else
			{
				num1 = FontHelper.GdipCreateFontFromLogfontW (hdc, lf, out ptr1);
			}

			if (num1 == 16)
			{
				throw new System.ArgumentException ("GDI+ not a TrueType font, no name");
			}
			if (num1 != 0)
			{
				throw new System.Exception ("StatusException" + num1);
			}
			if (ptr1 == System.IntPtr.Zero)
			{
				throw new System.ArgumentException (string.Concat ("GDI+ does not handle non True-type fonts: ", lf.ToString ()));
			}
			if (Marshal.SystemDefaultCharSize == 1)
			{
				flag1 = (Marshal.ReadByte (lf, 28) == 64);
			}
			else
			{
				flag1 = (Marshal.ReadInt16 (lf, 28) == 64);
			}

			//	return new System.Drawing.Font(ptr1, Marshal.ReadByte(lf, 23), flag1);

			return null;
		}
		#endregion
	}
}
