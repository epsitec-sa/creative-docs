using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Procédures de manipulation et d'arrangement de listes et d'arbres.
	/// </summary>
	public class Arrange
	{
		static public List<FieldDescription> Merge(List<FieldDescription> reference, List<FieldDescription> patch)
		{
			//	Retourne la liste fusionnée.
			return reference;  // TODO:
		}


		static public string Check(List<FieldDescription> list)
		{
			//	Vérifie une liste. Retourne null si tout est ok, ou un message d'erreur.
			int level = 0;

			foreach (FieldDescription field in list)
			{
				if (field.Type == FieldDescription.FieldType.InsertionPoint)
				{
					return "La liste ne doit pas contenir de InsertionPoint";
				}

				if (field.Type == FieldDescription.FieldType.Hide)
				{
					return "La liste ne doit pas contenir de Hide";
				}

				if (field.Type == FieldDescription.FieldType.Node)
				{
					return "La liste ne doit pas contenir de Node";
				}

				if (field.Type == FieldDescription.FieldType.BoxBegin)
				{
					level++;
				}

				if (field.Type == FieldDescription.FieldType.BoxEnd)
				{
					level--;
					if (level < 0)
					{
						return "Il manque un BoxBegin";
					}
				}
			}

			if (level > 0)
			{
				return "Il manque un BoxEnd";
			}

			return null;
		}


		static public List<FieldDescription> Develop(List<FieldDescription> fields)
		{
			//	Retourne une liste développée qui ne contient plus de noeuds.
			List<FieldDescription> dst = new List<FieldDescription>();

			Arrange.Develop(dst, fields);
			
			return dst;
		}

		static private void Develop(List<FieldDescription> dst, List<FieldDescription> fields)
		{
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.Node)
				{
					Arrange.Develop(dst, field.NodeDescription);
				}
				else if (field.Type == FieldDescription.FieldType.InsertionPoint)
				{
				}
				else if (field.Type == FieldDescription.FieldType.Hide)
				{
				}
				else
				{
					dst.Add(field);
				}
			}
		}
	}
}
