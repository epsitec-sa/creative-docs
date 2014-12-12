//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Engine
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
			this.fieldAssetName   = this.AddSettings (BaseType.AssetsUserFields, "Nom",    FieldType.String, true,  200, 380, 1,    1,  0);
			this.fieldAssetNumber = this.AddSettings (BaseType.AssetsUserFields, "Numéro", FieldType.String, false,  70,  90, 1, null,  0);
		}

		protected virtual void AddPersonsSettings()
		{
			this.fieldPersonLastName  = this.AddSettings (BaseType.PersonsUserFields, "Nom",           FieldType.String, true,  120, 380, 1, 2,     0);
			this.fieldPersonFirstName = this.AddSettings (BaseType.PersonsUserFields, "Prénom",        FieldType.String, false, 120, 380, 1, 1,     0);
			this.fieldPersonTitle     = this.AddSettings (BaseType.PersonsUserFields, "Titre",         FieldType.String, false,  80, 120, 1, null,  0);
			this.fieldPersonCompany   = this.AddSettings (BaseType.PersonsUserFields, "Entreprise",    FieldType.String, false, 120, 380, 1, 3,     0);
			this.fieldPersonAddress   = this.AddSettings (BaseType.PersonsUserFields, "Adresse",       FieldType.String, false, 150, 380, 2, null,  0);
			this.fieldPersonZip       = this.AddSettings (BaseType.PersonsUserFields, "NPA",           FieldType.String, true,   50,  60, 1, null,  0);
			this.fieldPersonCity      = this.AddSettings (BaseType.PersonsUserFields, "Ville",         FieldType.String, true,  120, 380, 1, null,  0);
			this.fieldPersonCountry   = this.AddSettings (BaseType.PersonsUserFields, "Pays",          FieldType.String, false, 120, 380, 1, null,  0);
			this.fieldPersonPhone1    = this.AddSettings (BaseType.PersonsUserFields, "Tél. prof.",    FieldType.String, false, 100, 120, 1, null, 10);
			this.fieldPersonPhone2    = this.AddSettings (BaseType.PersonsUserFields, "Tél. privé",    FieldType.String, false, 100, 120, 1, null,  0);
			this.fieldPersonPhone3    = this.AddSettings (BaseType.PersonsUserFields, "Tél. portable", FieldType.String, false, 100, 120, 1, null,  0);
			this.fieldPersonMail      = this.AddSettings (BaseType.PersonsUserFields, "E-mail",        FieldType.String, false, 200, 380, 1, null,  0);
			this.fieldPersonDesc      = this.AddSettings (BaseType.PersonsUserFields, "Description",   FieldType.String, false, 200, 380, 5, null, 10);
		}

		protected ObjectField AddSettings(BaseType baseType, string name, FieldType type, bool required, int columnWidth, int? lineWidth, int? lineCount, int? summaryOrder, int topMargin)
		{
			var field = this.accessor.UserFieldsAccessor.GetNewUserField ();
			int order = this.accessor.UserFieldsAccessor.GetUserFields (baseType).Count ();

			this.accessor.UserFieldsAccessor.AddUserField (baseType, new UserField (order, name, field, type, required, columnWidth, lineWidth, lineCount, summaryOrder, topMargin));

			return field;
		}


		protected virtual void AddAssetsSamples()
		{
		}

		protected DataObject AddAssetsSamples(System.DateTime date, string name, string number, decimal value, decimal? value1, decimal? value2, string owner1, string owner2, string cat, params string[] groups)
		{
			var guid = this.accessor.CreateObject (BaseType.Assets, date, name, Guid.Empty);
			var o = this.accessor.GetObject (BaseType.Assets, guid);

			var e = o.GetInputEvent ();

			e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (++this.eventNumber).ToString ()));

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
			e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (++this.eventNumber).ToString ()));
			return e;
		}

		protected void AddAssetAmortizedAmount(DataEvent e, decimal value)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

			var aa = AmortizedAmount.SetFinalAmount (p.Value, value);
			Amortizations.SetAmortizedAmount (e, aa);
		}

		protected void AddAssetAmortizedAmount(DataEvent e, decimal initialAmount, decimal finalAmount)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

			var aa = AmortizedAmount.SetAmounts(p.Value, initialAmount, finalAmount, null);
			Amortizations.SetAmortizedAmount (e, aa);
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
				var method = ObjectProperties.GetObjectPropertyGuid (cat, null, ObjectField.MethodGuid);
				var period = ObjectProperties.GetObjectPropertyInt  (cat, null, ObjectField.Periodicity);

				e.AddProperty (new DataStringProperty (ObjectField.CategoryName, catNane));
				e.AddProperty (new DataGuidProperty   (ObjectField.MethodGuid,   method));
				e.AddProperty (new DataIntProperty    (ObjectField.Periodicity,  period.GetValueOrDefault (12)));

				foreach (var field in DataAccessor.ArgumentFields)
				{
					var c = ObjectProperties.GetObjectPropertyDecimal (cat, null, field);
					if (c.HasValue)
					{
						e.AddProperty (new DataDecimalProperty (field, c.Value));
					}
				}

				foreach (var field in DataAccessor.AccountFields)
				{
					var c = ObjectProperties.GetObjectPropertyString  (cat, null, field);
					e.AddProperty (new DataStringProperty (field, c));
				}
			}
		}


		protected virtual void AddPersonsSamples()
		{
		}

		protected void AddPersonSample(string title, string firstName, string lastName, string company, string address, string zip, string city, string country, string phone1, string phone2, string phone3, string mail)
		{
			var persons = this.accessor.Mandat.GetData (BaseType.Persons);
			var start  = new Timestamp (this.accessor.Mandat.StartDate, 0);

			var o = new DataObject (this.accessor.UndoManager);
			persons.Add (o);

			var e = new DataEvent (this.accessor.UndoManager, start, EventType.Input);
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

		protected DataObject AddGroup(DataObject parent, string name, string number, bool groupSuggestedDuringCreation = false)
		{
			var groups = this.accessor.Mandat.GetData (BaseType.Groups);
			var start  = new Timestamp (this.accessor.Mandat.StartDate, 0);

			var o = new DataObject (this.accessor.UndoManager);
			groups.Add (o);

			var e = new DataEvent (this.accessor.UndoManager, start, EventType.Input);
			o.AddEvent (e);

			if (parent != null)
			{
				this.AddField (e, ObjectField.GroupParent, parent.Guid);
			}

			this.AddField (e, ObjectField.Name,   name);
			this.AddField (e, ObjectField.Number, number);
			this.AddField (e, ObjectField.GroupSuggestedDuringCreation, groupSuggestedDuringCreation);

			return o;
		}


		protected virtual void CreateCatsSamples()
		{
		}

		protected void AddCat(string name, string desc, string number,
			string methodName, Periodicity periodicity,
			string accountPurchaseDebit = null, string accountPurchaseCredit = null,
			string accountSaleDebit = null, string accountSaleCredit = null,
			string accountAmortizationAutoDebit = null, string accountAmortizationAutoCredit = null,
			string accountAmortizationExtraDebit = null, string accountAmortizationExtraCredit = null,
			string accountIncreaseDebit = null, string accountIncreaseCredit = null,
			string accountDecreaseDebit = null, string accountDecreaseCredit = null,
			string accountAdjustDebit = null, string accountAdjustCredit = null,
			params decimal?[] arguments)
		{
			var cats = this.accessor.Mandat.GetData (BaseType.Categories);
			var start = new Timestamp (this.accessor.Mandat.StartDate, 0);

			var o = new DataObject (this.accessor.UndoManager);
			cats.Add (o);

			var e = new DataEvent (this.accessor.UndoManager, start, EventType.Input);
			o.AddEvent (e);

			var method = this.GetMethod (methodName);

			this.AddField (e, ObjectField.Name, name);
			this.AddField (e, ObjectField.Description,                    desc);
			this.AddField (e, ObjectField.Number,                         number);
			this.AddField (e, ObjectField.MethodGuid,                     method.Guid);
			this.AddField (e, ObjectField.Periodicity,                    (int) periodicity);
			this.AddField (e, ObjectField.AccountPurchaseDebit,           accountPurchaseDebit);
			this.AddField (e, ObjectField.AccountPurchaseCredit,	      accountPurchaseCredit);
			this.AddField (e, ObjectField.AccountSaleDebit,	              accountSaleDebit);
			this.AddField (e, ObjectField.AccountSaleCredit,	          accountSaleCredit);
			this.AddField (e, ObjectField.AccountAmortizationAutoDebit,	  accountAmortizationAutoDebit);
			this.AddField (e, ObjectField.AccountAmortizationAutoCredit,  accountAmortizationAutoCredit);
			this.AddField (e, ObjectField.AccountAmortizationExtraDebit,  accountAmortizationExtraDebit);
			this.AddField (e, ObjectField.AccountAmortizationExtraCredit, accountAmortizationExtraCredit);
			this.AddField (e, ObjectField.AccountIncreaseDebit,	          accountIncreaseDebit);
			this.AddField (e, ObjectField.AccountIncreaseCredit,	      accountIncreaseCredit);
			this.AddField (e, ObjectField.AccountDecreaseDebit,	          accountDecreaseDebit);
			this.AddField (e, ObjectField.AccountDecreaseCredit,          accountDecreaseCredit);
			this.AddField (e, ObjectField.AccountAdjustDebit,             accountAdjustDebit);
			this.AddField (e, ObjectField.AccountAdjustCredit,            accountAdjustCredit);

			var field = ObjectField.ArgumentFirst;
			foreach (var argument in arguments)
			{
				if (argument.HasValue)
				{
					this.AddField (e, field, argument.Value);
				}

				field++;
			}
		}


		protected virtual void CreateMethodsSamples()
		{
			this.AddMethod ("Aucun",            AmortizationExpressionCollection.GetExpression (AmortizationExpressionType.None           ));
			this.AddMethod ("Taux linéaire",    AmortizationExpressionCollection.GetExpression (AmortizationExpressionType.RateLinear     ), "Rate",      "RoundAmount", "ResidualAmount", "Prorata");
			this.AddMethod ("Taux dégressif",   AmortizationExpressionCollection.GetExpression (AmortizationExpressionType.RateDegressive ), "Rate",      "RoundAmount", "ResidualAmount", "Prorata");
			this.AddMethod ("Durée linéaire",   AmortizationExpressionCollection.GetExpression (AmortizationExpressionType.YearsLinear    ), "YearCount", "RoundAmount", "ResidualAmount", "Prorata");
			this.AddMethod ("Durée dégressive", AmortizationExpressionCollection.GetExpression (AmortizationExpressionType.YearsDegressive), "YearCount", "RoundAmount", "ResidualAmount", "Prorata");
		}

		protected void AddMethod(string name, string expression, params string[] arguments)
		{
			var cats = this.accessor.Mandat.GetData (BaseType.Methods);
			var start  = new Timestamp (this.accessor.Mandat.StartDate, 0);

			var o = new DataObject (this.accessor.UndoManager);
			cats.Add (o);

			var e = new DataEvent (this.accessor.UndoManager, start, EventType.Input);
			o.AddEvent (e);

			this.AddField (e, ObjectField.Name, name);
			this.AddField (e, ObjectField.Expression, expression);

			foreach (var argument in arguments)
			{
				var a = this.GetArgument (argument);
				var field = (ObjectField) ObjectProperties.GetObjectPropertyInt (a, null, ObjectField.ArgumentField);

				this.AddField (e, field, a.Guid);
			}
		}


		public virtual void CreateArgumentsSamples()
		{
			this.AddArgument (ObjectField.ArgumentFirst+0, "Taux",              "Taux d'amortissement",               ArgumentType.Rate,   false, "Rate",           "0.1");
			this.AddArgument (ObjectField.ArgumentFirst+1, "Durée",             "Nombre d'années de l'amortissement", ArgumentType.Years,  false, "YearCount",      "10");
			this.AddArgument (ObjectField.ArgumentFirst+2, "Arrondi",           "Valeur de l'arrondi",                ArgumentType.Amount, false, "RoundAmount",    "1");
			this.AddArgument (ObjectField.ArgumentFirst+3, "Valeur résiduelle", "Valeur résiduelle",                  ArgumentType.Amount, false, "ResidualAmount", "1");
			this.AddArgument (ObjectField.ArgumentFirst+4, "Prorata",           "Amortissement au prorata",           ArgumentType.Bool,   false, "Prorata",        "false");
		}

		protected void AddArgument(ObjectField field, string name, string description, ArgumentType type, bool nullable, string variable, string def)
		{
			var args = this.accessor.Mandat.GetData (BaseType.Arguments);
			var start  = new Timestamp (this.accessor.Mandat.StartDate, 0);

			var o = new DataObject (this.accessor.UndoManager);
			args.Add (o);

			var e = new DataEvent (this.accessor.UndoManager, start, EventType.Input);
			o.AddEvent (e);

			this.AddField (e, ObjectField.ArgumentField,    (int) field);
			this.AddField (e, ObjectField.Name,             name);
			this.AddField (e, ObjectField.Description,      description);
			this.AddField (e, ObjectField.ArgumentType,     (int) type);
			this.AddField (e, ObjectField.ArgumentNullable, nullable ? 1:0);
			this.AddField (e, ObjectField.ArgumentVariable, variable);
			this.AddField (e, ObjectField.ArgumentDefault,  def);
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

		protected void AddField(DataEvent e, ObjectField field, bool value)
		{
			e.AddProperty (new DataIntProperty (field, value ? 1 : 0));
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

			System.Diagnostics.Debug.Fail (string.Format ("La catégorie {0} n'existe pas !", text));
			return null;
		}

		protected DataObject GetArgument(string variable)
		{
			var list = this.accessor.Mandat.GetData (BaseType.Arguments);

			foreach (var exp in list)
			{
				var v = ObjectProperties.GetObjectPropertyString (exp, null, ObjectField.ArgumentVariable);
				if (v == variable)
				{
					return exp;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("L'argument {0} n'existe pas !", variable));
			return null;
		}

		protected DataObject GetMethod(string text)
		{
			var list = this.accessor.Mandat.GetData (BaseType.Methods);

			foreach (var exp in list)
			{
				var nom = ObjectProperties.GetObjectPropertyString (exp, null, ObjectField.Name);
				if (nom == text)
				{
					return exp;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La méthode d'amortissement {0} n'existe pas !", text));
			return null;
		}


		protected virtual void AddReports()
		{
			var dateRange = new DateRange (this.accessor.Mandat.StartDate, this.accessor.Mandat.StartDate.AddYears (1));
			var timestamp = new Timestamp (this.accessor.Mandat.StartDate, 0);

			this.accessor.Mandat.Reports.Add (new MCH2SummaryParams (null, dateRange, Guid.Empty, 1, Guid.Empty));
			this.accessor.Mandat.Reports.Add (new AssetsParams (null, timestamp, Guid.Empty, null));
			this.accessor.Mandat.Reports.Add (new PersonsParams ());
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
