using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NetworkRouting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void clearAll()
        {
            startNodeIndex = -1;
            stopNodeIndex = -1;
            sourceNodeBox.Clear();
            sourceNodeBox.Refresh();
            targetNodeBox.Clear();
            targetNodeBox.Refresh();
            arrayTimeBox.Clear();
            arrayTimeBox.Refresh();
            heapTimeBox.Clear();
            heapTimeBox.Refresh();
            differenceBox.Clear();
            differenceBox.Refresh();
            pathCostBox.Clear();
            pathCostBox.Refresh();
            arrayCheckBox.Checked = false;
            arrayCheckBox.Refresh();
            return;
        }

        private void clearSome()
        {
            arrayTimeBox.Clear();
            arrayTimeBox.Refresh();
            heapTimeBox.Clear();
            heapTimeBox.Refresh();
            differenceBox.Clear();
            differenceBox.Refresh();
            pathCostBox.Clear();
            pathCostBox.Refresh();
            return;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            int randomSeed = int.Parse(randomSeedBox.Text);
            int size = int.Parse(sizeBox.Text);

            Random rand = new Random(randomSeed);
            seedUsedLabel.Text = "Random Seed Used: " + randomSeed.ToString();

            clearAll();
            this.adjacencyList = generateAdjacencyList(size, rand);
            List<PointF> points = generatePoints(size, rand);
            resetImageToPoints(points);
            this.points = points;
        }

        // Generates the distance matrix.  Values of -1 indicate a missing edge.  Loopbacks are at a cost of 0.
        private const int MIN_WEIGHT = 1;
        private const int MAX_WEIGHT = 100;
        private const double PROBABILITY_OF_DELETION = 0.35;

        private const int NUMBER_OF_ADJACENT_POINTS = 3;

        private List<HashSet<int>> generateAdjacencyList(int size, Random rand)
        {
            List<HashSet<int>> adjacencyList = new List<HashSet<int>>();

            for (int i = 0; i < size; i++)
            {
                HashSet<int> adjacentPoints = new HashSet<int>();
                while (adjacentPoints.Count < 3)
                {
                    int point = rand.Next(size);
                    if (point != i) adjacentPoints.Add(point);
                }
                adjacencyList.Add(adjacentPoints);
            }

            return adjacencyList;
        }

        private List<PointF> generatePoints(int size, Random rand)
        {
            List<PointF> points = new List<PointF>();
            for (int i = 0; i < size; i++)
            {
                points.Add(new PointF((float) (rand.NextDouble() * pictureBox.Width), (float) (rand.NextDouble() * pictureBox.Height)));
            }
            return points;
        }

        private void resetImageToPoints(List<PointF> points)
        {
            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics graphics = Graphics.FromImage(pictureBox.Image);
            Pen pen;

            if (points.Count < 100)
                pen = new Pen(Color.Blue);
            else
                pen = new Pen(Color.LightBlue);
            foreach (PointF point in points)
            {
                graphics.DrawEllipse(pen, point.X, point.Y, 2, 2);
            }

            this.graphics = graphics;
            pictureBox.Invalidate();
        }

        // These variables are instantiated after the "Generate" button is clicked
        private List<PointF> points = new List<PointF>();
        private Graphics graphics;
        private List<HashSet<int>> adjacencyList;

        // Use this to generate paths (from start) to every node; then, just return the path of interest from start node to end node
        private void solveButton_Click(object sender, EventArgs e)
        {
            // This was the old entry point, but now it is just some form interface handling
            bool ready = true;

            if(startNodeIndex == -1)
            {
                sourceNodeBox.Focus();
                sourceNodeBox.BackColor = Color.Red;
                ready = false;
            }
            if(stopNodeIndex == -1)
            {
                if(!sourceNodeBox.Focused)
                    targetNodeBox.Focus();
                targetNodeBox.BackColor = Color.Red;
                ready = false;
            }
            if (points.Count > 0)
            {
                resetImageToPoints(points);
                paintStartStopPoints();
            }
            else
            {
                ready = false;
            }
            if(ready)
            {
                clearSome();
                solveButton_Clicked();  // Here is the new entry point
            }
        }

        private void solveButton_Clicked()
        {
            // *** Implement this method, use the variables "startNodeIndex" and "stopNodeIndex" as the indices for your start and stop points, respectively ***

            Stopwatch stopwatch = new Stopwatch();
            double arrayTime, heapTime;
            int currentPoint;

            // Dijkstra's for the Heap Que
            // Total Space Complexity O(n)
            // Total Time Complexity O(nlogn)
            HeapQue heapQue = new HeapQue(points);                                                          //Space O(n)  Time O(n)
            stopwatch.Start();
            heapQue.decreaseKey(startNodeIndex, 0);                                                         //Time O(logn)
            while (!heapQue.isEmpty())                                                                      //Time O(n)
            {
                int min = heapQue.deleteMin();                                                              //Time O(logn)
                foreach (int adjNode in adjacencyList[min])
                {
                    double dist = heapQue.getDistance(min) + distance(points[min], points[adjNode]);
                    if (heapQue.getDistance(adjNode) > dist)
                    {
                        heapQue.decreaseKey(adjNode, dist);                                                 //Time O(logn)
                        heapQue.setPrev(adjNode, min);
                    }
                }
            }
            stopwatch.Stop();
            heapTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            heapTimeBox.Text = (heapTime / 1000).ToString();
            if(heapQue.getDistance(stopNodeIndex) == Double.MaxValue)
                pathCostBox.Text = "Unreachable";
            else
                pathCostBox.Text = heapQue.getDistance(stopNodeIndex).ToString();

            // Draw path for heap
            currentPoint = stopNodeIndex;
            while (heapQue.getPrev(currentPoint) != -1)
            {
                graphics.DrawLine(new Pen(Color.Blue, 2), points[currentPoint], points[heapQue.getPrev(currentPoint)]);
                float newX = (points[currentPoint].X + points[heapQue.getPrev(currentPoint)].X) / 2;
                float newY = (points[currentPoint].Y + points[heapQue.getPrev(currentPoint)].Y) / 2;
                graphics.DrawString("" + (int)(heapQue.getDistance(currentPoint) - heapQue.getDistance(heapQue.getPrev(currentPoint))), new Font(this.Font, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(newX, newY));
                currentPoint = heapQue.getPrev(currentPoint);
            }

            // Dijkstra's for the Array Que
            // Total Space Complexity O(n)
            // Total Time Complexity O(n^2)
            if (arrayCheckBox.Checked == true)
            {
                ArrayQue arrayQue = new ArrayQue(points);                                                   //Space O(n) Time O(n)
                stopwatch.Start();
                arrayQue.decreaseKey(startNodeIndex, 0);                                                    //Time O(1)
                while (!arrayQue.empty)                                                                     //Time O(n)
                {
                    int min = arrayQue.deleteMin();                                                         //Time O(n)
                    foreach (int adjNode in adjacencyList[min])
                    { 
                        double dist = arrayQue.getDistance(min) + distance(points[min], points[adjNode]);
                        if (arrayQue.getDistance(adjNode) > dist)
                        {
                            arrayQue.decreaseKey(adjNode, dist);                                            //Time O(1)
                            arrayQue.setPrev(adjNode, min);
                        }
                    }
                }
                stopwatch.Stop();
                arrayTime = stopwatch.ElapsedMilliseconds;
                arrayTimeBox.Text = (arrayTime / 1000).ToString();
                
                // Draw path for array
                currentPoint = stopNodeIndex;
                while (arrayQue.getPrev(currentPoint) != -1)
                {       
                    graphics.DrawLine(new Pen(Color.Red, 2), points[currentPoint], points[arrayQue.getPrev(currentPoint)]);
                    float newX = (points[currentPoint].X + points[arrayQue.getPrev(currentPoint)].X) / 2;
                    float newY = (points[currentPoint].Y + points[arrayQue.getPrev(currentPoint)].Y) / 2;
                    graphics.DrawString("" + (int)(arrayQue.getDistance(currentPoint) - arrayQue.getDistance(arrayQue.getPrev(currentPoint))), new Font(this.Font, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(newX, newY));
                    currentPoint = arrayQue.getPrev(currentPoint);
                }
                differenceBox.Text = (arrayTime / heapTime).ToString();
            }
        }

        private double distance(PointF point1, PointF point2)
        {
            double result = Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
            return result;
        }

        private Boolean startStopToggle = true;
        private int startNodeIndex = -1;
        private int stopNodeIndex = -1;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (points.Count > 0)
            {
                Point mouseDownLocation = new Point(e.X, e.Y);
                int index = ClosestPoint(points, mouseDownLocation);
                if (startStopToggle)
                {
                    startNodeIndex = index;
                    sourceNodeBox.ResetBackColor();
                    sourceNodeBox.Text = "" + index;
                }
                else
                {
                    stopNodeIndex = index;
                    targetNodeBox.ResetBackColor();
                    targetNodeBox.Text = "" + index;
                }
                resetImageToPoints(points);
                paintStartStopPoints();
            }
        }

        private void sourceNodeBox_Changed(object sender, EventArgs e)
        {
            if (points.Count > 0)
            {
                try{ startNodeIndex = int.Parse(sourceNodeBox.Text); }
                catch { startNodeIndex = -1; }
                if (startNodeIndex < 0 | startNodeIndex > points.Count-1)
                    startNodeIndex = -1;
                if(startNodeIndex != -1)
                {
                    sourceNodeBox.ResetBackColor();
                    resetImageToPoints(points);
                    paintStartStopPoints();
                    startStopToggle = !startStopToggle;
                }
            }
        }

        private void targetNodeBox_Changed(object sender, EventArgs e)
        {
            if (points.Count > 0)
            {
                try { stopNodeIndex = int.Parse(targetNodeBox.Text); }
                catch { stopNodeIndex = -1; }
                if (stopNodeIndex < 0 | stopNodeIndex > points.Count-1)
                    stopNodeIndex = -1;
                if(stopNodeIndex != -1)
                {
                    targetNodeBox.ResetBackColor();
                    resetImageToPoints(points);
                    paintStartStopPoints();
                    startStopToggle = !startStopToggle;
                }
            }
        }
        
        private void paintStartStopPoints()
        {
            if (startNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Green, 6), points[startNodeIndex].X, points[startNodeIndex].Y, 1, 1);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }

            if (stopNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Red, 2), points[stopNodeIndex].X - 3, points[stopNodeIndex].Y - 3, 8, 8);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }
        }

        private int ClosestPoint(List<PointF> points, Point mouseDownLocation)
        {
            double minDist = double.MaxValue;
            int minIndex = 0;

            for (int i = 0; i < points.Count; i++)
            {
                double dist = Math.Sqrt(Math.Pow(points[i].X-mouseDownLocation.X,2) + Math.Pow(points[i].Y - mouseDownLocation.Y,2));
                if (dist < minDist)
                {
                    minIndex = i;
                    minDist = dist;
                }
            }

            return minIndex;
        }
    }

    /// <summary>
    /// Class for the Array Implementation
    /// </summary>
    class ArrayQue
    {
        public bool empty;
        private double[] dist;
        private int[] prev;
        private bool[] visited;
        private int deleteCount;

        public ArrayQue(List<PointF> points)
        {
            // Make Que
            // Space O(n)
            // Time O(n)
            this.empty = false;
            deleteCount = 0;
            dist = new double[points.Count];
            prev = new int[points.Count];
            visited = new bool[points.Count];
            for(int i = 0; i < points.Count; i++)
            {
                dist[i] = Double.MaxValue;
                prev[i] = -1;
                visited[i] = false;
            }
        }

        public int deleteMin()
        {
            // Space O(1)
            // Time O(n)
            double min = Double.MaxValue;
            int minIndex = -1;
            for(int i = 0; i < dist.Length; i++)
            {
                if (dist[i] >= 0 && dist[i] <= min && !visited[i])
                {
                    min = dist[i];
                    minIndex = i; 
                }
            }
            deleteCount++;
            if(deleteCount == dist.Length)
            {
                this.empty = true;
            }
            visited[minIndex] = true;
            return minIndex;
        }

        public void decreaseKey(int index, double value)
        {
            // Space O(1)
            // Time O(1)
            dist[index] = value;
        }

        public double getDistance(int index)
        {
            // Space O(1)
            // Time O(1)
            return dist[index];
        }       

        public int getPrev(int index)
        {
            // Space O(1)
            // Time O(1)
            return prev[index];
        }

        public void setPrev(int index, int value)
        {
            // Space O(1)
            // Time O(1)
            prev[index] = value;
        }
    }

    /// <summary>
    /// Class for the Heap implementation
    /// </summary>
    class HeapQue
    {
        private double[] dist;
        private int[] heap;
        private int[] pointer;
        private int[] prev;
        private int endIndex;

        public HeapQue(List<PointF> points)
        {
            // Make Que
            // Size O(n)
            // Time O(n)
            dist = new double[points.Count];
            pointer = new int[points.Count];
            heap = new int[points.Count];
            prev = new int[points.Count];
            endIndex = points.Count - 1;

            for (int i = 0; i < points.Count; i++)
            {
                dist[i] = Double.MaxValue;
                heap[i] = i;
                pointer[i] = i;
                prev[i] = -1;
            }
        }

        public int deleteMin()
        {
            // Space O(1)
            // Time O(logn)
            int minIndex = heap[0];
            pointer[heap[0]] = -1;
            heap[0] = heap[endIndex];
            pointer[heap[0]] = 0;
            heap[endIndex] = -1;
            endIndex--;

            int currentIndex = 0;
            while(currentIndex < endIndex)
            {
                int child1 = (currentIndex + 1) * 2;
                int child2 = ((currentIndex + 1) * 2) - 1;

                if (child1 >= heap.Length || heap[child1] == -1)
                    break;
                
                if ((child1 < heap.Length && child2 >= heap.Length) || (child1 < heap.Length && heap[child2] == -1))
                {
                    if (dist[heap[currentIndex]] > dist[heap[child1]])
                    {
                        int temp = heap[currentIndex];
                        heap[currentIndex] = heap[child1];
                        pointer[heap[currentIndex]] = currentIndex;
                        heap[child1] = temp;
                        pointer[heap[child1]] = child1;
                    }
                    break;
                }
     
                if (dist[heap[currentIndex]] > dist[heap[child1]] || dist[heap[currentIndex]] > dist[heap[child2]])
                {
                    if (dist[heap[child1]] < dist[heap[child2]])
                    {
                        int temp = heap[currentIndex];
                        heap[currentIndex] = heap[child1];
                        pointer[heap[currentIndex]] = currentIndex;
                        heap[child1] = temp;
                        pointer[heap[child1]] = child1;
                        currentIndex = child1;
                    }
                    else
                    {
                        int temp = heap[currentIndex];
                        heap[currentIndex] = heap[child2];
                        pointer[heap[currentIndex]] = currentIndex;
                        heap[child2] = temp;
                        pointer[heap[child2]] = child2;
                        currentIndex = child2;
                    }
                }
                else
                    break;
            }
            return minIndex;
        }

        public void decreaseKey(int index, double value)
        {
            // Space O(1)
            // Time O(1)
            int inHeap = pointer[index];
            dist[index] = value;

            int currentIndex = inHeap;
            while(currentIndex > -1)
            {
                int parent = (currentIndex - 1) / 2;
                if(dist[heap[currentIndex]] < dist[heap[parent]])
                {
                    int temp = heap[currentIndex];
                    heap[currentIndex] = heap[parent];
                    pointer[heap[currentIndex]] = currentIndex;
                    heap[parent] = temp;
                    pointer[heap[parent]] = parent;
                    currentIndex = parent;
                }
                else
                {
                    break;
                }
            }
        }

        public bool isEmpty()
        {
            // Space O(1)
            // Time O(1)
            if(endIndex == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double getDistance(int index)
        {
            // Space O(1)
            // Time O(1)
            return dist[index];
        }

        public void setPrev(int index, int value)
        {
            // Space O(1)
            // Time O(1)
            prev[index] = value;
        }

        public int getPrev(int index)
        {
            // Space O(1)
            // Time O(1)
            return prev[index];
        }
    }
}
