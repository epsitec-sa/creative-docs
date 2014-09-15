Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryElement', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.QueryElement'],

    /* Properties */

    operatorComboStore: null,
    operatorCombo: null,
    fieldComboStore: null,
    fieldCombo: null,
    comparatorComboStore: null,
    comparatorCombo: null,
    valueField: null,
    components: null,

    /* Constructor */

    constructor: function(options) {
      var first   = options.isFirstElement(options.itemId);
      var builder = options.builder;

      this.components = [];
      var config = {
        itemId: options.itemId,
        minHeight: 50,
        minWidth: 300,
        layout: 'hbox',
        border: false,
        closable: !first,
        parent: builder,
        style: {
          borderRight: '1px solid #99BCE8',
          borderBottom: '1px solid #99BCE8',
          borderLeft: '1px solid #99BCE8'
        },
        bodyCls: 'tile',
        title: 'Condition ' + options.itemId,
        tools: [{
          type: 'plus',
          tooltip: 'Ajouter une condition',
          handler: builder.onAddElement,
          scope: builder
        }],
        items: this.components,
        listeners: {
          close: builder.onRemoveElement,
          scope: builder
        }
      };

      if (!first) {
        this.initOperatorDataStore();
        this.operatorCombo.on('select', function(combo, records, eOpts) {
          Ext.Array.each(records, function(record) {
            this.title = record.get('value');
          });
        });
      }
      this.initQueryFieldDataStore(options.columnDefinitions);
      this.initComparatorDataStore();
      this.initValueField();

      Ext.applyIf(config, options);
      this.callParent([config]);

      if(options.values !== undefined)
      {
        var values = options.values;
        if(values.op!==undefined)
        {
          this.operatorCombo.setValue(values.op);
        }

        this.valueField.setValue (values.condition.value);
        this.comparatorCombo.setValue (values.condition.comparator);
        this.fieldCombo.setValue (values.condition.field);

      }
      return this;
    },

    /* Methods */
    getOperator: function () {
      return this.operatorCombo !== null ? this.operatorCombo.getValue()
                                           : undefined;
    },

    getCondition: function () {
      return {
        id : this.itemId,
        field : this.fieldCombo.getValue(),
        comparator: this.comparatorCombo.getValue(),
        value : this.valueField.getValue()
      };
    },

    initValueField: function() {
      this.valueField = Ext.create('Ext.form.field.Text', {
        emptyText: 'Valeur'
      });
      this.components.push(this.valueField);
    },

    initComparatorDataStore: function() {
      this.comparatorComboStore = Ext.create('Ext.data.Store', {
        fields: ['id', 'symbol'],
        data: [
          { id: 0, symbol: ' = égal' },
          { id: 1, symbol: ' > plus grand que' },
          { id: 2, symbol: ' < plus petit que' },
          { id: 3, symbol: ' != pas égal' },
          { id: 4, symbol: ' % comme' }
        ]
      });
      this.comparatorCombo = Ext.create('Ext.form.ComboBox', {
        store: this.comparatorComboStore,
        queryMode: 'local',
        displayField: 'symbol',
        valueField: 'id'
      });

      this.components.push(this.comparatorCombo);
    },

    initQueryFieldDataStore: function(columnDefinitions) {
      this.fieldComboStore = Ext.create('Ext.data.Store', {
        fields: ['id', 'title'],
        data: this.initQueryFieldFromDef(columnDefinitions)
      });
      this.fieldCombo = Ext.create('Ext.form.ComboBox', {
        store: this.fieldComboStore,
        queryMode: 'local',
        displayField: 'title',
        valueField: 'id'
      });
      this.components.push(this.fieldCombo);
    },

    initQueryFieldFromDef: function(columnDefinitions) {
      return columnDefinitions.map(function(c) {
        var field = {
          id: c.name,
          title: c.title
        };
        return field;
      });
    },

    initOperatorDataStore: function() {
      this.operatorComboStore = Ext.create('Ext.data.Store', {
        fields: ['id', 'symbol'],
        data: [
          { id: 0, symbol: ' ET ' },
          { id: 1, symbol: ' OU ' }
        ]
      });
      this.operatorCombo = Ext.create('Ext.form.ComboBox', {
        store: this.operatorComboStore,
        width: 80,
        queryMode: 'local',
        displayField: 'symbol',
        valueField: 'id'
      });
      this.components.push(this.operatorCombo);
    }
  });
});
