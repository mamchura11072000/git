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
    /// Логика взаимодействия для NovNomer.xaml
    /// </summary>
    public partial class NovNomer : Window
    {
        NomeraWindow datag;
        SqlDataAdapter adapter;
        DataTable  nomeraTable;
        string connectionString;
        public NovNomer()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            bindcombo();
        }

        public void Show(NomeraWindow dg)
        {
            datag = dg;
            Show();
        }


        public List<Kategorii> Kateg { get; set; }
        private void bindcombo()
        {
            GostinicaEntities5 ge = new GostinicaEntities5();
            var item = ge.Kategoriis.ToList();
            Kateg = item;
            DataContext = Kateg;
            kategComboBox.ItemsSource=Kateg;
            kategComboBox.SelectedValue = "ID_kategorii";
            kategComboBox.DisplayMemberPath = "tip";

        }


        private void Otmena_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Dobavnom_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = null;
            connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand com = new SqlCommand("NovNomer", connection);
            com.CommandType = CommandType.StoredProcedure;

            StringBuilder errrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(Nom_nomera.Text))
                errrors.AppendLine("Введите № номера");
            if (string.IsNullOrWhiteSpace(kol_mest_TextBox.Text))
                errrors.AppendLine("Введите количество мест");
            if (string.IsNullOrWhiteSpace(stoimTextBox.Text))
                errrors.AppendLine("Введите стоимость");
            if ((DbApp.Kategorii)kategComboBox.SelectedValue==null)
                errrors.AppendLine("Выберите категорию номера");
           
            try
            {
                SqlParameter par1 = new SqlParameter("@Nomer_#", SqlDbType.Int);
                par1.Direction = ParameterDirection.Input;
                par1.Value = Nom_nomera.Text;
                //par1.Value = 1111;
                com.Parameters.Add(par1);

                SqlParameter par2 = new SqlParameter("@Kolichestvo_mest", SqlDbType.Int);
                par2.Direction = ParameterDirection.Input;
                par2.Value = kol_mest_TextBox.Text;
                // par2.Value = "FIO";
                com.Parameters.Add(par2);

                SqlParameter par3 = new SqlParameter("@Stoimost_v_sutki", SqlDbType.Int);
                par3.Direction = ParameterDirection.Input;
                par3.Value = stoimTextBox.Text;
                //par22.Value = 1234567;
                com.Parameters.Add(par3);

                SqlParameter par4 = new SqlParameter("@Kategoriya", SqlDbType.Int);
                par4.Direction = ParameterDirection.Input;
                par4.Value = ((DbApp.Kategorii)kategComboBox.SelectedValue).ID_kategorii;
                //par22.Value = 1234567;
                com.Parameters.Add(par4);
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
            adapter.Update(nomeraTable);
        }
    }
}
