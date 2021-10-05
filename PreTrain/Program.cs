using System;
using Microsoft.ML;

namespace PreTrain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start pre-train program");
            do
            {
                var mlContext = new MLContext();
                PreTrainBuilder.CreateModel(mlContext);
            } while (false);//true for looping
        }
    }
}
