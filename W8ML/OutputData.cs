using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace W8ML
{
    public class OutputData
    {
        [ColumnName("PredictedLabel")]
        public string Type { get; set; }
        [ColumnName("Score")]
        public float[] Scores { get; set; }
    }
}
