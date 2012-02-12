//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Data
{
	/// <summary>
	/// Cette classe décrit les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptions : ISettingsData
	{
		public virtual void SetComptaEntity(ComptaEntity compta)
		{
			this.comptaEntity = compta;
		}


		public virtual void Clear()
		{
			this.ComparisonEnable      = false;
			this.ComparisonShowed      = ComparisonShowed.None;
			this.ComparisonDisplayMode = ComparisonDisplayMode.Montant;
		}


		public bool Specialist
		{
			get;
			set;
		}


		public bool ComparisonEnable
		{
			get;
			set;
		}

		public ComparisonShowed ComparisonShowed
		{
			get;
			set;
		}

		public ComparisonDisplayMode ComparisonDisplayMode
		{
			get;
			set;
		}


		public bool IsEmpty
		{
			get
			{
				if (this.emptyOptions == null)
				{
					this.CreateEmpty ();
				}

				if (this.emptyOptions == null)
				{
					return false;
				}
				else
				{
					return this.CompareTo (this.emptyOptions);
				}
			}
		}

		protected virtual void CreateEmpty()
		{
		}

		public virtual bool CompareTo(AbstractOptions other)
		{
			return this.ComparisonEnable == other.ComparisonEnable &&
				   this.ComparisonShowed == other.ComparisonShowed &&
				   this.ComparisonDisplayMode == other.ComparisonDisplayMode;
		}


		protected ComptaEntity						comptaEntity;
		protected AbstractOptions					emptyOptions;
	}
}
