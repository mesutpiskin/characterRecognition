using NeuronDotNet.Core;
using NeuronDotNet.Core.Backpropagation;
using System;
using System.Drawing;
using System.Windows.Forms;
/*
    Geliştirici: Mesut Pişkin
    İletişim: mesutpiskin@outlook.com - www.mesutpiskin.com
    Üçüncü Parti Kütüphane: https://www.nuget.org/packages/NeuronDotNet/ 
    14.05.2017
*/
namespace ANN
{
    public partial class WFAnnRecognition : Form
    {

        #region Properties
        private static BackpropagationNetwork network;
        private LinearLayer inputLayer;
        private SigmoidLayer hiddenLayer;
        private SigmoidLayer outputLayer;
        #endregion

        #region Events


        public WFAnnRecognition()
        {
            InitializeComponent();
        }

        private void btnMatris_Click(object sender, EventArgs e)
        {

            if (((Button)sender).BackColor == Color.Black)
            {
                ((Button)sender).BackColor = SystemColors.Control;
                return;
            }
              ((Button)sender).BackColor = Color.Black;
        }

        private void btnMatris_DragOver(object sender, DragEventArgs e)
        {
            btnMatris_Click(sender, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (Button item in panelMatrisContainer.Controls)
            {
                if (item is Button)
                {
                    item.BackColor = SystemColors.Control;
                }
            }
        }

        private void btnTrain_Click(object sender, EventArgs e)
        {

            TrainingSet trainingSet = new TrainingSet(35, 5);
            //Load dataset                       //INPUT                    //OUTPUT
            trainingSet.Add(new TrainingSample(MyDataSet.A, new double[5] { 1, 0, 0, 0, 0 }));
            lstLog.Items.Insert(0, "Trained: A");
            trainingSet.Add(new TrainingSample(MyDataSet.B, new double[5] { 0, 1, 0, 0, 0 }));
            lstLog.Items.Insert(0, "Trained: B");
            trainingSet.Add(new TrainingSample(MyDataSet.C, new double[5] { 0, 0, 1, 0, 0 }));
            lstLog.Items.Insert(0, "Trained: C");
            trainingSet.Add(new TrainingSample(MyDataSet.D, new double[5] { 0, 0, 0, 1, 0 }));
            lstLog.Items.Insert(0, "Trained: D");
            trainingSet.Add(new TrainingSample(MyDataSet.E, new double[5] { 0, 0, 0, 0, 1 }));
            lstLog.Items.Insert(0, "Trained: E");
            network.SetLearningRate(Convert.ToDouble(txtLearningRate.Text));
            network.Learn(trainingSet, Convert.ToInt32(txtIteration.Text));
            lstLog.Items.Insert(0, "Training Completed");
            //UI
            txtLearningRate.Enabled = false;
            txtIteration.Enabled = false;

            lblError.Text += network.MeanSquaredError.ToString();
            txtLearning.Text += network.InputLayer.LearningRate.ToString();
            btnTrain.Enabled = false;
            btnClassify.Enabled = true;
        }

        private void WFAnnRecognition_Load(object sender, EventArgs e)
        {
            lstLog.Items.Insert(0, "Initialize ANN model");
            inputLayer = new LinearLayer(35);
            hiddenLayer = new SigmoidLayer(3);
            outputLayer = new SigmoidLayer(5);
            BackpropagationConnector connector = new BackpropagationConnector(inputLayer, hiddenLayer);
            BackpropagationConnector connector2 = new BackpropagationConnector(hiddenLayer, outputLayer);
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.Initialize();

        }

        private void btnClassify_Click(object sender, EventArgs e)
        {
            if (listBoxOutputs.Items.Count > 0)
            {
                listBoxOutputs.Items.Clear();
                txtMatris.Clear();
            }


            double[] inputMatris = getInputs(); // Get drawing matrix
            lstLog.Items.Insert(0, "Read imput matrix");
            #region classify
            double[] output = network.Run(inputMatris);
            int index = 1;
            foreach (double item in output)
            {
                listBoxOutputs.Items.Add(index + "-" + item.ToString() + "\n");
                index++;
            }
            lstLog.Items.Insert(0, "Classification Completed!");
            #endregion

            #region Print to text
            index = 1;
            foreach (var item in inputMatris)
            {

                txtMatris.Text += (item.ToString() + "    ");
                if (index % 5 == 0)
                {
                    txtMatris.Text += "\n";
                }
                index++;
            }
            txtMatris.SelectAll();
            txtMatris.SelectionAlignment = HorizontalAlignment.Center;
            txtMatris.DeselectAll();
            #endregion
        }
        #endregion

        #region Methods
        private double[] getInputs()
        {
            double[,] inputs =
            {
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0}
            };

            foreach (Control item in panelMatrisContainer.Controls)
            {

                if (item is Button)
                {
                    int index = Convert.ToInt32(((Button)item).Tag.ToString());
                    if (item.BackColor == Color.Black)
                    {

                        if (index <= 4)
                        {
                            inputs[0, index] = 1;
                        }
                        else if (index <= 9)
                        {
                            inputs[1, index % 5] = 1;
                        }
                        else if (index <= 14)
                        {
                            inputs[2, index % 5] = 1;
                        }
                        else if (index <= 19)
                        {
                            inputs[3, index % 5] = 1;
                        }
                        else if (index <= 24)
                        {
                            inputs[4, index % 5] = 1;
                        }
                        else if (index <= 29)
                        {
                            inputs[5, index % 5] = 1;
                        }
                        else if (index <= 34)
                        {
                            inputs[6, index % 5] = 1;
                        }

                    }
                    index++;
                }

            }
            double[] oResult = new double[35];
            Buffer.BlockCopy(inputs, 0, oResult, 0, 35 * sizeof(double));
            return oResult;

        }
        #endregion

        #region Class
        private static class MyDataSet
        {
            #region PATTERN
            public static double[] A =
              {
                 1, 1, 1, 1, 1,
                 1, 0, 0, 0, 1,
                 1, 0, 0, 0, 1,
                 1, 1, 1, 1, 1,
                 1, 0, 0, 0, 1,
                 1, 0, 0, 0, 1,
                 1, 0, 0, 0, 1
            };
            public static double[] B =
              {
                 1, 1, 1, 1, 1,
                 1, 0, 0, 0, 1,
                 1, 0, 0, 0, 1,
                 1, 1, 1, 1, 1,
                 1, 0, 0, 0, 1,
                 1, 0, 0, 0, 1,
                 1, 1, 1, 1, 1
            };
            public static double[] C =
            {
                 1, 1, 1, 1, 1,
                 1, 0, 0, 0, 0,
                 1, 0, 0, 0, 0,
                 1, 0, 0, 0, 0,
                 1, 0, 0, 0, 0,
                 1, 0, 0, 0, 0,
                 1, 1, 1, 1, 1
            };
            public static double[] D =
            {
                1, 1, 1, 1, 0,
                1, 0, 0, 0, 1,
                1, 0, 0, 0, 1,
                1, 0, 0, 0, 1,
                1, 0, 0, 0, 1,
                1, 0, 0, 0, 1,
                1, 1, 1, 1, 0
            };
            public static double[] E =
                {
                 1, 1, 1, 1, 1,
                 1, 0, 0, 0, 0,
                 1, 0, 0, 0, 0,
                 1, 1, 1, 1, 1,
                 1, 0, 0, 0, 0,
                 1, 0, 0, 0, 0,
                 1, 1, 1, 1, 1
            };
            #endregion

        }
        #endregion

    }
}
