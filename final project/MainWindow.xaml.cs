using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphSegmentationWPF
{
    public partial class MainWindow : Window
    {
        private List<Node> nodesList = new List<Node>();
        private List<Edge> edgesList = new List<Edge>();
        private Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateGraph_Click(object sender, RoutedEventArgs e)
        {
            ClearGraph();

            if (!int.TryParse(txtNumVertices.Text, out int numVertices) || numVertices < 1)
            {
                MessageBox.Show("Invalid number of vertices. Please enter a positive integer.");
                return;
            }

            AddGraphElements(numVertices);
            SegmentGraphElementsSequential();
        }

        private void ClearGraph()
        {
            canvas.Children.Clear();
            nodesList.Clear();
            edgesList.Clear();
        }

        private void AddGraphElements(int numVertices)
        {
            for (int i = 0; i < numVertices; i++)
            {
                AddNode(rand.Next(20, (int)canvas.ActualWidth - 40), rand.Next(20, (int)canvas.ActualHeight - 40));
            }

            for (int i = 0; i < numVertices; i++)
            {
                for (int j = i + 1; j < numVertices; j++)
                {
                    if (rand.NextDouble() < 0.5)
                    {
                        ConnectNodes(i, j);
                    }
                }
            }
        }

        private void AddNode(double x, double y)
        {
            Ellipse node = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(node, x);
            Canvas.SetTop(node, y);
            canvas.Children.Add(node);
            nodesList.Add(new Node(node));
        }

        private void ConnectNodes(int index1, int index2)
        {
            Line edge = new Line
            {
                X1 = Canvas.GetLeft(nodesList[index1].Ellipse) + 10,
                Y1 = Canvas.GetTop(nodesList[index1].Ellipse) + 10,
                X2 = Canvas.GetLeft(nodesList[index2].Ellipse) + 10,
                Y2 = Canvas.GetTop(nodesList[index2].Ellipse) + 10,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            canvas.Children.Add(edge);
            edgesList.Add(new Edge(nodesList[index1], nodesList[index2], edge));
        }

        private void SegmentGraphElementsSequential()
        {
            foreach (var node in nodesList)
            {
                node.Degree = CalculateNodeDegree(node);
            }

            Dictionary<int, List<Node>> nodesByDegree = new Dictionary<int, List<Node>>();

            foreach (var node in nodesList)
            {
                if (!nodesByDegree.ContainsKey(node.Degree))
                {
                    nodesByDegree[node.Degree] = new List<Node>();
                }
                nodesByDegree[node.Degree].Add(node);
            }

            SolidColorBrush[] colors = { Brushes.Red, Brushes.Blue, Brushes.Yellow, Brushes.Green, Brushes.Gray };
            int colorIndex = 0;
            foreach (var degreeGroup in nodesByDegree.Values)
            {
                foreach (var node in degreeGroup)
                {
                    node.Ellipse.Fill = colors[colorIndex];
                }
                colorIndex = (colorIndex + 1) % colors.Length;
            }
        }

        private int CalculateNodeDegree(Node node)
        {
            int degree = 0;
            foreach (var edge in edgesList)
            {
                if (edge.Node1 == node || edge.Node2 == node)
                {
                    degree++;
                }
            }
            return degree;
        }

        private class Node
        {
            public Ellipse Ellipse { get; }
            public int Degree { get; set; }
            public Node(Ellipse ellipse)
            {
                Ellipse = ellipse;
            }
        }

        private class Edge
        {
            public Node Node1 { get; }
            public Node Node2 { get; }
            public Line Line { get; }
            public Edge(Node node1, Node node2, Line line)
            {
                Node1 = node1;
                Node2 = node2;
                Line = line;
            }
        }
    }
}
