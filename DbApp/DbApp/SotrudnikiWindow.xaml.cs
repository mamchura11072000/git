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
    /// Логика взаимодействия для SotrudnikiWindow.xaml
    /// </summary>
    public partial class SotrudnikiWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable sotrudnikiTable;

        public SotrudnikiWindow()
        {
            InitializeComponent();
            // получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        }
        
        private void Test()
        {
            string sql = @"SELECT [Sotrudniki].[FIO],
                                  [Sotrudniki].[Telefon], 
                                  [Doljnosti].[Doljnost],
	                              [Doljnosti].[Oklad] 

                             FROM 
                                  Doljnosti,
                                  Sotrudniki

                            WHERE 
                                  ([Sotrudniki].[ID_sotrudnika]=[Sotrudniki].[ID_sotrudnika]) AND 
                                  ([Doljnosti].[ID_doljnosti] = [Sotrudniki].[ID_doljnosti]) ";
            sotrudnikiTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(sotrudnikiTable);
                sotrudnikiGrid.ItemsSource = sotrudnikiTable.DefaultView;
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

        private void Dobavsotr_Click(object sender, RoutedEventArgs e)
        {
            Novsotr novsotr = new Novsotr();
            novsotr.Show(this);
        }


       public void UpdateDB()
        {
            Test();

        }

        private void Removesotr_Click(object sender, RoutedEventArgs e)
        {
        
             var sotrudnikForRemoving = sotrudnikiGrid.SelectedItem;
            DataRowView row = (DataRowView)sotrudnikiGrid.SelectedItem;
            if (row == null)
                return;
            if (MessageBox.Show($"Вы точно хотите удалить следующи элементов?", "Внимание",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string comStr = "Delete from Sotrudniki where FIO= '" + row[0].ToString() +
                    "' and Telefon=" + row[1].ToString();
                    SqlCommand com = new SqlCommand(comStr, con);
                    com.ExecuteNonQuery();
                }
                Test();
            }

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    }

