Ext.require([
  'Epsitec.cresus.webcore.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.BooleanNullableColumn', {
    extend: 'Ext.grid.column.Boolean',
    alias: ['widget.booleannullablecolumn'],
    alternateClassName: ['Epsitec.BooleanNullableColumn'],

    /* Config */

    nullText: Epsitec.Texts.getNullItemText(),

    /* Additional methods */

    defaultRenderer: function(value) {
      if (value === null) {
        return this.nullText;
      }
      return this.callParent(arguments);
    }
  });
});
