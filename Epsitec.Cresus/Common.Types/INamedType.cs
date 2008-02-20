//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INamedType</c> interface is implemented by all type description
	/// classes. It is also the base for interfaces such as <see cref="T:INumericType" />,
	/// <see cref="T:IEnumType" /> and <see cref="T:IStringType" />.
	/// </summary>
	public interface INamedType : ICaption, IName, ISystemType
	{
		/// <summary>
		/// Gets the default controller used to represent data of this type.
		/// </summary>
		/// <value>The default controller.</value>
		string DefaultController
		{
			get;
		}

		/// <summary>
		/// Gets the parameter used with the default controller.
		/// </summary>
		/// <value>The default controller parameter.</value>
		string DefaultControllerParameters
		{
			get;
		}
	}

	#region NamedTypeSerializationConverter Class

	public class NamedTypeSerializationConverter : ISerializationConverter
	{
		#region ISerializationConverter Members

		public string ConvertToString(object value, IContextResolver context)
		{
			INamedType type = (INamedType) value;
			return type.CaptionId.ToString ();
		}

		public object ConvertFromString(string value, IContextResolver context)
		{
			Support.Druid id = Support.Druid.Parse (value);
			AbstractType type = TypeRosetta.GetTypeObject (id);

			if (type == null)
			{
				Support.ResourceManager manager = context.ExternalMap.GetValue (Serialization.Context.WellKnownTagResourceManager) as Support.ResourceManager;

				if (manager == null)
				{
					type = TypeRosetta.CreateTypeObject (id);
				}
				else
				{
					Caption caption = manager.GetCaption (id);
					type = caption == null ? null : TypeRosetta.CreateTypeObject (caption);
				}
			}

			return type;
		}

		#endregion
	}

	#endregion
}
