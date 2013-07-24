using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{

	/// <summary>
	/// The LabelExportItem class represents an entry that must appear in the export menu for a
	/// kind of label that can be exported.
	/// </summary>
	internal sealed class LabelExportItem
	{


		public LabelExportItem(int labelTextFactoryId, FormattedText text)
		{
			this.labelTextFactoryId = labelTextFactoryId;
			this.text = text;
		}


		public Dictionary<string, object> GetDataDictionary()
		{
			return new Dictionary<string, object> ()
			{
				{ "id", this.labelTextFactoryId },
				{ "text", this.text.ToString () },
			};
		}


		public readonly int labelTextFactoryId;


		public readonly FormattedText text;


	}


}
