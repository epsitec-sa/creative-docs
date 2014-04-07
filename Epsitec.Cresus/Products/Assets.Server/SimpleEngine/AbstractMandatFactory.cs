//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Classe de base pour fabriquer un nouveau mandat tout beau tout propre.
	/// </summary>
	public abstract class AbstractMandatFactory : System.IDisposable
	{
		public AbstractMandatFactory(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public void Dispose()
		{
		}


		public abstract DataMandat Create(string name, System.DateTime startDate, bool withSamples);


		protected virtual void AddAssetsSettings()
		{
			this.fieldAssetName   = this.AddSettings (BaseType.Assets, "Nom",         FieldType.String, 180, 380, 1,    1,  0);
			this.fieldAssetNumber = this.AddSettings (BaseType.Assets, "Num�ro",      FieldType.String,  90,  90, 1, null,  0);
			this.fieldAssetDesc   = this.AddSettings (BaseType.Assets, "Description", FieldType.String, 120, 380, 5, null, 10);
		}

		protected virtual void AddPersonsSettings()
		{
			this.fieldPersonLastName  = this.AddSettings (BaseType.Persons, "Nom",           FieldType.String, 120, 380, 1, 2,     0);
			this.fieldPersonFirstName = this.AddSettings (BaseType.Persons, "Pr�nom",        FieldType.String, 120, 380, 1, 1,     0);
			this.fieldPersonTitle     = this.AddSettings (BaseType.Persons, "Titre",         FieldType.String,  80, 120, 1, null,  0);
			this.fieldPersonCompany   = this.AddSettings (BaseType.Persons, "Entreprise",    FieldType.String, 120, 380, 1, 3,     0);
			this.fieldPersonAddress   = this.AddSettings (BaseType.Persons, "Adresse",       FieldType.String, 150, 380, 2, null,  0);
			this.fieldPersonZip       = this.AddSettings (BaseType.Persons, "NPA",           FieldType.String,  50,  60, 1, null,  0);
			this.fieldPersonCity      = this.AddSettings (BaseType.Persons, "Ville",         FieldType.String, 120, 380, 1, null,  0);
			this.fieldPersonCountry   = this.AddSettings (BaseType.Persons, "Pays",          FieldType.String, 120, 380, 1, null,  0);
			this.fieldPersonPhone1    = this.AddSettings (BaseType.Persons, "T�l. prof.",    FieldType.String, 100, 120, 1, null, 10);
			this.fieldPersonPhone2    = this.AddSettings (BaseType.Persons, "T�l. priv�",    FieldType.String, 100, 120, 1, null,  0);
			this.fieldPersonPhone3    = this.AddSettings (BaseType.Persons, "T�l. portable", FieldType.String, 100, 120, 1, null,  0);
			this.fieldPersonMail      = this.AddSettings (BaseType.Persons, "E-mail",        FieldType.String, 200, 380, 1, null,  0);
			this.fieldPersonDesc      = this.AddSettings (BaseType.Persons, "Description",   FieldType.String, 200, 380, 5, null, 10);
		}

		protected ObjectField AddSettings(BaseType baseType, string name, FieldType type, int columnWidth, int? lineWidth, int? lineCount, int? summaryOrder, int topMargin)
		{
			var field = this.accessor.Mandat.Settings.GetNewUserField ();
			this.accessor.Mandat.Settings.AddUserField (baseType, new UserField (name, field, type, columnWidth, lineWidth, lineCount, summaryOrder, topMargin));
			return field;
		}


		protected virtual void AddAssetsSamples()
		{
		}

		protected DataObject AddAssetsSamples(System.DateTime date, string name, string number, decimal value, decimal? value1, decimal? value2, string owner1, string owner2, string cat, params string[] groups)
		{
			var guid = this.accessor.CreateObject (BaseType.Assets, date, name, Guid.Empty);
			var o = this.accessor.GetObject (BaseType.Assets, guid);

			var e = o.GetEvent (0);

			e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (this.eventNumber++).ToString ()));

			this.AddField (e, this.fieldAssetNumber, number);
			this.AddAssetAmortizedAmount (e, value);

			this.AddAssetComputedAmount (e, this.fieldAssetValue1, value1);
			this.AddAssetComputedAmount (e, this.fieldAssetValue2, value2);

			this.AddAssetPerson (e, this.fieldAssetOwner1, owner1);
			this.AddAssetPerson (e, this.fieldAssetOwner2, owner2);

			this.AddAssetCategory (e, cat);

			int i = 0;
			foreach (var group in groups)
			{
				this.AddAssetGroup (e, i++, group);
			}

			return o;
		}

		protected DataEvent AddAssetEvent(DataObject o, System.DateTime date, EventType type)
		{
			var e = this.accessor.CreateAssetEvent (o, date, type);
			e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (this.eventNumber++).ToString ()));
			return e;
		}

		protected void AddAssetAmortizedAmount(DataEvent e, decimal value)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			var aa = p.Value;

			aa.InitialAmount = value;
		}

		protected void AddAssetAmortizedAmount(DataEvent e, decimal initialAmount, decimal finalAmount)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			var aa = p.Value;

			aa.AmortizationType = AmortizationType.Degressive;
			aa.InitialAmount    = initialAmount;
			aa.BaseAmount       = initialAmount;
			aa.EffectiveRate    = 1.0m - (finalAmount / initialAmount);
		}

		protected void AddAssetComputedAmount(DataEvent e, ObjectField field, decimal? value)
		{
			if (value.HasValue)
			{
				var ca = new ComputedAmount (value.Value);
				var property = new DataComputedAmountProperty (field, ca);
				e.AddProperty (property);
			}
		}

		protected void AddAssetPerson(DataEvent e, ObjectField field, string lastName)
		{
			if (!string.IsNullOrEmpty (lastName))
			{
				var person = this.GetPerson (lastName);
				this.AddField (e, field, person.Guid);
			}
		}

		protected void AddAssetGroup(DataEvent e, int index, string groupName)
		{
			if (!string.IsNullOrEmpty (groupName))
			{
				decimal? ratio = null;

				if (groupName.Contains ('/'))
				{
					var parts = groupName.Split ('/');

					groupName = parts[0];
					ratio = decimal.Parse (parts[1]);
				}

				e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+index, this.GetGroup (groupName, ratio)));
			}
		}

		protected void AddAssetCategory(DataEvent e, string catNane)
		{
			if (string.IsNullOrEmpty (catNane))
			{
				return;
			}

			var cat = this.GetCategory (catNane);

			if (cat != null)
			{
				var taux   = ObjectProperties.GetObjectPropertyDecimal (cat, null, ObjectField.AmortizationRate);
				var type   = ObjectProperties.GetObjectPropertyInt     (cat, null, ObjectField.AmortizationType);
				var period = ObjectProperties.GetObjectPropertyInt     (cat, null, ObjectField.Periodicity);
				var prorat = ObjectProperties.GetObjectPropertyInt     (cat, null, ObjectField.Prorata);
				var round  = ObjectProperties.GetObjectPropertyDecimal (cat, null, ObjectField.Round);
				var rest   = ObjectProperties.GetObjectPropertyDecimal (cat, null, ObjectField.ResidualValue);
				var c1     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account1);
				var c2     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account2);
				var c3     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account3);
				var c4     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account4);
				var c5     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account5);
				var c6     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account6);
				var c7     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account7);
				var c8     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account8);

				e.AddProperty (new DataStringProperty  (ObjectField.CategoryName,     catNane));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, taux.GetValueOrDefault ()));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, type.GetValueOrDefault (1)));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,      period.GetValueOrDefault (12)));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,          prorat.GetValueOrDefault ()));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round,            round.GetValueOrDefault ()));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,    rest.GetValueOrDefault ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account1,         c1));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account2,         c2));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account3,         c3));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account4,         c4));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account5,         c5));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account6,         c6));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account7,         c7));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account8,         c8));
			}
		}


		protected virtual void AddPersonsSamples()
		{
		}

		protected void AddPersonSample(string title, string firstName, string lastName, string company, string address, string zip, string city, string country, string phone1, string phone2, string phone3, string mail)
		{
			var persons = this.accessor.Mandat.GetData (BaseType.Persons);
			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o = new DataObject ();
			persons.Add (o);

			var e = new DataEvent (start, EventType.Input);
			o.AddEvent (e);

			this.AddField (e, this.fieldPersonTitle,     title);
			this.AddField (e, this.fieldPersonFirstName, firstName);
			this.AddField (e, this.fieldPersonLastName,  lastName);
			this.AddField (e, this.fieldPersonCompany,   company);
			this.AddField (e, this.fieldPersonAddress,   address);
			this.AddField (e, this.fieldPersonZip,       zip);
			this.AddField (e, this.fieldPersonCity,      city);
			this.AddField (e, this.fieldPersonCountry,   country);
			this.AddField (e, this.fieldPersonPhone1,    phone1);
			this.AddField (e, this.fieldPersonPhone2,    phone2);
			this.AddField (e, this.fieldPersonPhone3,    phone3);
			this.AddField (e, this.fieldPersonMail,      mail);
		}


		protected virtual void CreateGroupsSamples()
		{
		}

		protected DataObject AddGroup(DataObject parent, string name, string number)
		{
			var groups = this.accessor.Mandat.GetData (BaseType.Groups);
			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o = new DataObject ();
			groups.Add (o);

			var e = new DataEvent (start, EventType.Input);
			o.AddEvent (e);

			if (parent != null)
			{
				this.AddField (e, ObjectField.GroupParent, parent.Guid);
			}

			this.AddField(e, ObjectField.Name,   name);
			this.AddField(e, ObjectField.Number, number);

			return o;
		}


		protected virtual void CreateCatsSamples()
		{
		}

		protected void AddCat(string name, string number, decimal rate, AmortizationType type, Periodicity periodicity, ProrataType prorata, decimal round, decimal residual)
		{
			var cats = this.accessor.Mandat.GetData (BaseType.Categories);
			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o = new DataObject ();
			cats.Add (o);

			var e = new DataEvent (start, EventType.Input);
			o.AddEvent (e);

			this.AddField (e, ObjectField.Name,             name);
			this.AddField (e, ObjectField.Number,           number);
			this.AddField (e, ObjectField.AmortizationRate, rate);
			this.AddField (e, ObjectField.AmortizationType, (int) type);
			this.AddField (e, ObjectField.Periodicity,      (int) periodicity);
			this.AddField (e, ObjectField.Prorata,          (int) prorata);
			this.AddField (e, ObjectField.Round,            round);
			this.AddField (e, ObjectField.ResidualValue,    residual);
		}


		protected void AddField(DataEvent e, ObjectField field, string value)
		{
			if (!string.IsNullOrEmpty (value))
			{
				e.AddProperty (new DataStringProperty (field, value));
			}
		}

		protected void AddField(DataEvent e, ObjectField field, int? value)
		{
			if (value.HasValue)
			{
				e.AddProperty (new DataIntProperty (field, value.Value));
			}
		}

		protected void AddField(DataEvent e, ObjectField field, decimal? value)
		{
			if (value.HasValue)
			{
				e.AddProperty (new DataDecimalProperty (field, value.Value));
			}
		}

		protected void AddField(DataEvent e, ObjectField field, Guid value)
		{
			if (!value.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (field, value));
			}
		}


		protected DataObject GetPerson(string lastName)
		{
			var list = this.accessor.Mandat.GetData (BaseType.Persons);

			foreach (var person in list)
			{
				var s = ObjectProperties.GetObjectPropertyString (person, null, this.fieldPersonLastName);
				if (s == lastName)
				{
					return person;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La personne {0} n'existe pas !", lastName));
			return null;
		}

		protected GuidRatio GetGroup(string text, decimal? ratio = null)
		{
			var list = this.accessor.Mandat.GetData (BaseType.Groups);

			foreach (var group in list)
			{
				var nom = ObjectProperties.GetObjectPropertyString (group, null, ObjectField.Name);
				if (nom == text)
				{
					return new GuidRatio (group.Guid, ratio);
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("Le groupe {0} n'existe pas !", text));
			return GuidRatio.Empty;
		}

		protected DataObject GetCategory(string text)
		{
			var list = this.accessor.Mandat.GetData (BaseType.Categories);

			foreach (var group in list)
			{
				var nom = ObjectProperties.GetObjectPropertyString (group, null, ObjectField.Name);
				if (nom == text)
				{
					return group;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La cat�gorie {0} n'existe pas !", text));
			return null;
		}


		protected readonly DataAccessor			accessor;

		protected bool							withSamples;
		protected int							eventNumber;

		protected ObjectField					fieldAssetName;
		protected ObjectField					fieldAssetNumber;
		protected ObjectField					fieldAssetDesc;
		protected ObjectField					fieldAssetValue1;
		protected ObjectField					fieldAssetValue2;
		protected ObjectField					fieldAssetOwner1;
		protected ObjectField					fieldAssetOwner2;

		protected ObjectField					fieldPersonLastName;
		protected ObjectField					fieldPersonFirstName;
		protected ObjectField					fieldPersonTitle;
		protected ObjectField					fieldPersonCompany;
		protected ObjectField					fieldPersonAddress;
		protected ObjectField					fieldPersonZip;
		protected ObjectField					fieldPersonCity;
		protected ObjectField					fieldPersonCountry;
		protected ObjectField					fieldPersonPhone1;
		protected ObjectField					fieldPersonPhone2;
		protected ObjectField					fieldPersonPhone3;
		protected ObjectField					fieldPersonMail;
		protected ObjectField					fieldPersonDesc;
	}
}