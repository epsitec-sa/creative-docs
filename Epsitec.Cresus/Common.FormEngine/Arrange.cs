using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.ResourceAccessors;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Proc�dures de manipulation et d'arrangement de listes et d'arbres.
	/// </summary>
	public class Arrange
	{
		public Arrange(ResourceManager resourceManager, FormDescriptionFinder finder)
		{
			//	Constructeur.
			this.resourceManager = resourceManager;
			this.finder = finder;
		}


		public List<FieldDescription> Merge(List<FieldDescription> reference, List<FieldDescription> patch)
		{
			//	Retourne la liste fusionn�e.
			List<FieldDescription> merged = new List<FieldDescription>();

			//	G�n�re la liste fusionn�e de tous les champs. Les champs cach�s sont quand m�me dans la liste,
			//	mais avec la propri�t� Hidden = true.
			foreach (FieldDescription field in reference)
			{
				FieldDescription copy = new FieldDescription(field);

				int index = Arrange.IndexOfGuid(patch, FieldDescription.FieldType.PatchHide, field.Guid);
				if (index != -1)  // champ patch� ?
				{
					copy.PatchHidden = true;  // champ � cacher
				}

				merged.Add(copy);
			}

			//	Ins�re les champs dans la liste fusionn�e.
			foreach (FieldDescription field in patch)
			{
				if (field.Type != FieldDescription.FieldType.PatchHide &&
					field.Type != FieldDescription.FieldType.PatchAttach)  // champ � ins�rer ?
				{
					//	field.AttachGuid vaut System.Guid.Empty lorsqu'il faut d�placer l'�l�ment en t�te
					//	de liste. Par hazard, IndexOfGuid retourne -1 dans dst, ce qui correspond exactement
					//	� la valeur requise !
					int dst = Arrange.IndexOfGuid(merged, FieldDescription.FieldType.None, field.PatchAttachGuid);  // cherche o� l'ins�rer

					FieldDescription copy = new FieldDescription(field);
					copy.PatchInserted = true;
					merged.Insert(dst+1, copy);  // ins�re l'�l�ment apr�s dst
				}
			}

			//	D�place les champs dans la liste fusionn�e. Il faut d�placer les champs apr�s avoir ins�r�
			//	les champs PatchInsert, car un champ peut �tre positionn� par-rapport � un champ PatchInsert !
			foreach (FieldDescription field in patch)
			{
				if (field.Type == FieldDescription.FieldType.PatchAttach)  // champ � d�placer ?
				{
					int src = Arrange.IndexOfGuid(merged, FieldDescription.FieldType.None, field.Guid);  // cherche le champ � d�placer
					if (src != -1)
					{
						//	field.AttachGuid vaut System.Guid.Empty lorsqu'il faut d�placer l'�l�ment en t�te
						//	de liste. Par hazard, IndexOfGuid retourne -1 dans dst, ce qui correspond exactement
						//	� la valeur requise !
						int dst = Arrange.IndexOfGuid(merged, FieldDescription.FieldType.None, field.PatchAttachGuid);  // cherche o� le d�placer

						FieldDescription temp = merged[src];
						merged.RemoveAt(src);

						dst = Arrange.IndexOfGuid(merged, FieldDescription.FieldType.None, field.PatchAttachGuid);  // recalcule le "o�" apr�s suppression
						merged.Insert(dst+1, temp);  // remet l'�l�ment apr�s dst

						temp.PatchMoved = true;
					}
				}
			}

			return merged;
		}


		public string Check(List<FieldDescription> list)
		{
			//	V�rifie une liste. Retourne null si tout est ok, ou un message d'erreur.
			//	Les sous-masques (SubForm) n'ont pas encore �t� d�velopp�s.
			int level = 0;

			foreach (FieldDescription field in list)
			{
				if (field.Type == FieldDescription.FieldType.PatchHide)
				{
					return "La liste ne doit pas contenir de Hide";
				}

				if (field.Type == FieldDescription.FieldType.PatchAttach)
				{
					return "La liste ne doit pas contenir de Attach";
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
			//	Les sous-masques (SubForm) se comportent comme un d�but de groupe (BoxBegin),
			//	car ils ont d�j� �t� d�velopp�s en SubForm-Field-Field-BoxEnd.
			List<FieldDescription> list = new List<FieldDescription>();

			//	Copie la liste en rempla�ant les Glue successifs par un suel.
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

			//	Si un s�parateur est dans une 'ligne', d�place-le au d�but de la ligne.
			//	Par exemple:
			//	'Field-Glue-Field-Glue-Sep-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			//	'Field-Glue-Field-Sep-Glue-Field' -> 'Sep-Field-Glue-Field-Glue-Field'
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].Type == FieldDescription.FieldType.Line ||
					list[i].Type == FieldDescription.FieldType.Title)  // s�parateur ?
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

						if (j > 0 && j < list.Count-1 && list[j+1].Type == FieldDescription.FieldType.Glue)  // glue apr�s ?
						{
							FieldDescription sep = list[j];
							list.RemoveAt(j);
							list.Insert(j-1, sep);  // remplace 'Xxx-Sep-Glue' par 'Sep-Xxx-Glue'
							j--;
							move = true;
						}
					}
					while (move);  // recommence tant qu'on a pu d�placer
				}
			}

			return list;
		}


		public List<FieldDescription> DevelopSubForm(List<FieldDescription> list)
		{
			//	Retourne une liste d�velopp�e qui ne contient plus de sous-masque.
			//	Un sous-masque (SubForm) se comporte alors comme un d�but de groupe (BoxBegin).
			//	Un BoxEnd correspond � chaque SubForm.
			List<FieldDescription> dst = new List<FieldDescription>();

			this.DevelopSubForm(dst, list, null, null);

			return dst;
		}

		private void DevelopSubForm(List<FieldDescription> dst, List<FieldDescription> fields, FieldDescription source, string prefix)
		{
			foreach (FieldDescription field in fields)
			{
				if (field.Type == FieldDescription.FieldType.SubForm)
				{
					if (field.SubFormId.IsEmpty)  // aucun Form choisi ?
					{
						continue;
					}

					//	Cherche le sous-masque avec (dans Designer) ou sans (application finale) finder.
					FormDescription subForm = null;

					if (this.finder == null)  // pas de finder ?
					{
						string name = field.SubFormId.ToBundleId();
						ResourceBundle bundle = this.resourceManager.GetBundle(name, ResourceLevel.Default, null);
						if (bundle != null)
						{
							string xml = bundle[FormResourceAccessor.Strings.XmlSource].AsString;
							if (!string.IsNullOrEmpty(xml))
							{
								subForm = Serialization.DeserializeForm(xml, this.resourceManager);
							}
						}
					}
					else  // finder existe (on est dans Designer) ?
					{
						subForm = this.finder(field.SubFormId);
					}

					if (subForm != null)
					{
						dst.Add(field);  // met le SubForm, qui se comportera comme un BoxBegin

						string p = string.Concat(prefix, field.GetPath(null), ".");
						this.DevelopSubForm(dst, subForm.Fields, field, p);  // met les champs du sous-masque dans la bo�te

						FieldDescription boxEnd = new FieldDescription(FieldDescription.FieldType.BoxEnd);
						dst.Add(boxEnd);  // met le BoxEnd pour terminer la bo�te SubForm
					}
				}
				else
				{
					if (prefix == null || (field.Type != FieldDescription.FieldType.Field && field.Type != FieldDescription.FieldType.SubForm))
					{
						dst.Add(field);
					}
					else
					{
						FieldDescription copy = new FieldDescription(field);
						copy.SetFields(prefix+field.GetPath(null));
						copy.Source = source;
						dst.Add(copy);
					}
				}
			}
		}


		public List<FieldDescription> Develop(List<FieldDescription> fields)
		{
			//	Retourne une liste d�velopp�e qui ne contient plus de noeuds.
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
				else if (field.Type == FieldDescription.FieldType.PatchHide)
				{
				}
				else if (field.Type == FieldDescription.FieldType.PatchAttach)
				{
				}
				else
				{
					dst.Add(field);
				}
			}
		}


		static public int IndexOfNoPatchGuid(List<FieldDescription> list, System.Guid guid)
		{
			//	Retourne l'index de l'�l�ment utilisant un Guid donn�.
			//	Retourne -1 s'il n'en existe aucun.
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].Type != FieldDescription.FieldType.PatchHide &&
					list[i].Type != FieldDescription.FieldType.PatchAttach)
				{
					if (list[i].Guid == guid)
					{
						return i;
					}
				}
			}

			return -1;
		}

		static public int IndexOfGuid(List<FieldDescription> list, FieldDescription.FieldType type, System.Guid guid)
		{
			//	Retourne l'index de l'�l�ment utilisant un Guid donn�.
			//	Retourne -1 s'il n'en existe aucun.
			for (int i=0; i<list.Count; i++)
			{
				if (type == FieldDescription.FieldType.None)
				{
					if (list[i].Guid == guid)
					{
						return i;
					}
				}
				else
				{
					if (list[i].Type == type && list[i].Guid == guid)
					{
						return i;
					}
				}
			}

			return -1;
		}


		private readonly ResourceManager resourceManager;
		private FormDescriptionFinder finder;
	}
}
