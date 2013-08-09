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
        width: 400,
        height: 600,
        header: 'false',
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
      // Show needed column
      Ext.Array.each(this.form.items.items, function(item) {

        if (Ext.isDefined(item.lastValue) && !list.columns[index].isVisible()) {
          list.columns[index].show();
        }
        else {
          if (window.fields[index - 1].isHidden) {
            list.columns[index].hide();
          }
        }
        index += 1;
      });

      //appli filtering
      Ext.Array.each(this.form.items.items, function(item) {
        var filter = list.filters.getFilter(item.name);
        if (!Ext.isDefined(filter)) {
          if (Ext.isDefined(item.lastValue)) {

            list.filters.addFilter({
              type: 'string',
              dataIndex: item.name,
              value: item.lastValue,
              active: true
            });
            list.filters.getFilter(item.name).fireEventArgs(
                'update', list.filters.getFilter(item.name)
            );
          }
        }
        else {
          if (item.lastValue && item.lastValue.length > 0) {
            filter.setValue(item.lastValue);
            filter.setActive(true);
          }
          else {
            filter.setActive(false);
          }
        }
      });

      this.hide();
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
            field.xtype = 'numberfield';
            field.fieldLabel = c.title;
            field.name = c.name;
            break;

          case 'float':
            field.xtype = 'numberfield';
            field.fieldLabel = c.title;
            field.name = c.name;
            break;

          case 'boolean':
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
            field.xtype = 'fieldset';
            field.title = c.title;
            field.name = c.name;
            field.defaultType = 'datefield';
            field.layout = 'anchor';
            field.defaults = {
              anchor: '100%'
            };
            field.items = [{
              fieldLabel: 'Before',
              name: 'before',
              dateFormat: 'd.m.Y'
            }, {
              fieldLabel: 'After',
              name: 'after',
              dateFormat: 'd.m.Y'
            }, {
              fieldLabel: 'At',
              name: 'at',
              dateFormat: 'd.m.Y'
            }];
            break;

          case 'list':
            field.fieldLabel = c.title;
            field.xtype = 'combo';
            field.name = c.name;
            field.store = Epsitec.Enumeration.getStore(c.type.enumerationName);
            break;

          case 'time':
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
