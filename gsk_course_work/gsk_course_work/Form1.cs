using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gsk_course_work
{
    public partial class Form1 : Form
    {
        Color figureColor = Color.Black;
        Bitmap tempCanvas;
        Graphics g;
        List<Figure> figures;
        int FigOption = -1;
        int Operation = 0; //текущая операция
        // -1 - случайный клик (никакая операция)
        // 0 - ввод фигур
        // 1 - выделение
        // 2 - перемещение
        // 3 - Поворот вокруг заданного центра на произвольный угол
        // 4 - Зеркальное отражение относительно центра фигуры
        // 5 - Зеркальное отражение относительно вертикальной прямой
        List<PointF> customPoints = new List<PointF>();
        int countPoints = 0;
        int chosenFigure = -1;
        Point previousLocation = new Point();
        PointF chosenCenter = new PointF(-1, -1);
        int verLineX = -1;
        string helpCenterString = "Выберите центр, кликнув на форме";
        string helpVerLineString = "Выберите X для вертикальной прямой";
        int prevAngle = 0;

        Spline spline;
        Segment segment;
        Polygon polygon;
        public Form1()
        {
            InitializeComponent();
            colorPreview.BackColor = Color.Black;
            canvas.BackColor = Color.White;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            tempCanvas = new Bitmap(canvas.Width, canvas.Height);  
            g = Graphics.FromImage(tempCanvas);
            figures = new List<Figure>();
            helpLabel.Visible = false;
            angleTrackBar.Visible = false;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        private void ShowClrDlg_Click(object sender, EventArgs e)
        {
            if(colorDialog.ShowDialog() == DialogResult.OK)
            {
                figureColor = colorDialog.Color;
                colorPreview.BackColor = colorDialog.Color;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            figures.Clear();
            canvas.Image = tempCanvas;
        }

        private void AddFigure(Point e)
        {
            //Polygon polygon = new Polygon(figureColor, g);
            switch (FigOption)
            { //какой тип фигуры
                case 0:
                    {
                        if (spline == null) spline = new Spline(figureColor, g);
                        customPoints.Add(new Point(e.X, e.Y));
                        countPoints++;
                        if (countPoints == 4)
                        {
                            spline.VertexList = customPoints.ConvertAll(item => new PointF(item.X, item.Y));
                            figures.Add(spline);
                            countPoints = 0;
                            customPoints.Clear();
                            spline = null;
                        }
                    }
                    break;
                case 1:
                    {
                        if(polygon == null) polygon = new Polygon(figureColor, g);
                        polygon.CalcTrianglePoints(e);
                        figures.Add(polygon);
                        polygon = null;
                    }
                    break;
                case 2:
                    {
                        if (polygon == null) polygon = new Polygon(figureColor, g);
                        polygon.CalcFlagPoints(e);
                        figures.Add(polygon);
                        polygon = null;
                    }
                    break;
                case 3:
                    {
                        if (segment == null) segment = new Segment(figureColor, g);
                        customPoints.Add(new Point(e.X, e.Y));
                        countPoints++;
                        if (countPoints == 2)
                        {
                            segment.VertexList = customPoints.ConvertAll(item => new PointF(item.X, item.Y));
                            figures.Add(segment);
                            customPoints.Clear();
                            countPoints = 0;
                            segment = null;
                        }
                    }
                    break;
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            //Operation = 0;
            if(e.Button == MouseButtons.Right)
            {
                chosenFigure = -1;
                for (int i = 0; i < figures.Count; i++)
                {
                    if (figures[i].ThisFigure(e.Location))
                    {
                        figuresContextMenu.Show(this, new Point(e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top));
                        chosenFigure = i;
                        Operation = 1;
                    }
                }
                //Operation = 1;
            }
            switch (Operation)
            {
                case 0:
                    {
                        chosenFigure = -1;
                        AddFigure(e.Location);
                    }
                    break;
                case 2:
                    {
                        previousLocation = e.Location;
                    }
                    break;
                case 3:
                    {
                        if(chosenCenter.X == -1)
                        {
                            prevAngle = 0;
                            chosenCenter = e.Location;
                            helpLabel.Visible = false;
                            angleTrackBar.Visible = true;
                        }
                        else
                        {
                            prevAngle = 0;
                            chosenCenter = new PointF(-1, -1);
                            angleTrackBar.Value = 0;
                            angleTrackBar.Visible = false;
                            chosenFigure = -1;
                            Operation = -1; //никакая операция
                        }
                    }
                    break;
                case 4:
                    {
                        if(chosenCenter.X != -1)
                        {
                            chosenCenter = new PointF(-1, -1);
                            chosenFigure = -1;
                            Operation = -1;
                        }
                    }
                    break;
                case 5:
                    {
                        if(verLineX == -1)
                        {
                            verLineX = e.Location.X;
                            helpLabel.Visible = false;
                        }
                        else
                        {
                            figures[chosenFigure].VerLineReflection(new PointF(verLineX, 0));
                            chosenFigure = -1;
                            verLineX = -1;
                            Operation = -1;
                        }
                    }
                    break;
            }
            Redraw();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                switch (Operation)
                {
                    case 2:
                        {
                            if (chosenFigure != -1)
                            {
                                figures[chosenFigure].MoveFigure(e.X - previousLocation.X, e.Y - previousLocation.Y);
                            }
                        }
                        break;
                }
                Redraw();
                previousLocation = e.Location;
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                switch (Operation)
                {
                    case 2:
                        {
                            Operation = 0;
                            chosenFigure = -1;
                            FigOption = -1;
                        }
                        break;
                }
                Redraw();
                previousLocation = e.Location;
            }
        }

        private void Redraw()
        {
            g.Clear(Color.White);
            for (int i = 0; i < figures.Count; i++)
            {
                figures[i].DrawFigure();
                if (chosenFigure == i) figures[i].DrawSelection();
                if (chosenCenter.X != -1)
                {
                    g.DrawEllipse(new Pen(Color.Black), chosenCenter.X - 2, chosenCenter.Y - 2, 4, 4);
                    g.DrawEllipse(new Pen(Color.White), chosenCenter.X - 3, chosenCenter.Y - 3, 6, 6);
                    g.DrawEllipse(new Pen(Color.Black), chosenCenter.X - 4, chosenCenter.Y - 4, 8, 8);
                }
                if(verLineX != -1)
                {
                    g.DrawLine(new Pen(Color.Black), verLineX - 1, 0, verLineX - 1, canvas.Bottom);
                    g.DrawLine(new Pen(Color.White), verLineX, 0, verLineX, canvas.Bottom);
                    g.DrawLine(new Pen(Color.Black), verLineX + 1, 0, verLineX + 1, canvas.Bottom);
                }
            }
            canvas.Image = tempCanvas;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FigOption = 0;
            Operation = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FigOption = 1;
            Operation = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FigOption = 2;
            Operation = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FigOption = 3;
            Operation = 0;
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figures.RemoveAt(chosenFigure);
            chosenFigure = -1;
            Redraw();
        }

        private void переместитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = 2;
        }

        private void поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = 3;
            helpLabel.Text = helpCenterString;
            helpLabel.Visible = true;
        }

        private void зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = 4;
            chosenCenter = figures[chosenFigure].GetCenter();
            figures[chosenFigure].PointReflection(chosenCenter);
            chosenCenter = new PointF(-1, -1);
            Redraw();
        }

        private void зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = 5;
            helpLabel.Text = helpVerLineString;
            helpLabel.Visible = true;
        }

        private void AngleTrackBar_Scroll(object sender, EventArgs e)
        {
            if(chosenFigure != -1)
            {
                figures[chosenFigure].RotateFigure(angleTrackBar.Value - prevAngle, chosenCenter);
                prevAngle = angleTrackBar.Value;
                Redraw();
            }
        }
    }
}
