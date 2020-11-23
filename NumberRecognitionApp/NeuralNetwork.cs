﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognitionApp
{
    public class NeuralNetwork
    {
        int[] layer; 
        Layer[] layers;

        public NeuralNetwork(int[] layer)
        {
            
            this.layer = new int[layer.Length];
            for (int i = 0; i < layer.Length; i++)
                this.layer[i] = layer[i];

            
            layers = new Layer[layer.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layer[i], layer[i + 1]);
            }
        }

        public float[] FeedForward(float[] inputs)
        {
            layers[0].FeedForward(inputs);
            for (int i = 1; i < layers.Length; i++)
            {
                layers[i].FeedForward(layers[i - 1].outputs);
            }

            return layers[layers.Length - 1].outputs;
        }

        public void BackProp(float[] expected)
        {
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                if (i == layers.Length - 1)
                {
                    layers[i].BackPropOutput(expected);
                }
                else
                {
                    layers[i].BackPropHidden(layers[i + 1].gamma, layers[i + 1].weights);
                }
            }

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].UpdateWeights();
            }
        }

        public class Layer
        {
            int numberOfInputs;
            int numberOfOuputs;


            public float[] outputs; 
            public float[] inputs;
            public float[,] weights;
            public float[,] weightsDelta;
            public float[] gamma;
            public float[] error;

            public static Random random = new Random();

            public Layer(int numberOfInputs, int numberOfOuputs)
            {
                this.numberOfInputs = numberOfInputs;
                this.numberOfOuputs = numberOfOuputs;

                outputs = new float[numberOfOuputs];
                inputs = new float[numberOfInputs];
                weights = new float[numberOfOuputs, numberOfInputs];
                weightsDelta = new float[numberOfOuputs, numberOfInputs];
                gamma = new float[numberOfOuputs];
                error = new float[numberOfOuputs];

                InitilizeWeights(); 
            }

            public void InitilizeWeights()
            {
                for (int i = 0; i < numberOfOuputs; i++)
                {
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        weights[i, j] = (float)random.NextDouble() - (float)random.NextDouble();
                    }
                }
            }

            public void FeedForward(float[] inputs)
            {
                this.inputs = inputs;

                
                for (int i = 0; i < numberOfOuputs; i++)
                {
                    outputs[i] = 0;
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        outputs[i] += inputs[j] * weights[i, j];
                    }

                    //outputs[i] = (float)Math.Tanh(outputs[i]);
                    outputs[i] = (float)(1 / (1 + Math.Exp(-outputs[i])));
                }

                //return outputs;
            }

            public float TanHDer(float value)
            {
                return value - (value * value);
                //return (float)(1 - (Math.Pow(value, 2)));
            }

            public void BackPropOutput(float[] expected)
            {
                for (int i = 0; i < numberOfOuputs; i++)
                    error[i] = outputs[i] - expected[i];

                for (int i = 0; i < numberOfOuputs; i++)
                    gamma[i] = error[i] * TanHDer(outputs[i]);

                for (int i = 0; i < numberOfOuputs; i++)
                {
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        weightsDelta[i, j] = gamma[i] * inputs[j];
                    }
                }
            }

            public void BackPropHidden(float[] gammaForward, float[,] weightsFoward)
            {
                for (int i = 0; i < numberOfOuputs; i++)
                {
                    gamma[i] = 0;

                    for (int j = 0; j < gammaForward.Length; j++)
                    {
                        gamma[i] += gammaForward[j] * weightsFoward[j, i];
                    }

                    gamma[i] *= TanHDer(outputs[i]);
                }

                for (int i = 0; i < numberOfOuputs; i++)
                {
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        weightsDelta[i, j] = gamma[i] * inputs[j];
                    }
                }
            }

            public void UpdateWeights()
            {
                for (int i = 0; i < numberOfOuputs; i++)
                {
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        weights[i, j] -= weightsDelta[i, j] * 0.5f;
                    }
                }
            }
        }
    }
}
