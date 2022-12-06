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
        List<TMOFigure> TMOFigures;
        int FigOption = -1;
        int Operation = 0; //текущая операция
        // -1 - случайный клик (операция не выбрана)
        // 0 - ввод фигур
        // 1 - выделение
        // 2 - перемещение
        // 3 - поворот вокруг заданного центра на произвольный угол
        // 4 - зеркальное отражение относительно центра фигуры
        // 5 - зеркальное отражение относительно вертикальной прямой
        // 6 - выбор двух фигур для ТМО
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
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        //метод-обработчик диалогового окна выбора цвета
        private void ShowClrDlg_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                figureColor = colorDialog.Color;
                colorPreview.BackColor = colorDialog.Color;
            }
        }

        //метод-обработчик кнопки очистки формы
        private void ClearButton_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            figures.Clear();
            TMOFigures.Clear();
            canvas.Image = tempCanvas;
            Operation = -1;
        }

        private void AddSpline(Point e)
        {
            if (spline == null) spline = new Spline(figureColor, g);
            customPoints.Add(new Point(e.X, e.Y));
            countPoints++;
            if (countPoints == 4)
            {
                //когда набирается 4 точки, список копируется в поле новой фигуры и буфер очищается
                spline.VertexList = customPoints.ConvertAll(item => new PointF(item.X, item.Y));
                figures.Add(spline);
                countPoints = 0;
                customPoints.Clear();
                spline = null;
            }
        }

        private void AddTriangle(Point e) {
            if (polygon == null) polygon = new Polygon(figureColor, g);
            //треугольник рисуется вокруг выбранной точки
            polygon.CalcTrianglePoints(e);
            figures.Add(polygon);
            polygon = null;
        }

        private void AddFlag(Point e)
        {
            if (polygon == null) polygon = new Polygon(figureColor, g);
            //флаг рисуется вокруг выбранной точки
            polygon.CalcFlagPoints(e);
            figures.Add(polygon);
            polygon = null;
        }

        private void AddSegment(Point e)
        {
            if (segment == null) segment = new Segment(figureColor, g);
            customPoints.Add(new Point(e.X, e.Y));
            countPoints++;
            if (countPoints == 2)
            {
                //когда набирается 2 точки, список копируется в поле новой фигуры и буфер очищается
                segment.VertexList = customPoints.ConvertAll(item => new PointF(item.X, item.Y));
                figures.Add(segment);
                customPoints.Clear();
                countPoints = 0;
                segment = null;
            }
        }
        
        //метод добавления новой фигуры на форму
        private void AddFigure(Point e)
        {
            //в зависимости от выбранного типа фигуры
            switch (FigOption)
            { 
                case 0: //сплайн
                    {
                        AddSpline(e);
                    }
                    break;
                case 1: //треугольник
                    {
                        AddTriangle(e);
                    }
                    break;
                case 2: //флаг
                    {
                        AddFlag(e);
                    }
                    break;
                case 3: //отрезок
                    {
                        AddSegment(e);
                    }
                    break;
            }
        }

        //метод обработчик нажатия мыши на pictureBox
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            //если клик правой мыши
            if (e.Button == MouseButtons.Right)
            {
                commonChosen = -1; TMOChosen = -1;
                //проверка, находится ли выбранная точка в одной из фигур
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
                //либо в одной из фигур, образованных ТМО
                if(commonChosen == -1)
                {
                    TMOHandler handler;
                    for (int i = 0; i < TMOFigures.Count; i++)
                    {
                        handler = new TMOHandler(TMOFigures[i], canvas.Width, g);
                        if (handler.ThisFigure(e.Location))
                        {
                            figuresContextMenu.Show(this, new Point(e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top));
                            TMOChosen = i;
                            Operation = 1;
                            break;
                        }
                    }
                }
            }
            //если клик левой кнопкой мыши
            else
            {
                switch (Operation)
                {
                    case -1:
                        {
                            indexA = -1; indexB = -1;
                        }
                        break;
                    case 0:
                        {
                            indexA = -1; indexB = -1;
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
                        }
                        break;
                    case 5:
                        {
                            if (verLinePoint.X == -1)
                            {
                                verLinePoint.X = e.Location.X;
                                helpLabel.Visible = false;
                                previousLocation = e.Location;
                                verLinePoint.Y = e.Location.Y;
                            }
                        }
                        break;
                    case 6:
                        {
                            for (int i = 0; i < figures.Count; i++)
                            {
                                if (figures[i].ThisFigure(e.Location) && figures[i].GetType() == typeof(Polygon))
                                {
                                    if (indexA == -1)
                                    {
                                        indexA = i;
                                        chooseCount = 1;
                                        break;
                                    }
                                    else if (indexA != i)
                                    {
                                        indexB = i;
                                        chooseCount = 2;
                                        break;
                                    }
                                    if (chooseCount == 2)
                                    {
                                        chooseCount = 0;
                                        Operation = -1;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            
            Redraw();
            //если выбраны обе фигуры операнда для ТМО
            if(indexB != -1)
            {
                tmoContextMenu.Show(this, new Point(e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top));
            }
        }

        //метод обработчик перемещения курсора мыши по pictureBox
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
                }
                Redraw();
                previousLocation = e.Location;
            }
        }

        //метод обработчик поднятия клавиши мыши над pictureBox
        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (Operation)
                {
                    case 2:
                        {
                            
                            commonChosen = -1;
                            TMOChosen = -1;
                            FigOption = -1;
                            Operation = -1;
                        }
                        break;
                    case 5:
                        {
                            //зеркальное отражение относительно вертикальной прямой, когда пользователь закончит рисовать прямую
                            if (commonChosen != -1)
                            {
                                figures[commonChosen].VerLineReflection(new PointF(verLinePoint.X, 0));
                                commonChosen = -1;
                            }
                            if(TMOChosen != -1)
                            {
                                TMOFigures[TMOChosen].FigA.VerLineReflection(new PointF(verLinePoint.X, 0));
                                TMOFigures[TMOChosen].FigB.VerLineReflection(new PointF(verLinePoint.X, 0));
                                TMOChosen = -1;
                            }
                            Operation = -1;
                            verLinePoint = new Point(-1, -1);
                        }
                        break;
                }
                //Operation = -1;
                Redraw();
                previousLocation = e.Location;
            }
        }

        private void DrawTMOFigures()
        {
            TMOHandler handler;
            for (int i = 0; i < TMOFigures.Count; i++)
            {
                handler = new TMOHandler(TMOFigures[i], canvas.Width, g);
                if (TMOChosen == i)
                {
                    handler.DrawSelection();
                }
                handler.DrawTMO();
            }
        }

        private void DrawPrimitives()
        {
            for (int i = 0; i < figures.Count; i++)
            {
                if (i == indexA || i == indexB)
                {
                    figures[i].DrawSelection();
                }
                figures[i].DrawFigure();
                if (commonChosen == i) figures[i].DrawSelection();
            }
        }

        private void DrawCenter()
        {
            if (chosenCenter.X != -1)
            {
                g.DrawEllipse(new Pen(Color.Black), chosenCenter.X - 2, chosenCenter.Y - 2, 4, 4);
                g.DrawEllipse(new Pen(Color.White), chosenCenter.X - 3, chosenCenter.Y - 3, 6, 6);
                g.DrawEllipse(new Pen(Color.Black), chosenCenter.X - 4, chosenCenter.Y - 4, 8, 8);
            }
        }

        private void DrawVerLine()
        {
            if (verLinePoint.Y != -1)
            {
                float[] dashPattern = { 5, 5 };
                Pen verLinePen = new Pen(Color.Gray);
                verLinePen.DashPattern = dashPattern;
                g.DrawLine(verLinePen, verLinePoint.X, verLinePoint.Y, verLinePoint.X, previousLocation.Y);
            }
        }

        //метод перерисовки
        private void Redraw()
        {
            g.Clear(Color.White);
            //рисование фигур ТМО и области выделения, если выбрана фигура
            DrawTMOFigures();
            //рисование фигур примитивов и области выделения, если выбрана фигура
            DrawPrimitives();
            //если есть выбранный пользователем центр, он рисуется
            DrawCenter();
            //рисование прямой, которую проводит пользователь для зеркального отражения от прямой
            DrawVerLine();
            canvas.Image = tempCanvas;
        }

        //метод обработчик выбора фигуры: сплайн
        private void SplineButton_Click(object sender, EventArgs e)
        {
            FigOption = 0;
            Operation = 0;
        }

        //метод обработчик выбора фигуры: треугольник
        private void TriangleButton_Click(object sender, EventArgs e)
        {
            FigOption = 1;
            Operation = 0;
        }

        //метод обработчик выбора фигуры: флаг
        private void FlagButton_Click(object sender, EventArgs e)
        {
            FigOption = 2;
            Operation = 0;
        }

        //метод обработчик выбора фигуры: отрезок
        private void SegmentButton_Click(object sender, EventArgs e)
        {
            FigOption = 3;
            Operation = 0;
        }

        //метод обработчик пункта контекстного меню фигур - удаления
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(commonChosen != -1) figures.RemoveAt(commonChosen);
            if(TMOChosen != -1) TMOFigures.RemoveAt(TMOChosen);
            commonChosen = -1; TMOChosen = -1;
            Redraw();
        }

        //метод обработчик пункта контекстного меню фигур - перемещение
        private void MoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = 2;
        }

        //метод обработчик пункта контекстного меню фигур - поворот
        private void RotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = 3;
            //появляется подсказка
            helpLabel.Text = helpCenterString;
            helpLabel.Visible = true;
        }

        //метод обработчик пункта контекстного меню фигур - отражение относительно центра фигуры
        private void PointReflectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = -1;
            if (commonChosen != -1)
            {
                chosenCenter = figures[commonChosen].GetCenter();
                figures[commonChosen].PointReflection(chosenCenter);
                commonChosen = -1;
            }
            if(TMOChosen != -1)
            {
                TMOHandler handler = new TMOHandler(TMOFigures[TMOChosen], canvas.Width, g);
                chosenCenter = handler.GetTMOCenter();
                TMOFigures[TMOChosen].FigA.PointReflection(chosenCenter);
                TMOFigures[TMOChosen].FigB.PointReflection(chosenCenter);
                TMOChosen = -1;
            }
            chosenCenter = new PointF(-1, -1);
            Redraw();
        }

        //метод обработчик пункта контекстного меню фигур - отражение относительно вертикальной прямой
        private void VerLineReflectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = 5;
            helpLabel.Text = helpVerLineString;
            helpLabel.Visible = true;
        }

        //метод обработчик перемещения ползунка трекбара поворота фигур
        private void AngleTrackBar_Scroll(object sender, EventArgs e)
        {
            if(commonChosen != -1)
            {
                figures[commonChosen].RotateFigure(angleTrackBar.Value - prevAngle, chosenCenter);
            }
            if(TMOChosen != -1)
            {
                TMOFigures[TMOChosen].FigA.RotateFigure(angleTrackBar.Value - prevAngle, chosenCenter);
                TMOFigures[TMOChosen].FigB.RotateFigure(angleTrackBar.Value - prevAngle, chosenCenter);
            }
            prevAngle = angleTrackBar.Value;
            Redraw();
        }

        //метод обработчик поднятия клавиши мыши над трекбаром
        private void AngleTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            prevAngle = 0;
            chosenCenter = new PointF(-1, -1);
            angleTrackBar.Value = 0;
            angleTrackBar.Visible = false;
            commonChosen = -1;
            TMOChosen = -1;
            Operation = -1;
            Redraw();
        }

        //метод обработчик нажатия на кнопку "Выбрать операнды ТМО"
        private void ChoseOperands_Click(object sender, EventArgs e)
        {
            Operation = 6;
        }

        private void UpdateTMOList(int code)
        {
            TMOFigures.Add(new TMOFigure(figures[indexA], figures[indexB], code));
            //удаление фигур из списка примитивов
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
        }

        //метод обработчик пункта контекстного меню ТМО - объединение выбранных фигур
        private void UnionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMOCode = 1;
            UpdateTMOList(TMOCode);
            Redraw();
        }

        //метод обработчик пункта контекстного меню ТМО - разность между выбранными фигурами
        private void DifferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMOCode = 2;
            UpdateTMOList(TMOCode);
            Redraw();
        }
    }
}
