// This class is the base class of all edition fields that let the user pick an
// entity, in one way or another. It mainly contains an area where the summary
// of the currently selected entity is dislpayed, a button to clear the current
// selected entity and a button that the derived class must implement in order
// to let the user pick another entity.

Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.field.ReferenceField', {
    extend: 'Ext.form.TriggerField',
    alternateClassName: ['Epsitec.ReferenceField'],

    /* Configuration */

    trigger1Cls: 'x-form-clear-trigger',
    trigger2Cls: 'x-form-arrow-trigger',
    editable: false,

    /* Properties */

    // This property is supposed to be an object with the following properties:
    // {
    //   id: ...,
    //   summary: ...,
    // }
    currentValue: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        onTrigger1Click: this.onClearClick,
        onTrigger2Click: this.onPickClick
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

    onClearClick: function() {
      this.setValue(null);
    },

    onPickClick: function() {
      // This method should be overriden in child classes and the override is
      // supposed to call onPickClickCallback method with the selected items.
    },

    onPickClickCallback: function(selectedItems) {
      this.setValue(selectedItems.length === 1 ? selectedItems[0] : null);
    },

    setValue: function(value) {
      // We need to store the whole current value here as we require it in the
      // rawToValue method because we cannot get the value back from the raw
      // value only. The rawToValue method is called internally by the setValue
      // to actualy set the value and it is its return value that is assigned as
      // the value of the field.
      this.currentValue = value;
      this.callParent(arguments);
    },

    valueToRaw: function(value) {
      if (value === null || value.summary === null) {
        return '';
      }
      return value.summary;
    },

    rawToValue: function(raw) {
      if (this.currentValue === null || this.currentValue.summary !== raw) {
        return null;
      }
      return this.currentValue;
    },

    getSubmitValue: function() {
      // We need to override the getSubmitValue function as we don't want to
      // send the whole value to the server, but only part of it.
      var value = this.getValue();
      if (value === null || value.id === null) {
        return '';
      }
      return value.id;
    }
  });
});
