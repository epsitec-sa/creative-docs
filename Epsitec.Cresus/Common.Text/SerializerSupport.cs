//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe SerializerSupport impl�mente une s�rie de m�thodes utiles
	/// pour la s�rialisation/d�s�rialisation.
	/// </summary>
	public sealed class SerializerSupport
	{
		private SerializerSupport()
		{
		}
		
		
		public static string SerializeString(string value)
		{
			Debug.Assert.IsTrue (value != "<null>");
			
			if (value == null)
			{
				return "<null>";
			}
			else
			{
				return value;
			}
		}
		
		public static string DeserializeString(string value)
		{
			if (value == "<null>")
			{
				return null;
			}
			else
			{
				return value;
			}
		}
	}
}
