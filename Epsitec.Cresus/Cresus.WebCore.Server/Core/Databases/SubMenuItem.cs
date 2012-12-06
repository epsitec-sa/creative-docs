using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class SubMenuItem : AbstractMenuItem
	{


		public SubMenuItem(Caption caption, IEnumerable<AbstractMenuItem> items)
		{
			this.caption = caption;
			this.items = items.ToList ();
		}


		public override Dictionary<string, object> GetDataDictionary()
		{
			var data = base.GetDataDictionary ();

			data["items"] = this.items
				.Select (i => i.GetDataDictionary ())
				.ToList ();

			return data;
		}


		protected override string GetTitle()
		{
			return this.caption.DefaultLabel;
		}


		protected override string GetIconClass(IconSize iconSize)
		{
			return IconManager.GetCssClassName (this.caption.Icon, iconSize);
		}


		protected override string GetDataType()
		{
			return "subMenu";
		}


		private readonly Caption caption;


		private readonly List<AbstractMenuItem> items;

	
	}


}
