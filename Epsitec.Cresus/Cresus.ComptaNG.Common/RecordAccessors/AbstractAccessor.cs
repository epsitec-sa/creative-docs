using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RequestData;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.RecordAccessor
{
	/// <summary>
	/// Cette classe fait l'interface entre les données brutes ToClientData et ToServerData,
	/// pour y accéder d'une façon agréable.
	/// Elle permet d'accéder aux données à travers les classes AbstractRecord, ce
	/// qui n'est pas forcément utile ni nécessaire (implémenté partiellement, à discuter).
	/// </summary>
	public abstract class AbstractAccessor
	{
		public AbstractAccessor(TravellingRecord travellingRecord)
		{
			this.travellingRecord = travellingRecord;
		}

		// Retourne la liste des champs, pour chaque type de Record.
		// C'est peut-être inutile, à voir...
		public virtual IEnumerable<FieldType> Fields
		{
			get
			{
				return null;
			}
		}


		public FormattedText GetNiceField(FieldType field)
		{
			if (this.travellingRecord.FormattedTexts.ContainsKey (field))
			{
				return this.travellingRecord.FormattedTexts[field];
			}
			else
			{
				return FormattedText.Null;
			}
		}


		public FormattedText GetFormattedTextField(FieldType field)
		{
			return (FormattedText) this.GetField (field);
		}

		public void SetFormattedTextField(FieldType field, FormattedText value)
		{
			this.SetField (field, value);
		}


		public decimal GetDecimalField(FieldType field)
		{
			return (decimal) this.GetField (field);
		}

		public void SetDecimalField(FieldType field, decimal value)
		{
			this.SetField (field, value);
		}


		// TODO: Répéter pour les autres types...


		public object GetField(FieldType field)
		{
			if (this.travellingRecord.Fields.ContainsKey (field))
			{
				return this.travellingRecord.Fields[field];
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public void SetField(FieldType field, object value)
		{
			this.travellingRecord.Fields[field] = value;
		}


		public virtual void TravellingToRecord()
		{
		}

		public virtual void RecordToTravelling()
		{
		}


		private readonly TravellingRecord travellingRecord;
	}
}
