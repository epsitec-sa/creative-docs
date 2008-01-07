//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	public interface IFileExtensionDescription
	{
		void Add(string extension, string description);
		string FindDescription(string extension);
	}
}
