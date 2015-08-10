﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

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
		public ExtractionInstructions(ObjectField resultField, ExtractionAmount extractionAmount, DateRange range, bool directMode, bool inverted, params EventType[] filteredEventType)
		{
			switch (extractionAmount)
			{
				case ExtractionAmount.StateAt:
				case ExtractionAmount.UserColumn:
					System.Diagnostics.Debug.Assert (filteredEventType == null);
					break;

				case ExtractionAmount.LastFiltered:
				case ExtractionAmount.DeltaSum:
					System.Diagnostics.Debug.Assert (filteredEventType != null);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown ExtractionAmount {0}", extractionAmount));
			}

			this.ResultField       = resultField;
			this.ExtractionAmount  = extractionAmount;
			this.Range             = range;
			this.FilteredEventType = filteredEventType;
			this.DirectMode        = directMode;
			this.Inverted          = inverted;
		}

		public bool IsEmpty
		{
			get
			{
				return this.ResultField == ObjectField.Unknown;
			}
		}

		public static ExtractionInstructions Empty = new ExtractionInstructions (ObjectField.Unknown, ExtractionAmount.StateAt, DateRange.Empty, true, false);

		public readonly ObjectField				ResultField;
		public readonly ExtractionAmount		ExtractionAmount;
		public readonly DateRange				Range;
		public readonly bool					DirectMode;
		public readonly bool					Inverted;
		public readonly EventType[]				FilteredEventType;
	}
}