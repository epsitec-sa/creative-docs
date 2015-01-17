//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{
	/// <summary>
	/// The AbstractMenuItem class is the base class of all kinds of elements that may appear in
	/// the top menu of the application.
	/// </summary>
	public abstract class AbstractMenuItem
	{
		public virtual Dictionary<string, object> GetDataDictionary()
		{
			return new Dictionary<string, object> ()
			{
				{ "type", this.GetDataType () },
				{ "title", this.GetTitle () },
				{ "iconSmall", this.GetIconClass (IconSize.Sixteen) },
				{ "iconLarge", this.GetIconClass(IconSize.ThirtyTwo) },
			};
		}


		protected abstract string GetDataType();

		protected abstract string GetTitle();

		protected abstract string GetIconClass(IconSize iconSize);
	}
}
