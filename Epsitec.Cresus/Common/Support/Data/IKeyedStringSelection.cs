//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// The <c>IKeyedStringSelection</c> interface provides the basic mechanism used
	/// to select an item in a list, using either its key or its index.
	/// </summary>
	public interface IKeyedStringSelection : IStringSelection
	{
		string SelectedKey
		{
			get;
			set;
		}
	}
}
