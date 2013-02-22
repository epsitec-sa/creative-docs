using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.RequestData
{
	/// <summary>
	/// Cette classe représente les données qui sont envoyées ou reçues par le serveur.
	/// 
	/// Le dictionnaire Fields est le reflet des classes de données AbstractRecord.
	/// Le type object peut représenter:
	///  - string
	///  - FormattedText
	///  - decimal
	///  - DateTime
	///  - bool
	///  - enum quelconque
	///  - une classe AbstractRecord
	///  - une liste de classes AbstractRecord
	/// 
	/// Le dictionnaire FormattedTexts représente les mêmes données, sous une forme
	/// textuelle et mise en page. Par exemple, les caractères trouvés sont mis en
	/// évidence.
	/// Le serveur ignore ces données en réception.
	/// </summary>
	public class TravellingRecord
	{
		public Guid										DataGuid;
		public Dictionary<FieldType, object>			Fields;
		public Dictionary<FieldType, FormattedText>		FormattedTexts;
	}
}
