using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace UP01_Prasolova_Clinic.Pages
{
    public partial class DiagramPage : Page
    {
        private readonly string documentsFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public DiagramPage()
        {
            InitializeComponent();

            ChartServices.ChartAreas.Add(new ChartArea("Main"));
            ChartServices.Series.Add(new Series("Расходы, руб.") { IsValueShownAsLabel = true });

            ComboBoxPatient.ItemsSource = Core.Context.Users.Where(u => u.Role == "User").ToList();
            ComboBoxChartType.ItemsSource = new List<KeyValuePair<string, SeriesChartType>>
            {
                new KeyValuePair<string, SeriesChartType>("Гистограмма", SeriesChartType.Column),
                new KeyValuePair<string, SeriesChartType>("Линейчатая", SeriesChartType.Bar),
                new KeyValuePair<string, SeriesChartType>("Круговая", SeriesChartType.Pie),
                new KeyValuePair<string, SeriesChartType>("Кольцевая", SeriesChartType.Doughnut),
                new KeyValuePair<string, SeriesChartType>("График", SeriesChartType.Line)
            };

            ComboBoxPatient.SelectedIndex = 0;
            ComboBoxChartType.SelectedIndex = 0;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (!(ComboBoxPatient.SelectedItem is User patient) || ComboBoxChartType.SelectedItem == null)
                return;

            var chartType = ((KeyValuePair<string, SeriesChartType>)ComboBoxChartType.SelectedItem).Value;

            Series series = ChartServices.Series[0];
            series.ChartType = chartType;
            series.Points.Clear();

            foreach (var type in Core.Context.ServiceTypes.ToList())
            {
                decimal sum = patient.ServicesRendered
                    .Where(s => s.ServiceTypeID == type.ID)
                    .Sum(s => s.Total);
                series.Points.AddXY(type.Name, sum);
            }
        }

        private Word.Document CreateWordReport(Word.Application app)
        {
            var patients = Core.Context.Users.Where(u => u.Role == "User").ToList();
            var types = Core.Context.ServiceTypes.ToList();
            Word.Document document = app.Documents.Add();

            foreach (var patient in patients)
            {
                Word.Paragraph title = document.Paragraphs.Add();
                title.Range.Text = patient.FIO;
                title.Range.Font.Name = "Times New Roman";
                title.Range.Font.Size = 16;
                title.Range.Font.Bold = 1;
                title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                title.Range.InsertParagraphAfter();

                Word.Paragraph tableParagraph = document.Paragraphs.Add();
                Word.Table table = document.Tables.Add(tableParagraph.Range, types.Count + 2, 2);
                table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Range.Font.Name = "Times New Roman";
                table.Range.Font.Size = 12;
                table.Range.Font.Bold = 0;

                table.Cell(1, 1).Range.Text = "Тип услуги";
                table.Cell(1, 2).Range.Text = "Сумма, руб.";
                table.Rows[1].Range.Font.Bold = 1;

                decimal total = 0;
                for (int i = 0; i < types.Count; i++)
                {
                    decimal sum = patient.ServicesRendered
                        .Where(s => s.ServiceTypeID == types[i].ID)
                        .Sum(s => s.Total);
                    total += sum;

                    table.Cell(i + 2, 1).Range.Text = types[i].Name;
                    table.Cell(i + 2, 2).Range.Text = sum.ToString("N2");
                }

                table.Cell(types.Count + 2, 1).Range.Text = "Итого";
                table.Cell(types.Count + 2, 2).Range.Text = total.ToString("N2");
                table.Rows[types.Count + 2].Range.Font.Bold = 1;

                document.Paragraphs.Add();
                if (patient != patients.Last())
                    document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
            }

            return document;
        }

        private void ButtonWord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                MessageFilter.Register();
                var app = new Word.Application();
                app.Visible = true;
                Word.Document document = CreateWordReport(app);

                string path = Path.Combine(documentsFolder, "Отчет_по_услугам.docx");
                document.SaveAs2(path);
                app.Visible = true;

                Core.ShowInfo("Отчёт сохранён: " + path);
            }
            catch (Exception ex)
            {
                Core.ShowError("Не удалось создать документ Word: " + ex.Message);
            }
            finally
            {
                MessageFilter.Revoke();
            }
        }

        private void ButtonPdf_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                MessageFilter.Register();
                var app = new Word.Application();
                app.Visible = true;
                Word.Document document = CreateWordReport(app);

                string path = Path.Combine(documentsFolder, "Отчет_по_услугам.pdf");
                document.ExportAsFixedFormat(path, Word.WdExportFormat.wdExportFormatPDF);

                Core.ShowInfo("Отчёт сохранён: " + path);
                Process.Start("explorer.exe", "/select,\"" + path + "\"");
            }
            catch (Exception ex)
            {
                Core.ShowError("Не удалось создать PDF: " + ex.Message);
            }
            finally
            {
                MessageFilter.Revoke();
            }
        }

        private void ButtonExcel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                MessageFilter.Register();
                var patients = Core.Context.Users.Where(u => u.Role == "User").ToList();
                var types = Core.Context.ServiceTypes.ToList();

                var app = new Excel.Application();
                app.SheetsInNewWorkbook = patients.Count;
                Excel.Workbook workbook = app.Workbooks.Add(Type.Missing);

                for (int p = 0; p < patients.Count; p++)
                {
                    Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.Item[p + 1];
                    string name = patients[p].FIO;
                    sheet.Name = name.Length > 31 ? name.Substring(0, 31) : name;

                    sheet.Cells[1, 1] = "Тип услуги";
                    sheet.Cells[1, 2] = "Сумма, руб.";

                    for (int i = 0; i < types.Count; i++)
                    {
                        decimal sum = patients[p].ServicesRendered
                            .Where(s => s.ServiceTypeID == types[i].ID)
                            .Sum(s => s.Total);

                        sheet.Cells[i + 2, 1] = types[i].Name;
                        sheet.Cells[i + 2, 2] = (double)sum;
                    }

                    int lastRow = types.Count + 2;
                    sheet.Cells[lastRow, 1] = "Итого";
                    ((Excel.Range)sheet.Cells[lastRow, 2]).Formula = "=SUM(B2:B" + (lastRow - 1) + ")";

                    Excel.Range header = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 2]];
                    header.Font.Bold = true;
                    ((Excel.Range)sheet.Rows[lastRow]).Font.Bold = true;

                    Excel.Range table = sheet.Range[sheet.Cells[1, 1], sheet.Cells[lastRow, 2]];
                    table.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    sheet.Range["B2", "B" + lastRow].NumberFormat = "#,##0.00";
                    sheet.Columns.AutoFit();
                }

                string path = Path.Combine(documentsFolder, "Отчет_по_услугам.xlsx");
                app.DisplayAlerts = false;
                workbook.SaveAs(path);
                app.Visible = true;

                Core.ShowInfo("Отчёт сохранён: " + path);
            }
            catch (Exception ex)
            {
                Core.ShowError("Не удалось создать книгу Excel: " + ex.Message);
            }
            finally
            {
                MessageFilter.Revoke();
            }
        }
    }
}