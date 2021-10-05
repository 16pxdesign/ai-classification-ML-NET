using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace W8ML
{
    public class InputData
    {
        [ColumnName("EmotionLabel")]
        [LoadColumn(0)]
        public string Type { get; set; }
        [LoadColumn(1)]
        public float LeftEye { get; set; }
        [LoadColumn(2)]
        public float RightEye { get; set; }
        [LoadColumn(3)]
        public float LeftLip { get; set; }
        [LoadColumn(4)]
        public float RightLip { get; set; }
        [LoadColumn(5)]
        public float HeightLip { get; set; }
        [LoadColumn(6)]
        public float WidthLip { get; set; }

    }
}
