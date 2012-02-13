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
	public class SettingsList
	{
		public SettingsList()
		{
			this.settings = new Dictionary<SettingsType, AbstractSettingsData> ();
			this.errors = new Dictionary<SettingsType, FormattedText> ();

			this.Initialize ();
		}


		private void Initialize()
		{
			//	Réglages généraux :
			this.Add (new TextSettingsData (SettingsGroup.Global, SettingsType.GlobalTitre,        20, "vide", skipCompareTo: true));
			this.Add (new TextSettingsData (SettingsGroup.Global, SettingsType.GlobalDescription, 100, "",     skipCompareTo: true));
										     
			//	Réglages pur les écritures :
			this.Add (new BoolSettingsData   (SettingsGroup.Ecriture, SettingsType.EcriturePièces,          true));
			this.Add (new BoolSettingsData   (SettingsGroup.Ecriture, SettingsType.EcritureAutoPièces,      true));
			this.Add (new TextSettingsData   (SettingsGroup.Ecriture, SettingsType.EcritureProchainePièce,  10, "1"));
			this.Add (new IntSettingsData    (SettingsGroup.Ecriture, SettingsType.EcritureIncrémentPièce,  1));
			this.Add (new BoolSettingsData   (SettingsGroup.Ecriture, SettingsType.EcriturePlusieursPièces, false));
			this.Add (new BoolSettingsData   (SettingsGroup.Ecriture, SettingsType.EcritureForcePièces,     false));

			//	Réglages pour les montants :
			this.Add (new EnumSettingsData   (SettingsGroup.Price,   SettingsType.PriceDecimalDigits,    SettingsEnum.DecimalDigits2,        SettingsEnum.DecimalDigits0, SettingsEnum.DecimalDigits1, SettingsEnum.DecimalDigits2, SettingsEnum.DecimalDigits3, SettingsEnum.DecimalDigits4, SettingsEnum.DecimalDigits5));
			this.Add (new EnumSettingsData   (SettingsGroup.Price,   SettingsType.PriceDecimalSeparator, SettingsEnum.SeparatorDot,          SettingsEnum.SeparatorDot, SettingsEnum.SeparatorComma));
			this.Add (new EnumSettingsData   (SettingsGroup.Price,   SettingsType.PriceGroupSeparator,   SettingsEnum.SeparatorApostrophe,   SettingsEnum.SeparatorNone, SettingsEnum.SeparatorApostrophe, SettingsEnum.SeparatorSpace, SettingsEnum.SeparatorComma, SettingsEnum.SeparatorDot));
			this.Add (new EnumSettingsData   (SettingsGroup.Price,   SettingsType.PriceNullParts,        SettingsEnum.NullPartsZeroZero,     SettingsEnum.NullPartsZeroZero, SettingsEnum.NullPartsZeroDash, SettingsEnum.NullPartsDashZero, SettingsEnum.NullPartsDashDash));
			this.Add (new EnumSettingsData   (SettingsGroup.Price,   SettingsType.PriceNegativeFormat,   SettingsEnum.NegativeMinus,         SettingsEnum.NegativeMinus, SettingsEnum.NegativeParentheses));
			this.Add (new SampleSettingsData (SettingsGroup.Price,   SettingsType.PriceSample, this));

			//	Réglages pour les dates :
			this.Add (new EnumSettingsData   (SettingsGroup.Date,    SettingsType.DateSeparator, SettingsEnum.SeparatorDot,   SettingsEnum.SeparatorDot, SettingsEnum.SeparatorSlash, SettingsEnum.SeparatorDash));
			this.Add (new EnumSettingsData   (SettingsGroup.Date,    SettingsType.DateYear,      SettingsEnum.YearDigits4,    SettingsEnum.YearDigits2, SettingsEnum.YearDigits4));
			this.Add (new EnumSettingsData   (SettingsGroup.Date,    SettingsType.DateOrder,     SettingsEnum.YearDMY,        SettingsEnum.YearDMY, SettingsEnum.YearYMD));
			this.Add (new SampleSettingsData (SettingsGroup.Date,    SettingsType.DateSample, this));
		}

		private void Add(AbstractSettingsData data)
		{
			this.settings.Add (data.Type, data);
		}


		public IEnumerable<AbstractSettingsData> List
		{
			get
			{
				return this.settings.Values;
			}
		}


		public bool GetBool(SettingsType type)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as BoolSettingsData).Value;
			}
			else
			{
				return false;
			}
		}

		public void SetBool(SettingsType type, bool value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as BoolSettingsData).Value = value;
			}
		}


		public int? GetInt(SettingsType type)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as IntSettingsData).Value;
			}
			else
			{
				return null;
			}
		}

		public void SetInt(SettingsType type, int value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as IntSettingsData).Value = value;
			}
		}


		public FormattedText GetText(SettingsType type)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as TextSettingsData).Value;
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public void SetText(SettingsType type, FormattedText value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as TextSettingsData).Value = value;
			}
		}


		public SettingsEnum GetEnum(SettingsType type)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as EnumSettingsData).Value;
			}
			else
			{
				return SettingsEnum.Unknown;
			}
		}

		public void SetEnum(SettingsType type, SettingsEnum value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as EnumSettingsData).Value = value;
			}
		}


		public bool Compare(SettingsList other, SettingsGroup group)
		{
			//	Compare les valeurs des réglages d'un groupe avec celles d'un autre jeu de réglages.
			foreach (var settings in this.settings.Values.Where (x => x.Group == group))
			{
				AbstractSettingsData otherSettings;
				if (other.settings.TryGetValue (settings.Type, out otherSettings))
				{
					if (!settings.SkipCompareTo && !settings.CompareTo (otherSettings))
					{
						return false;
					}
				}
			}

			return true;
		}

		public void CopyFrom(SettingsList other, SettingsGroup group)
		{
			//	Reprend les valeurs des réglages d'un groupe d'après celles d'un autre jeu de réglages.
			foreach (var settings in this.settings.Values.Where (x => x.Group == group))
			{
				AbstractSettingsData otherSettings;
				if (other.settings.TryGetValue (settings.Type, out otherSettings))
				{
					settings.CopyFrom (otherSettings);
				}
			}
		}


		public void Validate()
		{
			//	Vérifie la cohérence de l'ensemble des réglages.
			this.errors.Clear ();
			this.errorCount = 0;

			if (this.GetEnum (SettingsType.PriceDecimalSeparator) == this.GetEnum (SettingsType.PriceGroupSeparator))
			{
				this.AddError (SettingsType.PriceDecimalSeparator, SettingsType.PriceGroupSeparator, "Mêmes séparateurs");
			}

			if (this.GetEnum (SettingsType.PriceNegativeFormat) == SettingsEnum.NegativeMinus &&
				(this.GetEnum (SettingsType.PriceNullParts) == SettingsEnum.NullPartsDashZero ||
				 this.GetEnum (SettingsType.PriceNullParts) == SettingsEnum.NullPartsDashDash))
			{
				this.AddError (SettingsType.PriceNegativeFormat, SettingsType.PriceNullParts, "Choix incompatibles");
			}
		}

		private void AddError(SettingsType type1, SettingsType type2, FormattedText message)
		{
			//	Ajoute une erreur à deux champs.
			this.errors.Add (type1, message);
			this.errors.Add (type2, message);

			this.errorCount++;  // on ne la compte que comme une seule erreur
		}

		public bool HasError
		{
			get
			{
				return this.errorCount != 0;
			}
		}

		public int ErrorCount
		{
			get
			{
				return this.errorCount;
			}
		}

		public FormattedText GetError(SettingsType type)
		{
			//	Retourne l'erreur éventuelle liée à un réglage.
			FormattedText error;

			if (errors.TryGetValue (type, out error))
			{
				return error;
			}
			else
			{
				return FormattedText.Null;
			}
		}


		private readonly Dictionary<SettingsType, AbstractSettingsData>		settings;
		private readonly Dictionary<SettingsType, FormattedText>			errors;

		private int errorCount;
	}
}