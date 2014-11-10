//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public abstract class AbstractDataProperty
	{
		public AbstractDataProperty(ObjectField field)
		{
			this.Field = field;
		}

		public AbstractDataProperty(AbstractDataProperty model)
		{
			this.Field = model.Field;
			this.State = model.State;
		}

		public AbstractDataProperty(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "ObjectField")
					{
						var s = reader.ReadElementContentAsString ();
						this.Field = IOHelpers.ParseObjectField  (s);

						break;  // fin de la lecture de la classe abstraite -> on passe à la classe dérivée
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}


		public static AbstractDataProperty Copy(AbstractDataProperty model)
		{
			if (model is DataComputedAmountProperty)
			{
				return new DataComputedAmountProperty (model as DataComputedAmountProperty);
			}
			else if (model is DataAmortizedAmountProperty)
			{
				return new DataAmortizedAmountProperty (model as DataAmortizedAmountProperty);
			}
			else if (model is DataDateProperty)
			{
				return new DataDateProperty (model as DataDateProperty);
			}
			else if (model is DataDecimalProperty)
			{
				return new DataDecimalProperty (model as DataDecimalProperty);
			}
			else if (model is DataGuidProperty)
			{
				return new DataGuidProperty (model as DataGuidProperty);
			}
			else if (model is DataGuidRatioProperty)
			{
				return new DataGuidRatioProperty (model as DataGuidRatioProperty);
			}
			else if (model is DataIntProperty)
			{
				return new DataIntProperty (model as DataIntProperty);
			}
			else if (model is DataStringProperty)
			{
				return new DataStringProperty (model as DataStringProperty);
			}
			else
			{
				return null;
			}
		}


		public virtual void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString ("ObjectField", this.Field.ToStringIO ());
		}


		public readonly ObjectField Field;
		public PropertyState State;  // TODO: beurk !
	}
}
