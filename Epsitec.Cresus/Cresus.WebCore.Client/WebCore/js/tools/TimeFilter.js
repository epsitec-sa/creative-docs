// This class is a time filter that is used for the filtersfeature of the grid
// panels. It is largely inspired from the official date filter.

Ext.require([
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.TimeFilter', {
    extend: 'Ext.ux.grid.filter.Filter',
    alias: 'gridfilter.time',
    uses: [
      'Ext.picker.Time',
      'Ext.menu.Menu'
    ],

    /* Properties */

    beforeText: Epsitec.Texts.getTimeFilterBefore(),
    onText: Epsitec.Texts.getTimeFilterOn(),
    afterText: Epsitec.Texts.getTimeFilterAfter(),
    compareMap: {
      before: 'lt',
      after: 'gt',
      on: 'eq'
    },
    timeFormat: 'H:i:s',
    menuItemIds: ['before', 'after', '-', 'on'],

    /* Methods */

    init: function(config) {
      var i, itemId, item;

      this.values = {};
      this.fields = {};

      for (i = 0; i < this.menuItemIds.length; i += 1) {
        itemId = this.menuItemIds[i];
        if (itemId === '-') {
          item = itemId;
        }
        else {
          item = this.buildCheckItem(itemId);
          this.fields[itemId] = item;
        }
        this.menu.add(item);
      }
    },

    buildCheckItem: function(itemId) {
      return Ext.create('Ext.menu.CheckItem', {
        itemId: 'range-' + itemId,
        text: this[itemId + 'Text'],
        menu: Ext.create('Ext.menu.Menu', {
          plain: true,
          items: [{
            xtype: 'timepicker',
            itemId: itemId,
            format: this.timeFormat,
            maxHeight: '250',
            listeners: {
              select: function(selectionModel, value) {
                this.onPickerSelect(itemId, value.data.date);
                this.onMenuSelect(itemId, value.data.date);
              },
              scope: this
            }
          }]
        }),
        listeners: {
          scope: this,
          checkchange: this.onCheckChange
        }
      });
    },

    onCheckChange: function(item, checked) {
      var picker, itemId, value;

      picker = item.menu.items.first();
      itemId = picker.itemId;
      value = this.getPickerValue(itemId);

      if (checked && value) {
        this.values[itemId] = value;
      }
      else {
        delete this.values[itemId];
      }

      this.setActive(this.isActivatable());
      this.fireEvent('update', this);
    },

    onInputKeyUp: function(field, e) {
      if (e.getKey() === e.RETURN && field.isValid()) {
        e.stopEvent();
        this.menu.hide();
      }
    },

    onMenuSelect: function(itemId, value) {
      var fields, field, picker;

      fields = this.fields;
      field = this.fields[itemId];

      field.setChecked(true);

      if (field === fields.on) {
        fields.before.setChecked(false, true);
        fields.after.setChecked(false, true);
      }
      else if (field === fields.after) {
        fields.on.setChecked(false, true);
        if (this.getFieldValue('before') < value) {
          fields.before.setChecked(false, true);
        }
      }
      else if (field === fields.before) {
        fields.on.setChecked(false, true);
        if (this.getFieldValue('after') > value) {
          fields.after.setChecked(false, true);
        }
      }

      this.fireEvent('update', this);

      picker = this.getPicker(itemId);
      picker.up('menu').hide();
    },

    getValue: function() {
      var itemId, result;

      result = {};

      for (itemId in this.fields) {
        if (this.fields[itemId].checked) {
          result[itemId] = this.getFieldValue(itemId);
        }
      }

      return result;
    },

    setValue: function(value, preserve) {
      var itemId;

      for (itemId in this.fields) {
        if (value[itemId]) {
          this.setPickerValue(itemId, value[itemId]);
          this.fields[itemId].setChecked(true);
        }
        else if (!preserve) {
          this.fields[itemId].setChecked(false);
        }
      }

      this.fireEvent('update', this);
    },

    isActivatable: function() {
      var itemId;
      for (itemId in this.fields) {
        if (this.fields[itemId].checked) {
          return true;
        }
      }
      return false;
    },

    getSerialArgs: function() {
      var args, itemId, value;

      args = [];

      for (itemId in this.fields) {
        if (this.fields[itemId].checked) {
          value = this.getFieldValue(itemId);
          if (value) {
            args.push({
              type: 'time',
              comparison: this.compareMap[itemId],
              value: Ext.Date.format(value, this.timeFormat)
            });
          }
        }
      }

      return args;
    },

    getFieldValue: function(itemId) {
      return this.values[itemId];
    },

    getPicker: function(itemId) {
      return this.fields[itemId].menu.items.first();
    },

    // Gets the value that is currently selected in the picker. We need this
    // method because the time picker API sucks and does not provide it
    // natively.
    getPickerValue: function(itemId) {
      var picker, selection;

      picker = this.getPicker(itemId);
      selection = picker.getSelectionModel().getSelection();

      return selection.length ?
          selection[0].get('date') : null;
    },

    // Sets the value that is to be selected in the picker. We need this
    // method because the time picker API sucks and does not provide it
    // natively.
    setPickerValue: function(itemId, value) {
      var picker, store, i, t1, t2, record, selectionModel;

      picker = this.getPicker(itemId);
      store = picker.getStore();
      record = null;

      for (i = 0; i < store.getCount(); i += 1) {
        if (record === null) {
          record = store.getAt(i);
        }
        else {
          t1 = Math.abs(value.getTime() - record.get('date').getTime());
          t2 = Math.abs(value.getTime() - store.getAt(i).get('date').getTime());
          if (t1 > t2) {
            record = store.getAt(i);
          }
        }
      }

      if (record !== null) {
        selectionModel = picker.getSelectionModel();
        // This is a hack aroung the picker that does not set up properly its
        // selection model before it has been displayed.
        if (!selectionModel.store) {
          selectionModel.bindComponent(picker);
        }
        selectionModel.select([record]);
      }
    },

    validateRecord: function(record) {
      var itemId, pickerValue, val;

      val = record.get(this.dataIndex);
      if (!Ext.isDate(val)) {
        return false;
      }
      val = val.getTime();

      for (itemId in this.fields) {
        if (this.fields[itemId].checked) {
          pickerValue = this.getFieldValue(itemId).getTime();
          if (itemId === 'before' && pickerValue <= val) {
            return false;
          }
          if (itemId === 'after' && pickerValue >= val) {
            return false;
          }
          if (itemId === 'on' && pickerValue !== val) {
            return false;
          }
        }
      }

      return true;
    },

    onPickerSelect: function(itemId, value) {
      // Keep track of the picker value separately because the menu gets
      // destroyed when columns order changes. We return this value from
      // getValue() instead of fetching the value directly from the picker.
      this.values[itemId] = value;
      this.fireEvent('update', this);
    }
  });
});
