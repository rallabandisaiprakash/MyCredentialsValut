using MyCredentialsValut.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
namespace MyCredentialsValut
{
    //    /// <summary>
    //    /// Interaction logic for MainWindow.xaml
    //    /// </summary>
    public partial class MainWindow : Window
    {
        //private const string FileName = "credentials.json";
        private readonly string FileName =
        Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MyCredentialsVault",
        "credentials.json");

        private Credential? selectedCredentialForEdit;

        //private List<Credential> credentials = new();
        private ObservableCollection<Credential> credentials = new();

        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show(FileName);
            LoadCredentials();
        }

        private void LoadCredentials()
        {
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);

                //credentials = JsonSerializer.Deserialize<List<Credential>>(json)
                //?? new List<Credential>();

                var loaded = JsonSerializer.Deserialize<List<Credential>>(json) ?? new List<Credential>();

                credentials = new ObservableCollection<Credential>(loaded);
            }
            RefreshGrid();
        }

        //private void SaveCredentials()
        //{
        //    string json = JsonSerializer.Serialize(
        //    credentials,
        //    new JsonSerializerOptions
        //    {
        //        WriteIndented = true
        //    });

        //    File.WriteAllText(FileName, json);
        //}

        private void SaveCredentials()
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(FileName)!);

            string json = JsonSerializer.Serialize(
                credentials,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(FileName, json);
        }



        private void ViewPassword_Click(object sender, RoutedEventArgs e)
        {
            if (dgCredentials.SelectedItem is not Credential credential)
            {
                MessageBox.Show("Please select a record.");
                return;
            }

            MessageBox.Show(
                $"Username : {credential.UserName}\nPassword : {credential.Password}",
                credential.EntityName);
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (dgCredentials.SelectedItem is Credential credential)
            {
                Clipboard.SetText(credential.Password);

                MessageBox.Show(
                    "Password copied to clipboard.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "Please select a record.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void RefreshGrid()
        {
            dgCredentials.ItemsSource = null;

            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgCredentials.ItemsSource = credentials;
            }
            else
            {
                dgCredentials.ItemsSource = credentials
                    .Where(x =>
                        x.EntityName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        x.UserName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            dgCredentials.Items.Refresh();
        }



        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                dgCredentials.ItemsSource = credentials;
                return;
            }

            var filteredData = credentials
                .Where(x =>
                    x.EntityName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.UserName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            dgCredentials.ItemsSource = filteredData;
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dgCredentials.SelectedItem is not Credential credential)
            {
                MessageBox.Show("Please select a record.");
                return;
            }

            txtEntity.Text = credential.EntityName;
            txtUserName.Text = credential.UserName;
            txtPassword.Text = credential.Password;

            selectedCredentialForEdit = credential;

            btnSave.Content = "Update";
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgCredentials.SelectedItem is not Credential credential)
            {
                MessageBox.Show("Please select a record.");
                return;
            }

            var result = MessageBox.Show(
                $"Delete '{credential.EntityName}' ?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                credentials.Remove(credential);

                SaveCredentials();

                RefreshGrid();
            }
        }

        //private void Save_Click(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(txtEntity.Text))
        //    {
        //        MessageBox.Show("Please enter entity name");
        //        return;
        //    }

        //    credentials.Add(new Credential
        //    {
        //        EntityName = txtEntity.Text,
        //        UserName = txtUserName.Text,
        //        Password = txtPassword.Text
        //    });

        //    SaveCredentials();

        //    RefreshGrid();

        //    txtEntity.Clear();
        //    txtUserName.Clear();
        //    txtPassword.Clear();
        //}

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEntity.Text))
            {
                MessageBox.Show("Please enter Entity Name");
                return;
            }

            if (selectedCredentialForEdit != null)
            {
                selectedCredentialForEdit.EntityName = txtEntity.Text;
                selectedCredentialForEdit.UserName = txtUserName.Text;
                selectedCredentialForEdit.Password = txtPassword.Text;

                selectedCredentialForEdit = null;
            }
            else
            {
                credentials.Add(new Credential
                {
                    EntityName = txtEntity.Text,
                    UserName = txtUserName.Text,
                    Password = txtPassword.Text
                });
            }

            SaveCredentials();
            RefreshGrid();

            txtEntity.Clear();
            txtUserName.Clear();
            txtPassword.Clear();

            btnSave.Content = "Save";
        }
    }
}

