//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct ExtractionInstructionsArray
	{
		public ExtractionInstructionsArray(params ExtractionInstructions[] instructions)
		{
			this.array = instructions.ToArray ();
		}

		public bool IsEmpty
		{
			get
			{
				return this.array == null || this.array.Length == 0;
			}
		}

		public IEnumerable<ExtractionInstructions> Array
		{
			get
			{
				return this.array;
			}
		}

		public ObjectField ResultField
		{
			get
			{
				if (this.IsEmpty)
				{
					return ObjectField.Unknown;
				}
				else
				{
					return this.array[0].ResultField;
				}
			}
		}


		public static ExtractionInstructionsArray Empty = new ExtractionInstructionsArray ();

		private readonly ExtractionInstructions[]	array;
	}
}