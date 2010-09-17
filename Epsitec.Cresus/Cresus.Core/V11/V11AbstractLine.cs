//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public abstract class V11AbstractLine
	{
		#region Enumerations
		public enum TypeEnum
		{
			Unknown,
			Type3,
			Type4,
		}
		#endregion


		public V11AbstractLine(TypeEnum type)
		{
			this.type = type;
		}


		public TypeEnum Type
		{
			get
			{
				return this.type;
			}
		}


		public virtual bool IsValid
		{
			get
			{
				return this.Type != TypeEnum.Unknown;
			}
		}


		private readonly TypeEnum type;
	}
}
