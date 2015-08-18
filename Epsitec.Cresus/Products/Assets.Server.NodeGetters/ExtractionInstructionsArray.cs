﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct ExtractionInstructionsArray
	{
		public ExtractionInstructionsArray(ObjectField resultField, params ExtractionInstructions[] instructions)
		{
			this.resultFieldield = resultField;
			this.array           = instructions.ToArray ();
		}

		public bool IsEmpty
		{
			get
			{
				return this.resultFieldield == ObjectField.Unknown || this.array == null || this.array.Length == 0;
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
				return this.resultFieldield;
			}
		}


		public static ExtractionInstructionsArray Empty = new ExtractionInstructionsArray ();

		private readonly ObjectField				resultFieldield;
		private readonly ExtractionInstructions[]	array;
	}
}