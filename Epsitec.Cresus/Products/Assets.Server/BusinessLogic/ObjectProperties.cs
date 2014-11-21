//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class ObjectProperties
	{
		public static int? GetObjectPropertyInt(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataIntProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static decimal? GetObjectPropertyDecimal(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataDecimalProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static ComputedAmount? GetObjectPropertyComputedAmount(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataComputedAmountProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static AmortizedAmount? GetObjectPropertyAmortizedAmount(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataAmortizedAmountProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static System.DateTime? GetObjectPropertyDate(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataDateProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static Guid GetObjectPropertyGuid(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true, bool inputValue = false)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataGuidProperty;

			if (p == null)
			{
				if (inputValue)
				{
					p = ObjectProperties.GetObjectInputProperty (obj, field) as DataGuidProperty;
					if (p != null)
					{
						return p.Value;
					}
				}

				return Guid.Empty;
			}
			else
			{
				return p.Value;
			}
		}

		public static GuidRatio GetObjectPropertyGuidRatio(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true, bool inputValue = false)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataGuidRatioProperty;

			if (p == null)
			{
				if (inputValue)
				{
					p = ObjectProperties.GetObjectInputProperty (obj, field) as DataGuidRatioProperty;
					if (p != null)
					{
						return p.Value;
					}
				}

				return GuidRatio.Empty;
			}
			else
			{
				return p.Value;
			}
		}

		public static string GetObjectPropertyString(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true, bool inputValue = false)
		{
			var p = ObjectProperties.GetObjectProperty (obj, timestamp, field, synthetic) as DataStringProperty;

			if (p == null)
			{
				if (inputValue)
				{
					p = ObjectProperties.GetObjectInputProperty (obj, field) as DataStringProperty;
					if (p != null && !string.IsNullOrEmpty (p.Value))
					{
						return string.Concat ("<i>", p.Value, "</i>");
					}
				}

				return null;
			}
			else
			{
				return p.Value;
			}
		}


		public static ComparableData GetComparableData(DataAccessor accessor, DataObject obj, Timestamp? timestamp, ObjectField field)
		{
			if (obj != null)
			{
				var p = ObjectProperties.GetObjectSyntheticProperty (obj, timestamp, field);

				if (p == null)
				{
					//	Pour le tri, si on n'a pas trouvé de propriété, on prend
					//	celle définie lors de l'événement d'entrée.
					p = ObjectProperties.GetObjectInputProperty (obj, field);
				}

				return ObjectProperties.GetComparableData (accessor, p);
			}

			return ComparableData.Empty;
		}

		public static ComparableData GetComparableData(DataAccessor accessor, AbstractDataProperty property)
		{
			if (property != null)
			{
				if (property is DataIntProperty)
				{
					return new ComparableData ((property as DataIntProperty).Value);
				}
				else if (property is DataDecimalProperty)
				{
					return new ComparableData ((property as DataDecimalProperty).Value);
				}
				else if (property is DataComputedAmountProperty)
				{
					return new ComparableData ((property as DataComputedAmountProperty).Value.FinalAmount.GetValueOrDefault ());
				}
				else if (property is DataAmortizedAmountProperty)
				{
					var aa = (property as DataAmortizedAmountProperty).Value;
					return new ComparableData (accessor.GetAmortizedAmount (aa).GetValueOrDefault ());
				}
				else if (property is DataDateProperty)
				{
					return new ComparableData ((property as DataDateProperty).Value);
				}
				else if (property is DataStringProperty)
				{
					return new ComparableData ((property as DataStringProperty).Value);
				}
				else if (property is DataGuidProperty)
				{
					var guid = (property as DataGuidProperty).Value;
					var text = PersonsLogic.GetSummary (accessor, guid);
					return new ComparableData (text);
				}
			}

			return ComparableData.Empty;
		}


		public static AbstractDataProperty GetObjectProperty(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic)
		{
			if (synthetic || !timestamp.HasValue)
			{
				return ObjectProperties.GetObjectSyntheticProperty (obj, timestamp, field);
			}
			else
			{
				return ObjectProperties.GetObjectSingleProperty (obj, timestamp.Value, field);
			}
		}

		public static AbstractDataProperty GetObjectSingleProperty(DataObject obj, Timestamp timestamp, ObjectField field)
		{
			//	Retourne l'état d'une propriété d'un objet à la date exacte.
			if (obj == null)
			{
				return null;
			}
			else
			{
				return obj.GetSingleProperty (timestamp, field);
			}
		}

		public static AbstractDataProperty GetObjectInputProperty(DataObject obj, ObjectField field)
		{
			//	Retourne l'état d'une propriété d'un objet lors de l'événement d'entrée.
			if (obj == null)
			{
				return null;
			}
			else
			{
				return obj.GetInputProperty (field);
			}
		}

		public static AbstractDataProperty GetObjectSyntheticProperty(DataObject obj, Timestamp? timestamp, ObjectField field)
		{
			//	Retourne l'état d'une propriété d'un objet à la date exacte ou antérieurement.
			if (obj == null)
			{
				return null;
			}
			else
			{
				if (!timestamp.HasValue)
				{
					timestamp = Timestamp.MaxValue;
				}

				return obj.GetSyntheticProperty (timestamp.Value, field);
			}
		}
	}
}
