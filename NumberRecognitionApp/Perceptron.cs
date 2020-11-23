using System;
using System.Collections.Generic;
using static NumberRecognitionApp.UtilityClass;

namespace NumberRecognitionApp
{
    public class Perceptron
    {
        private double[] _weights;
        private int _lifeTime;
        private double _threshold;
        private double _learningConst;

        private double[] _pocket;
        public Perceptron(int count, double learningConst)
        {
            _learningConst = learningConst;
            _weights = new double[count];
            _pocket = new double[count];
            Random rnd = new Random();

            RandomWeights(count);
            _pocket = _weights;
            _lifeTime = 0;
            _threshold = rnd.NextDouble();
        }

        /// <summary>
        /// Random the starting weights.
        /// </summary>
        /// <param name="count"></param>
        private void RandomWeights(int count)
        {
            Random rnd = new Random();
            for(int i = 0; i < count; i++)
            {
                _weights[i] = rnd.NextDouble();
            }
        }

        public double[] GetWeights()
        {
            return _weights;
        }

        public double GetThreshold()
        {
            return _threshold;
        }

        public void Train(NumberStruct[] inputData, int[] resultList, int iterations)
        {
            if(inputData.Length != resultList.Length) 
                throw new Exception("Number of testing examples is not equal to number of results");

            Random rnd = new Random();

            for(int i = 0; i < iterations; i++){

                int example = rnd.Next(inputData.Length);

                NumberStruct randomExp = inputData[example];
                int result = resultList[example];
                double err = result - ActivationFunction(randomExp.pixels);
                if(err == 0)
                {
                    inputData[example].lifeTime++;
                    if(inputData[example].lifeTime > _lifeTime)
                    {
                        for (int w = 0; w < _pocket.Length; w++){
                            _pocket[w] = _weights[w];
                        }
                        //_pocket = _weights;
                        _lifeTime = inputData[example].lifeTime;
                    }
                }
                else
                {
                    for(int j = 0; j < _weights.Length; j++){
                        _weights[j] = _weights[j] + _learningConst * err * randomExp.pixels[j];
                    }

                    _threshold = _threshold - _learningConst * err;
                    _lifeTime = 0;
                    //inputData[example].lifeTime = 0;
                }
                //Console.WriteLine(err);

            }

           /* for (int j = 0; j < _weights.Length; j++)
            {
                Console.WriteLine(_weights[j]);
            }

            Console.WriteLine("-------------------------------------");
            Console.WriteLine(_threshold);
            */
        }




        private int ActivationFunction(int[] pixels)
        {
            double product = 0;
            for(int i = 0; i < pixels.Length; i++)
            {
                product += pixels[i] * _weights[i];
            }
            product -= _threshold;

            if (product < 0) return 0;
            else return 1;
        }

        public bool GetResult(int[] inputs)
        {
            if(inputs.Length != _weights.Length)
                throw new Exception("Number of input values is not equal to number of weights");

            double product = 0;
            for (int i = 0; i < inputs.Length; i++){
                product += inputs[i] * _weights[i];
            }

            if (product < _threshold) return false;
            else return true;
        }


    }
}
