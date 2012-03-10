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
			this.settings = new Dictionary<SettingsType, AbstractSettingData> ();

			this.Initialize ();
		}


		private void Initialize()
		{
			//	Réglages généraux :
			this.Add (new TextSettingData (SettingsGroup.Global, SettingsType.GlobalTitre,        20, "vide", skipCompareTo: true));
			this.Add (new TextSettingData (SettingsGroup.Global, SettingsType.GlobalDescription, 100, "",     skipCompareTo: true));
			this.Add (new BoolSettingData (SettingsGroup.Global, SettingsType.GlobalRemoveConfirmation, true));
										     
			//	Réglages pur les écritures :
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureMontantZéro,     true));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcriturePièces,          true));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureAutoPièces,      true));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcriturePlusieursPièces, false));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureForcePièces,     false));
			this.Add (new IntSettingData     (SettingsGroup.Ecriture, SettingsType.EcritureMultiEditionLineCount, 5, 3, 10));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureTVA,             true));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureEditeMontantTVA, false));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureEditeMontantHT,  true));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureEditeCodeTVA,    true));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureEditeTauxTVA,    true));
			this.Add (new BoolSettingData    (SettingsGroup.Ecriture, SettingsType.EcritureMontreCompteTVA, false));
			this.Add (new DecimalSettingData (SettingsGroup.Ecriture, SettingsType.EcritureArrondiTVA, 0.05m, 0.0m, 1.0m));

			//	Réglages pour les montants :
			this.Add (new IntSettingData    (SettingsGroup.Price, SettingsType.PriceDecimalDigits,    2, 0, 5));
			this.Add (new EnumSettingData   (SettingsGroup.Price, SettingsType.PriceDecimalSeparator, SettingsEnum.SeparatorDot,        this.ValidateSeparator, SettingsEnum.SeparatorDot, SettingsEnum.SeparatorComma));
			this.Add (new EnumSettingData   (SettingsGroup.Price, SettingsType.PriceGroupSeparator,   SettingsEnum.SeparatorApostrophe, this.ValidateSeparator, SettingsEnum.SeparatorNone, SettingsEnum.SeparatorApostrophe, SettingsEnum.SeparatorSpace, SettingsEnum.SeparatorComma, SettingsEnum.SeparatorDot));
			this.Add (new EnumSettingData   (SettingsGroup.Price, SettingsType.PriceNullParts,        SettingsEnum.NullPartsZeroZero,   this.ValidateNegative,  SettingsEnum.NullPartsZeroZero, SettingsEnum.NullPartsZeroDash, SettingsEnum.NullPartsDashZero, SettingsEnum.NullPartsDashDash));
			this.Add (new EnumSettingData   (SettingsGroup.Price, SettingsType.PriceNegativeFormat,   SettingsEnum.NegativeMinus,       this.ValidateNegative,  SettingsEnum.NegativeMinus, SettingsEnum.NegativeParentheses));
			this.Add (new SampleSettingData (SettingsGroup.Price, SettingsType.PriceSample, this));

			//	Réglages pour les pourcents :
			this.Add (new EnumSettingData   (SettingsGroup.Percent, SettingsType.PercentDecimalSeparator, SettingsEnum.SeparatorDot, null, SettingsEnum.SeparatorDot, SettingsEnum.SeparatorComma));
			this.Add (new EnumSettingData   (SettingsGroup.Percent, SettingsType.PercentFracFormat, SettingsEnum.PercentFrac1, null, SettingsEnum.PercentFloating, SettingsEnum.PercentFrac1, SettingsEnum.PercentFrac2, SettingsEnum.PercentFrac3));
			this.Add (new SampleSettingData (SettingsGroup.Percent, SettingsType.PercentSample, this));

			//	Réglages pour les dates :
			this.Add (new EnumSettingData   (SettingsGroup.Date, SettingsType.DateSeparator, SettingsEnum.SeparatorDot, null, SettingsEnum.SeparatorDot, SettingsEnum.SeparatorSlash, SettingsEnum.SeparatorDash));
			this.Add (new EnumSettingData   (SettingsGroup.Date, SettingsType.DateYear,      SettingsEnum.YearDigits4,  null, SettingsEnum.YearDigits2, SettingsEnum.YearDigits4));
			this.Add (new EnumSettingData   (SettingsGroup.Date, SettingsType.DateOrder,     SettingsEnum.YearDMY,      null, SettingsEnum.YearDMY, SettingsEnum.YearYMD));
			this.Add (new SampleSettingData (SettingsGroup.Date, SettingsType.DateSample, this));
		}

		private void Add(AbstractSettingData data)
		{
			this.settings.Add (data.Type, data);
		}

		private FormattedText ValidateSeparator()
		{
			//	Valide le choix du séparateur pour les milliers et la partie fractionnaire des nombres.
			//	On ne peut pas utiliser le même séparateur pour les milliers et pour la partie fractionnaire.
			if (this.GetEditedEnum (SettingsType.PriceDecimalSeparator) == this.GetEditedEnum (SettingsType.PriceGroupSeparator))
			{
				return "Mêmes séparateurs";
			}
			else
			{
				return FormattedText.Null;
			}
		}

		private FormattedText ValidateNegative()
		{
			//	Valide le choix de la représentation des nombres négatifs.
			//	Les choix '-n' et '-.0' ou '-.-' sont incompatibles.
			if (this.GetEditedEnum (SettingsType.PriceNegativeFormat) == SettingsEnum.NegativeMinus &&
				(this.GetEditedEnum (SettingsType.PriceNullParts) == SettingsEnum.NullPartsDashZero ||
				 this.GetEditedEnum (SettingsType.PriceNullParts) == SettingsEnum.NullPartsDashDash))
			{
				return "Choix incompatibles";
			}
			else
			{
				return FormattedText.Null;
			}
		}


		public IEnumerable<AbstractSettingData> List
		{
			get
			{
				return this.settings.Values;
			}
		}

		public AbstractSettingData GetSettingData(SettingsType type)
		{
			return this.settings[type];
		}


		public bool GetBool(SettingsType type)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as BoolSettingData).Value;
			}
			else
			{
				return false;
			}
		}

		public void SetBool(SettingsType type, bool value)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as BoolSettingData).Value = value;
			}
		}


		public int? GetInt(SettingsType type)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as IntSettingData).Value;
			}
			else
			{
				return null;
			}
		}

		public void SetInt(SettingsType type, int value)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as IntSettingData).Value = value;
			}
		}


		public decimal? GetDecimal(SettingsType type)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as DecimalSettingData).Value;
			}
			else
			{
				return null;
			}
		}

		public void SetDecimal(SettingsType type, decimal value)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as DecimalSettingData).Value = value;
			}
		}


		public FormattedText GetText(SettingsType type)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as TextSettingData).Value;
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public void SetText(SettingsType type, FormattedText value)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as TextSettingData).Value = value;
			}
		}


		private SettingsEnum GetEditedEnum(SettingsType type)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as EnumSettingData).EditedValue;
			}
			else
			{
				return SettingsEnum.Unknown;
			}
		}

		public SettingsEnum GetEnum(SettingsType type)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				return (data as EnumSettingData).Value;
			}
			else
			{
				return SettingsEnum.Unknown;
			}
		}

		public void SetEnum(SettingsType type, SettingsEnum value)
		{
			AbstractSettingData data;
			if (this.settings.TryGetValue (type, out data))
			{
				(data as EnumSettingData).Value = value;
			}
		}


		public int ErrorCount
		{
			get
			{
				return this.settings.Values.Where (x => x.HasError).Count ();
			}
		}

		public bool HasError(params SettingsType[] types)
		{
			foreach (var setting in this.settings.Values)
			{
				if (setting.HasError && types.Contains (setting.Type))
				{
					return true;
				}
			}

			return false;
		}


		public bool Compare(SettingsList other, SettingsGroup group)
		{
			//	Compare les valeurs des réglages d'un groupe avec celles d'un autre jeu de réglages.
			foreach (var settings in this.settings.Values.Where (x => x.Group == group))
			{
				AbstractSettingData otherSettings;
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
				AbstractSettingData otherSettings;
				if (other.settings.TryGetValue (settings.Type, out otherSettings))
				{
					settings.CopyFrom (otherSettings);
				}
			}
		}


		private readonly Dictionary<SettingsType, AbstractSettingData>		settings;
	}
}