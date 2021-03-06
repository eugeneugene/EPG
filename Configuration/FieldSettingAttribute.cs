// -- FILE ------------------------------------------------------------------
// name       : FieldSettingAttribute.cs
// created    : Jani Giannoudis - 2009.01.14
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FieldSettingAttribute : Attribute
    {
        // ----------------------------------------------------------------------
        public string Name { get; set; }

        // ----------------------------------------------------------------------
        public object DefaultValue { get; set; }
    } // class FieldSettingAttribute
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
