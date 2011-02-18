//	Copyright © 2007-2008, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Protocols
{
	internal static class FileProtocol
	{
		public static byte[] ReadBytes(string name)
		{
			return System.IO.File.ReadAllBytes (name);
		}
	}
}
