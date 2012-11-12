using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class DatabaseMenuItem : AbstractMenuItem
	{


		public DatabaseMenuItem(DataSetMetadata dataSetMetadata)
		{
			this.dataSetMetadata = dataSetMetadata;
		}


		public override Dictionary<string, object> GetDataDictionary(IconSize iconSize)
		{
			var data = base.GetDataDictionary (iconSize);

			data["name"] = this.GetName ();

			return data;
		}


		private string GetName()
		{
			return Tools.TypeToString (this.dataSetMetadata.DataSetEntityType);
		}


		protected override string GetTitle()
		{
			return this.dataSetMetadata.BaseShowCommand.Caption.DefaultLabel;
		}


		protected override string GetIconClass(IconSize iconSize)
		{
			var iconUri = this.dataSetMetadata.BaseShowCommand.Caption.Icon;
			var type = this.dataSetMetadata.DataSetEntityType;

			return IconManager.GetCssClassName (iconUri, iconSize, type);
		}


		protected override string GetDataType()
		{
			return "database";
		}


		private readonly DataSetMetadata dataSetMetadata;


	}


}
