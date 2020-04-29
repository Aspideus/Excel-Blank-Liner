﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


//Excel Blank Liner
namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Excel.Application ex = new Microsoft.Office.Interop.Excel.Application();
        // Создаём экземпляр рабочий книги Excel
        Excel.Workbook workBook;
        // Создаём экземпляр листа Excel
        Excel.Worksheet workSheet;
        Excel.Worksheet workSheet2;
        Excel.Worksheet workSheetT;


        static int Poloska(int pB2f)
        {
            if (pB2f >= 100)//бар прогресса
            {
                pB2f = 0;
            }
            else
            {
                pB2f += 10;
            }
            return pB2f;
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName;
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файлы Excel (*.xls; *.xlsx) | *.xls; *.xlsx";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;

                    workBook = ex.Workbooks.Open(fileName);
                    workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(6); //страница с обычными данными
                    workSheet2 = (Excel.Worksheet)workBook.Worksheets.get_Item(5);//страница с данными без порогов
                    workSheetT = (Excel.Worksheet)workBook.Worksheets.get_Item(4);//страница с данными топов
                    //работать будем с 4, 5 и 6 страницей
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager taskbar = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
                    progressBar1.Maximum = 100;
                    progressBar2.Maximum = 100;
                    int pB2 = 0;
                    //использованное количество строк и колонок
                    int usedRowsNum = workSheet.UsedRange.Rows.Count;
                    int usedColumnsNum = workSheet.UsedRange.Columns.Count;

                    Excel.Range c1;
                    Excel.Range c2;
                    Excel.Range c3;
                    Excel.Range oRange;
                    Excel.Range oRange5;
                    Excel.Range oRangeT;
                    Excel.Range nameR = null;
                    Excel.Range nameR1 = null;
                    Excel.Range currentFind = null;


                    label2.Text = "Ищем ВГУЭС";
                    //если в файле есть вгуэс выделим его отдельным цветом
                    string filial = "Владивостокский государственный университет экономики и сервиса"; //Когда название сменится нужно это будет изменить
                    c1 = workSheet.Cells[2, 3];
                    oRange = (Excel.Range)workSheet.get_Range(c1, c1).Find(filial);
                    if (oRange != null)
                    {
                        // oRange.EntireColumn.Interior.ColorIndex = 7; //можно просто всю колонку окрасить но не надо
                        c1 = workSheet.Cells[1, oRange.Column];
                        c2 = workSheet.Cells[usedRowsNum, oRange.Column];
                        oRange = (Excel.Range)workSheet.get_Range(c1, c2);
                        oRange.Interior.ColorIndex = 8;
                        //на таблице топов повторим
                        c1 = workSheetT.Cells[1, oRange.Column];
                        c2 = workSheetT.Cells[usedRowsNum, oRange.Column];
                        oRange = (Excel.Range)workSheetT.get_Range(c1, c2);
                        oRange.Interior.ColorIndex = 8;
                    };
                    progressBar1.Value = 10;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    int scht = 0; //считаем и удаляем филиалы
                    int j; //переменная для всех циклов
                    filial = "илиал";
                    label2.Text = "Фильтруем филиалы";
                    for (j = 4; j <= usedColumnsNum; j++)
                    {
                        c2 = workSheet.Cells[2, 3]; //первая строка в колонке с данными
                        c1 = workSheet2.Cells[2, 3];//то же самое но на 5 странице
                        c3 = workSheetT.Cells[2, 3];

                        oRange = (Excel.Range)workSheet.get_Range(c2, c2).Find(filial); //ищем филиалы
                        oRange5 = (Excel.Range)workSheet2.get_Range(c1, c1).Find(filial);//и в 5 странице тоже ищем
                        oRangeT = (Excel.Range)workSheetT.get_Range(c3, c3).Find(filial);
                        if (oRange != null)
                        {
                            //удаляем найденные колонки с филиалами
                            oRange.EntireColumn.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            oRange5.EntireColumn.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            oRangeT.EntireColumn.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            scht++;
                        };


                        pB2 = Poloska(pB2);
                        progressBar2.Value = pB2;

                    }

                    progressBar1.Value = 20;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    label2.Text = "Ищем отношение заработной платы";
                    //ищем отношение заработной платы
                    int newColumnsNum = usedColumnsNum - scht; //новое количество колонок, без филиалов
                                                               //        oRange = (Excel.Range)workSheet.get_Range("B2", "B" + newColumnsNum.ToString()); //просматриваем названия критериев
                    bool flagZP = true;
                    c1 = workSheet.Cells[2, 2];
                    oRange = (Excel.Range)workSheet.get_Range(c1, c1).Find("Отношение заработной платы");
                    if (oRange != null)
                    {
                        //oRange.Borders.ColorIndex = 3;
                        //oRange.Interior.ColorIndex = 34;
                        string s1 = oRange.Row.ToString();
                        c3 = workSheet.Cells[s1, newColumnsNum];
                        currentFind = (Excel.Range)workSheet.get_Range(oRange, c3);
                        //currentFind.Interior.ColorIndex = 4;

                        string temp1 = oRange.get_Address(Excel.XlReferenceStyle.xlA1);
                        string temp2 = c3.get_Address(Excel.XlReferenceStyle.xlA1);

                        nameR1 = (Excel.Range)workSheet2.get_Range(temp1, temp2);
                        //nameR1.Interior.ColorIndex = 5;
                        //
                        c1 = workSheet.Cells[2, 2];
                        c3 = workSheet.Cells[2, newColumnsNum];
                        nameR = (Excel.Range)workSheet.get_Range(c1, c3);
                        //nameR.Interior.ColorIndex = 5;
                    }
                    else
                        flagZP = false;
                    progressBar1.Value = 30;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                    label2.Text = "Создаём книгу";
                    //создаём книгу в которую будем вносить отфильтрованные данные
                    Excel.Workbook workBook1;
                    Excel.Worksheet workSheet1;
                    Excel.Worksheet workSheet3; //вузики
                    Excel.Worksheet workSheet4; //колледжи
                    Excel.Worksheet workSheet3T;
                    Excel.Worksheet workSheet4T;
                    //создаём новую книгу в которые будем помещать данные
                    int n = 6;
                    ex.SheetsInNewWorkbook = n;
                    workBook1 = ex.Workbooks.Add(); //тупо пожилое создание книги

                    string sS;
                    int ls;
                    int rs;

                    workSheet1 = (Excel.Worksheet)workBook1.Worksheets.get_Item(5);
                    workSheet1.Name = "Данные о зп в регионе";

                    if (flagZP == true)
                    {
                        //помещаем на третью страницу графики по з/п
                        label2.Text = "помещаем на страницу данные о з/п";
                        string Name = usedColumnsNum.ToString();
                        workSheet.get_Range(nameR, nameR).Copy();
                        workSheet1.get_Range("A1", "A1").PasteSpecial();
                        workSheet2.get_Range(nameR1, nameR1).Copy();
                        workSheet1.get_Range("A2", "A2").PasteSpecial();
                        workSheet.get_Range(currentFind, currentFind).Copy();
                        workSheet1.get_Range("A3", "A3").PasteSpecial();

                        //Пороговые значения, убираем то что вне скобок
                        workSheet1.Cells[3, 1] = "Пороговое значение для учреждения";
                        label2.Text = "Создаём графики по з/п";
                        //создаём сами графики
                        Excel.Chart excelchart = (Excel.Chart)ex.Charts.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        c1 = workSheet1.Cells[2, 3];
                        c2 = workSheet1.Cells[2, workSheet1.UsedRange.Columns.Count + 1];

                        c3 = (Excel.Range)workSheet1.get_Range(c1, c2);
                        c3.NumberFormat = "0.00"; //данные приведём к правильному виду
                        excelchart.HasTitle = true;
                        excelchart.ChartTitle.Text = "Отношение заработной платы педагогических работников образовательной организации к средней заработной плате по экономике региона";

                        excelchart.SetSourceData(c3);
                        excelchart.HasLegend = false;
                        excelchart.Activate();
                        ex.ActiveChart.ChartType = Excel.XlChartType.xlLineMarkers;
                        ex.ActiveChart.Location(Excel.XlChartLocation.xlLocationAsObject, "Данные о зп в регионе");

                        c1 = workSheet1.Cells[1, 3];
                        c2 = workSheet1.Cells[1, workSheet1.UsedRange.Columns.Count + 1];
                        c3 = (Excel.Range)workSheet1.get_Range(c1, c2);
                        ex.ActiveChart.FullSeriesCollection(1).XValues = c3;

                        //Перемещаем диаграмму в нужное место
                        workSheet1.Shapes.Item(1).IncrementLeft(101);
                        workSheet1.Shapes.Item(1).IncrementTop((float)200.5);
                        //Задаем размеры диаграммы
                        workSheet1.Shapes.Item(1).Height = 550;
                        workSheet1.Shapes.Item(1).Width = 1500;
                        workSheet1.Columns.AutoFit();

                        label2.Text = "Определяем пороговые значения";
                        for (j = 3; j < newColumnsNum; j++)
                        {
                            sS = Convert.ToString(workSheet1.Cells[3, j].Text);
                            ls = sS.IndexOf('(');
                            rs = sS.IndexOf(')');
                            if ((ls != -1) && (rs != -1))
                            {
                                workSheet1.Cells[3, j] = Convert.ToDouble(sS.Substring(ls + 1, rs - ls - 1));
                                workSheet1.Cells[2, j] = Convert.ToDouble(workSheet1.Cells[2, j].Text); 
                            }
                            if (workSheet1.Cells[3, j].Value >= workSheet1.Cells[2, j].Value)
                                workSheet1.Cells[2, j].Interior.ColorIndex = 3;
                            else
                                workSheet1.Cells[2, j].Interior.ColorIndex = 4;


                            pB2 = Poloska(pB2);//заполняем полоску
                            progressBar2.Value = pB2; //присваиваем полоску
                        }

                    }
                    progressBar1.Value = 40;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                    //переносим высшее образование на первую страницу и среднее на вторую
                    workSheet3 = (Excel.Worksheet)workBook1.Worksheets.get_Item(1);
                    workSheet3.Name = "Вузы значения";
                    workSheet4 = (Excel.Worksheet)workBook1.Worksheets.get_Item(2);
                    workSheet4.Name = "Колледжи значения";
                    workSheet3T = (Excel.Worksheet)workBook1.Worksheets.get_Item(3);
                    workSheet3T.Name = "Топ Вузов";
                    workSheet4T = (Excel.Worksheet)workBook1.Worksheets.get_Item(4);
                    workSheet4T.Name = "Топ Колледжей";
                    //вставляем названия и обозначения критериев на страницы с колледжами и вузами
                    c2 = workSheet.Cells[1, 2];
                    c3 = workSheetT.Cells[1, 2];
                    c2.EntireColumn.Copy();
                    workSheet3.Cells[1, 1].PasteSpecial();
                    workSheet4.Cells[1, 1].PasteSpecial();
                    c3.EntireColumn.Copy();
                    workSheet3T.Cells[1, 1].PasteSpecial();
                    workSheet4T.Cells[1, 1].PasteSpecial();
                    c2 = workSheet.Cells[1, 3];
                    c3 = workSheet.Cells[1, 3];
                    c2.EntireColumn.Copy();
                    workSheet3.Cells[1, 2].PasteSpecial();
                    workSheet4.Cells[1, 2].PasteSpecial();
                    c3.EntireColumn.Copy();
                    workSheet3T.Cells[1, 2].PasteSpecial();
                    workSheet4T.Cells[1, 2].PasteSpecial();

                    progressBar1.Value = 50;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                    //сделать нижеследующее нужно и для четвёртой страницы(топы)
                    label2.Text = "Фильтруем высшее образование";
                    string vWord = "высшего"; //Примечание 1*
                    int vCount = 0;
                    for (j = 3; j <= newColumnsNum; j++)
                    {
                        c2 = workSheet.Cells[2, 3];
                        c1 = workSheetT.Cells[2, 3];
                        oRange = (Excel.Range)workSheet.get_Range(c2, c2).Find(vWord); //ищем вузы
                        if (oRange != null)
                        {
                            oRangeT = (Excel.Range)workSheetT.get_Range(c1, c1).Find(vWord);
                            oRange.EntireColumn.Copy();
                            workSheet3.Cells[1, j].PasteSpecial();
                            oRangeT.EntireColumn.Copy();
                            workSheet3T.Cells[1, j].PasteSpecial();
                            oRange.EntireColumn.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            oRangeT.EntireColumn.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            vCount++;
                        }
                        else
                            break;

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    };
                    c1 = workSheet3.Cells[2, 3];
                    label2.Text = "Определяем правильность построения высшего образования";
                    c2 = (Excel.Range)workSheet3.get_Range(c1, c1).Find("программам высшего образования");
                    bool flag = false;
                    if (c2 != null)
                    {
                        flag = true;
                        vCount--;
                    }
                    label2.Text = "Отфильтровываем среднее образование";
                    //скопируем колледжи
                    if (flag == true)
                        c1 = workSheet.Cells[1, 3];
                    else
                        c1 = workSheet.Cells[1, 4];
                    c2 = workSheet.Cells[usedRowsNum, newColumnsNum - vCount];
                    workSheet.get_Range(c1, c2).Copy(); //[1,3] - AL98 (usedRows, newcolumns) 
                    workSheet4.Cells[1, 3].PasteSpecial();
                    c1 = workSheet4.Cells[1, 1];
                    c2 = workSheet4.Cells[usedRowsNum, 1];
                    workSheet4.get_Range(c1, c2).Copy();
                    workSheet4.Cells[1, newColumnsNum - vCount].PasteSpecial();
                    progressBar1.Value = 60;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    //label2.Text = "подгоняем формат ячеек";
                    //tryparse
                    /*      for (j = 4; j <= usedRowsNum; j++)
                              for (int i = 3; i <= newColumnsNum; i++)
                              {
                                  c1 = workSheet4.Cells[j, i];
                                  c2 = (Excel.Range)workSheet4.get_Range(c1, c1).Find("—");
                                  c3 = (Excel.Range)workSheet4.get_Range(c1, c1).Find("некоррект");
                                  if ((c2 == null) && (c3 == null))
                                      workSheet4.Cells[j, i] = Convert.ToDouble(workSheet4.Cells[j, i]);

                                  c1 = workSheet3.Cells[j, i];
                                  c2 = (Excel.Range)workSheet3.get_Range(c1, c1).Find("—");
                                  c3 = (Excel.Range)workSheet3.get_Range(c1, c1).Find("некоррект");
                                  if ((c2 == null) && (c3 == null))
                                      workSheet3.Cells[j, i] = Convert.ToDouble(workSheet3.Cells[j, i]);
                              } */
                    // —
                    // предоставленынекорректные данные
                    // 1,04(1,05)

                    progressBar1.Value = 65;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    //проблема в том что в таблице строковые а не числовые значения
                    workSheet4.Cells[usedRowsNum + 2, 1] = "Ненулевые показатели";
                    workSheet4.Cells[usedRowsNum + 3, 1] = "Целевые показатели";
                    int nonullP = 0;
                    int desP = 0;
                    label2.Text = "Записываем ненулевые и целевые показатели";
                    //Записываем ненулевые и целевые показатели
                    for (j = 4; j <= newColumnsNum - vCount; j++)
                    {
                        for (int i = 4; i <= usedRowsNum; i++)
                        {
                            sS = Convert.ToString(workSheet4.Cells[j, i].Text);
                            ls = sS.IndexOf('(');
                            rs = sS.IndexOf('—');
                            if ((sS != "0,00") || (rs != -1))
                            {
                                nonullP++;
                            }

                            if (ls != -1)
                            {
                                desP++;
                            }
                        }
                        workSheet4.Cells[usedRowsNum + 2, j - 1] = nonullP;
                        workSheet4.Cells[usedRowsNum + 3, j - 1] = desP;
                        nonullP = 0;
                        desP = 0;

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }

                    int kCount = usedColumnsNum - scht - 3 - vCount;
                    workSheet3.Cells[usedRowsNum + 2, 1] = "Ненулевые показатели";
                    workSheet3.Cells[usedRowsNum + 3, 1] = "Целевые показатели";
                    nonullP = 0;
                    desP = 0;
                    label2.Text = "Записываем ненулевые и целевые показатели";
                    //Записываем ненулевые и целевые показатели
                    for (j = 4; j <= newColumnsNum - kCount; j++)
                    {
                        for (int i = 4; i <= usedRowsNum; i++)
                        {
                            sS = Convert.ToString(workSheet3.Cells[j, i].Text);
                            ls = sS.IndexOf('(');
                            rs = sS.IndexOf('—');
                            if ((sS != "0,00") || (rs != -1))
                            {
                                nonullP++;
                            }

                            if (ls != -1)
                            {
                                desP++;
                            }

                            pB2 = Poloska(pB2);//заполняем полоску
                            progressBar2.Value = pB2; //присваиваем полоску
                        }
                        workSheet3.Cells[usedRowsNum + 2, j - 1] = nonullP;
                        workSheet3.Cells[usedRowsNum + 3, j - 1] = desP;
                        nonullP = 0;
                        desP = 0;

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    if (flag == true)
                        c1 = workSheetT.Cells[1, 3];
                    else
                        c1 = workSheetT.Cells[1, 4];
                    c2 = workSheetT.Cells[usedRowsNum, newColumnsNum - vCount];
                    workSheetT.get_Range(c1, c2).Copy(); //[1,3] - AL98 (usedRows, newcolumns) 
                    workSheet4T.Cells[1, 3].PasteSpecial();
                    c1 = workSheet4T.Cells[1, 1];
                    c2 = workSheet4T.Cells[usedRowsNum, 1];
                    workSheet4T.get_Range(c1, c2).Copy();
                    workSheet4T.Cells[1, newColumnsNum - vCount].PasteSpecial();

                    label2.Text = "Сокращаем названия учреждений";
                    //Сократим названия, сначала на листе с зп
                    for (j = 2; j < newColumnsNum; j++)
                    {
                        sS = Convert.ToString(workSheet1.Cells[1, j].Text);
                        ls = sS.IndexOf('"');
                        rs = sS.LastIndexOf('"');
                        if ((ls != -1) && (rs != -1))
                            workSheet1.Cells[1, j] = sS.Substring(ls + 1, rs - ls - 1);

                        ls = sS.IndexOf('«');
                        rs = sS.LastIndexOf('»');
                        if ((ls != -1) && (rs != -1))
                            workSheet1.Cells[1, j] = sS.Substring(ls + 1, rs - ls - 1);

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    //топ колледж
                    for (j = 2; j <= kCount + 2; j++)
                    {
                        sS = Convert.ToString(workSheet4T.Cells[2, j].Text);
                        ls = sS.IndexOf('"');
                        rs = sS.LastIndexOf('"');
                        if ((ls != -1) && (rs != -1))
                            workSheet4T.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        ls = sS.IndexOf('«');
                        rs = sS.LastIndexOf('»');
                        if ((ls != -1) && (rs != -1))
                            workSheet4T.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    //топ вуз
                    for (j = 2; j <= vCount + 2; j++)
                    {
                        sS = Convert.ToString(workSheet3T.Cells[2, j].Text);
                        ls = sS.IndexOf('"');
                        rs = sS.LastIndexOf('"');
                        if ((ls != -1) && (rs != -1))
                            workSheet3T.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        ls = sS.IndexOf('«');
                        rs = sS.LastIndexOf('»');
                        if ((ls != -1) && (rs != -1))
                            workSheet3T.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    //знач колледж
                    for (j = 2; j <= kCount + 2; j++)
                    {
                        sS = Convert.ToString(workSheet4.Cells[2, j].Text);
                        ls = sS.IndexOf('"');
                        rs = sS.LastIndexOf('"');
                        if ((ls != -1) && (rs != -1))
                            workSheet4.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        ls = sS.IndexOf('«');
                        rs = sS.LastIndexOf('»');
                        if ((ls != -1) && (rs != -1))
                            workSheet4.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    //знач вуз
                    for (j = 2; j <= vCount + 2; j++)
                    {
                        sS = Convert.ToString(workSheet3.Cells[2, j].Text);
                        ls = sS.IndexOf('"');
                        rs = sS.LastIndexOf('"');
                        if ((ls != -1) && (rs != -1))
                            workSheet3.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        ls = sS.IndexOf('«');
                        rs = sS.LastIndexOf('»');
                        if ((ls != -1) && (rs != -1))
                            workSheet3.Cells[2, j] = sS.Substring(ls + 1, rs - ls - 1);

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }

                    label2.Text = "Определяем количество вузов и колледжей";
                    workSheet1.Cells[5, 2] = "Количество вузов";
                    workSheet1.Cells[5, 3] = vCount;
                    workSheet1.Cells[6, 2] = "Количество колледжей";
                    workSheet1.Cells[6, 3] = kCount;


                    progressBar1.Value = 70;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    label2.Text = "Создаём информационный лист";
                    //выбираем четвёртую страницу в новой книге и сливаем туда данные о дебаге
                    workSheet1 = (Excel.Worksheet)workBook1.Worksheets.get_Item(n);
                    workSheet1.Name = "Тесты";
                    //Данные для отладки в финале будут закомментированны
                    workSheet1.Cells[1, 1] = "Количество строк";
                    workSheet1.Cells[1, 2] = usedRowsNum;
                    workSheet1.Cells[2, 1] = "Количество колонок";
                    workSheet1.Cells[2, 2] = usedColumnsNum;
                    workSheet1.Cells[3, 1] = "Количество филиалов";
                    workSheet1.Cells[3, 2] = scht;
                    workSheet1.Cells[4, 1] = "Количество учреждений без филиалов";
                    workSheet1.Cells[4, 2] = usedColumnsNum - scht - 3;
                    workSheet1.Cells[5, 1] = "Количество вузов";
                    workSheet1.Cells[5, 2] = vCount;
                    workSheet1.Cells[6, 1] = "Количество колледжей";
                    workSheet1.Cells[6, 2] = kCount;
                    workSheet1.Cells[usedRowsNum, usedColumnsNum] = 2; //на последнюю ячейку поставим цифру для удобства
                    workSheet1.Visible = 0;
                    progressBar1.Value = 75;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                    //форматирование данных
                    Excel.Range rng2 = workSheet1.Range["A1", workSheet1.Cells[usedRowsNum, usedColumnsNum]];
                    rng2.Font.Size = 20;
                    rng2.Borders.ColorIndex = 3;
                    rng2.Interior.ColorIndex = 34;
                    rng2.Interior.PatternColorIndex = Excel.Constants.xlAutomatic;


                    pB2 = Poloska(pB2);//заполняем полоску
                    progressBar2.Value = pB2; //присваиваем полоску

                    progressBar1.Value = 80;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    label2.Text = "Готовим создание списков топов";
                    //не находит пустые ячейки - сосётся биба
                    oRange = workSheet4T.Range["C4", workSheet4T.Cells[usedRowsNum, kCount + 3]];
                    oRange.SpecialCells(Excel.XlCellType.xlCellTypeBlanks).Font.Color = Excel.XlRgbColor.rgbRed;
                    workSheet4T.Cells[usedRowsNum + 2, 2] = "Средний балл по всем показателям";
                    oRange.SpecialCells(Excel.XlCellType.xlCellTypeBlanks).Value = usedColumnsNum;
                    //И добавить покрас красным цветом шрифта кстати
                    oRange = workSheet3T.Range["C4", workSheet3T.Cells[usedRowsNum, vCount + 3]];
                    oRange.SpecialCells(Excel.XlCellType.xlCellTypeBlanks).Font.Color = Excel.XlRgbColor.rgbRed;
                    workSheet3T.Cells[usedRowsNum + 2, 2] = "Средний балл по всем показателям";
                    oRange.SpecialCells(Excel.XlCellType.xlCellTypeBlanks).Value = usedColumnsNum;
                    //и тут тоже покрас надо добавить

                    label2.Text = "Составляем списки топов для вузов";
                    //C4 начинаются топы (3,4) и записать среднее в usedrowsnum + 2, а usedrowsnum - 3 это количество показателей
                    for (int i = 4; i <= usedRowsNum; i++)
                    {
                        var Mfill = new List<int>();
                        for (j = 3; j < vCount + 3; j++)
                        {
                            ls = Convert.ToInt32(workSheet3T.Cells[i, j].Text);
                            
                            if (!Mfill.Contains(ls))
                                Mfill.Add(ls);

                            pB2 = Poloska(pB2);//заполняем полоску
                            progressBar2.Value = pB2; //присваиваем полоску
                        }
                        Mfill.Sort();
                      //Mfill.Reverse();
                        for (int k = 3; k < vCount + 3; k++)
                        {
                            workSheet3T.Cells[i, k] = Mfill.IndexOf(Convert.ToInt32(workSheet3T.Cells[i, k].Text)) + 1;
                            //формула
                            workSheet3T.Cells[i, k].NumberFormat = "0"; //данные приведём к правильному виду

                            pB2 = Poloska(pB2);//заполняем полоску
                            progressBar2.Value = pB2; //присваиваем полоску
                        }

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }

                    label2.Text = "Составляем списки топов для колледжей";
                    //то же самое и для колледжей
                    for (int i = 4; i <= usedRowsNum; i++)
                    {
                        var Mfill = new List<int>();
                        for (j = 3; j < kCount + 3; j++) //здесь было +3 но выдало ошибку на другом случае
                        {
                            ls = Convert.ToInt32(workSheet4T.Cells[i, j].Text);

                            if (!Mfill.Contains(ls))
                                Mfill.Add(ls);

                            pB2 = Poloska(pB2);//заполняем полоску
                            progressBar2.Value = pB2; //присваиваем полоску
                        }
                        Mfill.Sort();
                        //Mfill.Reverse();
                        for (int k = 3; k < kCount + 3; k++)
                        {
                            workSheet4T.Cells[i, k] = Mfill.IndexOf(Convert.ToInt32(workSheet4T.Cells[i, k].Text)) + 1;
                            //формула
                            workSheet4T.Cells[i, k].NumberFormat = "0"; //данные приведём к правильному виду

                            pB2 = Poloska(pB2);//заполняем полоску
                            progressBar2.Value = pB2; //присваиваем полоску
                        }

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    
                    label2.Text = "Выводим средние баллы в топах";
                    oRange = workSheet4T.Range["C" + (usedRowsNum + 2).ToString()];
                    oRange.Formula = "=SUM(C$4:C$" + usedRowsNum.ToString() + ")/" + (usedRowsNum - 3).ToString();
                    oRange.FormulaHidden = false;

                    c2 = workSheet4T.Cells[usedRowsNum + 2, 3];
                    workSheet4T.get_Range(c2, c2).Copy();
                    j = 4;
                    while (j < kCount + 3)
                    {
                          workSheet4T.Cells[usedRowsNum + 2, j].PasteSpecial();
                          j++;

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    
                    oRange = workSheet3T.Range["C" + (usedRowsNum + 2).ToString()];
                    oRange.Formula = "=SUM(C$4:C$" + usedRowsNum.ToString() + ")/" + (usedRowsNum - 3).ToString();
                    oRange.FormulaHidden = false;


                    c2 = workSheet3T.Cells[usedRowsNum + 2, 3];
                    workSheet3T.get_Range(c2, c2).Copy();
                    j = 4;
                    while (j < vCount + 3)
                    {
                        workSheet3T.Cells[usedRowsNum + 2, j].PasteSpecial();
                        j++;

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }

                    pB2 = Poloska(pB2);//заполняем полоску
                    progressBar2.Value = pB2; //присваиваем полоску

                    progressBar1.Value = 90;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    label2.Text = "Составляем графики для среднего образования";
                    int j1 = 200;
                    int i1 = kCount * 55 + 300;
                    workSheet4.Columns.AutoFit();
                    workSheet3.Columns.AutoFit();
                    Excel.ChartObjects chartsobjrcts = (Excel.ChartObjects)workSheet4.ChartObjects(Type.Missing);
                    for (j = 4; j <= usedRowsNum; j++)
                    {
                        Excel.ChartObject chartsobjrct = chartsobjrcts.Add(i1, j1, 600, 350);
                        c1 = workSheet4.Cells[j, 3];
                        c2 = workSheet4.Cells[j, kCount + 2];
                        c3 = workSheet4.get_Range(c1, c2);
                        
                        chartsobjrct.Chart.ChartWizard(c3,
                        Excel.XlChartType.xlColumnStacked, 2, Excel.XlRowCol.xlRows, Type.Missing,
                          0, false, workSheet4.Cells[j, 1], "Колледж", "Значение", Type.Missing);
                        j1 += 350;

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    j1 = 100;
                    i1 = vCount * 200 + 300;
                    label2.Text = "Составляем графики для высшего образования";
                    chartsobjrcts =
                    (Excel.ChartObjects)workSheet3.ChartObjects(Type.Missing);
                    for (j = 4; j <= usedRowsNum; j++)
                    {
                        Excel.ChartObject chartsobjrct = chartsobjrcts.Add(i1, j1, 600, 350);
                        c1 = workSheet3.Cells[j, 3];
                        c2 = workSheet3.Cells[j, vCount + 2];
                        c3 = workSheet3.get_Range(c1, c2);
                        //
                        chartsobjrct.Chart.ChartWizard(c3,
                        Excel.XlChartType.xlColumnStacked, 2, Excel.XlRowCol.xlRows, Type.Missing,
                        0, false, workSheet3.Cells[j, 1], "Вуз", "Значение", Type.Missing);
     
                        j1 += 350;

                        pB2 = Poloska(pB2);//заполняем полоску
                        progressBar2.Value = pB2; //присваиваем полоску
                    }
                    //
                    label2.Text = "Приводим ячейки в правильный вид";
                    c1 = workSheet3.Cells[2, 1];
                    c2 = workSheet3.Cells[2, vCount + 3];
                    c3 = workSheet3.get_Range(c1, c2);
                    c3.ColumnWidth = 30;
                    c3.RowHeight = 90;

                    c1 = workSheet3T.Cells[2, 1];
                    c2 = workSheet3T.Cells[2, vCount + 3];
                    c3 = workSheet3T.get_Range(c1, c2);
                    c3.ColumnWidth = 30;
                    c3.RowHeight = 90;

                    c1 = workSheet4.Cells[2, 1];
                    c2 = workSheet4.Cells[2, kCount + 3];
                    c3 = workSheet4.get_Range(c1, c2);
                    c3.ColumnWidth = 30;
                    c3.RowHeight = 90;

                    c1 = workSheet4T.Cells[2, 1];
                    c2 = workSheet4T.Cells[2, kCount + 3];
                    c3 = workSheet4T.get_Range(c1, c2);
                    c3.ColumnWidth = 30;
                    c3.RowHeight = 90;

                    //вставим колонку с названиями в вузы если флоаг не тру

                    if (flag == false)
                    {
                        c1 = workSheet3.Cells[1, 1];
                        c2 = workSheet3.Cells[usedRowsNum, 1];
                        workSheet3.get_Range(c1, c2).Copy();
                        workSheet3.Cells[1, vCount + 3].PasteSpecial();
                        workSheet3T.Cells[1, vCount + 3].PasteSpecial();
                    }

                    pB2 = Poloska(pB2);//заполняем полоску
                    progressBar2.Value = pB2; //присваиваем полоску

                    progressBar1.Value = 100;
                    taskbar.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                    label2.Text = "Открываем созданный файл";
                    // Открываем созданный excel-файл
                    ex.Visible = true;
                    ex.UserControl = true;

                    Marshal.ReleaseComObject(workSheet);
                    Marshal.ReleaseComObject(workSheet1);
                    Marshal.ReleaseComObject(workSheetT);
                    Marshal.ReleaseComObject(workSheet3);
                    Marshal.ReleaseComObject(workSheet3T);
                    Marshal.ReleaseComObject(workSheet4);
                    Marshal.ReleaseComObject(workSheet4T);
                    Marshal.ReleaseComObject(workSheet2);
                    //Marshal.ReleaseComObject(workBook);
                    Marshal.ReleaseComObject(ex);
                    label2.Text = "Освобождаем память компьютера";
                    if (checkBox1.Checked)
                        workBook.Close(false);
                    //workBook.Close(false);
                    Application.Exit();
                    //ex.Quit();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            // var Form2 = new Form2(); //Раньше для справки открывалась другая форма, 
            // Form2.Show();            //но после изменений в дизайне было принято решение от неё отказаться
        }

        private void toolTip2_Popup(object sender, PopupEventArgs e)
        {

        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }
    }
}

/* Примечание 1
 В колонках с названиями критерия встречается слово "высшего" а потому они тоже копируются при поиске вузов,
 поэтому счётчик вузов следует уменьшить на 1, а к колледжам добавить сбоку названия критериев
 В результате мы не только избавляемся от этой проблемы но и представляем данные в более удобном для чтения виде
 */