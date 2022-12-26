using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace gsk_course_work
{
    internal class TMOHandler
    {
        //ширина холста
        public int canvasWidth;
        //объект фигуры ТМО - две фигуры примитива, цвет и список отрезков (до ТМО он пустой или старый)
        public TMOFigure TMOFigure;
        //объект графики
        Graphics G;
        //переменная для допустимых значений суммы пороговых функций
        int[] SetQ;
        //минимальная строка Y - самая верхняя
        float Ymin;
        //максимальная строка Y - самая нижняя
        float Ymax;
        //список отрезков для хранения уже сделаннного ТМО
        List<TMOLine> lines = new List<TMOLine>();

        //далее для рисования, а не выполнения ТМО
        //минимальный X для выделения вокруг ТМО
        float selXmin;
        //и т.д.
        float selYmin;
        float selXmax;
        float selYmax;

        //конструктор принимает фигуру ТМО, ширину холста и объект графики
        public TMOHandler(TMOFigure TMOFigure, int canvasWidth, Graphics g)
        {
            this.TMOFigure = TMOFigure;
            this.canvasWidth = canvasWidth;
            G = g;
        }

        //метод рисования ТМО
        public void DrawTMO()
        {
            //если выбран код 1, то это объединение {1, 3}, иначе будет код 2 - разность первой со второй фигурой {2, 2}
            SetQ = (TMOFigure.TMOCode == 1) ? new int[] { 1, 3 } : new int[] { 2, 2 };
            //получение строк, в пределах которых может быть результат ТМО
            //миним Y
            Ymin = Math.Min(TMOFigure.FigA.VertexList.Min(item => item.Y), TMOFigure.FigB.VertexList.Min(item => item.Y));
            Ymax = Math.Max(TMOFigure.FigA.VertexList.Max(item => item.Y), TMOFigure.FigB.VertexList.Max(item => item.Y));
            //чистим старые отрезки ТМО
            lines.Clear();
            //зовем метод заполнения отрезков
            //в нем выполняется ТМО, осталось только нарисовать
            FillLines();
            //цвет от первой фигуры в объекте ТМО
            Pen drawPen = new Pen(TMOFigure.FigA.Color);
            //ставим в объект новый список отрезков lines
            TMOFigure.Lines = lines;
            //цикл по отрезкам ТМО
            for(int i = 0; i < lines.Count; i++)
            {
                //рисуем от левой до правой X нужной строки Y
                G.DrawLine(drawPen, lines[i].xLeft, lines[i].y, lines[i].xRight, lines[i].y);
            }
        }

        //маленький метод для заполнения списка границ М
        //унифицирован чтоб работать для любой фигуры
        //принимает  список М, ранее созданный, список отрезков для КОНКРЕТНОГО ПРИМИТИВА
        //это еще не отрезки ТМО, это ПРИМИТИВ
        //и еще dQ - приращение пороговой функции для каждой фигуры с учетом веса фигуры (т. е. 2 или 1)
        private void FillM(List<TMOBorder> M, List<TMOLine> lines, int dQ)
        {
            //цикл по отрезкам примитива
            for(int i = 0; i < lines.Count; i++)
            {
                //сохраняем координаты левой границы и dQ положительный - начало сегмента
                M.Add(new TMOBorder(lines[i].xLeft, dQ));
            }
            for (int i = 0; i < lines.Count; i++)
            {
                //правая граница, dQ в минус пушто это конец сегмента
                M.Add(new TMOBorder(lines[i].xRight, -dQ));
            }
        }

        //метод для заполнения списка отрезков, из которых состоит ТМО
        private void FillLines()
        {
            //ОНИ И ИДУТ В МЕТОД FillM
            //список отрезков фигуры A (у нее dQ = 2)
            List<TMOLine> figALines;
            //список отрезков фигуры B (у нее dQ = 1)
            List<TMOLine> figBLines;
            //список границ M - все границы на ТЕКУЩЕЙ СТРОКЕ от обоих примитивов
            List<TMOBorder> M;
            //цикл по строкам
            for (int i = (int)Ymin; i < Ymax; i++)
            {
                //обновляем список границ, пушто новая строка
                M = new List<TMOBorder>();
                //обновляем списки отрезков, новая строка
                figALines = new List<TMOLine>();
                figBLines = new List<TMOLine>();
                //заполнение списка сегментов фигур, лежащих на текущей строке
                //то есть сначала надо заполнить figALines и figBLines
                FindFigureSegments(TMOFigure.FigA, figALines, i);
                FindFigureSegments(TMOFigure.FigB, figBLines, i);
                //заполнение списка M - границы сегментов двух фигур
                FillM(M, figALines, 2);
                FillM(M, figBLines, 1);
                //сортируем границы в M по возрастанию X
                M.Sort((e1, e2) => e1.x.CompareTo(e2.x));
                //если на данной строке имеются сечения фигур, то над ними выполняется ТМО
                //то есть список не пустой
                if (M.Count > 0)
                {
                    //те самые кишки алгоритма ТМО
                    DoTMO(M, i);
                }
            }
        }

        //метод выполнения ТМО над найденными сегментами в строке Y
        private void DoTMO(List<TMOBorder> M, int Y)
        {
            //весь алгоритм колдует над одной строкой Y
            //отдельные списки для левых и правых границ ТМО, только координаты X
            List<int> TMOLeftX = new List<int>();
            List<int> TMORightX = new List<int>();
            //пока что обнуляем суммы приращений пороговых функций
            int Q = 0; int Qnew = 0;
            //исключительный случай - выход за левую границу формы
            if (M[0].x >= 0 && M[0].dQ < 0)
            {
                //значит надо в левые границы
                TMOLeftX.Add(0);
                //фигачим в старую сумму пороговых отрицательное приращение - следующая граница будет правой
                //то есть концом сегмента, но сам алгоритм без этого отрицалова в Q не сможет это понять
                //в итоге получится что одинаковые dQ, ток с плюсом и минусом сложатся  = 0, т. е.
                //сегмент фигуры кончился, пустота (или другая фигура)
                Q = -M[0].dQ;
            }
            //переменная для текущей границы X
            int x;
            //в цикле по границам на данной строке (берем из списка M)
            for (int k = 0; k < M.Count; k++)
            {
                //сохраняем в перем какая ща граница (координата X)
                x = M[k].x;
                //складываем в новую сумму старую сумму и приращение пороговой функции фигуры на данной границе
                Qnew = Q + M[k].dQ;
                //начало сегмента ТМО
                //первое условие - не входить в допустимое множество, старая сумма Q означает то, что было
                //до текущей точки
                //второе условие - входить, Qnew это что после точки
                //если оба условия соблюдены, то до точки ТМО нет, а после есть - левая граница
                if ((Q < SetQ[0] || Q > SetQ[1]) && (Qnew >= SetQ[0] && Qnew <= SetQ[1]))
                {
                    //сохранили в список левых
                    TMOLeftX.Add(x);
                }
                //конец сегмента ТМО
                //первое - старая сумма входит в допустимое множество
                //второе - новая сумма не входит
                //то есть до точки ТМО было, а после уже нет - правая граница
                if ((Q >= SetQ[0] && Q <= SetQ[1]) && (Qnew < SetQ[0] || Qnew > SetQ[1]))
                {
                    TMORightX.Add(x);
                }
                //старая сумма сохраняет в себе, что было в новой - просто обновляем считай
                Q = Qnew;
            }
            //исключительный случай - выход за правую границу формы
            //то есть последняя после цикла сумма Q все еще входит в допустимое множество - 
            //ТМО еще идет, а точки мы все перебрали - то есть не знаем где правая граница
            //остается за правую взять границу холста
            if (Q >= SetQ[0] && Q <= SetQ[1])
            {
                //сохранили ширину холста (у меня пикчабокса или битмапы одна фигня)
                TMORightX.Add(canvasWidth);
            }
            //сохранение сегментов ТМО на текущей строке
            //левых и правых одинаково, так что можем итерироваться по любому из двух списков
            for (int k = 0; k < TMORightX.Count; k++)
            {
                //сохраняем в список lines левую границу, правую границу, текущую строку
                lines.Add(new TMOLine(TMOLeftX[k], TMORightX[k], Y));
            }
        }

        //метод нахождения сегментов многоугольника на заданной строке Y
        //принимает ПРИМИТИВ, список отрезков этого примитива (он будет пустой) и строку, на которой ищем сегменты
        private void FindFigureSegments(Figure figure, List<TMOLine> list, int Y)
        {
            //фигачим новый список координат X
            List<int> xList = new List<int>();
            //цикл по всем точкам фигуры
            for (int i = 0; i < figure.VertexList.Count; i++)
            {
                //индекс для следующей точки
                int k;
                //если мы не в конце, то следующая точка буквально следующая
                if (i < figure.VertexList.Count - 1) k = i + 1;
                //иначе самая первая
                else k = 0;
                //если текущая строка где то между двух точек, еще и с учетом какую точку мы не
                //учитываем, приращение + не учитываем первую, приращение - не учитываем вторую
                if ((figure.VertexList[i].Y < Y && figure.VertexList[k].Y >= Y)
                    || (figure.VertexList[i].Y >= Y && figure.VertexList[k].Y < Y))
                {
                    //значит есть пересечение строки с отрезком между этими двумя точками, нужно найти координату X
                    //используется каноническое уравнение прямой на двух точках - выражен из него X 
                    //чтоб найти эту X подставляем конечные точки и текущую строку Y
                    int x = (int)Math.Ceiling((double)(figure.VertexList[k].X - figure.VertexList[i].X) * (Y - figure.VertexList[i].Y) /
                        (double)(figure.VertexList[k].Y - figure.VertexList[i].Y) + figure.VertexList[i].X);
                    //добавляем в список
                    xList.Add(x);
                }
            }
            //сортируем по возрастанию
            xList.Sort();
            //в цикле переписываем координаты X в список линий (можно и без этого, все ведь построчно, но я не стала убирать)
            for(int i = 0; i < xList.Count - 1; i += 2)
            {
                //в xList точки получаются попарные после сортировки - левая правая, левая правая и тд
                //каждая пара это один отрезок
                list.Add(new TMOLine(xList[i], xList[i + 1], Y));
            }
        }

        //метод получения точек для выделения - используются исходные фигуры
        public void GetSelection()
        {
            selXmin = Math.Min(TMOFigure.Lines.Min(p => p.xLeft), TMOFigure.Lines.Min(p => p.xRight));
            selXmax = Math.Max(TMOFigure.Lines.Max(p => p.xLeft), TMOFigure.Lines.Max(p => p.xRight));
            selYmin = TMOFigure.Lines.Min(p => p.y);
            selYmax = TMOFigure.Lines.Max(p => p.y);
        }

        //метод получения центра ТМО (центр описанного четырёхугольника)
        public PointF GetTMOCenter()
        {
            GetSelection();
            PointF center = new PointF
            {
                X = selXmin + ((selXmax - selXmin) / 2),
                Y = selYmin + ((selYmax - selYmin) / 2)
            };
            return center;
        }

        //метод рисования выделения (описанного четырёхугольника)
        public void DrawSelection()
        {
            GetSelection();
            float[] dashPattern = { 5, 5 };
            Pen selectPen = new Pen(Color.Gray);
            selectPen.DashPattern = dashPattern;
            G.DrawLine(selectPen, selXmin, selYmax, selXmin, selYmin);
            G.DrawLine(selectPen, selXmin, selYmin, selXmax, selYmin);
            G.DrawLine(selectPen, selXmax, selYmin, selXmax, selYmax);
            G.DrawLine(selectPen, selXmax, selYmax, selXmin, selYmax);
        }

        //метод проверки нажатия на фигуру
        public bool ThisFigure(Point point)
        {
            for(int i = 0; i < TMOFigure.Lines.Count; i++)
            {
                if(point.Y == TMOFigure.Lines[i].y)
                {
                    if(point.X >= TMOFigure.Lines[i].xLeft && point.X <= TMOFigure.Lines[i].xRight) return true;
                }
            }
            return false;
        }
    }
}
