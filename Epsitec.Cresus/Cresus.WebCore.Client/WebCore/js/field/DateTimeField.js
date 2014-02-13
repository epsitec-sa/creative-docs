// This class is an edition field that allows the user to edit a date time, by
// using a date picker and a time picker.

Ext.require([
  'Epsitec.cresus.webcore.field.CallbackHiddenField'
],
function() {
  Ext.define('Epsitec.cresus.webcore.field.DateTimeField', {
    extend: 'Ext.form.FieldContainer',
    alternateClassName: ['Epsitec.DateTimeField'],
    alias: 'widget.epsitec.datetimefield',

    /* Configuration */

    layout: 'hbox',

    /* Properties */

    dateField: null,
    timeField: null,
    hiddenField: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions, value;

      value = options.value;

      if (!Ext.isDefined(value)) {
        value = null;
      }

      if (value !== null && !Ext.isDate(value)) {
        value = Ext.Date.parse(value, 'd.m.Y H:i:s');
      }

      this.dateField = this.createDateField(value, options);
      this.timeField = this.createTimeField(value, options);
      this.hiddenField = this.createHiddenField(options.name);

      newOptions = {
        fieldLabel: options.fieldLabel,
        labelSeparator: options.labelSeparator,
        labelClsExtra: options.labelClsExtra,
        items: [this.dateField, this.timeField, this.hiddenField]
      };

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

    createDateField: function(value, options) {
      var newOptions = {
        value: this.getDatePart(value),
        format: "d.m.Y",
        altFormats: "d.m.Y|j.n.Y|j.m.Y|d.n.Y|d/m/Y|j/n/Y|j/m/Y|d/n/Y",
        submitValue: false,
        flex: 1
      };
      Ext.applyIf(newOptions, options);
      delete newOptions.name;
      delete newOptions.fieldLabel;
      delete newOptions.labelSeparator;
      delete newOptions.labelClsExtra;

      return Ext.create('Ext.form.field.Date', newOptions);
    },

    createTimeField: function(value, options) {
      var newOptions = {
        value: this.getTimePart(value),
        format: 'H:i:s',
        submitValue: false,
        flex: 1
      };
      Ext.applyIf(newOptions, options);
      delete newOptions.name;
      delete newOptions.fieldLabel;
      delete newOptions.labelSeparator;
      delete newOptions.labelClsExtra;

      return Ext.create('Ext.form.field.Time', newOptions);
    },

    createHiddenField: function(name) {
      var me = this;
      return Ext.create('Epsitec.CallbackHiddenField', {
        name: name,
        submitValueGetter: function() { return me.getSubmitValue(); },
        onReset: function() { me.resetField(); }
      });
    },

    getSubmitValue: function() {
      var date, time, result;

      date = this.dateField.getValue();
      time = this.timeField.getValue();

      if (date === null && time === null) {
        return '';
      }

      if (date === null) {
        date = new Date(0);
      }
      if (time === null) {
        time = new Date(0);
      }

      result = new Date(
          date.getFullYear(),
          date.getMonth(),
          date.getDate(),
          time.getHours(),
          time.getMinutes(),
          time.getSeconds(),
          time.getMilliseconds());

      return Ext.Date.format(result, 'd.m.Y H:i:s');
    },

    resetField: function() {
      // The reset is handled by the inner fields.
    },

    getDatePart: function(value) {
      if (value === null) {
        return null;
      }

      var newValue = Ext.Date.clone(value);

      newValue.setHours(0);
      newValue.setMinutes(0);
      newValue.setSeconds(0);
      newValue.setMilliseconds(0);

      return newValue;
    },

    getTimePart: function(value) {
      if (value === null) {
        return null;
      }

      var newValue = Ext.Date.clone(value);

      newValue.setYear(0);
      newValue.setMonth(0);
      newValue.setDate(0);

      return newValue;
    }
  });
});
