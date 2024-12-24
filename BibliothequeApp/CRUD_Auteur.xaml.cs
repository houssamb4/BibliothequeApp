using System;
using System.Collections.Generic;
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
using System.Data;
using Microsoft.Data.SqlClient;

namespace BibliothequeApp
{
    public partial class CRUD_Auteur : Window
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();

        public CRUD_Auteur()
        {
            InitializeComponent();
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            try
            {
                string query = "SELECT * FROM Auteurs";
                DataTable authorsTable = dbHelper.ExecuteQuery(query);
                dataGridAuteurs.ItemsSource = authorsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des auteurs : {ex.Message}");
            }
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nomAuteur = txtNomAuteur.Text.Trim();
                if (string.IsNullOrEmpty(nomAuteur))
                {
                    MessageBox.Show("Le champ Nom est obligatoire.");
                    return;
                }

                string query = "INSERT INTO Auteurs (Nom) VALUES (@Nom)";
                SqlParameter[] parameters = { new SqlParameter("@Nom", nomAuteur) };
                dbHelper.ExecuteQuery(query, parameters);

                MessageBox.Show("Auteur ajouté avec succès !");
                LoadAuthors();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridAuteurs.SelectedItem is DataRowView row)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    string nomAuteur = txtNomAuteur.Text.Trim();

                    if (string.IsNullOrEmpty(nomAuteur))
                    {
                        MessageBox.Show("Le champ Nom est obligatoire.");
                        return;
                    }

                    string query = "UPDATE Auteurs SET Nom = @Nom WHERE Id = @Id";
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@Nom", nomAuteur),
                        new SqlParameter("@Id", id)
                    };
                    dbHelper.ExecuteQuery(query, parameters);

                    MessageBox.Show("Auteur modifié avec succès !");
                    LoadAuthors();
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un auteur à modifier.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridAuteurs.SelectedItem is DataRowView row)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    string query = "DELETE FROM Auteurs WHERE Id = @Id";
                    SqlParameter[] parameters = { new SqlParameter("@Id", id) };
                    dbHelper.ExecuteQuery(query, parameters);

                    MessageBox.Show("Auteur supprimé avec succès !");
                    LoadAuthors();
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un auteur à supprimer.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private void DataGridAuteurs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataGridAuteurs.SelectedItem is DataRowView row)
            {
                txtNomAuteur.Text = row["Nom"].ToString();
            }
        }

        private void txtNomAuteur_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

