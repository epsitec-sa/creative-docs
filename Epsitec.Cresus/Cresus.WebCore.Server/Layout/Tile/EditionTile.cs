using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EditionTile : AbstractTile
	{


		public string EntityId
		{
			get;
			set;
		}


		public string Title
		{
			get;
			set;
		}


		public string IconClass
		{
			get;
			set;
		}


		public IList<AbstractEditionTilePart> Items
		{
			get
			{
				return this.items;
			}
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var panel = new Dictionary<string, object> ();

			panel["xtype"] = "epsitec.editiontile";
			panel["entityId"] = this.EntityId;

			if (this.IconClass != null)
			{
				panel["iconCls"] = this.IconClass;
			}

			panel["title"] = this.Title;
			panel["items"] = this.Items.SelectMany (i => i.ToDictionaries ()).ToList ();

			return panel;
		}


		private readonly IList<AbstractEditionTilePart> items = new List<AbstractEditionTilePart> ();


	}


}
