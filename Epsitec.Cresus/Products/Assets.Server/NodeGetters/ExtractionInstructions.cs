//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Les instructions d'extraction permettent de définir des montants calculés
	/// par CumulNodeGetter, en vue de la production de rapports.
	/// Ces montants sont inclus dans une période temporelle et peuvent concerner un
	/// seul type d'événement.
	/// </summary>
	public struct ExtractionInstructions
	{
		public ExtractionInstructions(ObjectField resultField, DateRange range, EventType eventType, ExtractionAmount extractionAmount)
		{
			this.ResultField      = resultField;
			this.Range            = range;
			this.EventType        = eventType;
			this.ExtractionAmount = extractionAmount;
		}

		public bool IsEmpty
		{
			get
			{
				return this.ResultField == ObjectField.Unknown;
			}
		}

		public static ExtractionInstructions Empty = new ExtractionInstructions (ObjectField.Unknown, DateRange.Empty, EventType.Unknown, ExtractionAmount.Final);

		public readonly ObjectField				ResultField;
		public readonly DateRange				Range;
		public readonly EventType				EventType;
		public readonly ExtractionAmount		ExtractionAmount;
	}
}