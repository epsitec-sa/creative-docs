using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal abstract class AbstractMenuItem
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
