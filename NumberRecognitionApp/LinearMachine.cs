using System;
using System.Collections.Generic;

namespace NumberRecognitionApp
{
    public class LinearMachine
    {
        private double[] _kat1_weights;
        private double[] _kat2_weights;

        private double[] _pocket1_weights;
        private double[] _pocket2_weights;

        private int _lifetime = 0;
        private int _record = 0;


        public LinearMachine(int count)
        {
            _kat1_weights = new double[count];
            _kat2_weights = new double[count];

            _pocket1_weights = new double[count];
            _pocket2_weights = new double[count];

            Random rnd = new Random();

            RandomWeights(count);
        }

        private void RandomWeights(int count)
        {
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                _kat1_weights[i] = rnd.NextDouble();
                _kat2_weights[i] = rnd.NextDouble();
            }
        }

        public void Train(List<int[]> result, int iterations, int pix)
        {
            Random rnd = new Random();

            Array.Copy(_kat1_weights, _pocket1_weights, _kat1_weights.Length);
            Array.Copy(_kat2_weights, _pocket2_weights, _kat2_weights.Length);

            for (int i = 0; i < iterations; i++)
            {
                int example = rnd.Next(result.Count);

                int[] image = result[example];

                double neuron = GetMax(image); 
                if(neuron != image[pix])
                {
                    _lifetime = 0;
                    if(neuron == 0)
                    {
                        for (int j = 0; j < 2500; j++)
                        {

                            _kat1_weights[j] -= image[j];
                            
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 2500; j++)
                        {

                            _kat2_weights[j] -= image[j];

                        }
                    }

                    if (image[pix] == 0)
                    {
                        for (int j = 0; j < 2500; j++)
                        {

                            _kat1_weights[j] += image[j];

                        }
                    }
                    else
                    {
                        for (int j = 0; j < 2500; j++)
                        {

                            _kat2_weights[j] += image[j];

                        }
                    }

                }
                else{
                    _lifetime++;
                    if(_lifetime > _record)
                    {
                        _record = _lifetime;
                        Array.Copy(_kat1_weights, _pocket1_weights, _kat1_weights.Length);
                        Array.Copy(_kat2_weights, _pocket2_weights, _kat2_weights.Length);
                    }
                }
            }
        }

        private double GetMax(int[] input)
        {
            double sum_first = 0;
            double sum_second = 0;

            
            for(int i = 0; i < input.Length; i++)
            {
                sum_first += _kat1_weights[i] * input[i];
                sum_second += _kat2_weights[i] * input[i];
            }

            sum_first += 1;
            sum_second += 1;

            return sum_first >= sum_second ? 0 : 1;
        }

        private double CountSum(int[] piksele, double[] wagi)
        {
            double sum = 0;
            for(int i = 0; i < 2500; i++)
            {
                sum += piksele[i] * wagi[i];
            }
            sum++;
            return sum;
        }

        public int GetAnswer(int[] piksele)
        {
            double sum1 = CountSum(piksele, _kat1_weights);
            double sum2 = CountSum(piksele, _kat2_weights);

            if (sum1 > sum2) return 0;
            else return 1;
        }
    }
}
