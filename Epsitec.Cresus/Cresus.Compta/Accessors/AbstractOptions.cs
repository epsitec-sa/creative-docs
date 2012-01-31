//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Cette classe décrit les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptions : ISettingsData
	{
		public AbstractOptions()
		{
		}

		public virtual void SetComptaEntity(ComptaEntity compta)
		{
			this.comptaEntity = compta;
		}


		public virtual void Clear()
		{
			this.Profondeur        = null;
			this.BudgetEnable      = false;
			this.BudgetShowed      = BudgetShowed.Budget;
			this.BudgetDisplayMode = BudgetDisplayMode.Montant;
		}


		public bool Specialist
		{
			get;
			set;
		}

		public int? Profondeur
		{
			get;
			set;
		}


		public bool BudgetEnable
		{
			get;
			set;
		}

		public BudgetShowed BudgetShowed
		{
			get;
			set;
		}

		public BudgetDisplayMode BudgetDisplayMode
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
			return this.Profondeur == other.Profondeur &&
				   this.BudgetEnable == other.BudgetEnable &&
				   this.BudgetShowed == other.BudgetShowed &&
				   this.BudgetDisplayMode == other.BudgetDisplayMode;
		}


		public FormattedText BudgetColumnDescription
		{
			get
			{
				switch (this.BudgetShowed)
				{
					case BudgetShowed.Budget:
					case BudgetShowed.Prorata:
						return "Budget";

					case BudgetShowed.Futur:
					case BudgetShowed.FuturProrata:
						return "Budget futur";

					case BudgetShowed.Précédent:
						return "Précédent";

					default:
						return FormattedText.Empty;
				}
			}
		}


		protected ComptaEntity						comptaEntity;
		protected AbstractOptions					emptyOptions;
	}
}
