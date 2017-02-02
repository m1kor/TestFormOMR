// Copyright (C) Mikhail Koronovskiy 2016

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Rec {
    public partial class MainWindow : Form {
        private CellIndexComparer cic;
        private Bitmap bitmapBlank, bitmapBlankOriginal;
        private GraphWindow graphWindow;
        public Cell[] cells;
        public int maxDiffI, m_size;
        int[,] intImg;

        public class Cell : IComparable<Cell> {
            public Cell() {
                Index = 0;
                Value = 0;
                Force = 0;
            }

            public Cell(int i) {
                Index = i;
                Value = 0;
                Force = 0;
            }

            public int Index { get; set; }
            public int Value { get; set; }
            public int Force { get; set; }

            override public string ToString() {
                string str = (Index / 5 + 1).ToString();
                if (str.Length == 1)
                    str = '0' + str;
                return str + " - " + (Index % 5 + 1).ToString();
            }

            public int CompareTo(Cell other) {
                 return Value.CompareTo(other.Value);
            }
        }

        public class CellIndexComparer : IComparer<Cell> {
            public int Compare(Cell a, Cell b) {
                return a.Index.CompareTo(b.Index);
            }
        }

        public MainWindow() {
            InitializeComponent();
            maxDiffI = -1;
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            cic = new CellIndexComparer();
            graphWindow = new GraphWindow(this);
        }

        private void ShowErrorMsgBox(string text) {
            MessageBox.Show("Warning! Error: " + text);
        }

        private void openScanToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName.Length > 0) {
                try {
                    bitmapBlank = new Bitmap(openFileDialog.FileName);
                    bitmapBlankOriginal = new Bitmap(bitmapBlank);
                    float zoomFactor = 0.5f;
                    Size newSize = new Size((int)(bitmapBlank.Width * zoomFactor), (int)(bitmapBlank.Height * zoomFactor));
                    IntegralImageAndBradleyLocalThresholding(bitmapBlank);
                    Recognize(bitmapBlank);
                    pictureBoxBlank.Image = new Bitmap(bitmapBlank, newSize);
                }
                catch (Exception ex) {
                    ShowErrorMsgBox("Can't access the file");
                    ShowErrorMsgBox(ex.Message);
                }
            }
        }

        private void IntegralImageAndBradleyLocalThresholding(Bitmap bitmap) {
            try {
                unsafe
                {
                    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                    int s = bitmapData.Width / 8;
                    float t = 0.85f;
                    int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                    int widthInBytes = bitmapData.Width * bytesPerPixel;
                    byte* ptrFirstPixel = (byte*)bitmapData.Scan0; // BGR
                    intImg = new int[bitmapData.Width,bitmapData.Height]; // Integral image, summed area table
                    int sum;
                    for (int x = 0; x < bitmapData.Width; x++) {
                        int xBytes = x * bytesPerPixel;
                        sum = 0;
                        for (int y = 0; y < bitmapData.Height; y++) {
                            byte* currentLine = ptrFirstPixel + y * bitmapData.Stride;
                            sum += currentLine[xBytes + 2];
                            if (x == 0)
                                intImg[x, y] = sum;
                            else
                                intImg[x, y] = intImg[x - 1, y] + sum;
                        }
                    }
                    int hs = s / 2;
                    Parallel.For(0, bitmapData.Width, x => {
                        int xBytes = x * bytesPerPixel;
                        int x1, x1m, y1, y1m, x2, y2, count;
                        for (int y = 0; y < bitmapData.Height; y++) {
                            byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                            x1 = Math.Max(0, x - hs);
                            y1 = Math.Max(0, y - hs);
                            x2 = Math.Min(bitmapData.Width - 1, x + hs);
                            y2 = Math.Min(bitmapData.Height - 1, y + hs);
                            if (x < s) {
                                x1 = 1;
                                x2 = s;
                            }
                            else if (x > bitmapData.Width - s - 1) {
                                x1 = bitmapData.Width - s;
                                x2 = bitmapData.Width - 1;
                            }
                            if (y < s) {
                                y1 = 1;
                                y2 = s;
                            }
                            else if (y > bitmapData.Height - s - 1) {
                                y1 = bitmapData.Height - s;
                                y2 = bitmapData.Height - 1;
                            }
                            count = (x2 - x1) * (y2 - y1);
                            x1m = Math.Max(0, x1 - 1);
                            y1m = Math.Max(0, y1 - 1);
                            sum = intImg[x2, y2] - intImg[x1m, y2] - intImg[x2, y1m] + intImg[x1m, y1m];
                            if (currentLine[xBytes + 2] * count < sum * t) {
                                currentLine[xBytes] = 0;
                                currentLine[xBytes + 1] = 0;
                                currentLine[xBytes + 2] = 0;
                            }
                            else {
                                currentLine[xBytes] = 255;
                                currentLine[xBytes + 1] = 255;
                                currentLine[xBytes + 2] = 255;
                            }
                        }
                    });
                    bitmap.UnlockBits(bitmapData);
                }
            }
            catch (InvalidOperationException ex) {
                ShowErrorMsgBox(ex.Message);
            }
        }

        private void DrawSight(Graphics g, Pen pen, int x, int y, float md) {
            g.DrawLine(pen, x - md * 0.1f, y, x + md * 0.1f, y);
            g.DrawLine(pen, x, y - md * 0.1f, x, y + md * 0.1f);
            g.DrawEllipse(pen, x - md * 0.025f, y - md * 0.025f, md * 0.05f, md * 0.05f);
        }

        private void DrawGraph(Bitmap bmp, Cell[] cells, int maxDiffI, bool advanced = false) {
            int nonZeroCells = 0;
            for (int i = 0; i < cells.Length; i++) {
                if (cells[i].Value != 0)
                    nonZeroCells++;
            }
            int graphOffset = (int)(bmp.Width * 0.05f);
            int graphZeroX = graphOffset, graphZeroY = bmp.Height - graphOffset;
            int graphEndX = bmp.Width - graphOffset, graphEndY = graphOffset;
            string str;
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.DrawLine(Pens.Black, graphZeroX, graphZeroY, graphZeroX, graphEndY);
                g.DrawLine(Pens.Black, graphZeroX, graphZeroY, graphEndX, graphZeroY);
                g.DrawLine(Pens.Black, graphZeroX, graphEndY, graphZeroX - graphOffset / 5, graphEndY + graphOffset / 2);
                g.DrawLine(Pens.Black, graphZeroX, graphEndY, graphZeroX + graphOffset / 5, graphEndY + graphOffset / 2);
                g.DrawLine(Pens.Black, graphEndX, graphZeroY, graphEndX - graphOffset / 2, graphZeroY - graphOffset / 5);
                g.DrawLine(Pens.Black, graphEndX, graphZeroY, graphEndX - graphOffset / 2, graphZeroY + graphOffset / 5);
                Font graphFont = new Font(new FontFamily("Courier New"), 8, FontStyle.Regular);
                g.DrawString("i", graphFont, Brushes.Black, graphEndX - 2, graphZeroY - 2);
                g.DrawString("v", graphFont, Brushes.Black, graphZeroX - 10, graphEndY - 9);
                float max = cells[0].Value * 1.25f;
                if (advanced) {
                    int v_step = 32;
                    for (int i = graphZeroY - (int)((graphZeroY - graphEndY) / 1.25f); i < graphZeroY; i += v_step) {
                        g.DrawLine(Pens.Black, graphZeroX - graphOffset / 7.5f, i, graphZeroX + graphOffset / 7.5f, i);
                        str = ((int)(((graphZeroY - i) / ((graphZeroY - graphEndY) / 1.25f)) * m_size * m_size)).ToString();
                        SizeF measure1 = g.MeasureString(str + ' ', graphFont);
                        g.DrawString(str, graphFont, Brushes.Black, graphZeroX - graphOffset / 7.5f - measure1.Width, i - measure1.Height / 2);
                    }
                    g.DrawRectangle(Pens.Red, graphZeroX - 4, graphZeroY - (graphZeroY - graphEndY) * (cells[0].Value / max) - 4, 9, 9);
                    g.FillEllipse(Brushes.Blue, graphZeroX - 2, graphZeroY - (graphZeroY - graphEndY) * (cells[0].Value / max) - 2, 5, 5);
                    str = '(' + (cells[0].Index / 5 + 1).ToString() + ',' + (cells[0].Index % 5 + 1).ToString() + ')';
                    SizeF measure = g.MeasureString(str, graphFont);
                    g.DrawString(str, graphFont, Brushes.Black, graphZeroX - measure.Width / 2, graphZeroY - (graphZeroY - graphEndY) * (cells[0].Value / max) + measure.Height * 0.5f);
                }
                for (int i = 1; i < nonZeroCells; i++) {
                    g.DrawLine(Pens.Black, graphZeroX + (graphEndX - graphZeroX) * ((i - 1) / (float)nonZeroCells), graphZeroY - (graphZeroY - graphEndY) * (cells[i - 1].Value / max),
                        graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells), graphZeroY - (graphZeroY - graphEndY) * (cells[i].Value / max));
                    if (advanced) {
                        if (i < maxDiffI)
                            g.DrawRectangle(Pens.Red, graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells) - 4, graphZeroY - (graphZeroY - graphEndY) * (cells[i].Value / max) - 4, 9, 9);
                        g.FillEllipse(Brushes.Blue, graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells) - 2, graphZeroY - (graphZeroY - graphEndY) * (cells[i].Value / max) - 2, 5, 5);
                        str = '(' + (cells[i].Index / 5 + 1).ToString() + ',' + (cells[i].Index % 5 + 1).ToString() + ')';
                        SizeF measure = g.MeasureString(str, graphFont);
                        if (i % 2 == 0)
                            g.DrawString(str, graphFont, Brushes.Black, graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells) - measure.Width / 2, graphZeroY - (graphZeroY - graphEndY) * (cells[i].Value / max) + measure.Height * 0.5f);
                        else
                            g.DrawString(str, graphFont, Brushes.Black, graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells) - measure.Width / 2, graphZeroY - (graphZeroY - graphEndY) * (cells[i].Value / max) - measure.Height * 1.5f);
                        g.DrawLine(Pens.Black, graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells), graphZeroY - graphOffset / 7.5f, graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells), graphZeroY + graphOffset / 7.5f);
                        str = i.ToString();
                        g.DrawString(str, graphFont, Brushes.Black, graphZeroX + (graphEndX - graphZeroX) * (i / (float)nonZeroCells) - g.MeasureString(str, graphFont).Width / 2, graphZeroY + graphOffset / 7.5f);
                    }
                }
                g.DrawLine(Pens.Red, graphZeroX + (graphEndX - graphZeroX) * ((maxDiffI - 0.5f) / nonZeroCells), 0, graphZeroX + (graphEndX - graphZeroX) * ((maxDiffI - 0.5f) / nonZeroCells), bmp.Height - 1);
            }
            
        }

        private void DrawBlank(Bitmap bmp, Cell[] cells, int maxDiffI) {
            using (Graphics g = Graphics.FromImage(bmp)) {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));
                int boxSize = bmp.Height / 16;
                int space = 2;
                int midX = bmp.Width / 2 - ((10 + space -1) * boxSize - 1) / 2;
                int midY = bmp.Height / 2 - (15 * boxSize - 1) / 2;
                string str;
                for (int i = 0; i < 150; i++) {
                    int c = i % 5;
                    int r = (i % 75) / 5;
                    if (i >= 75)
                        c += 5 + space;
                    g.DrawRectangle(Pens.Black, midX + c * boxSize, midY + r * boxSize, boxSize - 4, boxSize - 4);
                    if (i % 5 == 0) {
                        str = (i / 5 + 1).ToString();
                        if (str.Length == 1)
                            str = '0' + str;
                        g.DrawString(str, new Font(new FontFamily("Courier New"), boxSize / 2, FontStyle.Regular), Brushes.Black, midX + c * boxSize - boxSize * 1.1f, midY + r * boxSize);
                    }
                }
                for (int i = 0; i < 150; i++) {
                    if ((i < maxDiffI && cells[i].Force == 0) || cells[i].Force == 1) {
                        int c = cells[i].Index % 5;
                        int r = (cells[i].Index % 75) / 5;
                        if (cells[i].Index >= 75)
                            c += 5 + space;
                        g.FillRectangle(Brushes.Red, midX + c * boxSize + 2, midY + r * boxSize + 2, boxSize - 7, boxSize - 7);
                    }
                }
            }
        }

        public void DrawToolGraph() {
            if (maxDiffI >= 0) {
                Bitmap graphBmp = new Bitmap(graphWindow.ClientSize.Width, graphWindow.ClientSize.Height);
                DrawGraph(graphBmp, cells, maxDiffI, true);
                graphWindow.BackgroundImage = graphBmp;
            }
        }

        private void pictureBoxChecked_Click(object sender, EventArgs e) {
            if (maxDiffI > 0) {
                MouseEventArgs me = (MouseEventArgs)e;
                Point coordinates = me.Location;
                Bitmap checkedBmp = new Bitmap(pictureBoxChecked.Width - 2, pictureBoxChecked.Height - 2);
                int boxSize = checkedBmp.Height / 16;
                int space = 2;
                int midX = checkedBmp.Width / 2 - ((10 + space - 1) * boxSize - 1) / 2;
                int midY = checkedBmp.Height / 2 - (15 * boxSize - 1) / 2;
                bool found = false;
                for (int i = 0; i < 150; i++) {
                    int c = i % 5;
                    int r = (i % 75) / 5;
                    if (i >= 75)
                        c += 5 + space;
                    if (coordinates.X > midX + c * boxSize && coordinates.X < midX + c * boxSize + boxSize - 4 &&
                        coordinates.Y > midY + r * boxSize && coordinates.Y < midY + r * boxSize + boxSize - 4) {
                        int j = 0;
                        for (j = 0; j < cells.Length; j++)
                            if (cells[j].Index == i)
                                break;
                        if (j < maxDiffI) {
                            if (cells[j].Force == 0)
                                cells[j].Force = -1;
                            else
                                cells[j].Force = 0;
                        }
                        else {
                            if (cells[j].Force == 0)
                                cells[j].Force = 1;
                            else
                                cells[j].Force = 0;
                        }
                        found = true;
                        break;
                    }
                }
                if (found) {
                    DrawBlank(checkedBmp, cells, maxDiffI);
                    pictureBoxChecked.Image = checkedBmp;
                    formListBox();
                }
            }
        }

        public void formListBox() {
            listBoxChecked.Items.Clear();
            List<Cell> lbCells = new List<Cell>();
            for (int i = 0; i < cells.Length; i++) {
                if ((i < maxDiffI && cells[i].Force == 0) || cells[i].Force == 1)
                    lbCells.Add(cells[i]);
            }
            lbCells.Sort(cic);
            foreach (Cell cell in lbCells)
                listBoxChecked.Items.Add(cell.ToString());
            lbCells.Clear();
        }

        public void pictureBoxGraph_Click(object sender, EventArgs e) {
            if (maxDiffI >= 0) {
                graphWindow.ShowDialog(this);
            }
        }

        private void Recognize(Bitmap bitmap) {
            try {
                // http://csharpexamples.com/fast-image-processing-c/
                unsafe {
                    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                    int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                    int widthInBytes = bitmapData.Width * bytesPerPixel;
                    byte* ptrFirstPixel = (byte*)bitmapData.Scan0; // BGR
                    int leftMarkerX = -1, leftMarkerY = -1, rightMarkerX = -1, rightMarkerY = -1;
                    m_size = (int)(bitmapData.Width * 0.0385f);
                    int m_min = 255 * m_size * m_size, m_cur;
                    int m_sy = 1;
                    int m_fy = (int)(bitmapData.Height * 0.5f);
                    for (int x = m_size + 1 + (int)(bitmapData.Width * 0.03f); x < bitmapData.Width / 2; x++)
                        for (int y = m_sy + m_size; y < m_fy; y++) {
                            m_cur = intImg[x, y] - intImg[x - m_size - 1, y] - intImg[x, y - m_size - 1] + intImg[x - m_size - 1, y - m_size - 1];
                            if (m_cur < m_min) {
                                m_min = m_cur;
                                leftMarkerX = x - m_size / 2;
                                leftMarkerY = y - m_size / 2;
                            }
                        }
                    m_min = 255 * m_size * m_size;
                    for (int x = bitmapData.Width / 2 + m_size + 1; x < (int)(bitmapData.Width * 0.97f); x++)
                        for (int y = m_sy + m_size; y < m_fy; y++) {
                            m_cur = intImg[x, y] - intImg[x - m_size - 1, y] - intImg[x, y - m_size - 1] + intImg[x - m_size - 1, y - m_size - 1];
                            if (m_cur < m_min) {
                                m_min = m_cur;
                                rightMarkerX = x - m_size / 2;
                                rightMarkerY = y - m_size / 2;
                            }
                        }
                    labelLeftMarker.Text = "Left marker: (" + leftMarkerX.ToString() + "; " + leftMarkerY.ToString() + ")";
                    labelRightMarker.Text = "Right marker: (" + rightMarkerX.ToString() + "; " + rightMarkerY.ToString() + ")";
                    cells = new Cell[150];
                    for (int i = 0; i < 150; i++)
                        cells[i] = new Cell(i);
                    float angle = (float)(Math.Atan2(leftMarkerY - rightMarkerY, leftMarkerX - rightMarkerX) + Math.PI);
                    labelAngle.Text = "Rotation: " + angle.ToString() + '°';
                    float markersDistance = (float)Math.Sqrt((leftMarkerX - rightMarkerX) * (leftMarkerX - rightMarkerX) + (leftMarkerY - rightMarkerY) * (leftMarkerY - rightMarkerY));
                    labelMarkersDistance.Text = "Distance between markers: " + ((int)markersDistance).ToString();
                    float sina = (float)Math.Sin(angle);
                    float cosa = (float)Math.Cos(angle);
                    // x * cosa - y * sina;
                    // x * sina + y * cosa;
                    int firstBoxOffsetX = (int)(0.064543f * markersDistance);
                    int firstBoxOffsetY = (int)(0.176471f * markersDistance);
                    float boxOffsetX = 0.080882f * markersDistance;
                    float boxOffsetY = 0.0423f * markersDistance;
                    int boxInnerSideHalf = (int)(0.01f * markersDistance);
                    int columnOffsetX = (int)(0.677288 * markersDistance);
                    Parallel.For(0, 150, i => {
                        int bx, by, tx, ty, cx, cy, x, y, xBytes;
                        byte* currentLine;
                        bx = i % 5;
                        by = i / 5;
                        if (by < 15)
                            cx = (int)(firstBoxOffsetX + boxOffsetX * bx);
                        else {
                            cx = (int)(columnOffsetX + boxOffsetX * bx);
                            by -= 15;
                        }
                        cy = (int)(firstBoxOffsetY + boxOffsetY * by);
                        for (int ix = -boxInnerSideHalf; ix <= boxInnerSideHalf; ix++)
                            for (int iy = -boxInnerSideHalf; iy <= boxInnerSideHalf; iy++) {
                                tx = cx + ix;
                                ty = cy + iy;
                                x = (int)(tx * cosa - ty * sina + leftMarkerX);
                                y = (int)(tx * sina + ty * cosa + leftMarkerY);
                                xBytes = x * bytesPerPixel;
                                if (x > 0 && x < bitmapData.Width && y > 0 && y < bitmapData.Height) {
                                    currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                                    currentLine[xBytes + 0] = (byte)Math.Max(0, currentLine[xBytes + 0] - 75);
                                    currentLine[xBytes + 1] = (byte)Math.Max(0, currentLine[xBytes + 1] - 25);
                                    currentLine[xBytes + 2] = (byte)Math.Max(0, currentLine[xBytes + 2] - 50);
                                    if (currentLine[xBytes] == 0)
                                        cells[i].Value++;
                                }
                            }
                    });
                    bitmap.UnlockBits(bitmapData);
                    using (Graphics g = Graphics.FromImage(bitmap)) {
                        Pen redBold = new Pen(Brushes.Red, 3.0f);
                        DrawSight(g, redBold, leftMarkerX, leftMarkerY, markersDistance);
                        DrawSight(g, redBold, rightMarkerX, rightMarkerY, markersDistance);
                    }
                    Array.Sort(cells);
                    Array.Reverse(cells);
                    int maxDiff = 0;
                    for (int i = 1; i < cells.Length; i++) {
                            if (cells[i - 1].Value - cells[i].Value > maxDiff) {
                                maxDiff = cells[i - 1].Value - cells[i].Value;
                                maxDiffI = i;
                            }
                    }
                    Array.Sort(cells, 0, maxDiffI, cic);
                    formListBox();
                    Array.Sort(cells);
                    Array.Reverse(cells);
                    Bitmap graphBmp = new Bitmap(pictureBoxGraph.Width - 2, pictureBoxGraph.Height - 2);
                    DrawGraph(graphBmp, cells, maxDiffI);
                    pictureBoxGraph.Image = graphBmp;
                    Bitmap checkedBmp = new Bitmap(pictureBoxChecked.Width - 2, pictureBoxChecked.Height - 2);
                    DrawBlank(checkedBmp, cells, maxDiffI);
                    pictureBoxChecked.Image = checkedBmp;
                }
            }
            catch (InvalidOperationException ex) {
                ShowErrorMsgBox(ex.Message);
            }
        }
    }
}
