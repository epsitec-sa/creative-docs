// This class represent a complex edition field that is used to edit collections
// of entities. Basicall it is composed by an editable list of targets that the
// user can use to add and remove entities from the list. It used a hidden field
// to submit the value of the list to the server when the form is submitted.

Ext.require([
  'Epsitec.cresus.webcore.field.CallbackHiddenField',
  'Epsitec.cresus.webcore.field.EntityFieldList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.field.EntityCollectionField', {
    extend: 'Ext.form.FieldContainer',
    alternateClassName: ['Epsitec.EntityCollectionField'],
    alias: 'widget.epsitec.entitycollectionfield',

    /* Configuration */

    layout: 'vbox',

    /* Properties */

    entityFieldList: null,
    hiddenField: null,
    originalValues: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.entityFieldList = this.createEntityFieldList(options);
      this.hiddenField = this.createHiddenField(options);

      newOptions = {
        fieldLabel: options.fieldLabel,
        items: [this.entityFieldList, this.hiddenField],
        originalValues: options.value
      };

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

    createEntityFieldList: function(options) {
      var value = options.value,
          readOnly = options.readOnly;

      return Ext.create('Epsitec.EntityFieldList', value, readOnly, {
        width: '100%',
        minHeight: '50',
        maxHeight: '150',
        databaseName: options.databaseName
      });
    },

    createHiddenField: function(options) {
      var me = this;
      return Ext.create('Epsitec.CallbackHiddenField', {
        name: options.name,
        submitValueGetter: function() { return me.getSubmitValue(); },
        onReset: function() { me.resetEntityFieldList(); }
      });
    },

    getSubmitValue: function() {
      return this.entityFieldList
          .getItems()
          .map(function(item) { return item.id; })
          .join(';');
    },

    resetEntityFieldList: function() {
      this.entityFieldList.resetContent();
    }
  });
});
