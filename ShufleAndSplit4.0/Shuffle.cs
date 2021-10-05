using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;

namespace ShufleAndSplit4._0
{
    public class Shuffle
    {
        public static DataOperationsCatalog.TrainTestData SplitAndShuffle(IDataView dataView, MLContext mlContext, int seed)
        {
            dataView = ShuffleData(dataView, mlContext, seed);

            return SplitData(dataView, mlContext);
        }

        public static IDataView ShuffleData(IDataView dataView, MLContext mlContext, int seed)
        {
            dataView = mlContext.Data.ShuffleRows(dataView, seed);
            return dataView;
        }

        public static DataOperationsCatalog.TrainTestData SplitData(IDataView dataView, MLContext mlContext)
        {
            return mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
        }
    }
}
