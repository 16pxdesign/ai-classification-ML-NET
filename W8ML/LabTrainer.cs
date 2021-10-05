using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace W8ML
{
    public static class LabTrainer
    {
        public static int count = 0;
        public static double macro = 0;
        public static double micro = 0;
        public static double loglos = 0;
        public static double loglosR = 0;
        
        public static EstimatorChain<KeyToValueMappingTransformer> CreatePipeline(MLContext mlContext)
        {
            Console.WriteLine($"=============== Create the pipeline  ===============");
            var featureVectorName = "Features";
            var labelColumnName = "Label";
            var pipeline = mlContext.Transforms.Conversion
                .MapValueToKey(
                    inputColumnName: "EmotionLabel",
                    outputColumnName: labelColumnName)
                .Append(mlContext.Transforms.Concatenate(featureVectorName,
                    "LeftEye",
                    "RightEye",
                    "LeftLip",
                    "RightLip",
                    "HeightLip",
                    "WidthLip"))
                .AppendCacheCheckpoint(mlContext)
                .Append(mlContext.MulticlassClassification.Trainers
                    .SdcaMaximumEntropy(labelColumnName, featureVectorName))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            return pipeline;
        }


        public static TransformerChain<KeyToValueMappingTransformer> TrainModel(EstimatorChain<KeyToValueMappingTransformer> pipeline, IDataView dataView)
        {
            Console.WriteLine($"=============== Train the model  ===============");
            return pipeline.Fit(dataView);
        }

        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView dataView)
        {
            Console.WriteLine($"=============== Save the model  ===============");

            using (var fileStream = new FileStream("model.zip", FileMode.Create,
                FileAccess.Write, FileShare.Write))
            {
                mlContext.Model.Save(model, dataView.Schema,
                    fileStream);
            }
        }

        public static void EvaluateModel(MLContext mlContext, ITransformer model, IDataView dataView)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*    Metrics for multi-class classification model   ");
            Console.WriteLine($"*-----------------------------------------------------------");
            
            var testMetrics = mlContext.MulticlassClassification.Evaluate(model.Transform(dataView));

            Console.WriteLine($"* MacroAccuracy : {testMetrics.MacroAccuracy:0.###}");
            Console.WriteLine($"* MicroAccuracy: {testMetrics.MicroAccuracy:0.###}");
            Console.WriteLine($"* LogLoss: {testMetrics.LogLoss:0.###}");

            for (int i = 0; i < testMetrics.PerClassLogLoss.Count; i++)
            {
                Console.WriteLine($"    LogLoss for class {i + 1} = {testMetrics.PerClassLogLoss[i]:0.####}");
            }

            Console.WriteLine($"* LogLossReduction: {testMetrics.LogLossReduction:0.###}"); ;

            //Prompt matrix
            ConfusionMatrix validationMetricsConfusionMatrix = testMetrics.ConfusionMatrix;
            var formattedConfusionTable = validationMetricsConfusionMatrix.GetFormattedConfusionTable();
            Console.WriteLine(formattedConfusionTable);

            count++;
            micro += testMetrics.MicroAccuracy;
            macro += testMetrics.MacroAccuracy;
            loglos += testMetrics.LogLoss;
            loglosR += testMetrics.LogLossReduction;

        }

        public static void PredictInput(MLContext mlContext, ITransformer model, InputData inputData)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*   Results for predicted data   ");
            Console.WriteLine($"*-----------------------------------------------------------");

            var predictor =
                mlContext.Model.CreatePredictionEngine<InputData, OutputData>(model);
            var prediction = predictor.Predict(inputData);

            Console.WriteLine($"*** Prediction: {prediction.Type} ***");
            Console.WriteLine($"*** Scores: {string.Join("\n ", prediction.Scores)} ***");

        }

        public static void AvgMetrics()
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*    AVG Metrics for multi-class classification model   ");
            var macro = LabTrainer.macro / LabTrainer.count;
            var micro = LabTrainer.micro / LabTrainer.count;
            var loglos = LabTrainer.loglos / LabTrainer.count;
            var loglosR = LabTrainer.loglosR / LabTrainer.count;
            Console.WriteLine($"Macro Acc: {macro} \nMicro Acc: {micro} \nLoglos: {loglos}\nLoglosR: {loglosR}");
        }
    }
}

