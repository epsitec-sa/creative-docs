using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal abstract class AbstractField : AbstractEditionTilePart
	{


		public string Title
		{
			get;
			set;
		}


		public string FieldName
		{
			get;
			set;
		}


		public string LambdaFieldName
		{
			get;
			set;
		}


		public string PanelFieldAccessorId
		{
			get;
			set;
		}


		public override IEnumerable<Dictionary<string, object>> ToDictionary()
		{
			yield return this.GetFieldDictionary ();
			yield return this.GetLambdaDictionary ();
		}


		private Dictionary<string, object> GetLambdaDictionary()
		{
			var lambdaDictionary = new Dictionary<string, object> ();

			lambdaDictionary["xtype"] = "hiddenfield";
			lambdaDictionary["name"] = this.LambdaFieldName;
			lambdaDictionary["value"] = this.PanelFieldAccessorId;

			return lambdaDictionary;
		}


		protected virtual Dictionary<string, object> GetFieldDictionary()
		{
			var fieldDictionary = new Dictionary<string, object> ();
			
			fieldDictionary["fieldLabel"] = this.Title;
			fieldDictionary["name"] = this.FieldName;

			return fieldDictionary;
		}


	}


}
