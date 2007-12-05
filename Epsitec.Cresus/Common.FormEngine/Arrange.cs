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
		public Arrange(ResourceManager resourceManager, FindFormDescription finder)
		{
			//	Constructeur.
			this.resourceManager = resourceManager;
			this.finder = finder;
		}


		public List<FieldDescription> Merge(List<FieldDescription> reference, List<FieldDescription> patch)
		{
			//	Retourne la liste fusionnée.
			return reference;  // TODO:
		}


		public string Check(List<FieldDescription> list)
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


		public List<FieldDescription> Organize(List<FieldDescription> fields)
		{
			//	Arrange une liste.
			List<FieldDescription> list = new List<FieldDescription>();

			//	Copie la liste en remplaçant les Glue successifs par un suel.
			bool isGlue = false;
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.Glue)
				{
					if (isGlue)
					{
						continue;
					}

					isGlue = true;
				}
				else
				{
					isGlue = false;
				}

				list.Add(field);
			}

			//	Si un séparateur est dans une 'ligne', déplace-le au début de la ligne.
			//	Par exemple:
			//	'Field-Glue-Field-Glue-Sep-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			//	'Field-Glue-Field-Sep-Glue-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].Type == FieldDescription.FieldType.Line ||
					list[i].Type == FieldDescription.FieldType.Title)  // séparateur ?
				{
					int j = i;
					bool move;
					do
					{
						move = false;

						if (j > 0 && list[j-1].Type == FieldDescription.FieldType.Glue)  // glue avant ?
						{
							FieldDescription sep = list[j];
							list.RemoveAt(j);
							list.Insert(j-1, sep);  // remplace 'Glue-Sep' par 'Sep-Glue'
							j--;
							move = true;
						}

						if (j > 0 && j < list.Count-1 && list[j+1].Type == FieldDescription.FieldType.Glue)  // glue après ?
						{
							FieldDescription sep = list[j];
							list.RemoveAt(j);
							list.Insert(j-1, sep);  // remplace 'Xxx-Sep-Glue' par 'Sep-Xxx-Glue'
							j--;
							move = true;
						}
					}
					while (move);  // recommence tant qu'on a pu déplacer
				}
			}

			return list;
		}


		public List<FieldDescription> DevelopSubForm(List<FieldDescription> list)
		{
			//	Retourne une liste développée qui ne contient plus de sous-masque.
			List<FieldDescription> dst = new List<FieldDescription>();

			this.DevelopSubForm(dst, list);

			return dst;
		}

		private void DevelopSubForm(List<FieldDescription> dst, List<FieldDescription> fields)
		{
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.SubForm)
				{
					if (field.SubFormId.IsEmpty)
					{
						continue;
					}

					FormDescription subForm = null;

					if (this.finder == null)
					{
						string name = field.SubFormId.ToBundleId();
						ResourceBundle bundle = this.resourceManager.GetBundle(name, ResourceLevel.Default, null);
						if (bundle != null)
						{
							ResourceBundle.Field bundleField = bundle["Source"];
							if (bundleField.IsValid)
							{
								string xml = bundleField.AsString;
								if (!string.IsNullOrEmpty(xml))
								{
									subForm = Serialization.DeserializeForm(xml, this.resourceManager);
								}
							}
						}
					}
					else
					{
						subForm = this.finder(field.SubFormId);
					}

					if (subForm != null)
					{
						this.DevelopSubForm(dst, subForm.Fields);
					}
				}
				else
				{
					dst.Add(field);
				}
			}
		}


		public List<FieldDescription> Develop(List<FieldDescription> fields)
		{
			//	Retourne une liste développée qui ne contient plus de noeuds.
			List<FieldDescription> dst = new List<FieldDescription>();

			this.Develop(dst, fields);
			
			return dst;
		}

		private void Develop(List<FieldDescription> dst, List<FieldDescription> fields)
		{
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.Node)
				{
					this.Develop(dst, field.NodeDescription);
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


		protected readonly ResourceManager resourceManager;
		protected FindFormDescription finder;
	}
}
