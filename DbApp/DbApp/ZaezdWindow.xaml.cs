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
    /// Логика взаимодействия для ZaezdWindow.xaml
    /// </summary>
    public partial class ZaezdWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable phonesTable;
        MainWindow datag;
        //DataTable operaciiTable;
        public ZaezdWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            bindcombo_nom();
            bindcombo_oper();
            bindcombo_sotr();            
        }

        public void Show(MainWindow dg)
        {
            datag = dg;
            Show();
        }



        public List<Operacii> Operacii { get; set; }
        public void bindcombo_oper()
        {
            GostinicaEntities2 ge = new GostinicaEntities2();
            var item1 = ge.Operaciis.ToList();
            Operacii = item1;
            DataContext = Operacii;
            tipzakazaComboBox.ItemsSource = Operacii;
            tipzakazaComboBox.SelectedValue = "ID_operacii";
            tipzakazaComboBox.DisplayMemberPath = "Operacia";
        }

        public List<Nomera> Nomera { get; set; }
        public void bindcombo_nom()
        {   
            GostinicaEntities1 ge = new GostinicaEntities1();
            var item = ge.Nomeras.ToList();
            Nomera = item;
            DataContext = Nomera;
            nomerComboBox.ItemsSource =Nomera;
            nomerComboBox.SelectedValue = "ID_nomera";
            nomerComboBox.DisplayMemberPath = "Nomer__";
        }
        
               
        public List<Sotrudniki> Sotrudniki { get; set; }
        public void bindcombo_sotr()
        {
            GostinicaEntities3 ge = new GostinicaEntities3();
            var item3 = ge.Sotrudnikis.ToList();
            Sotrudniki = item3;
            DataContext = Sotrudniki;
            sotrudnikComboBox.ItemsSource = Sotrudniki;
            sotrudnikComboBox.SelectedValue = "ID_sotrudnika";
            sotrudnikComboBox.DisplayMemberPath = "FIO";

        }

        private void Dobavit_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = null;
            connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand com = new SqlCommand("Zaezd", connection);
            com.CommandType = CommandType.StoredProcedure;

            StringBuilder errrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(pasportTextBox.Text))
                errrors.AppendLine("Введите номер паспорта в формате 123457890");
            if (string.IsNullOrWhiteSpace(fioTextBox.Text))
                errrors.AppendLine("Введите ФИО в формате Иванов Иван Иванович");
            if (string.IsNullOrWhiteSpace(telephonTextBox.Text))
                errrors.AppendLine("Введите номер телефона в формате 9798789");
            if ((DbApp.Nomera)nomerComboBox.SelectedItem == null)
                errrors.AppendLine("Выберите номер");
            if ((DbApp.Operacii)tipzakazaComboBox.SelectedValue == null)
                errrors.AppendLine("Выберите тип заказа");
            if (datazaezdaDatePicker.Text == "")
                errrors.AppendLine("Выберите дату заезда");
            if (dataotezdaDatePicker.Text == "")
                errrors.AppendLine("Выберите дату отъезда");
            if ((DbApp.Sotrudniki)sotrudnikComboBox.SelectedValue == null)
                errrors.AppendLine("Выберите сотрудника");

            try
            {
                SqlParameter par1 = new SqlParameter("@Paspor", SqlDbType.Int);
                par1.Direction = ParameterDirection.Input;
                par1.Value = int.Parse(pasportTextBox.Text);
                com.Parameters.Add(par1);

                SqlParameter par2 = new SqlParameter("@FIO", SqlDbType.VarChar);
                par2.Direction = ParameterDirection.Input;
                par2.Value = fioTextBox.Text;
                // par2.Value = "FIO";
                com.Parameters.Add(par2);

                SqlParameter par22 = new SqlParameter("@Telephon", SqlDbType.Int);
                par22.Direction = ParameterDirection.Input;
                par22.Value = int.Parse(telephonTextBox.Text);
                //par22.Value = 1234567;
                com.Parameters.Add(par22);

                SqlParameter par3 = new SqlParameter("@Nomer", SqlDbType.Int);
                par3.Direction = ParameterDirection.Input;
                par3.Value = ((DbApp.Nomera)nomerComboBox.SelectedItem).ID_nomera;
                //par3.Value = 5;
                com.Parameters.Add(par3);

                SqlParameter par4 = new SqlParameter("@Tip_zakaza", SqlDbType.Int);
                par4.Direction = ParameterDirection.Input;
                par4.Value = ((DbApp.Operacii)tipzakazaComboBox.SelectedValue).ID_operacii;
                //par4.Value =1;
                com.Parameters.Add(par4);

                SqlParameter par5 = new SqlParameter("@Data_zaez", SqlDbType.Date);
                par5.Direction = ParameterDirection.Input;
                par5.Value = DateTime.Parse(datazaezdaDatePicker.Text);
                com.Parameters.Add(par5);

                SqlParameter par6 = new SqlParameter("@Data_otez", SqlDbType.Date);
                par6.Direction = ParameterDirection.Input;
                par6.Value = DateTime.Parse(dataotezdaDatePicker.Text);
                com.Parameters.Add(par6);

                SqlParameter par7 = new SqlParameter("@Sotr", SqlDbType.Int);
                par7.Direction = ParameterDirection.Input;
                par7.Value = ((DbApp.Sotrudniki)sotrudnikComboBox.SelectedValue).ID_sotrudnika;
                par7.Value = 1;
                com.Parameters.Add(par7);
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
            datag.UpdatedDB();
            Close();
        }

        private void UpdatedDB()
        {
            SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
            adapter.Update(phonesTable);
        }

        private void Otmena_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
