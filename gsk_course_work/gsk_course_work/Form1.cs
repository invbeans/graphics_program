using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        //ФИГА ФИГБ, и код операции ;))) список такой структуры будет просто рисоваться в редрав
        //если есть элементы будет просить класс ТМОхендлер нарисовать их
        //СВЯЗАТЬ УДАЛЕНИЕ ИЗ ОБОИХ СПИСКОВ
        List<TMOFigure> TMOFigures;
        int FigOption = -1;
        int Operation = 0; //текущая операция
        // -1 - случайный клик (никакая операция)
        // 0 - ввод фигур
        // 1 - выделение
        // 2 - перемещение
        // 3 - Поворот вокруг заданного центра на произвольный угол
        // 4 - Зеркальное отражение относительно центра фигуры
        // 5 - Зеркальное отражение относительно вертикальной прямой
        // 6 - Выбор двух фигур для ТМО
        List<PointF> customPoints = new List<PointF>();
        int countPoints = 0;
        int commonChosen = -1;
        int TMOChosen = -1;
        Point previousLocation = new Point();
        PointF chosenCenter = new PointF(-1, -1);
        Point verLinePoint = new Point(-1, -1);
        string helpCenterString = "Выберите центр, кликнув на форме";
        string helpVerLineString = "Выберите X для вертикальной прямой";
        int prevAngle = 0;
        int chooseCount = 0;
        int indexA = -1, indexB = -1;
        int TMOCode = -1;

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
            angleTrackBar.MouseUp += AngleTrackBar_MouseUp;

            tempCanvas = new Bitmap(canvas.Width, canvas.Height);
            g = Graphics.FromImage(tempCanvas);

            figures = new List<Figure>();
            TMOFigures = new List<TMOFigure>();

            helpLabel.Visible = false;
            angleTrackBar.Visible = false;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        private void ShowClrDlg_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                figureColor = colorDialog.Color;
                colorPreview.BackColor = colorDialog.Color;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            figures.Clear();
            TMOFigures.Clear();
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
                        if (polygon == null) polygon = new Polygon(figureColor, g);
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
            if (e.Button == MouseButtons.Right)
            {
                commonChosen = -1; TMOChosen = -1;
                for (int i = 0; i < figures.Count; i++)
                {
                    if (figures[i].ThisFigure(e.Location))
                    {
                        figuresContextMenu.Show(this, new Point(e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top));
                        commonChosen = i;
                        Operation = 1;
                        break;
                    }
                }
                if(commonChosen == -1)
                {
                    for (int i = 0; i < TMOFigures.Count; i++)
                    {
                        if (TMOFigures[i].FigA.ThisFigure(e.Location) || TMOFigures[i].FigB.ThisFigure(e.Location))
                        {
                            figuresContextMenu.Show(this, new Point(e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top));
                            TMOChosen = i;
                            Operation = 1;
                            break;
                        }
                    }
                }
                //Operation = 1;
            }
            switch (Operation)
            {
                case -1:
                    {
                        indexA = -1; indexB = -1;
                    }
                    break;
                case 0:
                    {
                        commonChosen = -1;
                        TMOChosen = -1;
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
                        if (chosenCenter.X == -1)
                        {
                            prevAngle = 0;
                            chosenCenter = e.Location;
                            helpLabel.Visible = false;
                            angleTrackBar.Visible = true;
                        }
                        /*else
                        {
                            prevAngle = 0;
                            chosenCenter = new PointF(-1, -1);
                            angleTrackBar.Value = 0;
                            angleTrackBar.Visible = false;
                            chosenFigure = -1;
                            Operation = -1; //никакая операция
                        }*/
                    }
                    break;
                /*case 4:
                    {
                        if(chosenCenter.X != -1)
                        {
                            chosenCenter = new PointF(-1, -1);
                            chosenFigure = -1;
                            Operation = -1;
                        }
                    }
                    break;*/
                case 5:
                    {
                        if (verLinePoint.X == -1)
                        {
                            verLinePoint.X = e.Location.X;
                            helpLabel.Visible = false;
                            previousLocation = e.Location;
                            verLinePoint.Y = e.Location.Y;
                        }
                        /*else
                        {
                            previousLocation = e.Location;
                            verLinePoint.Y = e.Location.Y;
                        }*/
                        /*else
                        {
                            figures[chosenFigure].VerLineReflection(new PointF(verLineX, 0));
                            chosenFigure = -1;
                            verLineX = -1; verLinePoint = new Point(-1, -1);
                            Operation = -1;
                        }*/
                    }
                    break;
                case 6:
                    {
                        //нужно проверять, какая фигура выделена (индекс), многоуг ли
                        //это, сколько фигур (1я и 2я, не больше)
                        
                        for (int i = 0; i < figures.Count; i++)
                        {
                            if (figures[i].ThisFigure(e.Location) && figures[i].GetType() == typeof(Polygon))
                            {
                                if (indexA == -1) indexA = i;
                                else indexB = i;
                                chooseCount++;
                                if (chooseCount == 2)
                                {
                                    chooseCount = 0;
                                    Operation = -1;
                                }
                            }
                        }
                    }
                    break;
            }
            Redraw();
            if(indexB != -1)
            {
                tmoContextMenu.Show(this, new Point(e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top));
                //figures[indexA].inTMO = true; figures[indexB].inTMO = true;
                //TMOFigures.Add(new TMOFigure(figures[indexA], figures[indexB], TMOCode));
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (Operation)
                {
                    case 2:
                        {
                            if (commonChosen != -1)
                            {
                                figures[commonChosen].MoveFigure(e.X - previousLocation.X, e.Y - previousLocation.Y);
                            }
                            if(TMOChosen != -1)
                            {
                                TMOFigures[TMOChosen].FigA.MoveFigure(e.X - previousLocation.X, e.Y - previousLocation.Y);
                                TMOFigures[TMOChosen].FigB.MoveFigure(e.X - previousLocation.X, e.Y - previousLocation.Y);
                            }
                        }
                        break;
                        /*case 5:
                            {
                                //verLinePoint.Y = e.Location.Y;
                                previousLocation = e.Location;

                            }
                            break;*/
                }
                Redraw();
                previousLocation = e.Location;
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (Operation)
                {
                    case 2:
                        {
                            Operation = 0;
                            commonChosen = -1;
                            TMOChosen = -1;
                            FigOption = -1;
                        }
                        break;
                    case 5:
                        {
                            if(commonChosen != -1) figures[commonChosen].VerLineReflection(new PointF(verLinePoint.X, 0));
                            if(TMOChosen != -1)
                            {
                                TMOFigures[TMOChosen].FigA.VerLineReflection(new PointF(verLinePoint.X, 0));
                                TMOFigures[TMOChosen].FigB.VerLineReflection(new PointF(verLinePoint.X, 0));
                            }
                            commonChosen = -1; TMOChosen = -1;
                            verLinePoint = new Point(-1, -1);
                            Operation = -1;
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
            TMOHandler handler;
            //рисование обычных фигур и тмо будут накладываться, как-то поменять код
            for (int i = 0; i < TMOFigures.Count; i++)
            {
                if (TMOChosen == i)
                {
                    handler = new TMOHandler(TMOFigures[i].FigA, TMOFigures[i].FigB, TMOFigures[i].TMOCode, canvas.Width, g);
                    handler.DrawSelection();
                }
            }
            for (int i = 0; i < figures.Count; i++)
            { 
                if (i == indexA || i == indexB)
                {
                    figures[i].DrawSelection();
                }
                figures[i].DrawFigure();
                if (commonChosen == i) figures[i].DrawSelection();                
            }
            for (int i = 0; i < TMOFigures.Count; i++)
            {
                handler = new TMOHandler(TMOFigures[i].FigA, TMOFigures[i].FigB, TMOFigures[i].TMOCode, canvas.Width, g);
                handler.DrawTMO();
            }
            if (chosenCenter.X != -1)
            {
                g.DrawEllipse(new Pen(Color.Black), chosenCenter.X - 2, chosenCenter.Y - 2, 4, 4);
                g.DrawEllipse(new Pen(Color.White), chosenCenter.X - 3, chosenCenter.Y - 3, 6, 6);
                g.DrawEllipse(new Pen(Color.Black), chosenCenter.X - 4, chosenCenter.Y - 4, 8, 8);
            }
            if (verLinePoint.Y != -1)
            {
                float[] dashPattern = { 5, 5 };
                Pen verLinePen = new Pen(Color.Gray);
                verLinePen.DashPattern = dashPattern;
                g.DrawLine(verLinePen, verLinePoint.X, verLinePoint.Y, verLinePoint.X, previousLocation.Y);
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
            //взмж нужен еще один индекс из другой истории
            if(commonChosen != -1) figures.RemoveAt(commonChosen);
            if(TMOChosen != -1) TMOFigures.RemoveAt(TMOChosen);
            commonChosen = -1; TMOChosen = -1;
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
            //добавить для тмо
            Operation = -1;
            if (commonChosen != -1)
            {
                chosenCenter = figures[commonChosen].GetCenter();
                figures[commonChosen].PointReflection(chosenCenter);
            }
            if(TMOChosen != -1)
            {
                TMOHandler handler = new TMOHandler(TMOFigures[TMOChosen].FigA, TMOFigures[TMOChosen].FigB, TMOFigures[TMOChosen].TMOCode, canvas.Width, g);
                chosenCenter = handler.GetTMOCenter();
                TMOFigures[TMOChosen].FigA.PointReflection(chosenCenter);
                TMOFigures[TMOChosen].FigB.PointReflection(chosenCenter);
            }
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
            if(commonChosen != -1)
            {
                figures[commonChosen].RotateFigure(angleTrackBar.Value - prevAngle, chosenCenter);
                prevAngle = angleTrackBar.Value;
                Redraw();
            }
            if(TMOChosen != -1)
            {
                TMOFigures[TMOChosen].FigA.RotateFigure(angleTrackBar.Value - prevAngle, chosenCenter);
                TMOFigures[TMOChosen].FigB.RotateFigure(angleTrackBar.Value - prevAngle, chosenCenter);
                prevAngle = angleTrackBar.Value;
                Redraw();
            }
        }

        private void AngleTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            prevAngle = 0;
            chosenCenter = new PointF(-1, -1);
            angleTrackBar.Value = 0;
            angleTrackBar.Visible = false;
            commonChosen = -1;
            TMOChosen = -1;
            Operation = -1; //никакая операция
            Redraw();
        }

        private void ChoseOperands_Click(object sender, EventArgs e)
        {
            Operation = 6;
        }

        private void объединениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMOCode = 1;
            TMOFigures.Add(new TMOFigure(figures[indexA], figures[indexB], TMOCode));
            //figures[indexA].inTMO = true; figures[indexB].inTMO = true;
            if(indexA > indexB)
            {
                figures.RemoveAt(indexB);
                figures.RemoveAt(indexA - 1);
            }
            else
            {
                figures.RemoveAt(indexA);
                figures.RemoveAt(indexB - 1);
            }
            indexA = -1; indexB = -1;
            Redraw();
        }

        private void разностьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMOCode = 2;
            TMOFigures.Add(new TMOFigure(figures[indexA], figures[indexB], TMOCode));
            //figures[indexA].inTMO = true; figures[indexB].inTMO = true;
            if (indexA > indexB)
            {
                figures.RemoveAt(indexB);
                figures.RemoveAt(indexA - 1);
            }
            else
            {
                figures.RemoveAt(indexA);
                figures.RemoveAt(indexB - 1);
            }
            indexA = -1; indexB = -1;
            Redraw();
        }
    }
}
