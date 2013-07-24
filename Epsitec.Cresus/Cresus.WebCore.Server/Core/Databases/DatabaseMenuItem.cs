using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.WebCore.Server.Core.IO;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	/// <summary>
	/// The DatabaseMenuItem class represents an entry in the top menu that lets the user show a
	/// given database.
	/// </summary>
	internal sealed class DatabaseMenuItem : AbstractMenuItem
	{


		public DatabaseMenuItem(DataSetMetadata dataSetMetadata)
		{
			this.dataSetMetadata = dataSetMetadata;
		}


		public override Dictionary<string, object> GetDataDictionary()
		{
			var data = base.GetDataDictionary ();

			data["name"] = this.GetName ();

			return data;
		}


		private string GetName()
		{
			return DataIO.DruidToString (this.dataSetMetadata.Command.Caption.Id);
		}


		protected override string GetTitle()
		{
			return this.dataSetMetadata.Command.Caption.DefaultLabel;
		}


		protected override string GetIconClass(IconSize iconSize)
		{
			var iconUri = this.dataSetMetadata.Command.Caption.Icon;
			var type = this.dataSetMetadata.EntityTableMetadata.EntityType;

			return IconManager.GetCssClassName (iconUri, iconSize, type);
		}


		protected override string GetDataType()
		{
			return "database";
		}


		private readonly DataSetMetadata dataSetMetadata;


	}


}
