//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	internal static class Win32Const
	{
		public const int INFOTIPSIZE			= 1024;
		public const int MAX_PATH				= 260;
		public const int MAX_SHORTPATH			= 14;

		public const int SW_SHOWNORMAL			= 1;
		public const int SW_SHOWMINIMIZED		= 2;
		public const int SW_SHOWMAXIMIZED		= 3;
		public const int SW_SHOWMINNOACTIVE		= 7;
	}
}
