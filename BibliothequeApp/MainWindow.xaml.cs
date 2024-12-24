using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace BibliothequeApp
{

    public partial class MainWindow : Window
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();
        private DataView booksView; 

        public MainWindow()
        {
            InitializeComponent();
            LoadBooks();
            LoadAuthors(); 
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CRUD_Auteur auteurWindow = new CRUD_Auteur();
            auteurWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CRUD_Livre livreWindow = new CRUD_Livre();
            livreWindow.Show();
        }

        private void LoadBooks()
        {
            try
            {
                string booksQuery = @"
        SELECT L.Id, L.Titre, L.Nb_Pages, L.Prix, A.Nom AS Auteur
        FROM Livres L
        INNER JOIN Auteurs A ON L.Id_Auteur = A.Id";

                string statsQuery = @"
        SELECT 
            COUNT(*) AS TotalBooks, 
            ISNULL(AVG(L.Prix), 0) AS AveragePrice 
        FROM Livres L";

                DataTable booksTable = dbHelper.ExecuteQuery(booksQuery);
                booksView = booksTable.DefaultView; 
                myDataGrid.ItemsSource = booksView;

                DataTable statsTable = dbHelper.ExecuteQuery(statsQuery);
                if (statsTable.Rows.Count > 0)
                {
                    totallivre.Content = statsTable.Rows[0]["TotalBooks"].ToString();
                    moyennedesprix.Content = statsTable.Rows[0]["AveragePrice"].ToString();
                }
                else
                {
                    totallivre.Content = "0";
                    moyennedesprix.Content = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des livres : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filtrer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (booksView != null)
            {
                string filterText = filtrer.Text.Trim();
                if (!string.IsNullOrEmpty(filterText))
                {
                    booksView.RowFilter = $"Titre LIKE '%{filterText.Replace("'", "''")}%'"; 
                }
                else
                {
                    booksView.RowFilter = string.Empty;
                }
            }
        }


        private void LoadAuthors()
        {
            string query = "SELECT Id, Nom FROM Auteurs";

            DataTable authorsTable = dbHelper.ExecuteQuery(query);
            FilterAuthorComboBox.ItemsSource = authorsTable.DefaultView;
            FilterAuthorComboBox.DisplayMemberPath = "Nom"; 
            FilterAuthorComboBox.SelectedValuePath = "Id";  
        }

        private void FilterAuthorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterAuthorComboBox.SelectedValue != null)
            {
                int authorId = (int)FilterAuthorComboBox.SelectedValue;

                string query = @"
            SELECT L.Id, L.Titre, L.Nb_Pages, L.Prix, A.Nom AS Auteur
            FROM Livres L
            INNER JOIN Auteurs A ON L.Id_Auteur = A.Id
            WHERE A.Id = @AuthorId";

                DataTable filteredTable = dbHelper.ExecuteQuery(query, new SqlParameter[]
                {
            new SqlParameter("@AuthorId", authorId)
                });

                myDataGrid.ItemsSource = filteredTable.DefaultView;
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedCriteria = (SortCriteriaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string sortOrder = AscendingRadioButton.IsChecked == true ? "ASC" : "DESC";

            string sortColumn = selectedCriteria switch
            {
                "Nombre de Pages" => "Nb_Pages",
                "Prix" => "Prix",
                "Titre" => "Titre",
                _ => "Titre" 
            };

            string query = $@"
        SELECT L.Id, L.Titre, L.Nb_Pages, L.Prix, A.Nom AS Auteur
        FROM Livres L
        INNER JOIN Auteurs A ON L.Id_Auteur = A.Id
        ORDER BY {sortColumn} {sortOrder}";

            DataTable sortedTable = dbHelper.ExecuteQuery(query);
            myDataGrid.ItemsSource = sortedTable.DefaultView;
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myDataGrid.SelectedItem is DataRowView row)
            {
                string titre = row["Titre"].ToString();
                string auteur = row["Auteur"].ToString();
                MessageBox.Show($"Livre sélectionné : {titre} par {auteur}");
            }
        }
    }
}

