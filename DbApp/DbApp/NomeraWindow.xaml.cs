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
using System.Windows.Shapes;

namespace DbApp
{
    /// <summary>
    /// Логика взаимодействия для NomeraWindow.xaml
    /// </summary>
    public partial class NomeraWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable nomeraTable;
        public NomeraWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void Test()
        {
            string sql = @"SELECT [Nomera].[Nomer_#],
                                  [Kategorii].[tip], 
                                  [Nomera].[Kolichestvo_mest], 
                                  [Nomera].[Stoimost_v_sutki] 

                             FROM Nomera,
                                  Kategorii

                            WHERE ([Nomera].[ID_nomera]=[Nomera].[ID_nomera]) AND 
                                  ([Nomera].[ID_kategorii] = [Kategorii].[ID_kategorii]) ";
            nomeraTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);



                connection.Open();
                adapter.Fill(nomeraTable);
                nomeraGrid.ItemsSource = nomeraTable.DefaultView;
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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Test();
        }

        private void Filtr_Click(object sender, RoutedEventArgs e)
        {
            string sql = @"SELECT [Nomera].[Nomer_#],
                                  [Nomera].[Kolichestvo_mest], 
                                  [Nomera].[Stoimost_v_sutki],
                                  [Kategorii].[tip]
                             FROM 
                                  Nomera
                             JOIN Kategorii ON [Nomera].[ID_nomera]=[Nomera].[ID_nomera]";
            if (znachenie_vibora.Text != "")// если набран текст в поле фильтра
            {
                sql = sql + " AND ([Nomera].[Kolichestvo_mest] = '" + znachenie_vibora.Text + "')";
            }
            if (stoimost.Text != "")// если набран текст в поле фильтра
            {
                sql = sql + " AND ([Nomera].[Stoimost_v_sutki] = '" + stoimost.Text + "')";
            }
            if (Vibor.SelectedIndex == 0) //выбор типа номера
                    sql = sql + " AND ([Kategorii].[tip] = '" + Vibor.Text + "')";
                if (Vibor.SelectedIndex == 1) //номер
                    sql = sql + " AND ([Kategorii].[tip] = '" + Vibor.Text + "') ";
                if (Vibor.SelectedIndex == 2) //Операция
                    sql = sql + " AND ([Kategorii].[tip] = '" + Vibor.Text + "') ";
                if (Vibor.SelectedIndex == 3) //операция
                    sql = sql + " AND [Kategorii].[tip] = '" + Vibor.Text + "') ";
                if (Vibor.SelectedIndex == 5) //операция
                    sql = sql + " AND [Kategorii].[tip] = '" + Vibor.Text + "') ";

                nomeraTable = new DataTable();
                SqlConnection connection = null;
                try
                {
                    connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(sql, connection);
                    adapter = new SqlDataAdapter(command);



                    connection.Open();
                    adapter.Fill(nomeraTable);
                    nomeraGrid.ItemsSource = nomeraTable.DefaultView;
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

        private void DobavitNomer_Click(object sender, RoutedEventArgs e)
        {
            NovNomer novnomWindow = new NovNomer();
            novnomWindow.Show(this);
        }

        public void UpdateDB()
        {
            Test();
        }

        private void RemoveNomer_Click(object sender, RoutedEventArgs e)
        {
            var nomerForRemoving = nomeraGrid.SelectedItem;
            DataRowView row = (DataRowView)nomeraGrid.SelectedItem;
            if (row == null)
                return;
            if (MessageBox.Show($"Вы точно хотите удалить следующи элементов?", "Внимание",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string comStr = "Delete from Nomera where Nomer_#= '" + row[0].ToString() +
                    "' and Kolichestvo_mest=" + row[2].ToString();
                    SqlCommand com = new SqlCommand(comStr, con);
                    com.ExecuteNonQuery();
                }
                Test();
            }

        }

        private void CloseNomer_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    }

