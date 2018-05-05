using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public interface IStatistics
    {
        /// <summary>
        /// Force computation of statistics
        /// </summary>
        void ComputeStatistics();

        /// <summary>
        /// Gets the maximum data value in the grid.
        /// </summary>
        double Maximum
        {
            get;
        }

        /// <summary>
        /// Gets the mean of the values in this grid.
        /// </summary>
        double Mean
        {
            get;
        }

        /// <summary>
        /// Gets the minimum data value  in this raster.
        /// </summary>
        double Minimum
        {
            get;
        }
    }

    public interface IGrid<T> where T:struct
    {
        /// <summary>
        /// Gets or sets a value at the 0 row, 0 column index.
        /// </summary>
        /// <param name="row">The 0 based vertical row index from the top</param>
        /// <param name="column">The 0 based horizontal column index from the left</param>
        /// <returns>An object reference to the actual value in the data member.</returns>
        T this[long row, long column]
        {
            get;

            set;
        }

        /// <summary>
        /// Gets the width of the grid.
        /// </summary>
        int Width
        {
            get;
        }

        /// <summary>
        /// Gets the height of the grid.
        /// </summary>
        int Height
        {
            get;
        }
    }
}
