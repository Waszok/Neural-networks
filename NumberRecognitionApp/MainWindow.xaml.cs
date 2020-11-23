using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Rectangle = System.Windows.Shapes.Rectangle;

using static NumberRecognitionApp.UtilityClass;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
//using System.Windows.Interop;

namespace NumberRecognitionApp
{

    public partial class MainWindow : Window
    {
        bool _isDrawing = false;
        bool _isDrawingImg = false;
        UtilityClass _util;
        int[] _examples = { 6, 8, 4, 3, 5, 2, 2, 3, 6, 5 };
        int[] _examplesNew = { 60, 80, 40, 30, 50, 20, 20, 30, 60, 50 };
        int[] _tab;
        int[] _tab2;
        Random r = new Random();

        bool _brush = true;
        bool _brushImg = true;

        //Zadanie 2
        private int[] _currentImage;
        private List<int[]> images = new List<int[]>();
        private int _imageIndex = 0;

        private LinearMachine[] _linearMachines;

        int[] resultPixels = new int[2500];

        //Zadanie 3
        ImageBrush brush = new ImageBrush();

        //Zadanie 4
        Graph<Tuple<float, float, float>> _graph;
        Tuple<float, float, float> _color;
        //ImageBrush brush4 = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();

            InitializeCanvas(canvas, 5, 9, 30);

            InitializeCanvas(canvasImg, 50, 50, 10);
            InitializeCanvas(canvasImgResult, 50, 50, 10);


            _tab = new int[45];
            _tab2 = new int[2500];
            _util = new UtilityClass(_examples);

            _linearMachines = new LinearMachine[2500];
            for(int i = 0; i < 2500; i++)
            {
                LinearMachine m = new LinearMachine(2500);
                _linearMachines[i] = m;
            }

            for (int i = 0; i < 5; i++)
            {
                images.Add(_util.images[i]);
            }

            _currentImage = _util.images[0];
            DrawImgOnCanvas(_currentImage, canvasImg);

            //LearnMachine();
        }

        /// <summary>
        /// Initialize the rectangle grid for drawing numbers
        /// </summary>
        private void InitializeCanvas(Canvas canvas, int width, int height, int size)
        {
            canvas.Children.Clear();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Rectangle rec = new Rectangle();
                    Canvas.SetTop(rec, j * size);
                    Canvas.SetLeft(rec, i * size);
                    rec.Width = size;
                    rec.Height = size;
                    rec.Fill = new SolidColorBrush(Colors.White);
                    canvas.Children.Add(rec);

                }
            }
        }

        #region Handle mouse events

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;

            int x = (int)Mouse.GetPosition(canvas).X / 30;
            int y = (int)Mouse.GetPosition(canvas).Y / 30;

            AddPixel(x, y, 30, canvas);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                int x = (int)Mouse.GetPosition(canvas).X / 30;
                int y = (int)Mouse.GetPosition(canvas).Y / 30;

                AddPixel(x, y, 30, canvas);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Zadanie 2 ///////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CanvasImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawingImg = true;

            int x = (int)Mouse.GetPosition(canvasImg).X / 10;
            int y = (int)Mouse.GetPosition(canvasImg).Y / 10;

            Console.WriteLine(x);
            Console.WriteLine(y);

            AddPixel(x, y, 10, canvasImg);
        }

        private void CanvasImg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawingImg = false;
        }

        private void CanvasImg_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawingImg)
            {
                int x = (int)Mouse.GetPosition(canvasImg).X / 10;
                int y = (int)Mouse.GetPosition(canvasImg).Y / 10;

                Console.WriteLine(x);
                Console.WriteLine(y);

                AddPixel(x, y, 10, canvasImg);
            }
        }

        #endregion

        private void DrawImgOnCanvas(int[] pixImg, Canvas c)
        {
            c.Children.Clear();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Rectangle rec = new Rectangle();
                    Canvas.SetTop(rec, j * 10);
                    Canvas.SetLeft(rec, i * 10);
                    rec.Width = 10;
                    rec.Height = 10;

                    if (pixImg[i * 50 + j] == 0)
                        rec.Fill = new SolidColorBrush(Colors.White);
                    else
                        rec.Fill = new SolidColorBrush(Colors.Black);

                    c.Children.Add(rec);
                }
            }
        }

        /// <summary>
        /// Draw a single pixel on the canvas.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void AddPixel(double x, double y, int size, Canvas c)
        {
            Point p = new Point(x * size, y * size);
            Rectangle rec = c.InputHitTest(p) as Rectangle;

            if ((_brush && c.Name == "canvas") || (_brushImg && c.Name == "canvasImg"))
                rec.Fill = new SolidColorBrush(Colors.Black);
            else
                rec.Fill = new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// Clear whole canvas after clicking "clear" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Name == "ClearZad1")
                InitializeCanvas(canvas, 5, 9, 30);
            else if ((sender as Button).Name == "ClearZad2")
                InitializeCanvas(canvasImg, 50, 50, 10);
        }


        void Learn()
        {
            for (int i = 1; i <= 10; i++)
            {
                int[] result = new int[440];
                result = _util.PrepareOutputArray(i, _examplesNew);

                //foreach (int k in result) Console.WriteLine(k);
                //Console.WriteLine("----------------------------------------------------------");

                Perceptron p = new Perceptron(45, 0.5);
                p.Train(_util.TestData, result, 1000000);
                double[] w = p.GetWeights();
                double t = p.GetThreshold();

                string path = @"C:\Users\Kamil\source\repos\NumberRecognitionApp\" + "data" + i.ToString();
                if (File.Exists(path)) File.WriteAllText(path, string.Empty);
                foreach (double we in w)
                {
                    _util.WriteToFile(path, we);
                }
                _util.WriteToFile(path, t);

            }
        }

        void GetCanvasData(Canvas c, bool k)
        {
            int num = 0;
            if (k == false)
            {
                foreach (Rectangle rect in c.Children.OfType<Rectangle>())
                {
                    if (((SolidColorBrush)rect.Fill).Color.ToString() == "#FFFFFFFF") _tab[num] = 0;
                    else _tab[num] = 1;
                    num++;
                }
            }
            else if(k == true)
            {
                foreach (Rectangle rect in c.Children.OfType<Rectangle>())
                {
                    if (((SolidColorBrush)rect.Fill).Color.ToString() == "#FFFFFFFF") _tab2[num] = 0;
                    else _tab2[num] = 1;
                    num++;
                }
                using (var tw = new StreamWriter(@"C:\Users\Kamil\source\repos\NumberRecognitionApp\dataWynik", true))
                {
                    int t = 0;
                    for (int i = 0; i < _tab2.Length; i++)
                    {
                        
                        t++;
                        if(t % 50 == 0)
                        {
                            tw.Write("\n");
                        }
                        tw.Write(_tab2[i]);
                    }
                }
            }
        }


        void RecognizeNumber()
        {
            List<int> resultRecognition = new List<int>();
            for (int i = 1; i <= 10; i++)
            {
                string path = @"C:\Users\Kamil\source\repos\NumberRecognitionApp\data" + i.ToString();
                List<double> weights = _util.ReadFromFile(path);

                double product = 0;
                for (int j = 0; j < _tab.Length; j++)
                {
                    product += _tab[j] * weights[j];
                    //Console.WriteLine(weights[j]);
                }

                if (product < weights[45])
                {
                    Console.WriteLine(0);
                    resultRecognition.Add(0);
                }
                else
                {
                    Console.WriteLine(1);
                    resultRecognition.Add(1);
                }


            }
            Console.WriteLine("----------------------------------------------------------");
            recognizedNumbersBox.Text = string.Empty;
            resultRecognition.ForEachWithIndex((item, idx) =>
            {
                if (item == 1) recognizedNumbersBox.Text += idx.ToString() + "; ";
            });
        }

        private void Recognize(object sender, RoutedEventArgs e)
        {
            GetCanvasData(canvas, false);
            RecognizeNumber();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Name == "whiteBtn")
            {
                whiteBtn.BorderThickness = new Thickness(2);
                blackBtn.BorderThickness = new Thickness(0);
                _brush = false;
            }
            else if ((sender as Button).Name == "whiteBtnImg")
            {
                whiteBtnImg.BorderThickness = new Thickness(2);
                blackBtnImg.BorderThickness = new Thickness(0);
                _brushImg = false;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Name == "blackBtn")
            {
                whiteBtn.BorderThickness = new Thickness(0);
                blackBtn.BorderThickness = new Thickness(2);
                _brush = true;
            }
            else if ((sender as Button).Name == "blackBtnImg")
            {
                whiteBtnImg.BorderThickness = new Thickness(0);
                blackBtnImg.BorderThickness = new Thickness(2);
                _brushImg = true;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if(_imageIndex > 0)
            {
                _imageIndex--;
                _currentImage = _util.images[_imageIndex];
                DrawImgOnCanvas(_currentImage, canvasImg);
                if (_imageIndex == 0) PrevBtn.IsEnabled = false;
            }
            NextBtn.IsEnabled = true;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_imageIndex < 4)
            {
                _imageIndex++;
                _currentImage = _util.images[_imageIndex];
                DrawImgOnCanvas(_currentImage, canvasImg);
                if (_imageIndex == 4) NextBtn.IsEnabled = false;
            }
            PrevBtn.IsEnabled = true;
        }

        private void LearnMachine()
        {
            for (int i = 0; i < _linearMachines.Length; i++)
            {
                _linearMachines[i].Train(images, 10000, i);
            }
        }

        private void RecognizeMachine(object sender, RoutedEventArgs e)
        {
            GetCanvasData(canvasImg, true);
            
            for(int i = 0; i < _linearMachines.Length; i++)
            {
                resultPixels[i] = _linearMachines[i].GetAnswer(_tab2);
            }

            DrawImgOnCanvas(resultPixels, canvasImgResult);
        }

        private void ReloadImage(object sender, RoutedEventArgs e)
        {
            InitializeCanvas(canvasImgResult, 50, 50, 10);
            DrawImgOnCanvas(resultPixels, canvasImg);
        }



        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        private void LoadImage(Canvas canvas)
        {
            Microsoft.Win32.OpenFileDialog dl1 = new Microsoft.Win32.OpenFileDialog();
            dl1.FileName = "OpenedFile";
            dl1.DefaultExt = ".png";
            dl1.Filter = "Image documents (.png)|*.png";
            Nullable<bool> result = dl1.ShowDialog();

            if (result == true)
            {
                string filename = dl1.FileName;

                brush.ImageSource = new BitmapImage(new Uri(@filename, UriKind.Relative));
                canvas.Background = brush;
            }
        }
        private void LoadImage(object sender, RoutedEventArgs e)
        {
            if(((Button)sender).Name == "Load3")
                LoadImage(canvasImg3);
            else if (((Button)sender).Name == "Load4")
                LoadImage(canvasImg4);
        }

        private System.Drawing.Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new System.Drawing.Bitmap(bitmap);
            }
        }

        private BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return (BitmapImage)i;
        }



        public float GetColor(System.Drawing.Bitmap bitmap, int color, int x, int y)
        {
            if (color == 1)
                return bitmap.GetPixel(x, y).R / (float)255 * 0.8f + 0.1f;
            else if (color == 2)
                return bitmap.GetPixel(x, y).G / (float)255 * 0.8f + 0.1f;
            else if (color == 3)
                return bitmap.GetPixel(x, y).B / (float)255 * 0.8f + 0.1f;
            return 0f;
        }

        public int GetOutputRgb(float color)
        {
            if (color < 0f)
                return 0;
            else if (color > 255f)
                return 255;
            else
                return (int)color;
        }

        private void LearnRGB(object sender, RoutedEventArgs e)
        {
            ImageBrush w = (ImageBrush)canvasImg3.Background;
            System.Drawing.Bitmap bitmap = BitmapImage2Bitmap((BitmapImage)w.ImageSource);
            NeuralNetwork net = new NeuralNetwork(new int[] { 2, 32, 32, 32, 32, 32, 3 });
            for (int k = 0; k < 5000000; k++)
            {
                int i = r.Next((int)canvasImg3.Width);
                int j = r.Next((int)canvasImg3.Height);

                net.FeedForward(new float[] { i/ (float)canvasImg3.Width, j/ (float)canvasImg3.Height });
                net.BackProp(new float[] { GetColor(bitmap, 1, i, j), GetColor(bitmap, 2, i, j), GetColor(bitmap, 3, i, j) });

                if(k%1000 == 0)
                Console.WriteLine(k);

            } 

            System.Drawing.Bitmap b = BitmapImage2Bitmap((BitmapImage)brush.ImageSource);
            
            for (int i = 0; i < canvasImg3.Width; i++)
            {
                for (int j = 0; j < canvasImg3.Height; j++)
                {
                    System.Drawing.Color myRgbColor = new System.Drawing.Color();
                    myRgbColor = System.Drawing.Color.FromArgb(255, GetOutputRgb((net.FeedForward(new float[] { i/ (float)canvasImg3.Width, j/ (float)canvasImg3.Height })[0]-0.1f)/0.8f * 255), GetOutputRgb((net.FeedForward(new float[] { i / (float)canvasImg3.Width, j / (float)canvasImg3.Height })[1] - 0.1f) / 0.8f * 255), GetOutputRgb((net.FeedForward(new float[] { i / (float)canvasImg3.Width, j / (float)canvasImg3.Height })[2] - 0.1f) / 0.8f * 255));
                    b.SetPixel(i, j, myRgbColor);
                }
            }

            MemoryStream Ms = new MemoryStream();
            System.Drawing.Bitmap ObjBitmap = null;
            ObjBitmap = b;
            ObjBitmap.Save(Ms, System.Drawing.Imaging.ImageFormat.Bmp);
            //Ms.Position = 0;
            BitmapImage ObjBitmapImage = new BitmapImage();
            ObjBitmapImage.BeginInit();
            ObjBitmapImage.StreamSource = Ms;
            ObjBitmapImage.EndInit();
            ImageBrush brushB = new ImageBrush();
            brushB.ImageSource = ObjBitmapImage;
            canvasImgResult3.Background = brushB;
        }




        public Graph<Tuple<float, float, float>> CreateGraph(int verticesNumber)
        {
            if (verticesNumber > 0)
            {
                Random rdn = new Random();
                int _numbersOfVertices = verticesNumber;
                Graph<Tuple<float, float, float>> graph = new Graph<Tuple<float, float, float>>(_numbersOfVertices);

                for (int i = 0; i < _numbersOfVertices; i++)
                {
                    float colorR = (float)rdn.NextDouble() * 255;
                    float colorG = (float)rdn.NextDouble() * 255;
                    float colorB = (float)rdn.NextDouble() * 255;
                    Tuple<float, float, float> rgb = new Tuple<float, float, float>(colorR, colorG, colorB);

                    graph.AddVertex(rgb);
                }

                if (verticesNumber == 2)
                {
                    graph.Edges[0, 1] = true;
                    graph.Edges[1, 0] = true;
                }
                if (verticesNumber > 2)
                {
                    for (int k = 0; k < _numbersOfVertices; k++)
                    {
                        int numbersOfEdgesVertex = rdn.Next(1, _numbersOfVertices);
                        for (int l = 0; l < numbersOfEdgesVertex; l++)
                        {
                            int vertexIndex = rdn.Next(_numbersOfVertices - 1);
                            while (vertexIndex == k)
                            {
                                vertexIndex = rdn.Next(_numbersOfVertices - 1);
                            }

                            graph.Edges[k, vertexIndex] = true;
                            graph.Edges[vertexIndex, k] = true;
                        }
                    }
                }

                for (int g = 0; g < _numbersOfVertices; g++)
                {
                    for (int h = 0; h < _numbersOfVertices; h++)
                    {
                        if (graph.Edges[g, h] == false)
                            Console.Write(string.Format("{0} ", 0));
                        else if (graph.Edges[g, h] == true)
                            Console.Write(string.Format("{0} ", 1));
                    }
                    Console.Write(Environment.NewLine);
                }

                Console.Write(Environment.NewLine);

                return graph;
            }
            else return null;
        }

        private void CreateGraph(object sender, RoutedEventArgs e)
        {
            try
            {
                int result = Int32.Parse(inputVertices.Text);
                _graph = CreateGraph(result);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{inputVertices.Text}'");
            }      
        }

        private void LearnKohonen(object sender, RoutedEventArgs e)
        {
            if (_graph != null)
            {
                ImageBrush w = (ImageBrush)canvasImg4.Background;
                System.Drawing.Bitmap bitmap = BitmapImage2Bitmap((BitmapImage)w.ImageSource);
                Kohonen k = new Kohonen();
                k.LearnKohonen(_graph, bitmap, canvasImg4.Width, canvasImg4.Height, 10000);



                System.Drawing.Bitmap b = BitmapImage2Bitmap((BitmapImage)brush.ImageSource);

                for (int i = 0; i < canvasImg4.Width; i++)
                {
                    for (int j = 0; j < canvasImg4.Height; j++)
                    {
                        System.Drawing.Color myRgbColor = new System.Drawing.Color();
                        //_color = new Tuple<float, float, float>(k.GetRGB(bitmap, i, j).Item1, k.GetRGB(bitmap, i, j).Item2, k.GetRGB(bitmap, i, j).Item3);

                        myRgbColor = System.Drawing.Color.FromArgb(255, GetOutputRgb(k.GetRGB(bitmap, i, j).Item1), GetOutputRgb(k.GetRGB(bitmap, i, j).Item2), GetOutputRgb(k.GetRGB(bitmap, i, j).Item3));
                        b.SetPixel(i, j, myRgbColor);
                    }
                }

                MemoryStream Ms = new MemoryStream();
                System.Drawing.Bitmap ObjBitmap = null;
                ObjBitmap = b;
                ObjBitmap.Save(Ms, System.Drawing.Imaging.ImageFormat.Bmp);

                BitmapImage ObjBitmapImage = new BitmapImage();
                ObjBitmapImage.BeginInit();
                ObjBitmapImage.StreamSource = Ms;
                ObjBitmapImage.EndInit();
                ImageBrush brushB = new ImageBrush();
                brushB.ImageSource = ObjBitmapImage;
                canvasImgResult4.Background = brushB;
            }
        }
    }

    public static class ForEachExtensions
    {
        public static void ForEachWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> handler)
        {
            int idx = 0;
            foreach (T item in enumerable)
                handler(item, idx++);
        }
    }
}
