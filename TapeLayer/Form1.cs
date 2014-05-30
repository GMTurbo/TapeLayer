using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TapeLayer
{
    public enum OptimizeBy
    {
        CLOSEST,
        X,
        Y,
        XY
    };


    public partial class Form1 : Form
    {
        OptimizeBy opt = OptimizeBy.CLOSEST;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

        }

        List<Tape> Tapes = new List<Tape>();

        Point MouseDownPoint = Point.Empty;

        Point Anchor = Point.Empty;

        Point Anchor2 = Point.Empty;

        bool marking = false;

        private void splitContainer2_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                marking = true;
                MouseDownPoint = new Point(e.X, e.Y);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                optAnchor.Checked = false;
                Anchor = new Point(e.X, e.Y);
            }

        }

        private void splitContainer2_Panel1_MouseMove(object sender, MouseEventArgs e)
        {

            if (marking)
                MouseDownPoint = new Point(e.X, e.Y);
        }

        private void splitContainer2_Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Tape t = GetAvailableTape();

                if (t == null)
                    Tapes.Add(new Tape(MouseDownPoint));
                else
                    t.EndPoint = new Point(MouseDownPoint.X, MouseDownPoint.Y);

                MouseDownPoint = Point.Empty;
                marking = false;
            }
            splitContainer2.Panel1.Invalidate();
        }

        Tape GetAvailableTape()
        {
            if (Tapes.Count == 0)
                return null;

            for (int i = 0; i < Tapes.Count; i++)
                if (Tapes[i].EndPointAvailable)
                    return Tapes[i];

            return null;
        }

        private void Form1_Paint(object sender, PaintEventArgs e) { }

        private void Form1_Load(object sender, EventArgs e)
        {
            splitContainer2.Panel1.Paint += Panel1_Paint;
            splitContainer2.Panel2.Paint += Panel2_Paint;

        }

        Point ScaleToSecondScreen(Point original)
        {
            //splitContainer2.Panel1.Width
            return original;
        }

        //Right Pane
        void Panel2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            double[] vec = new double[2];
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 12);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            if (SortedClusters.Count > 0 && checkBox2.Checked)
            {
                for (int i = 0; i < SortedClusters.Count; i++)
                {
                    if (!ColorLookup.ContainsKey(parents[i].ID))
                        ColorLookup.Add(parents[i].ID, GetRandomColor());
                    if (SortedClusters[i].Count > 0)
                    {
                        for (int j = 0; j < SortedClusters[i].Count; j++)
                        {
                            vec = new double[] { sats[i][j].EndPoint.X - sats[i][j].StartPoint.X, sats[i][j].EndPoint.Y - sats[i][j].StartPoint.Y };
                            e.Graphics.FillEllipse(new SolidBrush(ColorLookup[parents[i].ID]), sats[i][j].StartPoint.X - 10, sats[i][j].StartPoint.Y - 10, 20, 20);
                            e.Graphics.FillEllipse(new SolidBrush(ColorLookup[parents[i].ID]), sats[i][j].EndPoint.X - 10, sats[i][j].EndPoint.Y - 10, 20, 20);
                            e.Graphics.DrawString((j + 1).ToString(), drawFont, drawBrush, (float)(sats[i][j].StartPoint.X + vec[0] / 2.0), (float)(sats[i][j].StartPoint.Y + vec[1] / 2.0));
                            e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Wheat), sats[i][j].StartPoint, sats[i][j].EndPoint);
                        }
                    }
                }

                double totalLength = 0;
                if (checkBox1.Checked)
                {
                    for (int j = 0; j < SortedClusters.Count; j++)
                    {
                        if (SortedClusters[j].Count > 0)
                        {
                            for (int i = 1; i < SortedClusters[j].Count; i++)
                            {
                                if (SortedClusters[j][i - 1].EndPoint != Point.Empty && SortedClusters[j][i].StartPoint != Point.Empty)
                                {
                                    vec = new double[] { ScaleToSecondScreen(SortedClusters[j][i].StartPoint).X - ScaleToSecondScreen(SortedClusters[j][i - 1].EndPoint).X, ScaleToSecondScreen(SortedClusters[j][i].StartPoint).Y - ScaleToSecondScreen(SortedClusters[j][i - 1].EndPoint).Y };
                                    e.Graphics.DrawString(GetLength(SortedClusters[j][i - 1].EndPoint, SortedClusters[j][i].StartPoint).ToString("#0.00"), drawFont, drawBrush, (float)(ScaleToSecondScreen(SortedClusters[j][i - 1].EndPoint).X + vec[0] / 2.0), (float)(ScaleToSecondScreen(SortedClusters[j][i - 1].EndPoint).Y + vec[1] / 2.0));
                                    e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Pink), ScaleToSecondScreen(SortedClusters[j][i].StartPoint), ScaleToSecondScreen(SortedClusters[j][i - 1].EndPoint));
                                    totalLength += GetLength(SortedClusters[j][i - 1].EndPoint, SortedClusters[j][i].StartPoint);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < SortedClusters.Count; j++)
                    {
                        if (SortedClusters[j].Count > 0)
                        {
                            for (int i = 1; i < SortedClusters[j].Count; i++)
                            {
                                if (SortedClusters[j][i - 1].EndPoint != Point.Empty && SortedClusters[j][i].StartPoint != Point.Empty)
                                    totalLength += GetLength(SortedClusters[j][i - 1].EndPoint, SortedClusters[j][i].StartPoint);
                            }
                        }
                    }
                }

                if (Anchor2 != Point.Empty)
                    e.Graphics.FillEllipse(Brushes.RoyalBlue, new Rectangle(Anchor, new Size(15, 15)));

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                SortedDistanceLabel.Text = "Total Travel Distance: " + totalLength.ToString("#0.00");
                drawFont.Dispose();
                drawBrush.Dispose();
            }
            else if (sorted.Count > 0)
            {
                sorted.ForEach(tape =>
                {
                    e.Graphics.FillEllipse(Brushes.Lime, ScaleToSecondScreen(tape.StartPoint).X - 10, ScaleToSecondScreen(tape.StartPoint).Y - 10, 20, 20);
                    if (tape.EndPoint != Point.Empty)
                    {
                        vec = new double[] { ScaleToSecondScreen(tape.EndPoint).X - ScaleToSecondScreen(tape.StartPoint).X, ScaleToSecondScreen(tape.EndPoint).Y - ScaleToSecondScreen(tape.StartPoint).Y };
                        e.Graphics.FillEllipse(Brushes.Red, ScaleToSecondScreen(tape.EndPoint).X - 10, ScaleToSecondScreen(tape.EndPoint).Y - 10, 20, 20);
                        e.Graphics.DrawString((sorted.IndexOf(tape) + 1).ToString(), drawFont, drawBrush, (float)(ScaleToSecondScreen(tape.StartPoint).X + vec[0] / 2.0), (float)(ScaleToSecondScreen(tape.StartPoint).Y + vec[1] / 2.0));
                        e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.White), ScaleToSecondScreen(tape.StartPoint), ScaleToSecondScreen(tape.EndPoint));
                    }
                });

                drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Lime);

                double totalLength = 0;
                if (checkBox1.Checked)
                {
                    for (int i = 1; i < sorted.Count; i++)
                    {
                        if (sorted[i - 1].EndPoint != Point.Empty && sorted[i].StartPoint != Point.Empty)
                        {
                            vec = new double[] { ScaleToSecondScreen(sorted[i].StartPoint).X - ScaleToSecondScreen(sorted[i - 1].EndPoint).X, ScaleToSecondScreen(sorted[i].StartPoint).Y - ScaleToSecondScreen(sorted[i - 1].EndPoint).Y };
                            e.Graphics.DrawString(GetLength(sorted[i - 1].EndPoint, sorted[i].StartPoint).ToString("#0.00"), drawFont, drawBrush, (float)(ScaleToSecondScreen(sorted[i - 1].EndPoint).X + vec[0] / 2.0), (float)(ScaleToSecondScreen(sorted[i - 1].EndPoint).Y + vec[1] / 2.0));
                            e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Lime), ScaleToSecondScreen(sorted[i].StartPoint), ScaleToSecondScreen(sorted[i - 1].EndPoint));
                            totalLength += GetLength(sorted[i - 1].EndPoint, sorted[i].StartPoint);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < sorted.Count; i++)
                    {
                        if (sorted[i - 1].EndPoint != Point.Empty && sorted[i].StartPoint != Point.Empty)
                            totalLength += GetLength(sorted[i - 1].EndPoint, sorted[i].StartPoint);
                    }
                }

                if (Anchor != Point.Empty)
                    e.Graphics.FillEllipse(Brushes.RoyalBlue, new Rectangle(Anchor, new Size(15, 15)));

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                SortedDistanceLabel.Text = "Total Travel Distance: " + totalLength.ToString("#0.00");
                drawFont.Dispose();
                drawBrush.Dispose();
            }
        }

        //Left Pane
        void Panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            Pen TapePen = null;

            if (Tapes.Count != 0)
            {
                double[] vec = new double[2];
                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 12);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

                if (sats.Count > 0 && checkBox2.Checked)
                {
                    for (int i = 0; i < sats.Count; i++)
                    {
                        if (!ColorLookup.ContainsKey(parents[i].ID))
                            ColorLookup.Add(parents[i].ID, GetRandomColor());
                        if (sats[i].Count > 0)
                        {
                            for (int j = 0; j < sats[i].Count; j++)
                            {
                                vec = new double[] { sats[i][j].EndPoint.X - sats[i][j].StartPoint.X, sats[i][j].EndPoint.Y - sats[i][j].StartPoint.Y };
                                e.Graphics.FillEllipse(new SolidBrush(ColorLookup[parents[i].ID]), sats[i][j].StartPoint.X - 10, sats[i][j].StartPoint.Y - 10, 20, 20);
                                e.Graphics.FillEllipse(new SolidBrush(ColorLookup[parents[i].ID]), sats[i][j].EndPoint.X - 10, sats[i][j].EndPoint.Y - 10, 20, 20);
                                e.Graphics.DrawString((Tapes.IndexOf(sats[i][j]) + 1).ToString(), drawFont, drawBrush, (float)(sats[i][j].StartPoint.X + vec[0] / 2.0), (float)(sats[i][j].StartPoint.Y + vec[1] / 2.0));
                                e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.White), sats[i][j].StartPoint, sats[i][j].EndPoint);
                            }
                        }
                    }
                }
                else
                {
                    Tapes.ForEach(tape =>
                    {
                        e.Graphics.FillEllipse(Brushes.Lime, tape.StartPoint.X - 10, tape.StartPoint.Y - 10, 20, 20);
                        if (tape.EndPoint != Point.Empty)
                        {
                            vec = new double[] { tape.EndPoint.X - tape.StartPoint.X, tape.EndPoint.Y - tape.StartPoint.Y };
                            e.Graphics.FillEllipse(Brushes.Red, tape.EndPoint.X - 10, tape.EndPoint.Y - 10, 20, 20);
                            e.Graphics.DrawString((Tapes.IndexOf(tape) + 1).ToString(), drawFont, drawBrush, (float)(tape.StartPoint.X + vec[0] / 2.0), (float)(tape.StartPoint.Y + vec[1] / 2.0));
                            e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.White), tape.StartPoint, tape.EndPoint);
                        }
                    });
                }

                drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Lime);

                double totalLength = 0;
                if (checkBox1.Checked)
                {
                    for (int i = 1; i < Tapes.Count; i++)
                    {
                        if (Tapes[i - 1].EndPoint != Point.Empty && Tapes[i].StartPoint != Point.Empty)
                        {
                            vec = new double[] { Tapes[i].StartPoint.X - Tapes[i - 1].EndPoint.X, Tapes[i].StartPoint.Y - Tapes[i - 1].EndPoint.Y };
                            e.Graphics.DrawString(GetLength(Tapes[i - 1].EndPoint, Tapes[i].StartPoint).ToString("#0.00"), drawFont, drawBrush, (float)(Tapes[i - 1].EndPoint.X + vec[0] / 2.0), (float)(Tapes[i - 1].EndPoint.Y + vec[1] / 2.0));
                            e.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Lime), Tapes[i].StartPoint, Tapes[i - 1].EndPoint);
                            totalLength += GetLength(Tapes[i - 1].EndPoint, Tapes[i].StartPoint);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < Tapes.Count; i++)
                    {
                        if (Tapes[i - 1].EndPoint != Point.Empty && Tapes[i].StartPoint != Point.Empty)
                            totalLength += GetLength(Tapes[i - 1].EndPoint, Tapes[i].StartPoint);
                    }
                }

                if (parents.Count > 0)
                {
                    for (int i = 0; i < parents.Count; i++)
                    {
                        if (!ColorLookup.ContainsKey(parents[i].ID))
                            ColorLookup.Add(parents[i].ID, GetRandomColor());
                        e.Graphics.FillEllipse(Brushes.White, parents[i].Location.X - 10, parents[i].Location.Y - 10, 20, 20);
                    }
                }

                OriginalDistanceLabel.Text = "Total Travel Distance: " + totalLength.ToString("#0.00");

                drawFont.Dispose();
                drawBrush.Dispose();
            }

            if (MouseDownPoint != Point.Empty)
            {
                TapePen = new System.Drawing.Pen(System.Drawing.Color.Azure);
                e.Graphics.DrawEllipse(TapePen, new Rectangle(MouseDownPoint, new Size(10, 10)));
            }
            if (Anchor != Point.Empty)
                e.Graphics.FillEllipse(Brushes.RoyalBlue, new Rectangle(Anchor, new Size(25, 25)));

            if (TapePen != null)
                TapePen.Dispose();

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        }

        double GetLength(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Tapes.Clear();
            MouseDownPoint = Point.Empty;
            Anchor = Point.Empty;
            sorted.Clear();
            splitContainer2.Panel1.Invalidate();
            splitContainer2.Panel2.Invalidate();
            OriginalDistanceLabel.Text = "Total Travel Distance: 0";
            SortedDistanceLabel.Text = "Total Travel Distance: 0";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer2.Panel1.Invalidate();
            splitContainer2.Panel2.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                SaveToFile(Tapes);

            if (checkBox2.Checked)
            {
                NewAlgo(Tapes);
                return;
            }

            if (bruteForceIt.Checked)
            {
                if (Anchor != Point.Empty && !optAnchor.Checked)
                    RunBruteForceOptimizer();
                else
                    RunBruteForceWithMultiAnchor();
            }
            else if (GAit.Checked)
            {
                if (Anchor != Point.Empty)
                    RunGA();
            }
        }

        private void SaveToFile(List<Tape> Tapes)
        {
            string saveFilePath = "Tapes.sav";

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFilePath))
                Tapes.ForEach(tape => { sw.WriteLine(String.Format("{0},{1}-{2},{3}", tape.StartPoint.X, tape.StartPoint.Y, tape.EndPoint.X, tape.EndPoint.Y)); });

        }

        Dictionary<int, Color> ColorLookup = new Dictionary<int, Color>();
        List<Cluster> parents = new List<Cluster>();
        List<List<Tape>> sats = new List<List<Tape>>();
        List<List<Tape>> SortedClusters = new List<List<Tape>>();

        void NewAlgo(List<Tape> AllTapes)
        {
            Point Max = Point.Empty;
            Point Min = Point.Empty;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            double disx1 = 0;
            double disy1 = 0;
            double disx2 = 0;
            double disy2 = 0;
            Tapes.ForEach(tape =>
                {
                    disx1 = tape.StartPoint.X;
                    disy1 = tape.StartPoint.Y;
                    disx2 = tape.EndPoint.X;
                    disy2 = tape.EndPoint.Y;

                    if (disx1 > maxX)
                        maxX = disx1;
                    if (disx2 > maxX)
                        maxX = disx2;

                    if (disy1 > maxY)
                        maxY = disy1;
                    if (disy2 > maxY)
                        maxY = disy2;

                });

            int ClusterCount = 4;

            sats = new List<List<Tape>>();
            parents = new List<Cluster>();
            List<Cluster> Prevparents = new List<Cluster>() { };

            bool go = true;
            for (int i = 0; i < ClusterCount; i++)
            {
                parents.Add(new Cluster((int)(rand.NextDouble() * maxX), (int)(rand.NextDouble() * maxY), GetAvailableGUID()));
                Prevparents.Add(new Cluster());
            }

            int count = 0;
            while (go)
            {
                sats = new List<List<Tape>>();
                for (int i = 0; i < ClusterCount; i++)
                    sats.Add(new List<Tape>());

                double min = double.MaxValue;
                int index1 = -1;
                double distance1 = 0;
                double distance2 = 0;

                foreach (Tape tape in Tapes)
                {
                    min = double.MaxValue;
                    for (int i = 0; i < parents.Count; i++)
                    {
                        distance1 = GetLength(tape.StartPoint, parents[i].Location);
                        distance2 = GetLength(tape.EndPoint, parents[i].Location);
                        if (Math.Min(distance1, distance2) < min)
                        {
                            min = Math.Min(distance1, distance2);
                            index1 = i;
                        }
                    }
                    sats[index1].Add(new Tape(tape));
                }

                for (int i = 0; i < sats.Count; i++)
                    parents[i].Location = GetAveragePoint(sats[i]);


                for (int i = 0; i < parents.Count; i++)
                    go = (parents[i].X != Prevparents[i].X) || (parents[i].Y != Prevparents[i].Y);

                Prevparents = new List<Cluster>(parents);

                splitContainer2.Panel1.Invalidate();
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
                count++;
            }

            bool old = false;
            if (old)
            {
                SortedClusters = new List<List<Tape>>(parents.Count);
                sorted.Clear();
                for (int i = 0; i < sats.Count; i++)
                {
                    if (sats[i].Count > 0)
                    {
                        SortedClusters.Add(SortTapes(sats[i], parents[i].Location));
                        sorted.AddRange(SortedClusters.Last());
                    }
                }
            }
            else
            {
                Cluster_Threads = new List<ClusterThread>(ThreadCount);
                ClusterThreadsDoneEvents = new System.Threading.ManualResetEvent[ThreadCount];
                List<Point> anchorPnts = new List<Point>();
                Point tmp = Point.Empty;

                for (int i = 0; i < ThreadCount; i++)
                {
                    tmp = GetRandomAnchorPoint();
                    while (anchorPnts.Contains(tmp))
                        tmp = GetRandomAnchorPoint();
                    anchorPnts.Add(new Point(tmp.X, tmp.Y));

                }
                for (int i = 0; i < ThreadCount; i++)
                {
                    ClusterThreadsDoneEvents[i] = new System.Threading.ManualResetEvent(false);
                    ClusterThread gathr = new ClusterThread(ClusterThreadsDoneEvents[i], new List<List<Tape>>(sats), parents);
                    Cluster_Threads.Add(gathr);
                    System.Threading.ThreadPool.QueueUserWorkItem(gathr.ThreadPoolCallback, i);
                }

                // Wait for all threads in pool to finish.
                foreach (var e in ClusterThreadsDoneEvents) e.WaitOne();

                double min = double.MaxValue;

                count = 0;
                int bestIndex = -1;
                Cluster_Threads.ForEach(thread =>
                {
                    if (min > thread.PathLength)
                    {
                        bestIndex = count;
                        min = thread.PathLength;
                    }
                    count++;
                });

                sorted = new List<Tape>(Cluster_Threads[bestIndex].Sorted);
                //Anchor = new Point(Cluster_Threads[bestIndex].anchor.X, Cluster_Threads[bestIndex].anchor.Y);
                //splitContainer2.Panel1.Invalidate();
                //splitContainer2.Panel2.Invalidate();
            }
            splitContainer2.Panel2.Invalidate();



            //List<Tape> sorted = new List<Tape>();


            // SHIT IS BROKEN.  YOU HAVE TO LOOK FROM EACH TAPE 

            /*
             * foreach cluster, optimize closest distance within (if possible)
             * 
             * then handle border conditions but finding the closest cluster, then within each cluster, get the two closest points
             */

            //bool AllMarked = false;
            //int stepCount = parents.Count;
            //bool start = false;
            //Cluster Current = null;

            //double distance = double.MinValue;

            //Current = parents[0];

            //for (int i = 0; i < parents.Count; i++)
            //{
            //    while(!Current.Marked)
            //    {
            //        Point ClosestToAverage = Point.Empty;

            //        for (int j = 0; j < sats[i].Count; j++)
            //        {
            //            if (distance < GetLength(sats[i][j], Current, ref start))
            //            {
            //                distance = GetLength(sats[i][j], Current, ref start);
            //                ClosestToAverage = start ? sats[i][j].StartPoint : sats[i][j].EndPoint;
            //            }
            //        }

            //        Current.Marked = true;

            //        Current = MoveToClosestCluster(ref Current, ref parents);
            //    }
            //}

        }

        private List<Tape> SortTapes(List<Tape> list, Point anchor)
        {
            List<Tape> original = new List<Tape>();
            list.ForEach(t => { original.Add(new Tape(t)); });
            List<Tape> ret = new List<Tape>();

            double directionX = rand.NextDouble() > 0.5 ? -1 : 1;
            double directionY = rand.NextDouble() > 0.5 ? -1 : 1;
            anchor.X += (int)(directionX * rand.NextDouble() * 20);
            anchor.Y += (int)(directionY * rand.NextDouble() * 20);
            ret.Add(GetClosestTapeToAnchor(original, anchor));
            original.Remove(ret[0]);

            List<double> distances = new List<double>();
            double distanceS = 0;
            double distanceE = 0;

            while (original.Count > 0)
            {
                distances.Clear();
                distanceS = 0;
                distanceE = 0;

                for (int i = 0; i < original.Count; i++)
                {
                    switch (opt)
                    {
                        case OptimizeBy.CLOSEST:
                            {
                                distanceS = GetLength(original[i].StartPoint, ret.Last().EndPoint);
                                distanceE = GetLength(original[i].EndPoint, ret.Last().EndPoint);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.X:
                            {
                                distanceS = Math.Pow(anchor.X - original[i].StartPoint.X, 2);
                                distanceE = Math.Pow(anchor.X - original[i].EndPoint.X, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.Y:
                            {
                                distanceS = Math.Pow(anchor.Y - original[i].StartPoint.Y, 2);
                                distanceE = Math.Pow(anchor.Y - original[i].EndPoint.Y, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.XY:
                            {
                                distanceS = GetLength(original[i].StartPoint, anchor);
                                distanceE = GetLength(original[i].EndPoint, anchor);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                    }

                }

                double[] dArray = distances.ToArray();
                Tape[] oArray = original.ToArray();

                Array.Sort(dArray, oArray);

                ret.Add(oArray[0]);
                original.Remove(oArray[0]);
            }

            return ret;
        }

        private Cluster MoveToClosestCluster(ref Cluster Current, ref List<Cluster> parents)
        {
            int indexOfClosest = -1;
            double min = double.MaxValue;
            for (int i = 0; i < parents.Count; i++)
            {
                if (parents[i].X == Current.X && parents[i].Y == Current.Y)
                    continue;

                if (min > GetLength(Current.Location, parents[i].Location))
                {
                    min = GetLength(Current.Location, parents[i].Location);
                    indexOfClosest = i;
                }
            }

            return parents[indexOfClosest];

        }

        private double GetLength(Tape tape, Cluster cluster, ref bool start)
        {
            double distance1 = Math.Sqrt(Math.Pow(tape.StartPoint.X - cluster.X, 2) + Math.Pow(tape.StartPoint.Y - cluster.Y, 2));
            double distance2 = Math.Sqrt(Math.Pow(tape.EndPoint.X - cluster.X, 2) + Math.Pow(tape.EndPoint.Y - cluster.Y, 2));

            distance1 = Math.Min(distance1, distance2);

            start = Math.Min(distance1, distance2) == distance1;

            return distance1;
        }

        private Point GetAveragePoint(List<Tape> list)
        {
            if (list.Count == 0)
                return new Point(0, 0);
            double averageX = list.Average(pnt => pnt.StartPoint.X);
            double averageY = list.Average(pnt => pnt.StartPoint.Y);

            averageX = Math.Max(averageX, list.Average(pnt => pnt.EndPoint.X));
            averageY = Math.Max(averageY, list.Average(pnt => pnt.EndPoint.Y));

            return new Point((int)averageX, (int)averageY);
        }

        System.Threading.ManualResetEvent[] PathThreadsDoneEvents;
        List<PathThread> Path_Threads;

        System.Threading.ManualResetEvent[] ClusterThreadsDoneEvents;
        List<ClusterThread> Cluster_Threads;

        int ThreadCount = 500;

        private void RunBruteForceWithMultiAnchor()
        {
            int MaxThreadCount = 144; // 12^2

            ThreadCount = Tapes.Count * 2; //if we have tapes we have to check start and end points, if only mark, just start

            bool useRandomSeeds = ThreadCount > MaxThreadCount; //this is here incase we try to spawn too many threads.  We have to have a max count

            if (useRandomSeeds)
                ThreadCount = MaxThreadCount;

            Path_Threads = new List<PathThread>(ThreadCount);
            PathThreadsDoneEvents = new System.Threading.ManualResetEvent[ThreadCount];
            List<Point> anchorPnts = new List<Point>();
            PointF tmp = PointF.Empty;
            if (useRandomSeeds)
            {
                // System.Drawing.Point tmp = System.Drawing.Point.Empty;
                double maxX = double.MinValue;
                double minX = double.MaxValue;
                double maxY = double.MinValue;
                double minY = double.MaxValue;

                maxX = Math.Max(Tapes.Max(tape => tape.StartPoint.X), Tapes.Max(tape => tape.EndPoint.X));
                maxY = Math.Max(Tapes.Max(tape => tape.StartPoint.Y), Tapes.Max(tape => tape.EndPoint.Y));
                minX = Math.Min(Tapes.Min(tape => tape.StartPoint.X), Tapes.Min(tape => tape.EndPoint.X));
                minY = Math.Min(Tapes.Min(tape => tape.StartPoint.Y), Tapes.Min(tape => tape.EndPoint.Y));


                #region Random Anchor Points
                //Random Starting Points
                //for (int i = 0; i < ThreadCount; i++)
                //{
                //    tmp = GetRandomAnchorPoint(maxX, minX, maxY, minY);
                //    while (anchorPnts.Contains(tmp))
                //        tmp = GetRandomAnchorPoint(maxX, minX, maxY, minY);
                //    anchorPnts.Add(new System.Drawing.Point(tmp.X, tmp.Y));
                //} 
                #endregion

                //Even grid anchor points
                double rows = 12; double cols = 12;

                for (double i = 0; i < rows; i++)
                    for (double j = 0; j < cols; j++)
                        anchorPnts.Add(new System.Drawing.Point((int)(minX + (i / (rows - 1)) * (maxX - minX)), (int)(minY + (j / (cols - 1)) * (maxY - minY))));

            }
            else
            {

                for (int i = 0; i < ThreadCount / 2; i++)
                {
                    anchorPnts.Add(new Point(Tapes[i].StartPoint.X, Tapes[i].StartPoint.Y));
                    anchorPnts.Add(new Point(Tapes[i].EndPoint.X, Tapes[i].EndPoint.Y));
                    //tmp = GetRandomAnchorPoint();
                    //while (anchorPnts.Contains(tmp))
                    //    tmp = GetRandomAnchorPoint();
                    //anchorPnts.Add(new Point(tmp.X, tmp.Y));

                }
            }
            for (int i = 0; i < ThreadCount; i++)
            {
                PathThreadsDoneEvents[i] = new System.Threading.ManualResetEvent(false);
                PathThread gathr = new PathThread(PathThreadsDoneEvents[i], new List<Tape>(Tapes), anchorPnts[i], opt);
                Path_Threads.Add(gathr);
                System.Threading.ThreadPool.QueueUserWorkItem(gathr.ThreadPoolCallback, i);
            }

            // Wait for all threads in pool to finish.
            foreach (var e in PathThreadsDoneEvents) e.WaitOne();

            double min = double.MaxValue;

            int count = 0;
            int bestIndex = -1;
            Path_Threads.ForEach(thread =>
                {
                    if (min > thread.PathLength)
                    {
                        bestIndex = count;
                        min = thread.PathLength;
                    }
                    count++;
                });

            sorted = new List<Tape>(Path_Threads[bestIndex].Sorted);
            Anchor2 = new Point(Path_Threads[bestIndex].anchor.X, Path_Threads[bestIndex].anchor.Y);
            splitContainer2.Panel1.Invalidate();
            splitContainer2.Panel2.Invalidate();

        }

        private Point GetRandomAnchorPoint()
        {
            Point ret = new Point(-100, -100);

            ret = new Point((int)(splitContainer1.Panel1.Width * rand.NextDouble()), (int)(splitContainer1.Panel1.Height * rand.NextDouble()));

            return ret;
        }

        Random rand = new Random();

        List<Tape> sorted = new List<Tape>();

        private void RunBruteForceOptimizer()
        {
            sorted.Clear();
            List<Tape> original = new List<Tape>();
            Tapes.ForEach(t => { original.Add(new Tape(t)); });

            sorted.Add(GetClosestTapeToAnchor(original, Anchor));
            original.Remove(sorted[0]);

            List<double> distances = new List<double>();
            double distanceS = 0;
            double distanceE = 0;

            while (original.Count > 0)
            {
                distances.Clear();
                distanceS = 0;
                distanceE = 0;

                for (int i = 0; i < original.Count; i++)
                {
                    switch (opt)
                    {
                        case OptimizeBy.CLOSEST:
                            {
                                distanceS = GetLength(original[i].StartPoint, sorted.Last().EndPoint);
                                distanceE = GetLength(original[i].EndPoint, sorted.Last().EndPoint);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.X:
                            {
                                distanceS = Math.Pow(Anchor.X - original[i].StartPoint.X, 2);
                                distanceE = Math.Pow(Anchor.X - original[i].EndPoint.X, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.Y:
                            {
                                distanceS = Math.Pow(Anchor.Y - original[i].StartPoint.Y, 2);
                                distanceE = Math.Pow(Anchor.Y - original[i].EndPoint.Y, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.XY:
                            {
                                distanceS = GetLength(original[i].StartPoint, Anchor);
                                distanceE = GetLength(original[i].EndPoint, Anchor);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                    }

                }

                double[] dArray = distances.ToArray();
                Tape[] oArray = original.ToArray();

                Array.Sort(dArray, oArray);

                sorted.Add(oArray[0]);
                original.Remove(oArray[0]);

                //splitContainer2.Panel2.Invalidate();
                //Application.DoEvents();
                //System.Threading.Thread.Sleep(trackBar1.Value * 50);
            }

            splitContainer2.Panel2.Invalidate();
        }

        private Tape GetClosestTapeToAnchor(List<Tape> tapes, Point anchorPoint)
        {
            double min = double.MaxValue;
            double distanceS = 0;
            double distanceE = 0;
            int count = 0;
            int MinIndex = -1;
            tapes.ForEach(t =>
            {
                distanceS = GetLength(t.StartPoint, anchorPoint);
                distanceE = GetLength(t.EndPoint, anchorPoint);
                if (distanceE < distanceS)
                    t.Reverse();
                if (min > Math.Min(distanceS, distanceE))
                {
                    min = Math.Min(distanceS, distanceE);
                    MinIndex = count;
                }
                count++;
            });

            return tapes[MinIndex];
        }

        private void RunGA()
        {
            throw new NotImplementedException();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
             * Closest Neighbor
                X
                Y
                XY
             */
            switch (comboBox1.Text)
            {
                case "Closest Neighbor":
                    opt = OptimizeBy.CLOSEST;
                    break;
                case "X":
                    opt = OptimizeBy.X;
                    break;
                case "Y":
                    opt = OptimizeBy.Y;
                    break;
                case "XY":
                    opt = OptimizeBy.XY;
                    break;
            }
        }

        private void optAnchor_CheckedChanged(object sender, EventArgs e)
        {
            if (optAnchor.Checked)
            {
                Anchor = Point.Empty;
                splitContainer2.Panel1.Invalidate();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Tape> original = new List<Tape>();
            sorted.ForEach(t => { original.Add(new Tape(t)); });
            sorted.Clear();

            sorted.Add(GetClosestTapeToAnchor(original, Anchor));
            original.Remove(sorted[0]);

            List<double> distances = new List<double>();
            double distanceS = 0;
            double distanceE = 0;

            while (original.Count > 0)
            {
                distances.Clear();
                distanceS = 0;
                distanceE = 0;

                for (int i = 0; i < original.Count; i++)
                {
                    switch (opt)
                    {
                        case OptimizeBy.CLOSEST:
                            {
                                distanceS = GetLength(original[i].StartPoint, sorted.Last().EndPoint);
                                distanceE = GetLength(original[i].EndPoint, sorted.Last().EndPoint);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.X:
                            {
                                distanceS = Math.Pow(Anchor.X - original[i].StartPoint.X, 2);
                                distanceE = Math.Pow(Anchor.X - original[i].EndPoint.X, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.Y:
                            {
                                distanceS = Math.Pow(Anchor.Y - original[i].StartPoint.Y, 2);
                                distanceE = Math.Pow(Anchor.Y - original[i].EndPoint.Y, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.XY:
                            {
                                distanceS = GetLength(original[i].StartPoint, Anchor);
                                distanceE = GetLength(original[i].EndPoint, Anchor);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                    }

                }

                double[] dArray = distances.ToArray();
                Tape[] oArray = original.ToArray();

                Array.Sort(dArray, oArray);

                sorted.Add(oArray[0]);
                original.Remove(oArray[0]);

                splitContainer2.Panel2.Invalidate();
                Application.DoEvents();
                System.Threading.Thread.Sleep(trackBar1.Value * 25);
            }
            splitContainer2.Panel2.Invalidate();
        }

        int GetAvailableGUID()
        {

            int GUID = rand.Next(0, 100);
            while (ParentsContainsGUID(GUID))
                GUID = GetAvailableGUID();

            return GUID;
        }

        bool ParentsContainsGUID(int key)
        {
            Cluster k = parents.Find(p => { return p.ID == key; });
            return k != null;
        }

        Color GetRandomColor()
        {
            return Color.FromArgb(100 + (int)(rand.NextDouble() * 155), (int)(rand.NextDouble() * 255), (int)(rand.NextDouble() * 255), (int)(rand.NextDouble() * 255));
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer2.Panel1.Invalidate();
            splitContainer2.Panel2.Invalidate();
        }

        private void splitContainer2_Panel1_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.Copy;// allow them to continue(without this, the cursor stays a "NO" symbol)
        }

        private void splitContainer2_Panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in filenames)
                if (System.IO.Path.GetExtension(file).ToLower() == ".sav")
                    LoadSavFile(file);
        }

        private void LoadSavFile(string file)
        {
            if (!System.IO.File.Exists(file))
                return;
            List<Tape> tmp = new List<Tape>();
            string line = "";
            string[] data = null;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line.Contains("anchor:"))
                    {
                        line = line.Replace("anchor:", "");
                        data = line.Split(new char[] { '-' });
                        Anchor = new Point(Convert.ToInt32(data[0].Split(new char[] { ',' })[0]), Convert.ToInt32(data[0].Split(new char[] { ',' })[1]));
                    }
                    else
                    {

                        data = line.Split(new char[] { '-' });

                        tmp.Add(new Tape(new Point(Convert.ToInt32(data[0].Split(new char[] { ',' })[0]), Convert.ToInt32(data[0].Split(new char[] { ',' })[1])),
                                            new Point(Convert.ToInt32(data[1].Split(new char[] { ',' })[0]), Convert.ToInt32(data[1].Split(new char[] { ',' })[1]))));
                    }
                }
            }

            Normalize(tmp);
            splitContainer2.Panel1.Invalidate();
        }

        private void Normalize(List<Tape> tmp)
        {
            Size OurWindowSize = splitContainer2.Panel1.Size;

            double MaxX = double.MinValue;
            double MinX = double.MaxValue;

            double MaxY = double.MinValue;
            double MinY = double.MaxValue;

            // find the range for scaling
            tmp.ForEach(entry =>
                {
                    if (MaxX < entry.StartPoint.X)
                        MaxX = entry.StartPoint.X;
                    if (MinX > entry.StartPoint.X)
                        MinX = entry.StartPoint.X;

                    if (MaxY < entry.StartPoint.Y)
                        MaxY = entry.StartPoint.Y;
                    if (MinY > entry.StartPoint.Y)
                        MinY = entry.StartPoint.Y;
                });


            tmp.ForEach(tape =>
                {
                    Tapes.Add(new Tape(new Point((int)((tape.StartPoint.X / MaxX) * (OurWindowSize.Width / 1.1)), (int)((tape.StartPoint.Y / MaxY) * (OurWindowSize.Height / 1.1))),
                        new Point((int)((tape.EndPoint.X / MaxX) * (OurWindowSize.Width / 1.1)), (int)((tape.EndPoint.Y / MaxY) * (OurWindowSize.Height / 1.1)))));
                });

            Anchor = new Point((int)((Anchor.X / MaxX) * (OurWindowSize.Width / 1.1)), (int)((Anchor.Y / MaxY) * (OurWindowSize.Height / 1.1)));
        }
    }

    public class Cluster
    {
        int guid = -1;
        public Cluster() { }
        public Cluster(int x, int y, int uid) { Location = new Point(x, y); guid = uid; }
        public bool Marked = false;
        public Point Location = Point.Empty;
        public int X { get { return Location.X; } }
        public int Y { get { return Location.Y; } }
        bool Empty { get { return Location.IsEmpty; } }
        public int ID { get { return guid; } set { guid = value; } }
    }

    public class Tape
    {
        private Point start = Point.Empty;
        private Point end = Point.Empty;
        private bool reversed = false;

        public bool Reversed
        {
            get { return reversed; }
        }

        public Point StartPoint
        {
            get { return start; }
            set { start = value; }
        }

        public Point EndPoint
        {
            get { return end; }
            set { end = value; }
        }

        public Tape(Point Start)
        {
            start = new Point(Start.X, Start.Y);
        }

        public void Reverse()
        {
            reversed = !reversed;
            Point tmp = new Point(EndPoint.X, EndPoint.Y);
            EndPoint = new Point(StartPoint.X, StartPoint.Y);
            StartPoint = new Point(tmp.X, tmp.Y);
        }

        public Tape(Point Start, Point End)
        {
            start = new Point(Start.X, Start.Y);
            end = new Point(End.X, End.Y);
        }

        public Tape(Tape old)
        {
            start = new Point(old.StartPoint.X, old.StartPoint.Y);
            end = new Point(old.EndPoint.X, old.EndPoint.Y);
            reversed = old.reversed;
        }

        public bool EndPointAvailable
        {
            get { return EndPoint == Point.Empty; }
        }

        public double Length
        {
            get { return Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2)); }
        }

    }

    public class PathThread
    {
        public Point anchor = Point.Empty;
        OptimizeBy opt = OptimizeBy.CLOSEST;
        List<Tape> ts = new List<Tape>();
        List<Tape> sorted = new List<Tape>();

        public List<Tape> Sorted
        {
            get { return sorted; }
            set { sorted = value; }
        }

        double pathLength = 0;

        public double PathLength
        {
            get { return pathLength; }
            set { pathLength = value; }
        }

        /// <summary>
        /// Thread Done Event
        /// </summary>
        private System.Threading.ManualResetEvent doneEvent;

        double GetLength(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }

        public PathThread(System.Threading.ManualResetEvent _doneEvent, List<Tape> tapes, Point anchorPoint, OptimizeBy o)
        {
            opt = o;

            ts = tapes;

            anchor = new Point(anchorPoint.X, anchorPoint.Y);

            doneEvent = _doneEvent;
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            RunPath();
            doneEvent.Set();
        }

        private Tape GetClosestTapeToAnchor(List<Tape> tapes, Point anchorPoint)
        {
            double min = double.MaxValue;
            double distanceS = 0;
            double distanceE = 0;
            int count = 0;
            int MinIndex = -1;
            tapes.ForEach(t =>
            {
                distanceS = GetLength(t.StartPoint, anchorPoint);
                distanceE = GetLength(t.EndPoint, anchorPoint);
                if (distanceE < distanceS)
                    t.Reverse();
                if (min > Math.Min(distanceS, distanceE))
                {
                    min = Math.Min(distanceS, distanceE);
                    MinIndex = count;
                }
                count++;
            });

            return tapes[MinIndex];
        }

        void RunPath()
        {
            sorted.Clear();
            List<Tape> original = new List<Tape>();
            ts.ForEach(t => { original.Add(new Tape(t)); });


            sorted.Add(GetClosestTapeToAnchor(original, anchor));
            original.Remove(sorted[0]);

            List<double> distances = new List<double>();
            double distanceS = 0;
            double distanceE = 0;

            while (original.Count > 0)
            {
                distances.Clear();
                distanceS = 0;
                distanceE = 0;

                for (int i = 0; i < original.Count; i++)
                {
                    switch (opt)
                    {
                        case OptimizeBy.CLOSEST:
                            {
                                distanceS = GetLength(original[i].StartPoint, sorted.Last().EndPoint);
                                distanceE = GetLength(original[i].EndPoint, sorted.Last().EndPoint);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.X:
                            {
                                distanceS = Math.Pow(anchor.X - original[i].StartPoint.X, 2);
                                distanceE = Math.Pow(anchor.X - original[i].EndPoint.X, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.Y:
                            {
                                distanceS = Math.Pow(anchor.Y - original[i].StartPoint.Y, 2);
                                distanceE = Math.Pow(anchor.Y - original[i].EndPoint.Y, 2);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                        case OptimizeBy.XY:
                            {
                                distanceS = GetLength(original[i].StartPoint, anchor);
                                distanceE = GetLength(original[i].EndPoint, anchor);
                                if (distanceE < distanceS)
                                    original[i].Reverse();
                                distances.Add(Math.Min(distanceS, distanceE));
                            }
                            break;
                    }

                }

                double[] dArray = distances.ToArray();
                Tape[] oArray = original.ToArray();

                Array.Sort(dArray, oArray);

                sorted.Add(oArray[0]);
                original.Remove(oArray[0]);
            }
            PathLength = 0;
            for (int i = 1; i < sorted.Count; i++)
                if (sorted[i - 1].EndPoint != Point.Empty && sorted[i].StartPoint != Point.Empty)
                    PathLength += GetLength(sorted[i - 1].EndPoint, sorted[i].StartPoint);

        }
    }

    public class ClusterThread
    {
        public Point anchor = Point.Empty;
        List<List<Tape>> sats = new List<List<Tape>>();
        List<List<Tape>> SortedClusters = new List<List<Tape>>();
        List<Tape> sorted = new List<Tape>();
        List<Point> parents = new List<Point>();
        Random rand = new Random();
        bool useAnchor = false;

        public List<Tape> Sorted
        {
            get { return sorted; }
            set { sorted = value; }
        }

        double pathLength = 0;

        public double PathLength
        {
            get { return pathLength; }
            set { pathLength = value; }
        }

        /// <summary>
        /// Thread Done Event
        /// </summary>
        private System.Threading.ManualResetEvent doneEvent;

        double GetLength(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }

        public ClusterThread(System.Threading.ManualResetEvent _doneEvent, List<List<Tape>> tapes, List<Cluster> centers)
        {
            parents = new List<Point>(centers.Count);
            centers.ForEach(c => { parents.Add(new Point(c.X, c.Y)); });
            sats = tapes;
            doneEvent = _doneEvent;
        }
        public ClusterThread(System.Threading.ManualResetEvent _doneEvent, List<List<Tape>> tapes, List<Point> centers, Point Anchor)
        {
            parents = new List<Point>(centers);
            useAnchor = true;
            anchor = Anchor;
            sats = tapes;
            doneEvent = _doneEvent;
        }
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            RunPath();
            doneEvent.Set();
        }

        private Tape GetClosestTapeToAnchor(List<Tape> tapes, Point anchorPoint)
        {
            double min = double.MaxValue;
            double distanceS = 0;
            double distanceE = 0;
            int count = 0;
            int MinIndex = -1;
            tapes.ForEach(t =>
            {
                distanceS = GetLength(t.StartPoint, anchorPoint);
                distanceE = GetLength(t.EndPoint, anchorPoint);
                if (distanceE < distanceS)
                    t.Reverse();
                if (min > Math.Min(distanceS, distanceE))
                {
                    min = Math.Min(distanceS, distanceE);
                    MinIndex = count;
                }
                count++;
            });

            return tapes[MinIndex];
        }

        void RunPath()
        {
            Point anchor = Point.Empty;
            for (int i = 0; i < sats.Count; i++)
            {
                for (int j = 0; j < sats[i].Count; j++)
                {
                    if (!useAnchor)
                        anchor = GetRandomAnchor(sats[i]);
                    SortedClusters.Add(SortTapes(sats[i], anchor));
                }
            }

            List<int> order = new List<int>();
            List<Point> original = new List<Point>(parents);
            int randomStart = (int)(rand.NextDouble() * (original.Count - 1));
            order.Add(randomStart);
            parents.RemoveAt(randomStart);

            List<double> distances = new List<double>();
            double distance = 0;
            while (parents.Count > 0)
            {
                distances.Clear();
                distance = 0;
                for (int i = 0; i < parents.Count; i++)
                {
                    distance = GetLength(original[order.Last()], parents[i]);
                    distances.Add(distance);
                }

                double[] dArray = distances.ToArray();
                Point[] oArray = parents.ToArray();

                Array.Sort(dArray, oArray);

                order.Add(original.IndexOf(oArray[0]));
                parents.Remove(oArray[0]);
            }

            for (int i = 0; i < order.Count; i++)
                sorted.AddRange(sats[order[i]]);
        }

        private Point GetRandomAnchor(List<Tape> list)
        {
            double maxX = list.Max(pnt => pnt.StartPoint.X);
            maxX = Math.Max(maxX, list.Max(pnt => pnt.EndPoint.X));

            double minX = list.Min(pnt => pnt.StartPoint.X);
            minX = Math.Min(minX, list.Min(pnt => pnt.EndPoint.X));

            double maxY = list.Max(pnt => pnt.StartPoint.Y);
            maxY = Math.Max(maxY, list.Max(pnt => pnt.EndPoint.Y));

            double minY = list.Max(pnt => pnt.StartPoint.Y);
            minY = Math.Min(minY, list.Max(pnt => pnt.EndPoint.Y));

            return new Point((int)(minX + rand.NextDouble() * (maxX - minX)), (int)(minY + rand.NextDouble() * (maxY - minY)));
        }

        private List<Tape> SortTapes(List<Tape> list, Point anchor)
        {
            List<Tape> original = new List<Tape>();
            list.ForEach(t => { original.Add(new Tape(t)); });
            List<Tape> ret = new List<Tape>();

            double directionX = rand.NextDouble() > 0.5 ? -1 : 1;
            double directionY = rand.NextDouble() > 0.5 ? -1 : 1;
            anchor.X += (int)(directionX * rand.NextDouble() * 20);
            anchor.Y += (int)(directionY * rand.NextDouble() * 20);
            ret.Add(GetClosestTapeToAnchor(original, anchor));
            original.Remove(ret[0]);

            List<double> distances = new List<double>();
            double distanceS = 0;
            double distanceE = 0;

            while (original.Count > 0)
            {
                distances.Clear();
                distanceS = 0;
                distanceE = 0;

                for (int i = 0; i < original.Count; i++)
                {
                    distanceS = GetLength(original[i].StartPoint, ret.Last().EndPoint);
                    distanceE = GetLength(original[i].EndPoint, ret.Last().EndPoint);
                    if (distanceE < distanceS)
                        original[i].Reverse();
                    distances.Add(Math.Min(distanceS, distanceE));
                }

                double[] dArray = distances.ToArray();
                Tape[] oArray = original.ToArray();

                Array.Sort(dArray, oArray);

                ret.Add(oArray[0]);
                original.Remove(oArray[0]);
            }

            return ret;
        }

    }
}
