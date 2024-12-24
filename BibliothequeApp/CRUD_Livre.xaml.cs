using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace BibliothequeApp
{
    public partial class CRUD_Livre : Window
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();

        public CRUD_Livre()
        {
            InitializeComponent();
            LoadAuthors();
            LoadBooks();
        }

        private void LoadAuthors()
        {
            string query = "SELECT Id, Nom FROM Auteurs";

            try
            {
                DataTable authors = dbHelper.ExecuteQuery(query);
                cbxAuteur.ItemsSource = authors.DefaultView;
                cbxAuteur.DisplayMemberPath = "Nom";
                cbxAuteur.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des auteurs : {ex.Message}");
            }
        }

        private void LoadBooks()
        {
            string query = @"
                SELECT L.Id, L.Titre, L.Nb_Pages, L.Prix, A.Nom AS Auteur
                FROM Livres L
                INNER JOIN Auteurs A ON L.Id_Auteur = A.Id";

            try
            {
                DataTable books = dbHelper.ExecuteQuery(query);
                dataGridLivres.ItemsSource = books.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des livres : {ex.Message}");
            }
        }

        private void BtnAjouterLivre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInputs())
                {
                    string query = @"
                        INSERT INTO Livres (Titre, Id_Auteur, Nb_Pages, Prix)
                        VALUES (@Titre, @IdAuteur, @NbPages, @Prix)";

                    SqlParameter[] parameters = {
                        new SqlParameter("@Titre", txtTitreLivre.Text),
                        new SqlParameter("@IdAuteur", cbxAuteur.SelectedValue),
                        new SqlParameter("@NbPages", int.Parse(txtNbPages.Text)),
                        new SqlParameter("@Prix", decimal.Parse(txtPrix.Text))
                    };

                    dbHelper.ExecuteQuery(query, parameters);
                    MessageBox.Show("Livre ajouté avec succès.");
                    ClearInputs();
                    LoadBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout du livre : {ex.Message}");
            }
        }

        private void BtnModifierLivre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridLivres.SelectedItem is DataRowView selectedRow && ValidateInputs())
                {
                    string query = @"
                        UPDATE Livres
                        SET Titre = @Titre, Id_Auteur = @IdAuteur, Nb_Pages = @NbPages, Prix = @Prix
                        WHERE Id = @Id";

                    SqlParameter[] parameters = {
                        new SqlParameter("@Titre", txtTitreLivre.Text),
                        new SqlParameter("@IdAuteur", cbxAuteur.SelectedValue),
                        new SqlParameter("@NbPages", int.Parse(txtNbPages.Text)),
                        new SqlParameter("@Prix", decimal.Parse(txtPrix.Text)),
                        new SqlParameter("@Id", selectedRow["Id"])
                    };

                    dbHelper.ExecuteQuery(query, parameters);
                    MessageBox.Show("Livre modifié avec succès.");
                    ClearInputs();
                    LoadBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification du livre : {ex.Message}");
            }
        }

        private void BtnSupprimerLivre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridLivres.SelectedItem is DataRowView selectedRow)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Êtes-vous sûr de vouloir supprimer ce livre ?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        string query = "DELETE FROM Livres WHERE Id = @Id";
                        SqlParameter[] parameters = {
                            new SqlParameter("@Id", selectedRow["Id"])
                        };

                        dbHelper.ExecuteQuery(query, parameters);
                        MessageBox.Show("Livre supprimé avec succès.");
                        ClearInputs();
                        LoadBooks();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression du livre : {ex.Message}");
            }
        }

        private void DataGridLivres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridLivres.SelectedItem is DataRowView row)
            {
                txtTitreLivre.Text = row["Titre"].ToString();
                cbxAuteur.Text = row["Auteur"].ToString();
                txtNbPages.Text = row["Nb_Pages"].ToString();
                txtPrix.Text = row["Prix"].ToString();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTitreLivre.Text) ||
                cbxAuteur.SelectedValue == null ||
                !int.TryParse(txtNbPages.Text, out _) ||
                !decimal.TryParse(txtPrix.Text, out _))
            {
                MessageBox.Show("Veuillez remplir tous les champs correctement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            txtTitreLivre.Clear();
            txtNbPages.Clear();
            txtPrix.Clear();
            cbxAuteur.SelectedIndex = -1;
        }
    }
}
