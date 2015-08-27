//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Cette classe contient une collection d'instructions d'extraction permettent de définir des
	/// montants calculés par CumulNodeGetter, en vue de la production de rapports.
	/// Le montant est toujours égal à la somme des instructions individuelles.
	/// </summary>
	public struct ExtractionInstructionsArray
	{
		public ExtractionInstructionsArray(ObjectField resultField, params ExtractionInstructions[] instructions)
		{
			this.resultField            = resultField;
			this.extractionInstructions = instructions.ToArray ();
		}

		public bool								IsEmpty
		{
			get
			{
				return this.resultField == ObjectField.Unknown || this.extractionInstructions == null || this.extractionInstructions.Length == 0;
			}
		}

		public ObjectField						ResultField
		{
			get
			{
				return this.resultField;
			}
		}

		public AbstractCumulValue GetSum(DataAccessor accessor, DataObject obj,
			System.Func<DataAccessor, DataObject, ObjectField, ExtractionInstructions, AbstractCumulValue> action)
		{
			//	Retourne la somme des toutes les instructions contenues.
			AbstractCumulValue v = null;

			if (!this.IsEmpty)
			{
				foreach (var ei in this.extractionInstructions)
				{
					var x = action (accessor, obj, this.resultField, ei);
					if (v == null)
					{
						v = x;
					}
					else
					{
						v = v.Merge (x);
					}
				}
			}

			return v;
		}


		private readonly ObjectField				resultField;
		private readonly ExtractionInstructions[]	extractionInstructions;
	}
}