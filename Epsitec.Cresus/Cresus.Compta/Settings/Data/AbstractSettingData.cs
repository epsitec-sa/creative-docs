//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Data
{
	/// <summary>
	/// Données génériques pour un réglage.
	/// </summary>
	public abstract class AbstractSettingData
	{
		public AbstractSettingData(SettingsGroup group, SettingsType type, System.Func<FormattedText> validateAction = null, bool skipCompareTo = false)
		{
			this.Group          = group;
			this.Type           = type;
			this.ValidateAction = validateAction;
			this.SkipCompareTo  = skipCompareTo;

			this.error = FormattedText.Null;
		}

		public SettingsGroup Group
		{
			get;
			private set;
		}

		public SettingsType Type
		{
			get;
			private set;
		}

		public System.Func<FormattedText> ValidateAction
		{
			//	Valide le réglage et retourne un éventuel message d'erreur.
			//	On l'utilise pour valider les réglages qui dépendent d'autres réglages.
			get;
			private set;
		}

		public bool SkipCompareTo
		{
			get;
			private set;
		}

		public virtual bool CompareTo(AbstractSettingData other)
		{
			return true;
		}

		public virtual void CopyFrom(AbstractSettingData other)
		{
		}


		public bool HasError
		{
			get
			{
				return !this.error.IsNullOrEmpty ();
			}
		}

		public void SetError(FormattedText error)
		{
			this.error = error;
		}

		public FormattedText Error
		{
			get
			{
				return this.error;
			}
		}


		protected FormattedText						error;
	}
}