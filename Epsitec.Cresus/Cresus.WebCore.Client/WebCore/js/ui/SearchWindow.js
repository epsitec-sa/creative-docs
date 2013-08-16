Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.SearchWindow', {
    extend: 'Ext.Window',
    alternateClassName: ['Epsitec.SearchWindow'],

    /* Properties */

    parent: null,
    fields: null,
    form: null,
    panel: null,
    caller: null,

    /* Constructor */

    constructor: function(columnDefinitions, caller) {
      var tabManager, config;

      this.caller = caller;
      Ext.QuickTips.init();
      this.fields = this.createSearchFormFields(columnDefinitions);
      this.form = Ext.widget({
        xtype: 'form',
        layout: 'form',
        url: '',
        bodyPadding: '5 5 0',
        width: 350,
        fieldDefaults: {
          msgTarget: 'side',
          labelWidth: 75
        },
        plugins: {
          ptype: 'datatip'
        },
        defaultType: 'textfield',
        items: this.fields,
        buttons: [{
          text: 'Reinitialiser',
          handler: this.resetFullSearch,
          scope: this
        }, {
          text: 'Rechercher',
          handler: this.executeFullSearch,
          scope: this
        }]
      });
      this.panel = Ext.widget({
        xtype: 'panel',
        autoHeight: true,
        items: this.form
      });
      tabManager = Epsitec.Cresus.Core.getApplication().tabManager;
      this.parent = Ext.get(tabManager.getLayout().getActiveItem().el);
      config = {
        title: 'Recherche',
        width: 500,
        height: 600,
        header: 'true',
        autoScroll: true,
        constrain: true,
        renderTo: this.parent,
        closable: true,
        closeAction: 'hide',
        items: this.panel
      };

      this.callParent([config]);
    },

    /* Methods */
    executeFullSearch: function() {

      var list, window, index;

      list = this.caller;
      window = this;

      list.dockedItems.items[2].items.items[0].setValue(
          this.form.items.items[0].lastValue
      );

      // We start at one because we have a first numbered column.
      index = 1;
      list.isSearching = true;
      // Show needed columns and appli filters
      Ext.Array.each(this.form.items.items, function(item) {   
        if (window.valueExist(item) && !list.columns[index].isVisible()) {
          list.columns[index].show();
        }
        else {
          if (window.fields[index - 1].isHidden) {
            list.columns[index].hide();
          }
        }        
        index += 1;
      }); 
      this.appliFilters();
      this.hide();
    },

    appliFilters: function() {
      var list = this.caller;
      var window = this;
      Ext.Array.each(this.form.items.items, function(item) {
        var filter = list.filters.getFilter(item.name);
        
        if (!Ext.isDefined(filter)&&window.valueExist(item)) {
            window.addFilterToList(item,list);     
        }    
        if (Ext.isDefined(filter)&&window.valueExist(item)) {
          var value = window.getFormItemValue(item);
          filter.setValue(value);
          filter.setActive(true);
        }
        if(Ext.isDefined(filter)&&!window.valueExist(item)) {
          filter.setActive(false);
        } 
      });     
    },

    addFilterToList: function(item,list) {
      var value = this.getFormItemValue(item);
      switch(item.filterType)
      {
        case 'list':
            list.filters.addFilter({
              type: 'list',
              dataIndex: item.name,
              value: value,
              active: true
            });
            var filter = list.filters.getMenuFilter(item.name);
            filter.fireEventArgs(
              'update', filter
            );            
        break;
        case 'date':         
            list.filters.addFilter({
              type: 'date',
              dataIndex: item.name,
              value: value,
              active: true
            });
            var filter = list.filters.getFilter(item.name);
            filter.fireEventArgs(
              'update', filter
            );
        break;
        case 'datetime':
        break;
        case 'boolean':
        break;
        case 'string':
            list.filters.addFilter({
              type: 'string',
              dataIndex: item.name,
              value: value,
              active: true
            });
            var filter = list.filters.getFilter(item.name);
            filter.fireEventArgs(
              'update', filter
            );
        break;
      }
    }, 

    valueExist: function(item) {
      if(Ext.isDefined(item.lastValue))
      {
        return true;
      }
      else
      {
        if(item.filterType=='date' || item.filterType=='datetime')
        {
          var exist = false;
          Ext.Array.each(item.items.items, function(subitem) {
            if(Ext.isDefined(subitem.lastValue))
            {
              exist = true;
            }
          });
          if(exist)
          {
            return true;
          } 
          else
          {
            return false;
          }
        } 
        else
        {
          return false;
        }
        
      }
    },

    getFormItemValue: function(item) {
      if(Ext.isDefined(item.lastValue))
      {
        return item.lastValue;
      }
      else
      {
        switch(item.filterType)
        {
          case 'date':
          case 'datetime':
            var values = {}; 
            var valuesExist = false;
            Ext.Array.each(item.items.items, function(subitem) {
              if(Ext.isDefined(subitem.lastValue))
              {
                valuesExist = true;
                switch(subitem.name)
                {
                  case 'after':
                  values.after = subitem.lastValue;
                  break;
                  case 'before':
                  values.before = subitem.lastValue;
                  break;
                  case 'on':
                  values.on = subitem.lastValue;
                  break;
                }      
              }
            });

            if(valuesExist)
            {
              return values; 
            }
          break;
          default:
              return null;
          break;
        }
      }
    },

    resetFullSearch: function() {
      var list = this.caller;

      Ext.Array.each(this.form.items.items, function(item) {
        item.reset();
        if (list.filters.filters.containsKey(item.name)) {
          list.filters.filters.getKey(item.name).setValue(item.lastValue);
          list.filters.filters.getKey(item.name).setActive(false);
        }
      });
      this.parent.dockedItems.items[2].items.items[0].setValue(
          this.form.items.items[0].lastValue
      );
    },

    setQuickSearchValue: function(val) {
      this.form.items.items[0].setValue(val);
    },

    onEnterExecuteFullSearch: function(field, e) {
      if (e.getKey() === e.ENTER) {
        this.executeFullSearch();
      }
    },

    createSearchFormFields: function(columnDefinitions) {
      var list = this;
      return columnDefinitions.map(function(c) {
        var field = {
          name: c.name,
          type: c.type.type,
          isHidden: c.isHidden
        };

        switch (c.type.type) {
          case 'int':
            field.filterType = 'numeric';
            field.xtype = 'numberfield';
            field.fieldLabel = c.title;
            field.name = c.name;
            break;

          case 'float':
            field.filterType = 'numeric';
            field.xtype = 'numberfield';
            field.fieldLabel = c.title;
            field.name = c.name;
            break;

          case 'boolean':
            field.filterType = 'boolean';
            field.xtype = 'fieldset';
            field.useNull = true;
            field.title = c.title;
            field.name = c.name;
            field.defaultType = 'checkbox';
            field.layout = 'anchor';
            field.defaults = {
              anchor: '100%'
            };
            field.items = [{
              boxLabel: 'True',
              name: 'isTrue'
            }, {
              boxLabel: 'False',
              name: 'isFalse'
            }, {
              boxLabel: 'Null',
              name: 'isNull'
            }];
            break;

          case 'date':
            field.filterType = 'date';
            field.xtype = 'fieldset';
            field.title = c.title;
            field.name = c.name;
            field.defaultType = 'datefield';
            field.layout = 'anchor';
            field.defaults = {
              anchor: '100%'
            };
            field.items = [{
              fieldLabel: 'Avant le',
              name: 'before',
            }, {
              fieldLabel: 'Après le',
              name: 'after',
            }, {
              fieldLabel: 'Le',
              name: 'at',
            }];
            break;

          case 'dateTime':
            field.filterType = 'datetime';
            field.xtype = 'fieldset';
            field.title = c.title;
            field.name = c.name;
            field.defaultType = 'epsitec.datetimefield';
            field.layout = 'anchor';
            field.defaults = {
              anchor: '100%'
            };
            field.items = [{
              fieldLabel: 'Avant le',
              format: 'm/d/Y',
              name: 'before'
            }, {
              fieldLabel: 'Après le',
              format: 'm/d/Y',
              name: 'after'
            }, {
              fieldLabel: 'Le',
              format: 'm/d/Y',
              name: 'at'
            }];
            break;

          case 'list':
            field.filterType = 'list';
            field.fieldLabel = c.title;
            field.xtype = 'combobox';
            field.name = c.name;
            field.displayField = 'text';
            field.valueField = 'id';
            field.multiSelect = true;
            field.store = Epsitec.Enumeration.getStore(c.type.enumerationName);
            break;

          case 'time':
            field.filterType = 'datetime';
            field.xtype = 'fieldset';
            field.title = c.title;
            field.name = c.name;
            field.defaultType = 'timefield';
            field.layout = 'anchor';
            field.defaults = {
              anchor: '100%'
            };
            field.items = [{
              fieldLabel: 'Before',
              name: 'before',
              format: 'H:i:s'
            }, {
              fieldLabel: 'After',
              name: 'after',
              format: 'H:i:s'
            }, {
              fieldLabel: 'At',
              name: 'at',
              format: 'H:i:s'
            }];
            break;

          default:
            field.filterType = 'string';
            field.fieldLabel = c.title;
            field.name = c.name;
            field.xtype = 'textfield';
            field.tooltip = 'Touche ENTER pour lancer';
            field.listeners = {
              specialkey: list.onEnterExecuteFullSearch,
              scope: list
            };
            break;
        }

        return field;
      });
    }
  });
});
