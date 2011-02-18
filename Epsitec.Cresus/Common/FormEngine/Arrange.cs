using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.ResourceAccessors;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Procédures de manipulation et d'arrangement de listes et d'arbres.
	/// </summary>
	public class Arrange
	{
		public Arrange(IFormResourceProvider resourceProvider)
		{
			//	Constructeur.
			this.resourceProvider = resourceProvider;
		}


		public void Build(FormDescription baseForm, FormDescription deltaForm, out List<FieldDescription> baseFields, out List<FieldDescription> finalFields, out Druid entityId)
		{
			//	Construit la liste des FieldDescription finale.
			//	S'il s'agit d'un Form delta, cherche tous les Forms qui servent à le définir, jusqu'au Form de base initial:
			//	 - baseFields contient la liste de base (la génération précédente n-1)
			//   - finalFields contient la liste finale (la dernière génération n)
			//	S'il s'agit d'un Form de base:
			//	 - baseFields est nul
			//   - finalFields contient la liste finale
			List<List<FieldDescription>> list = new List<List<FieldDescription>>();
			Druid parentId;

			if (deltaForm != null)
			{
				list.Add(deltaForm.Fields);
			}

			list.Add(baseForm.Fields);
			parentId = baseForm.DeltaBaseFormId;
			entityId = baseForm.EntityId;

			while (parentId.IsValid)
			{
				string xml = this.resourceProvider.GetFormXmlSource(parentId);
				if (string.IsNullOrEmpty(xml))
				{
					break;
				}

				FormDescription form = Serialization.DeserializeForm(xml);
				list.Add(form.Fields);
				parentId = form.DeltaBaseFormId;
				entityId = form.EntityId;
			}

			//	A partir du Form de base initial, fusionne avec tous les Forms delta.
			if (list.Count == 1)
			{
				baseFields = null;
				finalFields = list[0];
			}
			else
			{
				finalFields = list[list.Count-1];
				baseFields = null;
				for (int i=list.Count-2; i>=0; i--)
				{
					baseFields = finalFields;
					finalFields = this.Merge(baseFields, list[i]);
				}
			}

			if (baseFields == null)
			{
				baseFields = new List<FieldDescription>();
			}
		}

#if false
		public void Build(FormDescription form, out List<FieldDescription> baseFields, out List<FieldDescription> finalFields)
		{
			//	Construit la liste des FieldDescription finale.
			//	S'il s'agit d'un Form delta, cherche tous les Forms qui servent à le définir, jusqu'au Form de base initial:
			//	 - baseFields contient la liste de base (la génération précédente n-1)
			//   - finalFields contient la liste finale (la dernière génération n)
			//	S'il s'agit d'un Form de base:
			//	 - baseFields est nul
			//   - finalFields contient la liste finale
			if (form.IsDelta)
			{
				List<FormDescription> baseForms = new List<FormDescription>();
				FormDescription baseForm = form;
				baseForms.Add(baseForm);

				//	Cherche tous les Forms de base, jusqu'à trouver le Form initial qui n'est pas un Form delta.
				//	Par exemple:
				//	- Form1 est un masque de base
				//	- Form2 est un masque delta basé sur Form1
				//	- Form3 est un masque delta basé sur Form2
				//	Si on cherche à construire Form3, la liste baseForms contiendra Form3, Form2 et Form1.
				while (baseForm != null && baseForm.IsDelta)
				{
					string xml = this.resourceProvider.GetFormXmlSource(baseForm.DeltaBaseFormId);
					if (string.IsNullOrEmpty(xml))
					{
						baseForm = null;
					}
					else
					{
						baseForm = Serialization.DeserializeForm(xml);
						baseForms.Add(baseForm);
					}
				}

				//	A partir du Form de base initial, fusionne avec tous les Forms delta.
				finalFields = baseForms[baseForms.Count-1].Fields;
				baseFields = null;
				for (int i=baseForms.Count-2; i>=0; i--)
				{
					baseFields = finalFields;
					finalFields = this.Merge(baseFields, baseForms[i].Fields);
				}
			}
			else
			{
				baseFields = null;
				finalFields = form.Fields;
			}
		}
#endif

		public List<FieldDescription> Merge(List<FieldDescription> baseList, List<FieldDescription> deltaList)
		{
			//	Retourne la liste finale fusionnée.
			List<FieldDescription> finalList = new List<FieldDescription>();

			//	Génère la liste fusionnée de tous les champs. Les champs cachés sont quand même dans la liste,
			//	mais avec la propriété DeltaHidden = true.
			if (baseList != null)
			{
				foreach (FieldDescription field in baseList)
				{
					FieldDescription copy = new FieldDescription(field);

					//	Un élément inséré dans un masque delta 'A' ne doit pas être vu comme DeltaInserted
					//	dans les masques suivants qui héritent de 'A'. En effet, il doit être alors possible
					//	de le montrer/cacher, mais pas de le supprimer réellement (par exemple) !
					copy.DeltaInserted = false;

					if (deltaList != null)
					{
						int index = Arrange.IndexOfGuid(deltaList, field.Guid);
						if (index != -1 && deltaList[index].DeltaHidden)
						{
							copy.DeltaHidden = true;  // champ à cacher
						}
					}
					finalList.Add(copy);
				}
			}

			if (deltaList != null)
			{
				foreach (FieldDescription field in deltaList)
				{
					//	On considère qu'un champ provenant de la liste delta a des liens corrects, tant que
					//	l'on n'a pas effectivement échoué de le lier aux champs de la liste finale.
					field.DeltaBrokenAttach = false;

					if (field.DeltaShowed)  // champ à montrer (pour inverser un DeltaHidden) ?
					{
						int src = Arrange.IndexOfGuid(finalList, field.Guid);  // cherche le champ à déplacer
						if (src != -1)
						{
							finalList[src].DeltaHidden = false;
							finalList[src].DeltaShowed = true;
						}
					}

					if (field.DeltaMoved)  // champ à déplacer ?
					{
						int src = Arrange.IndexOfGuid(finalList, field.Guid);  // cherche le champ à déplacer
						if (src != -1)
						{
							//	field.DeltaAttachGuid vaut System.Guid.Empty lorsqu'il faut déplacer l'élément en tête
							//	de liste.
							int dst = -1;  // position pour mettre en-tête de liste
							if (field.DeltaAttachGuid != System.Guid.Empty)
							{
								dst = Arrange.IndexOfGuid(finalList, field.DeltaAttachGuid);  // cherche où le déplacer
								if (dst == -1)  // l'élément d'attache n'existe plus ?
								{
									field.DeltaBrokenAttach = true;
									continue;  // on laisse le champ ici
								}
							}

							FieldDescription temp = finalList[src];
							finalList.RemoveAt(src);

							dst = Arrange.IndexOfGuid(finalList, field.DeltaAttachGuid);  // recalcule le "où" après suppression
							finalList.Insert(dst+1, temp);  // remet l'élément après dst

							temp.DeltaMoved = true;
						}
					}

					if (field.DeltaInserted)  // champ à insérer ?
					{
						//	field.DeltaAttachGuid vaut System.Guid.Empty lorsqu'il faut déplacer l'élément en tête
						//	de liste.
						int dst = -1;  // position pour mettre en-tête de liste
						if (field.DeltaAttachGuid != System.Guid.Empty)
						{
							dst = Arrange.IndexOfGuid(finalList, field.DeltaAttachGuid);  // cherche où le déplacer
							if (dst == -1)  // l'élément d'attache n'existe plus ?
							{
								dst = finalList.Count-1;  // on insère le champ à la fin
								field.DeltaBrokenAttach = true;
							}
						}

						FieldDescription copy = new FieldDescription(field);
						copy.DeltaInserted = true;
						finalList.Insert(dst+1, copy);  // insère l'élément après dst
					}

					if (field.DeltaModified || field.DeltaForwardTab)  // champ à modifier ?
					{
						int index = Arrange.IndexOfGuid(finalList, field.Guid);
						if (index != -1)
						{
							finalList.RemoveAt(index);  // supprime le champ original

							FieldDescription copy = new FieldDescription(field);
							finalList.Insert(index, copy);  // et remplace-le par le champ modifié
						}
					}
				}
			}
			
			return finalList;
		}


		public string Check(List<FieldDescription> list)
		{
			//	Vérifie une liste. Retourne null si tout est ok, ou un message d'erreur.
			//	Les sous-masques (SubForm) n'ont pas encore été développés.
			int level = 0;

			foreach (FieldDescription field in list)
			{
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
						return "Il manque un début de boîte";
					}
				}
			}

			if (level > 0)
			{
				return "Il manque une fin de boîte";
			}

			return null;
		}


		public List<FieldDescription> Organize(List<FieldDescription> fields)
		{
			//	Arrange une liste.
			//	Les sous-masques (SubForm) se comportent comme un début de groupe (BoxBegin),
			//	car ils ont déjà été développés en SubForm-Field-Field-BoxEnd.
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
			//	Un sous-masque (SubForm) se comporte alors comme un début de groupe (BoxBegin).
			//	Un BoxEnd correspond à chaque SubForm.
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

					//	Cherche le sous-masque
					FormDescription subForm = null;
					string xml = this.resourceProvider.GetFormXmlSource(field.SubFormId);
					if (!string.IsNullOrEmpty(xml))
					{
						subForm = Serialization.DeserializeForm(xml);
					}

					if (subForm != null)
					{
						dst.Add(field);  // met le SubForm, qui se comportera comme un BoxBegin

						string p = string.Concat(prefix, field.GetPath(null), ".");
						this.DevelopSubForm(dst, subForm.Fields, field, p);  // met les champs du sous-masque dans la boîte

						FieldDescription boxEnd = new FieldDescription(FieldDescription.FieldType.BoxEnd);
						dst.Add(boxEnd);  // met le BoxEnd pour terminer la boîte SubForm
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
				else
				{
					dst.Add(field);
				}
			}
		}


		static public int IndexOfGuid(List<FieldDescription> list, System.Guid guid)
		{
			//	Retourne l'index de l'élément utilisant un Guid donné.
			//	Retourne -1 s'il n'en existe aucun.
			for (int i=0; i<list.Count; i++)
			{
				if (list[i].Guid == guid)
				{
					return i;
				}
			}

			return -1;
		}


		private readonly IFormResourceProvider resourceProvider;
	}
}
