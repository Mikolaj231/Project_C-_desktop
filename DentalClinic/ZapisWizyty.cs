﻿using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DentalClinic
{
    public partial class ZapisWizyty : Form
    {
        private string userName; // Zmienna do przechowywania nazwy użytkownika
        private long visitId; // Zmienna do przechowywania ID wizyty

        // Konstruktor przyjmujący UserName i opcjonalnie ID wizyty
        public ZapisWizyty(string userName, long visitId = 0)
        {
            InitializeComponent();
            this.userName = userName; // Zapisz UserName
            this.visitId = visitId; // Zapisz ID wizyty
        }

        //ladowanie danych lekarza i DT
        public void LoadData(string imieLekarza, string dataIczas, long visitId)
        {
            // Ustawienie tekstu w odpowiednich kontrolkach
            labelImieLekarza.Text = $"Imię Lekarza: {imieLekarza}";
            labelDataICzas.Text = $"Data i Czas: {dataIczas}";


            // Ładowanie danych wizyty (jeśli potrzebne)
            if (visitId > 0)
            {
                LoadVisitData(visitId);
            }
        }


        //zaladoowanie bd danych
        public void LoadVisitData(long visitId)
        {
            // Ładowanie danych wizyty z bazy danych na podstawie ID wizyty
            using (var connection = new SQLiteConnection(@"Data Source=C:\Users\halin\Desktop\DentalClinic\DataBase\DentalClinic.db;Version=3;"))
            {
                connection.Open();
                string query = "SELECT * FROM Wizyty WHERE id = @id"; // Użyj małej litery "id"
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", visitId); // Użyj małej litery "id"
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            NazwiskoPacjenta.Text = reader["NazwiskoPacjenta"].ToString();
                            ImiePacjenta.Text = reader["ImiePacjenta"].ToString();
                            WiekPacjenta.Text = reader["WiekPacjenta"].ToString();
                            NrTelefonuPacjenta.Text = reader["NrTelKlienta"].ToString();
                            EmailPacjenta.Text = reader["MailKlienta"].ToString();
                            Plec.Text = reader["PlecP"].ToString();
                            OpiszProblem.Text = reader["OpisProblemu"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Nie znaleziono wizyty o podanym ID.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        //wyjdz
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Zamknięcie formularza
        }

        //akceptacja zapisu
        private void button1_Click(object sender, EventArgs e)
        {
            // Sprawdzenie, czy wszystkie pola zostały wypełnione
            if (string.IsNullOrWhiteSpace(NazwiskoPacjenta.Text) ||
                string.IsNullOrWhiteSpace(ImiePacjenta.Text) ||
                string.IsNullOrWhiteSpace(WiekPacjenta.Text) ||
                string.IsNullOrWhiteSpace(NrTelefonuPacjenta.Text) ||
                string.IsNullOrWhiteSpace(EmailPacjenta.Text) ||
                string.IsNullOrWhiteSpace(Plec.Text) ||
                string.IsNullOrWhiteSpace(OpiszProblem.Text)) // Upewnij się, że opis problemu jest też wypełniony
            {
                MessageBox.Show("Proszę wypełnić wszystkie pola przed zapisaniem wizyty.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Zatrzymaj wykonanie, jeśli nie wszystkie pola są wypełnione
            }

            // Wywołanie metody zapisu wizyty
            SaveVisit();
        }

        //Zapis wizyt do bazy
        private void SaveVisit()
        {
            try
            {
                // Przygotowanie danych do zapisania wizyty
                string nazwisko = NazwiskoPacjenta.Text;
                string imie = ImiePacjenta.Text;
                int wiek;
                if (!int.TryParse(WiekPacjenta.Text, out wiek))
                {
                    MessageBox.Show("Proszę wprowadzić poprawny wiek.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Zatrzymaj wykonanie, jeśli wiek jest niepoprawny
                }

                string nrTelefonu = NrTelefonuPacjenta.Text;
                string email = EmailPacjenta.Text;
                string plec = Plec.Text;
                string opisProblemu = OpiszProblem.Text;

                // Wyciąganie imienia lekarza i daty wizyty z etykiet
                string imieLekarza = labelImieLekarza.Text.Replace("Imię Lekarza: ", "").Trim();
                string dataIczas = labelDataICzas.Text.Replace("Data i Czas: ", "").Trim();

                // Nawiązanie połączenia z bazą danych
                using (var connection = new SQLiteConnection(@"Data Source=C:\Users\halin\Desktop\DentalClinic\DataBase\DentalClinic.db;Version=3;"))
                {
                    connection.Open(); // Otwieranie połączenia

                    if (visitId > 0)
                    {
                        // Wykonaj aktualizację wizyty
                        string updateQuery = "UPDATE Wizyty SET NazwiskoPacjenta = @NazwiskoPacjenta, OpisProblemu = @OpisProblemu, ImiePacjenta = @ImiePacjenta, " +
                                             "WiekPacjenta = @WiekPacjenta, NrTelKlienta = @NrTelKlienta, MailKlienta = @MailKlienta, PlecP = @PlecP, ImieLekarza = @ImieLekarza, DataICzas = @DataICzas " +
                                             "WHERE id = @id"; // Użyj małej litery "id"

                        using (var updateCommand = new SQLiteCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@NazwiskoPacjenta", nazwisko);
                            updateCommand.Parameters.AddWithValue("@OpisProblemu", opisProblemu);
                            updateCommand.Parameters.AddWithValue("@ImiePacjenta", imie);
                            updateCommand.Parameters.AddWithValue("@WiekPacjenta", wiek);
                            updateCommand.Parameters.AddWithValue("@NrTelKlienta", nrTelefonu); // Dodanie numeru telefonu
                            updateCommand.Parameters.AddWithValue("@MailKlienta", email);
                            updateCommand.Parameters.AddWithValue("@PlecP", plec);
                            updateCommand.Parameters.AddWithValue("@ImieLekarza", imieLekarza);
                            updateCommand.Parameters.AddWithValue("@DataICzas", dataIczas);
                            updateCommand.Parameters.AddWithValue("@id", visitId); // Użycie małej litery "id"

                            updateCommand.ExecuteNonQuery(); // Wykonaj aktualizację
                        }
                    }
                    else
                    {
                        // Pacjent nie istnieje, wykonaj wstawienie
                        string insertQuery = "INSERT INTO Wizyty (NazwiskoPacjenta, OpisProblemu, ImiePacjenta, WiekPacjenta, NrTelKlienta, MailKlienta, PlecP, ImieLekarza, DataICzas) " +
                                             "VALUES (@NazwiskoPacjenta, @OpisProblemu, @ImiePacjenta, @WiekPacjenta, @NrTelKlienta, @MailKlienta, @PlecP, @ImieLekarza, @DataICzas)";

                        using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@NazwiskoPacjenta", nazwisko);
                            insertCommand.Parameters.AddWithValue("@OpisProblemu", opisProblemu);
                            insertCommand.Parameters.AddWithValue("@ImiePacjenta", imie);
                            insertCommand.Parameters.AddWithValue("@WiekPacjenta", wiek);
                            insertCommand.Parameters.AddWithValue("@NrTelKlienta", nrTelefonu); // Dodanie numeru telefonu
                            insertCommand.Parameters.AddWithValue("@MailKlienta", email);
                            insertCommand.Parameters.AddWithValue("@PlecP", plec);
                            insertCommand.Parameters.AddWithValue("@ImieLekarza", imieLekarza);
                            insertCommand.Parameters.AddWithValue("@DataICzas", dataIczas);

                            insertCommand.ExecuteNonQuery(); // Wykonaj wstawienie
                        }
                    }

                    MessageBox.Show("Dane wizyty zostały zapisane pomyślnie.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas zapisywania wizyty: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ZapisWizyty_Load(object sender, EventArgs e){}
        private void labelImieLekarza_TextChanged(object sender, EventArgs e) { }
        private void labelDataICzas_TextChanged(object sender, EventArgs e) { }
        private void OpiszProblem_TextChanged(object sender, EventArgs e) { }
        private void NazwiskoPacjenta_TextChanged(object sender, EventArgs e) { }
        private void ImiePacjenta_TextChanged(object sender, EventArgs e) { }
        private void WiekPacjenta_TextChanged(object sender, EventArgs e) { }
        private void NrTelefonuPacjenta_TextChanged(object sender, EventArgs e) { }
        private void EmailPacjenta_TextChanged(object sender, EventArgs e) { }
        private void Plec_TextChanged(object sender, EventArgs e) { }
    }
}
