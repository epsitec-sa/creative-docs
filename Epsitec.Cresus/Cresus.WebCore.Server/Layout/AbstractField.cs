using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal abstract class AbstractField : AbstractEditionTilePart
	{


		public string Title
		{
			get;
			set;
		}


		public string Id
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


		protected abstract object GetValue();


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();
			
			brick["title"] = this.Title;
			brick["name"] = this.Id;
			brick["readOnly"] = this.IsReadOnly;
			brick["value"] = this.GetValue ();
			brick["allowBlank"] = this.AllowBlank;

			return brick;
		}


	}


}
