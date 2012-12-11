Ext.require([
  'Epsitec.cresus.webcore.Callback',
  'Epsitec.cresus.webcore.EntityPicker'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EntityReferenceField', {
    extend: 'Ext.form.TriggerField',
    alternateClassName: ['Epsitec.EntityReferenceField'],
    alias: 'widget.epsitec.entityreferencefield',

    /* Config */

    trigger1Cls: 'x-form-clear-trigger',
    trigger2Cls: 'x-form-arrow-trigger',
    editable: false,

    /* Properties */

    databaseName: null,

    // This property is supposed to be an object with the following properties:
    // {
    //   displayed: ..., <- the value displayed to the user
    //   submitted: ..., <- the value that is submitted to the server
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

    onClearClick: function() {
      this.setValue(null);
    },

    onPickClick: function() {
      var callback = Epsitec.Callback.create(this.entityPickerCallback, this);
      Epsitec.EntityPicker.show(this.databaseName, false, callback);
    },

    entityPickerCallback: function(selectedItems) {
      var entityItem, value;

      if (selectedItems.length === 1) {
        entityItem = selectedItems[0];
        value = {
          displayed: entityItem.summary,
          submitted: entityItem.id
        };
      }
      else {
        value = null;
      }

      this.setValue(value);
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
      if (value === null || value.displayed === null) {
        return '';
      }
      return value.displayed;
    },

    rawToValue: function(object) {
      var value = this.currentValue;
      if (value === null || value.displayed !== object) {
        return null;
      }
      return value;
    },

    getSubmitValue: function() {
      // We need to override the getSubmitValue function as we don't want to
      // send the whole value to the server, but only part of it.
      var value = this.getValue();
      if (value === null || value.submitted === null) {
        return '';
      }
      return value.submitted;
    }
  });
});
