#nullable disable

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Biaui.StandardControls.Internal
{
    // Based on this code
    // https://github.com/dotnet/wpf/blob/master/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Primitives/TabPanel.cs

    public class TabPanelInternal : TabPanel
    {
        static TabPanelInternal()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TabPanelInternal), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TabPanelInternal), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        /// <summary>
        /// Updates DesiredSize of the TabPanelInternal.  Called by parent UIElement.  This is the first pass of layout.
        /// </summary>
        /// <remarks>
        /// TabPanelInternal
        /// </remarks>
        /// <param name="constraint">Constraint size is an "upper limit" that TabPanelInternal should not exceed.</param>
        /// <returns>TabPanelInternal' desired size.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var contentSize = new Size();
            var tabAlignment = TabStripPlacement;

            _numRows = 1;
            _numHeaders = 0;
            _rowHeight = 0;

            switch (tabAlignment)
            {
                // For top and bottom placement the panel flow its children to calculate the number of rows and
                // desired vertical size
                case Dock.Top:
                case Dock.Bottom:
                {
                    var numInCurrentRow = 0;
                    var currentRowWidth = 0.0;
                    var maxRowWidth = 0.0;

                    foreach (UIElement child in InternalChildren)
                    {
                        if (child.Visibility == Visibility.Collapsed)
                            continue;

                        _numHeaders++;

                        // Helper measures child, and deals with Min, Max, and base Width & Height properties.
                        // Helper returns the size a child needs to take up (DesiredSize or property specified size).
                        child.Measure(constraint);
                        var childSize = GetDesiredSizeWithoutMargin(child);

                        if (_rowHeight < childSize.Height)
                            _rowHeight = childSize.Height;

                        if (currentRowWidth + childSize.Width > constraint.Width && numInCurrentRow > 0)
                        {
                            // If child does not fit in the current row - create a new row
                            if (maxRowWidth < currentRowWidth)
                                maxRowWidth = currentRowWidth;

                            currentRowWidth = childSize.Width;
                            numInCurrentRow = 1;
                            _numRows++;
                        }
                        else
                        {
                            currentRowWidth += childSize.Width;
                            numInCurrentRow++;
                        }
                    }

                    if (maxRowWidth < currentRowWidth)
                        maxRowWidth = currentRowWidth;

                    contentSize.Height = _rowHeight * _numRows;

                    // If we don't have constraint or content width is smaller than constraint width then size to content
                    if (double.IsInfinity(contentSize.Width) || double.IsNaN(contentSize.Width) ||
                        maxRowWidth < constraint.Width)
                        contentSize.Width = maxRowWidth;
                    else
                        contentSize.Width = constraint.Width;
                    break;
                }

                case Dock.Left:
                case Dock.Right:
                {
                    foreach (UIElement child in InternalChildren)
                    {
                        if (child.Visibility == Visibility.Collapsed)
                            continue;

                        _numHeaders++;

                        // Helper measures child, and deals with Min, Max, and base Width & Height properties.
                        // Helper returns the size a child needs to take up (DesiredSize or property specified size).
                        child.Measure(constraint);

                        var childSize = GetDesiredSizeWithoutMargin(child);

                        if (contentSize.Width < childSize.Width)
                            contentSize.Width = childSize.Width;

                        contentSize.Height += childSize.Height;
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Returns our minimum size & sets DesiredSize.
            return contentSize;
        }

        /// <summary>
        /// TabPanelInternal arranges each of its children.
        /// </summary>
        /// <param name="arrangeSize">Size that TabPanelInternal will assume to position children.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var tabAlignment = TabStripPlacement;

            switch (tabAlignment)
            {
                case Dock.Top:
                case Dock.Bottom:
                    ArrangeHorizontal(arrangeSize);
                    break;

                case Dock.Left:
                case Dock.Right:
                    ArrangeVertical(arrangeSize);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return arrangeSize;
        }

        /// <summary>
        /// Override of <seealso cref="UIElement.GetLayoutClip"/>.
        /// </summary>
        /// <returns>Geometry to use as additional clip in case when element is larger then available space</returns>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }

        private static Size GetDesiredSizeWithoutMargin(UIElement element)
        {
            var margin = (Thickness) element.GetValue(MarginProperty);
            Size desiredSizeWithoutMargin = default;
            desiredSizeWithoutMargin.Height = Math.Max(0d, element.DesiredSize.Height - margin.Top - margin.Bottom);
            desiredSizeWithoutMargin.Width = Math.Max(0d, element.DesiredSize.Width - margin.Left - margin.Right);
            return desiredSizeWithoutMargin;
        }

        private void GetHeadersSize(Span<double> headerSize)
        {
            var childIndex = 0;

            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                    continue;

                var childSize = GetDesiredSizeWithoutMargin(child);
                headerSize[childIndex] = Math.Floor(childSize.Width);
                childIndex++;
            }
        }

        private void ArrangeHorizontal(Size arrangeSize)
        {
            var numSeparators = _numRows - 1;
        
            var buffer = ArrayPool<byte>.Shared.Rent(
                Unsafe.SizeOf<double>() * _numHeaders +
                Unsafe.SizeOf<int>() + numSeparators);

            var headerSize = MemoryMarshal.Cast<byte, double>(buffer.AsSpan(0, Unsafe.SizeOf<double>() *_numHeaders));
            var solution = MemoryMarshal.Cast<byte, int>(buffer.AsSpan(Unsafe.SizeOf<double>() *_numHeaders, Unsafe.SizeOf<int>() * numSeparators));
            
            try
            {
                var tabAlignment = TabStripPlacement;
                var isMultiRow = _numRows > 1;
                var activeRow = 0;
                var childOffset = new Vector();

                GetHeadersSize(headerSize);

                // If we have multirows, then calculate the best header distribution
                if (isMultiRow)
                {
                    solution = CalculateHeaderDistribution(arrangeSize.Width, headerSize, solution);
                    activeRow = GetActiveRow(solution);

                    childOffset.Y = tabAlignment switch
                    {
                        // TabPanelInternal starts to layout children depend on activeRow which should be always on bottom (top)
                        // The first row should start from Y = (_numRows - 1 - activeRow) * _rowHeight
                        Dock.Top => ((_numRows - 1 - activeRow) * _rowHeight),
                        Dock.Bottom when activeRow != 0 => ((_numRows - activeRow) * _rowHeight),
                        _ => childOffset.Y
                    };
                }
                else
                {
                    solution = Array.Empty<int>();
                }

                var childIndex = 0;
                var separatorIndex = 0;

                foreach (UIElement child in InternalChildren)
                {
                    if (child.Visibility == Visibility.Collapsed)
                        continue;

                    var margin = (Thickness) child.GetValue(MarginProperty);
                    var leftOffset = margin.Left;
                    var rightOffset = margin.Right;
                    var topOffset = margin.Top;
                    var bottomOffset = margin.Bottom;

                    var lastHeaderInRow = isMultiRow &&
                                          (separatorIndex < solution.Length && solution[separatorIndex] == childIndex ||
                                           childIndex == _numHeaders - 1);

                    //Length left, top, right, bottom;
                    var cellSize = new Size(headerSize[childIndex], _rowHeight);

                    // Align the last header in the row; If headers are not aligned directional nav would not work correctly
                    if (lastHeaderInRow)
                        cellSize.Width = arrangeSize.Width - childOffset.X;

                    child.Arrange(new Rect(childOffset.X, childOffset.Y, cellSize.Width, cellSize.Height));

                    var childSize = cellSize;
                    childSize.Height = Math.Max(0d, childSize.Height - topOffset - bottomOffset);
                    childSize.Width = Math.Max(0d, childSize.Width - leftOffset - rightOffset);

                    // Calculate the offset for the next child
                    childOffset.X += cellSize.Width;

                    if (lastHeaderInRow)
                    {
                        if ((separatorIndex == activeRow && tabAlignment == Dock.Top) ||
                            (separatorIndex == activeRow - 1 && tabAlignment == Dock.Bottom))
                            childOffset.Y = 0d;
                        else
                            childOffset.Y += _rowHeight;

                        childOffset.X = 0d;
                        separatorIndex++;
                    }

                    childIndex++;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private void ArrangeVertical(Size arrangeSize)
        {
            var childOffsetY = 0d;

            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    var childSize = GetDesiredSizeWithoutMargin(child);
                    child.Arrange(new Rect(0, childOffsetY, arrangeSize.Width, childSize.Height));

                    // Calculate the offset for the next child
                    childOffsetY += childSize.Height;
                }
            }
        }

        // Returns the row which contain the child with IsSelected==true
        private int GetActiveRow(Span<int> solution)
        {
            var activeRow = 0;
            var childIndex = 0;

            if (solution.Length > 0)
            {
                foreach (UIElement child in InternalChildren)
                {
                    if (child.Visibility == Visibility.Collapsed)
                        continue;

                    var isActiveTab = (bool) child.GetValue(Selector.IsSelectedProperty);

                    if (isActiveTab)
                        return activeRow;

                    if (activeRow < solution.Length && solution[activeRow] == childIndex)
                        activeRow++;

                    childIndex++;
                }
            }

            // If the is no selected element and alignment is Top  - then the active row is the last row 
            if (TabStripPlacement == Dock.Top)
                activeRow = _numRows - 1;

            return activeRow;
        }

        /*   TabPanelInternal layout calculation:
         
        After measure call we have:
        rowWidthLimit - width of the TabPanelInternal
        Header[0..n-1]  - headers
        headerWidth[0..n-1] - header width
         
        Calculated values:
        numSeparators                       - number of separators between numSeparators+1 rows
        rowWidth[0..numSeparators]           - row width
        rowHeaderCount[0..numSeparators]    - Row Count = number of headers on that row
        rowAverageGap[0..numSeparators]     - Average Gap for the row i = (rowWidth - rowWidth[i])/rowHeaderCount[i]
        currentSolution[0..numSeparators-1] - separator currentSolution[i]=x means Header[x] and h[x+1] are separated with new line
        bestSolution[0..numSeparators-1]    - keep the last Best Solution
        bestSolutionRowAverageGap           - keep the last Best Solution Average Gap

        Between all separators distribution the best solution have minimum Average Gap - 
        this is the amount of pixels added to the header (to justify) in the row

        How does it work:
        First we flow the headers to calculate the number of necessary rows (numSeparators+1).
        That means we need to insert numSeparators separators between n headers (numSeparators<n always).
        For each current state rowAverageGap[1..numSeparators+1] are calculated for each row.
        Current state rowAverageGap = MAX (rowAverageGap[1..numSeparators+1]).
        Our goal is to find the solution with MIN (rowAverageGap).
        On each iteration step we move a header from a previous row to the row with maximum rowAverageGap.
        We continue the iterations only if we move to better solution, i.e. rowAverageGap is smaller.
        Maximum iteration steps are less the number of headers.

        */
        // Input: Row width and width of all headers
        // Output: int array which size is the number of separators and contains each separator position
        private Span<int> CalculateHeaderDistribution(double rowWidthLimit, Span<double> headerWidth, Span<int> bestSolution)
        {
            var numSeparators = _numRows - 1;

            var doubleArray = ArrayPool<double>.Shared.Rent(_numRows + _numRows + _numRows);
            var intArray = ArrayPool<int>.Shared.Rent(numSeparators + _numRows);

            var rowWidth = doubleArray.AsSpan(0, _numRows);
            var rowAverageGap = doubleArray.AsSpan(_numRows, _numRows);
            var bestSolutionRowAverageGap = doubleArray.AsSpan(_numRows + _numRows, _numRows);
            var currentSolution = intArray.AsSpan(0, numSeparators);
            var rowHeaderCount = intArray.AsSpan(numSeparators, _numRows);

            try
            {
                var bestSolutionMaxRowAverageGap = 0.0;
                var numHeaders = headerWidth.Length;
                var currentRowWidth = 0.0;
                var numberOfHeadersInCurrentRow = 0;
                double currentAverageGap;

                // Initialize the current state; Do the initial flow of the headers
                var currentRowIndex = 0;

                for (var index = 0; index < numHeaders; index++)
                {
                    if (currentRowWidth + headerWidth[index] > rowWidthLimit && numberOfHeadersInCurrentRow > 0)
                    {
                        // if we cannot add next header - flow to next row
                        // Store current row before we go to the next
                        rowWidth[currentRowIndex] = currentRowWidth; // Store the current row width
                        rowHeaderCount[currentRowIndex] = numberOfHeadersInCurrentRow; // For each row we store the number os headers inside
                        currentAverageGap =
                            Math.Max(0d,
                                (rowWidthLimit - currentRowWidth) /
                                numberOfHeadersInCurrentRow); // The amount  of width that should be added to justify the header
                        rowAverageGap[currentRowIndex] = currentAverageGap;
                        currentSolution[currentRowIndex] = index - 1; // Separator points to the last header in the row
                        if (bestSolutionMaxRowAverageGap < currentAverageGap
                        ) // Remember the maximum of all currentAverageGap
                            bestSolutionMaxRowAverageGap = currentAverageGap;

                        // Iterate to next row
                        currentRowIndex++;
                        currentRowWidth = headerWidth[index]; // Accumulate header widths on the same row
                        numberOfHeadersInCurrentRow = 1;
                    }
                    else
                    {
                        currentRowWidth += headerWidth[index]; // Accumulate header widths on the same row
                        // Increase the number of headers only if they are not collapsed (width=0)
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (headerWidth[index] != 0)
                            numberOfHeadersInCurrentRow++;
                    }
                }

                // If everything  fit in 1 row then exit (no separators needed)
                if (currentRowIndex == 0)
                    return bestSolution.Slice(0, 0);

                // Add the last row
                rowWidth[currentRowIndex] = currentRowWidth;
                rowHeaderCount[currentRowIndex] = numberOfHeadersInCurrentRow;
                currentAverageGap = (rowWidthLimit - currentRowWidth) / numberOfHeadersInCurrentRow;
                rowAverageGap[currentRowIndex] = currentAverageGap;
                if (bestSolutionMaxRowAverageGap < currentAverageGap)
                    bestSolutionMaxRowAverageGap = currentAverageGap;

                currentSolution.CopyTo(bestSolution); // Remember the first solution as initial bestSolution
                rowAverageGap.CopyTo(bestSolutionRowAverageGap); // bestSolutionRowAverageGap is used in ArrangeOverride to calculate header sizes

                // Search for the best solution
                // The exit condition if when we cannot move header to the next row 
                while (true)
                {
                    // Find the row with maximum AverageGap
                    var worstRowIndex = 0; // Keep the row index with maximum AverageGap
                    var maxAg = 0.0;

                    for (var i = 0; i < _numRows; i++) // for all rows
                    {
                        if (maxAg < rowAverageGap[i])
                        {
                            maxAg = rowAverageGap[i];
                            worstRowIndex = i;
                        }
                    }

                    // If we are on the first row - cannot move from previous
                    if (worstRowIndex == 0)
                        break;

                    // From the row with maximum AverageGap we try to move a header from previous row
                    var moveToRow = worstRowIndex;
                    var moveFromRow = moveToRow - 1;
                    var moveHeader = currentSolution[moveFromRow];
                    var movedHeaderWidth = headerWidth[moveHeader];

                    rowWidth[moveToRow] += movedHeaderWidth;

                    // If the moved header cannot fit - exit. We have the best solution already.
                    if (rowWidth[moveToRow] > rowWidthLimit)
                        break;

                    // If header is moved successfully to the worst row
                    // we update the arrays keeping the row state
                    currentSolution[moveFromRow]--;
                    rowHeaderCount[moveToRow]++;
                    rowWidth[moveFromRow] -= movedHeaderWidth;
                    rowHeaderCount[moveFromRow]--;
                    rowAverageGap[moveFromRow] = (rowWidthLimit - rowWidth[moveFromRow]) / rowHeaderCount[moveFromRow];
                    rowAverageGap[moveToRow] = (rowWidthLimit - rowWidth[moveToRow]) / rowHeaderCount[moveToRow];

                    // EvaluateSolution:
                    // If the current solution is better than bestSolution - keep it in bestSolution
                    maxAg = 0;

                    for (var i = 0; i < _numRows; i++) // for all rows
                    {
                        if (maxAg < rowAverageGap[i])
                        {
                            maxAg = rowAverageGap[i];
                        }
                    }

                    if (maxAg < bestSolutionMaxRowAverageGap)
                    {
                        bestSolutionMaxRowAverageGap = maxAg;
                        currentSolution.CopyTo(bestSolution);
                        rowAverageGap.CopyTo(bestSolutionRowAverageGap);
                    }
                }

                // Each header size should be increased so headers in the row stretch to fit the row
                currentRowIndex = 0;

                for (var index = 0; index < numHeaders; index++)
                {
                    headerWidth[index] += bestSolutionRowAverageGap[currentRowIndex];
                    if (currentRowIndex < numSeparators && bestSolution[currentRowIndex] == index)
                        currentRowIndex++;
                }
            }
            finally
            {
                ArrayPool<int>.Shared.Return(intArray);
                ArrayPool<double>.Shared.Return(doubleArray);
            }

            return bestSolution;
        }

        private Dock TabStripPlacement => (TemplatedParent is TabControl tc) ? tc.TabStripPlacement : Dock.Top;

        private int _numRows = 1; // Number of row calculated in measure and used in arrange
        private int _numHeaders; // Number of headers excluding the collapsed items
        private double _rowHeight; // Maximum of all headers height
    }
}