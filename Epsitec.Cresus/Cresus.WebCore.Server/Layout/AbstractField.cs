using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal abstract class AbstractField : AbstractEditionTilePart
	{


		public string Id
		{
			get;
			set;
		}


		public string Title
		{
			get;
			set;
		}


		public bool IsReadOnly
		{
			get;
			set;
		}


		public bool AllowBlank
		{
			get;
			set;
		}


		public object Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["name"] = this.Id;
			brick["title"] = this.Title;
			brick["readOnly"] = this.IsReadOnly;
			brick["allowBlank"] = this.AllowBlank;
			brick["value"] = this.Value;

			return brick;
		}


	}


}
