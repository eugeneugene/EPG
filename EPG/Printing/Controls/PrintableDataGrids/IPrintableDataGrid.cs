﻿namespace EPG.Printing.Controls
{
    public interface IPrintableDataGrid
    {
        /// <summary>
        /// Gets the measure (width or height) of the item at the specified index.
        /// </summary>
        double ItemMeasure(int index);

        /// <summary>
        /// Gets the measure (width or height) of the area to display items.
        /// If the value is less than sum of <see cref="ItemMeasure(int)"/>,
        /// some of items are clipped.
        /// </summary>
        double ActualMeasure { get; }
    }
}
