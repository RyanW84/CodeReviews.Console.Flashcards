﻿using Dapper;
using Flashcards.RyanW84;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

public class DataAccess
{
    IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    private string? ConnectionString;

    public DataAccess()
    {
        ConnectionString = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public bool ConfirmConnection() //Confirms the connection
    {
        try
        {
            Console.WriteLine("*_*_*_* Flashcards *_*_*_* ");
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                Console.Write(
                    $"\nConnection Status: {System.Data.ConnectionState.Open}\nPress any Key to continue:"
                );
                Console.ReadKey();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    internal void CreateTables()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string createStackTableSql =
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                    CREATE TABLE Stacks (
                        Id int IDENTITY(1,1) NOT NULL,
                        Name NVARCHAR(30) NOT NULL UNIQUE,
                        PRIMARY KEY (Id)
                    );";
            connection.Execute(createStackTableSql);

            string createFlashcardTableSql =
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Flashcards')
                    CREATE TABLE Flashcards (
                        Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        Question NVARCHAR(30) NOT NULL,
                        Answer NVARCHAR(30) NOT NULL,
                        StackId int NOT NULL 
                            FOREIGN KEY 
                            REFERENCES Stacks(Id) 
                            ON DELETE CASCADE 
                            ON UPDATE CASCADE
                    );";
            connection.Execute(createFlashcardTableSql);
        }
    }

    internal void StudyArea() { }

    internal void AddStack()
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                Console.WriteLine("Enter the name of the stack you want to add: ");

                string? stackName = Console.ReadLine();

                string addStackSql = @"INSERT INTO Stacks (Name) VALUES (@Name);";

                connection.Execute(addStackSql, new { Name = stackName });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem in the adding section {ex.Message}");
        }
        UserInterface.StackMenu();
    }

    internal void DeleteStack() { }

    internal void UpdateStack() { }

    internal IEnumerable<Stacks> GetStacks()
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string getStacksSQL = @"SELECT * FROM stacks;";

                var stacks = connection.Query<Stacks>(getStacksSQL);

                return stacks;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when viewing stacks: {ex.Message}");
        }
    }

    internal void ViewStack(IEnumerable<Stacks> stacks)
    {
        Console.Clear();
        Console.ReadKey();
        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Name");

        foreach (var stack in stacks)
        {
            table.AddRow(stack.Id.ToString(), stack.Name.ToString());
        }

        AnsiConsole.Write(table);
    }

    internal void AddFlashcard() { }

    internal void DeleteFlashcard() { }

    internal void UpdateFlashcard() { }

    internal void GetFlashcards() { }

    internal void ViewFlashcard() { }

    internal class Stacks
    {
        internal int Id { get; set; }
        internal string? Name { get; set; }
    }

    internal class Flashcards
    {
        internal int Id { get; set; }
        internal string? Question { get; set; }
        internal string? Answer { get; set; }
        internal int StackID { get; set; }
    }
}
