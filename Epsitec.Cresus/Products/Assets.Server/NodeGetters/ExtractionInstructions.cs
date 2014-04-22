//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct ExtractionInstructions
	{
		public ExtractionInstructions(ObjectField resultField, Timestamp startTimestamp, Timestamp endTimestamp, EventType eventType, ExtractionAmount extractionAmount)
		{
			this.ResultField      = resultField;
			this.StartTimestamp   = startTimestamp;
			this.EndTimestamp     = endTimestamp;
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

		public static ExtractionInstructions Empty = new ExtractionInstructions (ObjectField.Unknown, Timestamp.MaxValue, Timestamp.MaxValue, EventType.Unknown, ExtractionAmount.Final);

		public readonly ObjectField				ResultField;
		public readonly Timestamp				StartTimestamp;
		public readonly Timestamp				EndTimestamp;
		public readonly EventType				EventType;
		public readonly ExtractionAmount		ExtractionAmount;
	}
}