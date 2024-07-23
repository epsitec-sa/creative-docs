/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Common.Support
{
    public sealed partial class ResourceManager
    {
        private sealed class BundleRelatedCache : IResourceBoundSource
        {
            public BundleRelatedCache(ResourceBundle bundle)
            {
                this.bundle = bundle;
            }

            public ResourceBundle Bundle
            {
                get
                {
                    lock (this.exclusion)
                    {
                        return this.bundle;
                    }
                }
            }

            public void SwitchToBundle(ResourceBundle bundle, ResourceManager manager)
            {
                if (this.bundle != bundle)
                {
                    lock (this.exclusion)
                    {
                        this.bundle = bundle;

                        this.SyncBindings(manager);
                        this.SyncCaptions(manager);
                    }
                }
            }

            public void AddBinding(Binding binding)
            {
                lock (this.exclusion)
                {
                    this.bindings.Add(new Weak<Binding>(binding));
                }
            }

            public void AddCaption(Caption caption)
            {
                lock (this.exclusion)
                {
                    this.captions.Add(new Weak<Caption>(caption));
                }
            }

            public void TrimCache()
            {
                lock (this.exclusion)
                {
                    this.TrimBindingCache();
                    this.TrimCaptionCache();
                }
            }

            internal int CountLiveBindings()
            {
                lock (this.exclusion)
                {
                    return this.TrimCaptionCache();
                }
            }

            internal int CountLiveCaptions()
            {
                lock (this.exclusion)
                {
                    return this.TrimCaptionCache();
                }
            }

            private void SyncBindings(ResourceManager manager)
            {
                this.bindings.RemoveAll(item =>
                {
                    Binding binding = item.Target;

                    if (binding == null)
                    {
                        return true;
                    }
                    else
                    {
                        binding.UpdateTargets(BindingUpdateMode.Reset);
                        return false;
                    }
                });
            }

            private void SyncCaptions(ResourceManager manager)
            {
                var captions = this.captions.ToArray();

                ResourceLevel level = this.bundle.ResourceLevel;
                CultureInfo culture = this.bundle.Culture;

                for (int i = 0; i < captions.Length; i++)
                {
                    Caption caption = captions[i].Target;

                    if (caption == null)
                    {
                        this.captions.Remove(captions[i]);
                    }
                    else
                    {
                        Caption update = manager.GetCaption(
                            caption.Id,
                            level,
                            culture,
                            cache: false
                        );

                        if (update != null)
                        {
                            DependencyObject.CopyDefinedProperties(update, caption);
                        }
                    }
                }
            }

            private int TrimBindingCache()
            {
                this.bindings = this.bindings.Where(x => x.IsAlive).ToList();

                return this.bindings.Count;
            }

            private int TrimCaptionCache()
            {
                this.captions = this.captions.Where(x => x.IsAlive).ToList();

                return this.captions.Count;
            }

            #region IResourceBoundSource Members

            object IResourceBoundSource.GetValue(string id)
            {
                lock (this.exclusion)
                {
                    System.Diagnostics.Debug.Assert(this.bundle != null);
                    System.Diagnostics.Debug.Assert(this.bundle.Contains(id));

                    return this.bundle[id].Data;
                }
            }

            #endregion


            private readonly object exclusion = new object();
            private ResourceBundle bundle;

            private List<Weak<Binding>> bindings = new List<Weak<Binding>>();
            private List<Weak<Caption>> captions = new List<Weak<Caption>>();
        }
    }
}
