using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	/// <summary>
	/// The AbstractContexttualMenuItem is a class that represents an entry in the contextual menu
	/// that must be shown when the user clicks on an entity within a database.
	/// </summary>
	internal abstract class AbstractContextualMenuItem
	{


		protected AbstractContextualMenuItem(FormattedText title)
		{
			this.title = title;
		}


		protected abstract string GetDataType();


		public virtual Dictionary<string, object> GetDataDictionary(Caches caches)
		{
			return new Dictionary<string, object> ()
			{
				{ "type", this.GetDataType () },
				{ "title", this.title.ToString () },
			};
		}


		private readonly FormattedText title;


	}


}
