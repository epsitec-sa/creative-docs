//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données génériques pour la comptabilité.
	/// </summary>
	public abstract class AbstractData	{
		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}
 

		public bool IsBold
		{
			get;
			set;
		}

		public bool IsItalic
		{
			get;
			set;
		}

		public bool HasBottomSeparator
		{
			get;
			set;
		}


		public FormattedText Typo(FormattedText value)
		{
			//	Retourne un texte décoré par les propriétés typographiques IsBold et IsItalic, s'il ne
			//	s'agit pas d'un contenu spécial "$${_xxx_}$$".

			if (this.IsBold && !value.IsNullOrEmpty && !value.ToString ().StartsWith (StringArray.SpecialContentStart))
			{
				value = value.ApplyBold ();
			}

			if (this.IsItalic && !value.IsNullOrEmpty && !value.ToString ().StartsWith (StringArray.SpecialContentStart))
			{
				value = value.ApplyItalic ();
			}

			return value;
		}


		protected AbstractEntity entity;
	}
}