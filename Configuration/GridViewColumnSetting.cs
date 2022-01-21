// -- FILE ------------------------------------------------------------------
// name       : GridViewColumnSetting.cs
// created    : Jani Giannoudis - 2008.05.09
// language   : c#
// environment: .NET 2.0
// --------------------------------------------------------------------------
using System.Windows.Controls;

namespace EPG.Configuration
{
    // ------------------------------------------------------------------------
    [Serializable]
    internal class GridViewColumnSetting
    {
        // ----------------------------------------------------------------------
        public GridViewColumnSetting(GridViewColumn gridViewColumn, int index, int displayIndex)
        {
            if (gridViewColumn == null)
            {
                throw new ArgumentNullException(nameof(gridViewColumn));
            }

            this.index = index;
            this.displayIndex = displayIndex;
            width = gridViewColumn.Width;
        } // GridViewColumnSetting

        // ----------------------------------------------------------------------
        public int Index
        {
            get { return index; }
        } // Index

        // ----------------------------------------------------------------------
        public int DisplayIndex
        {
            get { return displayIndex; }
        } // DisplayIndex

        // ----------------------------------------------------------------------
        public double Width
        {
            get { return width; }
        } // Width

        // ----------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj is not GridViewColumnSetting compare)
            {
                return false;
            }

            return
                int.Equals(index, compare.index) &&
                int.Equals(displayIndex, compare.displayIndex) &&
                double.Equals(width, compare.width);
        } // Equals

        // ----------------------------------------------------------------------
        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = AddHashCode(hash, index);
            hash = AddHashCode(hash, displayIndex);
            hash = AddHashCode(hash, width);
            return hash;
        } // GetHashCode

        // ----------------------------------------------------------------------
        private static int AddHashCode(int hash, object obj)
        {
            int combinedHash = obj != null ? obj.GetHashCode() : 0;
            if (hash != 0)
            {
                combinedHash += hash * 31;
            }
            return combinedHash;
        } // AddHashCode

        // ----------------------------------------------------------------------
        // members
        private readonly int index;
        private readonly int displayIndex;
        private readonly double width;

    } // class GridViewColumnSetting
} // namespace EPG.Configuration
// -- EOF -------------------------------------------------------------------
