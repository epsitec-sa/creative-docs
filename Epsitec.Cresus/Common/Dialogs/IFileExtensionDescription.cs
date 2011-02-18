//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	public interface IFileExtensionDescription
	{
		void Add(string extension, string description);
		string FindDescription(string extension);
	}
}
