using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace _2_convex_hull
{
    class ConvexHullSolver
    {
        System.Drawing.Graphics g;
        System.Windows.Forms.PictureBox pictureBoxView;

        public ConvexHullSolver(System.Drawing.Graphics g, System.Windows.Forms.PictureBox pictureBoxView)
        {
            this.g = g;
            this.pictureBoxView = pictureBoxView;
        }

        public void Refresh()
        {
            // Use this especially for debugging and whenever you want to see what you have drawn so far
            pictureBoxView.Refresh();
        }

        public void Pause(int milliseconds)
        {
            // Use this especially for debugging and to animate your algorithm slowly
            pictureBoxView.Refresh();
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void Solve(List<System.Drawing.PointF> pointList)
        {
            pointList.Sort(new Comparor());

            ConvexHull convexHull = computeConvexHull(pointList);
            List<PointF> points = convexHull.getPoints();
            for (int i = 1; i < points.Count; i++)
            {
                g.DrawLine(new Pen(Color.Red, 3), points[i - 1], points[i]);
                Pause(10);
            }
            g.DrawLine(new Pen(Color.Red, 3), points[points.Count - 1], points[0]);
            Console.WriteLine("Done");
        }

        private ConvexHull computeConvexHull(List<PointF> pointList)
        {
            //Console.WriteLine("Compute Convex size: " + pointList.Count);
            if(pointList.Count == 1)
            {
                return new ConvexHull(pointList[0]);
            }
            
            int halfWay = (pointList.Count) / 2;
            List<PointF> leftHalf = pointList.GetRange(0, halfWay);
            List<PointF> rightHalf = pointList.GetRange(halfWay, pointList.Count - halfWay);

            ConvexHull leftHull = computeConvexHull(leftHalf);
            ConvexHull rightHull = computeConvexHull(rightHalf);

            //combine the left and right hulls
            findUT(leftHull, rightHull);
            findBT(leftHull, rightHull);

            ConvexHull newHull = new ConvexHull(leftHull.getUpperTangent());
            PointF nextPoint = leftHull.getUpperTangent();
            while(nextPoint != leftHull.getBottomTangent())
            {
                nextPoint = leftHull.getCounterWise(nextPoint);
                newHull.add(nextPoint);
            }
            nextPoint = rightHull.getBottomTangent();
            newHull.add(nextPoint);
            while(nextPoint != rightHull.getUpperTangent())
            {
                nextPoint = rightHull.getCounterWise(nextPoint);
                newHull.add(nextPoint);
            }
            return newHull;
        }

        private void findUT(ConvexHull leftHull, ConvexHull rightHull)
        {
            //Console.WriteLine("FindUT");
            PointF leftTangent = leftHull.getRightMost();
            PointF rightTangent = rightHull.getLeftMost();
            double slope = (leftTangent.Y - rightTangent.Y) / (leftTangent.X - rightTangent.X);

            bool done = false;
            while (!done)
            {
                //Console.WriteLine("While");
                done = true;
                double nextSlope;
                do
                {
                    //Console.WriteLine("Do 1 Upper");
                    PointF rightNext = rightHull.getClockWise(rightTangent);
                    nextSlope = (leftTangent.Y - rightNext.Y) / (leftTangent.X - rightNext.X);
                    if (nextSlope < slope)
                    {
                        slope = nextSlope;
                        rightTangent = rightNext;
                        done = false;
                    }
                    if(rightNext == rightHull.getClockWise(rightTangent))
                    {
                        break;
                    }
                } while (slope == nextSlope);
                do
                {
                    //Console.WriteLine("Do 2 Upper");
                    PointF leftNext = leftHull.getCounterWise(leftTangent);
                    nextSlope = (rightTangent.Y - leftNext.Y) / (rightTangent.X - leftNext.X);
                    if (nextSlope > slope)
                    {
                        slope = nextSlope;
                        leftTangent = leftNext;
                        done = false;
                    }
                    if (leftNext == leftHull.getCounterWise(leftTangent))
                    {
                        break;
                    }
                } while (slope == nextSlope);
            }
            leftHull.setUpperTangent(leftTangent);
            rightHull.setUpperTangent(rightTangent);
            g.DrawLine(new Pen(Color.Blue, 2), leftTangent, rightTangent);
            Pause(50);
        }

        private void findBT(ConvexHull leftHull, ConvexHull rightHull)
        {
            //Console.WriteLine("FindBT");
            PointF leftTangent = leftHull.getRightMost();
            PointF rightTangent = rightHull.getLeftMost();
            double slope = (leftTangent.Y - rightTangent.Y) / (leftTangent.X - rightTangent.X);

            bool done = false;
            while (!done)
            {
                //Console.WriteLine("While");
                done = true;
                double nextSlope;
                do
                {
                    //Console.WriteLine("Do 1");
                    PointF rightNext = rightHull.getCounterWise(rightTangent);
                    nextSlope = (leftTangent.Y - rightNext.Y) / (leftTangent.X - rightNext.X);
                    if (nextSlope > slope)
                    {
                        slope = nextSlope;
                        rightTangent = rightNext;
                        done = false;
                    }
                    if (rightNext == rightHull.getCounterWise(rightTangent))
                    {
                        break;
                    }
                } while (slope == nextSlope);
                do
                {
                    //Console.WriteLine("Do 2");
                    PointF leftNext = leftHull.getClockWise(leftTangent);
                    nextSlope = (rightTangent.Y - leftNext.Y) / (rightTangent.X - leftNext.X);
                    if (nextSlope < slope)
                    {
                        slope = nextSlope;
                        leftTangent = leftNext;
                        done = false;
                    }
                    if (leftNext == leftHull.getClockWise(leftTangent))
                    {
                        break;
                    }
                } while (slope == nextSlope);
            }
            leftHull.setBottomTangent(leftTangent);
            rightHull.setBottomTangent(rightTangent);
            g.DrawLine(new Pen(Color.Blue, 2), leftTangent, rightTangent);
            Pause(50);
        }
    }

    class Comparor : IComparer<PointF>
    {
        public int Compare(PointF x, PointF y)
        {
            if (x.X < y.X)
                return -1;
            else
                return 1;
        }
    }

    class ConvexHull 
    {
        private PointF leftMost;
        private PointF rightMost;
        private PointF upperTangent;
        private PointF bottomTangent;
        private List<PointF> points;

        public ConvexHull(PointF point)
        {
            //Console.WriteLine("Creating new Hull");
            points = new List<PointF>();
            points.Add(point);
            leftMost = point;
            rightMost = point;
        }

        public void add(PointF point)
        {
            if(point.X < leftMost.X)
            {
                leftMost = point;
            }
            else if(point.X > rightMost.X)
            {
                rightMost = point;
            }
            points.Add(point);
        }

        public PointF getCounterWise(PointF point)
        {
            int index = points.IndexOf(point) + 1;
            if(index >= points.Count)
            {
                return points[0];
            }
            return points[index];
        }

        public PointF getClockWise(PointF point)
        {
            int index = points.IndexOf(point) - 1;
            if (index < 0)
            {
                return points[points.Count - 1];
            }
            return points[index];
        }

        public PointF getLeftMost()
        {
            return leftMost;
        }

        public PointF getRightMost()
        {
            return rightMost;
        }

        public PointF getUpperTangent()
        {
            return upperTangent;
        }

        public void setUpperTangent(PointF tangent)
        {
            upperTangent = tangent;
        }

        public PointF getBottomTangent()
        {
            return bottomTangent;
        }

        public void setBottomTangent(PointF tangent)
        {
            bottomTangent = tangent;
        }

        public int size()
        {
            return points.Count;
        }

        public List<PointF> getPoints()
        {
            return points;
        }
    }
}
