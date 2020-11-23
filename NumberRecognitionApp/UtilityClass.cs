using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace NumberRecognitionApp
{
    public class UtilityClass
    {
        private Dictionary<int, (string, string)> _filePaths = new Dictionary<int, (string, string)> {
            { 1, ("zero", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Zero\")},
            { 2, ("one", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\One\")},
            { 3, ("two", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Two\")},
            { 4, ("three", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Three\")},
            { 5, ("four", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Four\")},
            { 6, ("five", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Five\")},
            { 7, ("six", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Six\")},
            { 8, ("seven", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Seven\")},
            { 9, ("eight", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Eight\")},
            { 10, ("nine", @"C:\Users\Kamil\source\repos\NumberRecognitionApp\Numbers\Nine\")} };

        private string _imgPath = @"C:\Users\Kamil\source\repos\NumberRecognitionApp\imgs\";

        public List<int[]> images;

        public struct NumberStruct
        {
            public int number;
            public int[] pixels;
            public int lifeTime;
        }

        public NumberStruct[] ImgData { get; set; } //zapis wszystkich danych z obrazkow testowych
        public NumberStruct[] TestData { get; set; } //przyklady uczace - 440
        private int[] _expNum;

        public UtilityClass(int[] expNum)
        {
            _expNum = expNum;
            ImgData = new NumberStruct[SumDataFromArray(_expNum, 10)];
            TestData = new NumberStruct[SumDataFromArray(_expNum, 10) * 10];


            images = new List<int[]>();
            //LoadDataFromImg(_filePaths, _expNum);
            //GetMoreData();


            for(int i = 1; i <= 5; i++)
            {
                images.Add(GetImgData(i));
            }
        }

        private void GetMoreData()
        {
            int num = 0;
            foreach(NumberStruct s in ImgData)
            {
                TestData[num] = s;
                num++;
                for(int i = 0; i < 9; i++)
                {
                    Random rnd = new Random();
                    int randToChange = rnd.Next(3);
                    NumberStruct a = new NumberStruct();
                    a.number = s.number;
                    a.lifeTime = 0;
                    a.pixels = s.pixels;
                    for (int j = 0; j < randToChange; j++)
                    {


                        int randIndex1 = rnd.Next(45);
                        //int randIndex2 = rnd.Next(45);
                        
                        if (a.pixels[randIndex1] == 1 /*&& a.pixels[randIndex2] == 1*/)
                        {
                            a.pixels[randIndex1] = 0;
                            //a.pixels[randIndex2] = 0;
                            
                        }
                        else if (a.pixels[randIndex1] == 0 /*&& a.pixels[randIndex2] == 0*/)
                        {
                            a.pixels[randIndex1] = 1;
                            //a.pixels[randIndex2] = 1;
                            

                        }
                    }

                    //NumberStruct tmp;
                    //tmp.pixels = a.pixels;
                    //tmp.number = a.number;
                    //tmp.lifeTime = 0;
                    //NumberStruct b = new NumberStruct();
                    //b = a;
                    TestData[num] = a;

                    //a.pixels = s.pixels;
                    /*else if (a.pixels[randIndex1] == 1 && a.pixels[randIndex2] == 0)
                    {
                        a.pixels[randIndex1] = 0;
                        a.pixels[randIndex2] = 1;
                        NumberStruct tmp;
                        tmp.pixels = a.pixels;
                        tmp.number = a.number;
                        tmp.lifeTime = 0;

                        TestData[num] = tmp;

                        a.pixels = s.pixels;
                    }
                    else if (a.pixels[randIndex1] == 0 && a.pixels[randIndex2] == 1)
                    {
                        a.pixels[randIndex1] = 1;
                        a.pixels[randIndex2] = 0;
                        NumberStruct tmp;
                        tmp.pixels = a.pixels;
                        tmp.number = a.number;
                        tmp.lifeTime = 0;

                        TestData[num] = tmp;

                        a.pixels = s.pixels;
                    }*/
                    num++;
                }
            }

            for (int i = 0; i < TestData.Length; i++) Console.WriteLine(TestData[i]);
        }

        /// <summary>
        /// Save data (structs) about each image example, like array of 1 & 0, number and number of example.
        /// </summary>
        /// <param name="fp"></param>
        /// <param name="expNum"></param>
        private void LoadDataFromImg(Dictionary<int, (string, string)> fp, int[] expNum)
        {
            int num = 0;
            for (int i = 1; i <= fp.Count; i++)
            {
                for (int j = 1; j <= expNum[i - 1]; j++)
                {
                    string imgScr = $"{fp[i].ToTuple().Item2}{fp[i].ToTuple().Item1}_ex_{j}.png";

                    NumberStruct tmp;
                    tmp.pixels = ParseImage(imgScr, 45);
                    tmp.number = i - 1;
                    tmp.lifeTime = 0;

                    ImgData[num] = tmp;

                    num++;
                }
            }
        }

        /// <summary>
        /// Transfer image png into 0 & 1 array.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private int[] ParseImage(string filePath, int size)
        {
            Bitmap img = new Bitmap(filePath);
            int[] pixTab = new int[size];
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(i, j);
                    if (pixel.R.ToString() == "0" && pixel.G.ToString() == "0" && pixel.B.ToString() == "0")
                    {
                        pixTab[j + i * img.Height] = 1;
                    }
                    else if (pixel.R.ToString() == "255" && pixel.G.ToString() == "255" && pixel.B.ToString() == "255")
                    {
                        pixTab[j + i * img.Height] = 0;
                    }
                    //Console.WriteLine(pixel.R.ToString("D3") + ", " + pixel.G.ToString("D3") + ", " + pixel.B.ToString("D3"));
                }
            }
            return pixTab;
        }


        public int[] GetImgData(int num)
        {
            string path = $"{_imgPath}{num}.png";
            int[] pix = ParseImage(path, 2500);
            return pix;
        }

        /// <summary>
        /// Prepare the output (results) array containing 0 & 1 values for every single image example.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="expNum"></param>
        /// <returns></returns>
        public int[] PrepareOutputArray(int num, int[] expNum)
        {
            int[] outputArray = new int[SumDataFromArray(expNum, 10)];
            int tmpSum = SumDataFromArray(expNum, num - 1);
            for (int i = 0; i < outputArray.Length; i++)
            {
                if (i >= tmpSum && i < tmpSum + expNum[num - 1]) outputArray[i] = 1;
                else outputArray[i] = 0;
            }
            return outputArray;
        }

        private int SumDataFromArray(int[] data, int numLimit)
        {
            int sum = 0;
            for (int i = 0; i < numLimit; i++)
            {
                sum += data[i];
            }
            return sum;
        }

        /// <summary>
        /// Method uses to write weights and threshold got from perceptron.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void WriteToFile(string path, double data)
        {
            using (var tw = new StreamWriter(path, true))
            {
                tw.WriteLine(data);
            }
        }

        public List<double> ReadFromFile(string path)
        {
            List<double> data = new List<double>();
            try
            {   
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        data.Add(double.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture));                
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return data;
        }
    }
}
