﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
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
		public ExtractionInstructions(ObjectField resultField, ExtractionAmount extractionAmount, DateRange range, EventType filteredEventType)
		{
			switch (extractionAmount)
			{
				case ExtractionAmount.StateAt:
					System.Diagnostics.Debug.Assert (filteredEventType == EventType.Unknown);
					break;

				case ExtractionAmount.Filtered:
					System.Diagnostics.Debug.Assert (filteredEventType != EventType.Unknown);
					break;

				case ExtractionAmount.Amortizations:
					System.Diagnostics.Debug.Assert (filteredEventType != EventType.Unknown);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown ExtractionAmount {0}", extractionAmount));
			}

			this.ResultField       = resultField;
			this.ExtractionAmount  = extractionAmount;
			this.Range             = range;
			this.FilteredEventType = filteredEventType;
		}

		public bool IsEmpty
		{
			get
			{
				return this.ResultField == ObjectField.Unknown;
			}
		}

		public static ExtractionInstructions Empty = new ExtractionInstructions (ObjectField.Unknown, ExtractionAmount.StateAt, DateRange.Empty, EventType.Unknown);

		public readonly ObjectField				ResultField;
		public readonly ExtractionAmount		ExtractionAmount;
		public readonly DateRange				Range;
		public readonly EventType				FilteredEventType;
	}
}