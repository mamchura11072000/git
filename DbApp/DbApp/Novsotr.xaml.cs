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
    /// Логика взаимодействия для Novsotr.xaml
    /// </summary>
    /// 

    public partial class Novsotr : Window
    {
      
        string connectionString;
        SotrudnikiWindow datag;
        SqlDataAdapter adapter;
        DataTable sotrudnikiTable;
        public Novsotr()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            bindcombo();
        }

        public void Show(SotrudnikiWindow dg)
        {
            datag = dg;
            Show();
        }

        public List<Doljnosti> Dolj { get; set; }
        private void bindcombo()
        {
            GostinicaEntities ge = new GostinicaEntities();
            var item = ge.Doljnostis.ToList();
            Dolj = item;
            DataContext = Dolj;

        }


        private void Otmena_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Dobavsotr_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = null;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand com = new SqlCommand("NovSotr", connection);
            com.CommandType = CommandType.StoredProcedure;

            StringBuilder errrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(fioTextBox.Text))
                errrors.AppendLine("Введите ФИО сотрудника");
            if ((DbApp.Doljnosti)doljComboBox.SelectedValue == null)
                errrors.AppendLine("Выберите должность");
            if (string.IsNullOrWhiteSpace(telTextBox.Text))
                errrors.AppendLine("Введите телефон");

            try
            {
                SqlParameter par1 = new SqlParameter("@FIO", SqlDbType.VarChar);
                par1.Direction = ParameterDirection.Input;
                par1.Value = fioTextBox.Text;
                //par1.Value = 1111;
                com.Parameters.Add(par1);

                SqlParameter par2 = new SqlParameter("@Doljnost", SqlDbType.Int);
                par2.Direction = ParameterDirection.Input;
                par2.Value = ((DbApp.Doljnosti)doljComboBox.SelectedValue).ID_doljnosti;
                // par2.Value = "FIO";
                com.Parameters.Add(par2);

                SqlParameter par22 = new SqlParameter("@Telefon", SqlDbType.Int);
                par22.Direction = ParameterDirection.Input;
                par22.Value = telTextBox.Text;
                //par22.Value = 1234567;
                com.Parameters.Add(par22);
            }
            catch
            {
                if (errrors.Length > 0)
                {
                    MessageBox.Show(errrors.ToString());
                    return;
                }
            }


            try
            {
                MessageBox.Show("Информация сохранена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
              
         
            com.ExecuteNonQuery();
            datag.UpdateDB();

            Close();

        }
        private void UpdateDB()
        {
            SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
            adapter.Update(sotrudnikiTable);
        }
    }
}
