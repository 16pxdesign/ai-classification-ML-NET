using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FeaturesExtraction;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Vision;
using ShufleAndSplit4._0;
using W8ML;

namespace PreTrain
{
    class PreTrainBuilder
    {
        
        public static void CreateModel(MLContext mlContext)
        {
            //Data
            string allPath = GoPath.DatasetPath("7.0.all.imagespaths.csv");

            var alltDataView = mlContext.Data.LoadFromTextFile<ModelInput>(allPath, hasHeader: true, separatorChar: ',');
            var shuffleData = Shuffle.ShuffleData(alltDataView,mlContext,1);
            var data = Shuffle.SplitData(shuffleData,mlContext);

            //Create pipeline
            var pipeline = CreatePipeline(mlContext);
            //Train Model
            ITransformer model = TrainModel(mlContext,data.TrainSet,pipeline);
            //Save Model
            LabTrainer.SaveModel(mlContext, model, alltDataView);
            //Test model
            EvaluateModel(mlContext,model, data.TestSet); //TODO change input data to test data
            //Cross validation
            CrossValidation(mlContext, shuffleData, pipeline);
            //Predict
            var samplePrediction = mlContext.Data.CreateEnumerable<ModelInput>(data.TestSet, false).First();

            Console.WriteLine($"Predicting type: {samplePrediction.Label}");
            PredictInput(mlContext, model, samplePrediction);
        }


        public static void PredictInput(MLContext mlContext, ITransformer model, ModelInput inputData)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*   Results for predicted data   ");
            Console.WriteLine($"*-----------------------------------------------------------");

            var predictor =
                mlContext.Model.CreatePredictionEngine<ModelInput, OutputData>(model);
            var prediction = predictor.Predict(inputData);

            Console.WriteLine($"*** Prediction: {prediction.Type} ***");
            Console.WriteLine($"*** Scores: {string.Join("\n ", prediction.Scores)} ***");

        }


        private static ITransformer TrainModel(MLContext mlContext, IDataView inputData, IEstimator<ITransformer> pipeline)
        {
            var model = pipeline.Fit(inputData);
            return model;
        }

        private static IEstimator<ITransformer> CreatePipeline(MLContext mlContext)
        {
            var dataProcess = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                .Append(mlContext.Transforms.LoadRawImageBytes("ImageSource_featurized", null, "ImageSource"))
                .Append(mlContext.Transforms.CopyColumns("Features", "ImageSource_featurized"));
            var trainter = mlContext.MulticlassClassification.Trainers.ImageClassification(new ImageClassificationTrainer.Options() { LabelColumnName = "Label", FeatureColumnName = "Features" })
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var pipeline= dataProcess.Append(trainter);
            return pipeline;
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
        }

        #region CrossValidation

        private static void CrossValidation(MLContext mlContext, IDataView inputData, IEstimator<ITransformer> pipeline)
        {
            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults =
                mlContext.MulticlassClassification.CrossValidate(inputData, pipeline, numberOfFolds: 5,
                    labelColumnName: "Label");
            MulticlassClassificationAvgMetrics(crossValidationResults);
        }

        public static void MulticlassClassificationAvgMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> crossValResults)
        {
            var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

            var microAccuracyValues = metricsInMultipleFolds.Select(m => m.MicroAccuracy);
            var microAccuracyAverage = microAccuracyValues.Average();
            var microAccuraciesStdDeviation = CalculateStandardDeviation(microAccuracyValues);
            var microAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(microAccuracyValues);

            var macroAccuracyValues = metricsInMultipleFolds.Select(m => m.MacroAccuracy);
            var macroAccuracyAverage = macroAccuracyValues.Average();
            var macroAccuraciesStdDeviation = CalculateStandardDeviation(macroAccuracyValues);
            var macroAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(macroAccuracyValues);

            var logLossValues = metricsInMultipleFolds.Select(m => m.LogLoss);
            var logLossAverage = logLossValues.Average();
            var logLossStdDeviation = CalculateStandardDeviation(logLossValues);
            var logLossConfidenceInterval95 = CalculateConfidenceInterval95(logLossValues);

            var logLossReductionValues = metricsInMultipleFolds.Select(m => m.LogLossReduction);
            var logLossReductionAverage = logLossReductionValues.Average();
            var logLossReductionStdDeviation = CalculateStandardDeviation(logLossReductionValues);
            var logLossReductionConfidenceInterval95 = CalculateConfidenceInterval95(logLossReductionValues);

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model      ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       Average MicroAccuracy:    {microAccuracyAverage:0.###}  - Standard deviation: ({microAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({microAccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average MacroAccuracy:    {macroAccuracyAverage:0.###}  - Standard deviation: ({macroAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({macroAccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average LogLoss:          {logLossAverage:#.###}  - Standard deviation: ({logLossStdDeviation:#.###})  - Confidence Interval 95%: ({logLossConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average LogLossReduction: {logLossReductionAverage:#.###}  - Standard deviation: ({logLossReductionStdDeviation:#.###})  - Confidence Interval 95%: ({logLossReductionConfidenceInterval95:#.###})");
            Console.WriteLine($"*************************************************************************************************************");

        }


        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
            return standardDeviation;
        }

        public static double CalculateConfidenceInterval95(IEnumerable<double> values)
        {
            double confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1));
            return confidenceInterval95;
        }


        #endregion


    }
}
