using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EPG.Printing.Controls
{
    public partial class PrintableDataGrid : UserControl, IPrintableDataGrid
    {
        readonly Grid grid;
        readonly StackPanel stackPanel;

        /// <summary>
        /// Gets an collection to store columns.
        /// </summary>
        public ObservableCollection<PrintableDataGridColumn> Columns { get; } = new();

        object[]? items;

        /// <summary>
        /// Gets the dependency property of <see cref="ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(PrintableDataGrid),
                new FrameworkPropertyMetadata()
                {
                    PropertyChangedCallback = OnItemsSourceChanged,
                }
            );

        /// <summary>
        /// Gets or sets the sequence of items.
        /// This control don't care <see cref="INotifyCollectionChanged"/>
        /// implemented by the sequence.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var @this = (PrintableDataGrid)obj;
            var itemsSource = @this.ItemsSource;
            if (itemsSource is not null)
            {
                @this.items = itemsSource.Cast<object>().ToArray();
                @this.Reset();
            }
        }

        /// <summary>
        /// Gets the dependency property of <see cref="RowHeight"/>.
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(
                "RowHeight",
                typeof(GridLength),
                typeof(PrintableDataGrid),
                new FrameworkPropertyMetadata()
                {
                    DefaultValue = GridLength.Auto,
                }
            );

        /// <summary>
        /// Gets or sets the height of rows.
        /// </summary>
        public GridLength RowHeight
        {
            get => (GridLength)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        void AddColumnDefinition(PrintableDataGridColumn column)
        {
            var columnDefinition = new ColumnDefinition()
            {
                Width = column.Width,
            };
            grid.ColumnDefinitions.Add(columnDefinition);
        }

        void AddHeaderRowDefinition()
        {
            var rowDefinition = new RowDefinition()
            {
                Height = GridLength.Auto
            };
            grid.RowDefinitions.Add(rowDefinition);
        }

        void AddRowDefinition()
        {
            var rowDefinition = new RowDefinition();
            rowDefinition.SetBinding(RowDefinition.HeightProperty, new Binding(nameof(RowHeight))
            {
                Source = this
            });
            grid.RowDefinitions.Add(rowDefinition);
        }

        void AddHeaderCell(int columnIndex)
        {
            var column = Columns[columnIndex];
            var cell = new PrintableDataGridCell()
            {
                Content = column.Header,
                ContentTemplate = column.HeaderTemplate,
                ContentTemplateSelector = column.HeaderTemplateSelector,
            };
            Grid.SetRow(cell, 0);
            Grid.SetColumn(cell, columnIndex);
            grid.Children.Add(cell);
        }

        void AddRowCell(int rowIndex, int columnIndex, object dataContext)
        {
            var column = Columns[columnIndex];
            var cell = new PrintableDataGridCell()
            {
                ContentTemplate = column.CellTemplate,
                ContentTemplateSelector = column.CellTemplateSelector,
                DataContext = dataContext,
            };

            cell.SetBinding(ContentProperty, column.CellBinding);

            var cellStyle = column.CellStyle;
            if (cellStyle is not null)
                cell.Style = cellStyle;

            Grid.SetRow(cell, rowIndex);
            Grid.SetColumn(cell, columnIndex);
            grid.Children.Add(cell);
        }

        void Reset()
        {
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            AddHeaderRowDefinition();

            foreach (var columnIndex in Enumerable.Range(0, Columns.Count))
            {
                var column = Columns[columnIndex];
                if (column.IsVisible)
                {
                    AddColumnDefinition(column);
                    AddHeaderCell(columnIndex);
                }
            }

            if (items is not null)
            {
                var rowIndex = 1;
                foreach (var item in items)
                {
                    AddRowDefinition();

                    foreach (var columnIndex in Enumerable.Range(0, Columns.Count))
                    {
                        var column = Columns[columnIndex];
                        if (column.IsVisible)
                            AddRowCell(rowIndex, columnIndex, item);
                    }

                    rowIndex++;
                }
            }
        }

        void OnColumnAdded(IEnumerable<PrintableDataGridColumn> columns, int columnIndex)
        {
            foreach (var column in columns)
            {
                AddColumnDefinition(column);
                AddHeaderCell(columnIndex);

                if (items is not null)
                {
                    var rowIndex = 1;
                    foreach (var item in items)
                    {
                        AddRowCell(rowIndex, columnIndex, item);
                        rowIndex++;
                    }
                }

                columnIndex++;
            }
        }

        void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e is null)
                throw new System.ArgumentNullException(nameof(e));

            if (e.Action == NotifyCollectionChangedAction.Add && grid.ColumnDefinitions.Count == e.NewStartingIndex)
            {
                if (e.NewItems is not null)
                    OnColumnAdded(e.NewItems.Cast<PrintableDataGridColumn>(), e.NewStartingIndex);
                return;
            }

            Reset();
        }

        const int HeaderRowCount = 1;

        double IPrintableDataGrid.ItemMeasure(int index)
        {
            return grid.RowDefinitions[HeaderRowCount + index].ActualHeight;
        }

        double IPrintableDataGrid.ActualMeasure => ActualHeight - grid.RowDefinitions[0].ActualHeight;

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public PrintableDataGrid()
        {
            grid = new Grid();
            stackPanel = new StackPanel();
            stackPanel.Children.Add(grid);
            Content = stackPanel;

            InitializeComponent();
        }
    }
}
