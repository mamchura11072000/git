using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;



namespace DbApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable phonesTable;

        public MainWindow()
        {
            InitializeComponent();
            // получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        }
        public void TestMain() {
            string sql = @"SELECT [Nomera].[Nomer_#],
                                  [Kategorii].[tip],
                                  [Nomera].[Kolichestvo_mest], 
                                  [Nomera].[Stoimost_v_sutki], 
                                  [Klient].[FIO],
                                  [Uchet].[Date_vezda], 
                                  [Uchet].[Date_viezda],
	                              [Uchet].[Stoimost], 
                                  [Operacii].[Operacia]
                             FROM
                                  Nomera,
                                  Kategorii,
                                  Klient,
                                  Uchet,
                                  Operacii
                            WHERE
                                  ([Uchet].[ID_ucheta]=[Uchet].[ID_ucheta]) AND
                                  ([Uchet].[ID_nomera] = [Nomera].[ID_nomera]) AND
                                  ([Nomera].[ID_kategorii] = [Kategorii].[ID_kategorii]) AND
                                  ([Uchet].[ID_operacii] = [Operacii].[ID_operacii]) AND
                                  ([Uchet].[ID_klienta] = [Klient].[ID_klienta]) ";
            phonesTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(phonesTable);
                dataGrid1.ItemsSource = phonesTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        } 
       
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TestMain();
        }


        public void UpdatedDB()
        {
            TestMain();
        }

        public void updateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatedDB();
        }

        private void Sotrudniki_Click(object sender, RoutedEventArgs e) //переход на форму sotrudniki
        {
            SotrudnikiWindow sotrudnikiWindow = new SotrudnikiWindow();
            sotrudnikiWindow.Show();
        }

        private void Nomera_Click(object sender, RoutedEventArgs e) //переход на форму nomera
        {
            NomeraWindow nomeraWindow = new NomeraWindow();
            nomeraWindow.Show();
        }

        private void Zaezd_Click(object sender, RoutedEventArgs e) //переход на форму zaezd
        {
            ZaezdWindow zaezdWindow = new ZaezdWindow();
            zaezdWindow.Show(this);
        }

        private void Filtr_Click(object sender, RoutedEventArgs e) //кнопка фильтрации данных
        {
            string sql = @"SELECT [Nomera].[Nomer_#],
                                  [Kategorii].[tip],
                                  [Nomera].[Kolichestvo_mest], 
                                  [Nomera].[Stoimost_v_sutki], 
                                  [Klient].[FIO],
                                  [Uchet].[Date_vezda], 
                                  [Uchet].[Date_viezda],
	                              [Uchet].[Stoimost], 
                                  [Operacii].[Operacia]
                             FROM
                                   Nomera,
                                   Kategorii,
                                   Klient,
                                   Uchet,
                                   Operacii
                            WHERE
                                ([Uchet].[ID_ucheta]=[Uchet].[ID_ucheta]) AND
                                ([Uchet].[ID_nomera] = [Nomera].[ID_nomera]) AND
                                ([Nomera].[ID_kategorii] = [Kategorii].[ID_kategorii]) AND
                                ([Uchet].[ID_operacii] = [Operacii].[ID_operacii]) AND
                                ([Uchet].[ID_klienta] = [Klient].[ID_klienta]) ";
            if (znachenie_vibora.Text != "")  // если набран текст в поле фильтра
            {
                if (Vibor.SelectedIndex == 0) //ФИО клиента
                    sql = sql + " AND ([Klient].[FIO] = '" + znachenie_vibora.Text + "')";
                if (Vibor.SelectedIndex == 1) //номер
                    sql = sql + " AND ([Nomera].[Nomer_#] = '" + znachenie_vibora.Text + "') ";
                if (Vibor.SelectedIndex == 2) //Операция
                    sql = sql + " AND ([Operacii].[Operacia] = '" + znachenie_vibora.Text + "') ";

            }

            phonesTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(phonesTable);
                dataGrid1.ItemsSource = phonesTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
       
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application exApp = new Excel.Application();
            exApp.Visible = false;
            Excel.Workbook workBook=exApp.Workbooks.Add();
            Excel.Worksheet wsh = (Excel.Worksheet)workBook.Worksheets.get_Item(1);



            //Excel.Worksheets wsh = (Excel.Worksheets)exApp.Worksheets.get_Item(1);

            int i=2, j=1;
            wsh.Columns[4].ColumnWidth = 100;
            foreach (System.Data.DataRowView dr in dataGrid1.ItemsSource)
            {
                string Nomer = dr[0].ToString();
                string Kateg = dr[1].ToString();
                string kol_vo = dr[2].ToString();
                string stoim = dr[3].ToString();
                string FIO = dr[4].ToString();
                string date_zaez = dr[5].ToString();
                string date_otez = dr[6].ToString();
                string ob_stoim = dr[7].ToString();
                string oper = dr[8].ToString();
                //MessageBox.Show(dr[0].ToString());
                //wsh.Cell
                wsh.Cells[1, 1] = String.Format("{0}", "Номер");
                wsh.Cells[1, 2] = String.Format("{0}", "Категория");
                wsh.Cells[1, 3] = String.Format("{0}", "Количество мест");
                wsh.Cells[1, 4] = String.Format("{0}", "Стоимость в сутки");
                wsh.Cells[1, 5] = String.Format("{0}", "ФИО");
                wsh.Cells[1, 6] = String.Format("{0}", "Дата заезда");
                wsh.Cells[1, 7] = String.Format("{0}", "Дата отъезда");
                wsh.Cells[1, 8] = String.Format("{0}", "Стоимость");
                wsh.Cells[1, 9] = String.Format("{0}", "Операция");

                wsh.Cells[i, 1] = Nomer;
                wsh.Cells[i, 2] = Kateg;
                wsh.Cells[i, 3] = kol_vo;
                wsh.Cells[i, 4] = stoim;
                wsh.Cells[i, 5] = FIO;
                wsh.Cells[i, 6] = date_zaez;
                wsh.Cells[i, 7] = date_otez;
                wsh.Cells[i, 8] = ob_stoim;
                wsh.Cells[i, 9] = oper;

                wsh.Cells[i, j].Font.Name = "Times New Roman";
                wsh.Cells[i, j].Font.Size = 14;
                //wsh.Cells[i, j].Characters[0, 3].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);

                //wsh.Cells[i, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);
                i++;
            }


       
            object filename = @"E:\testExel.xls";
            workBook.SaveAs(filename);
            //workBook.Close();

            exApp.Quit();
            MessageBox.Show("Инфрмация экспортрована!");

            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
