using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryRecordSystem
{
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                using var writer = new StreamWriter(_filePath);
                writer.Write(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("No data file found.");
                    return;
                }

                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                _log.Clear();
                if (items != null)
                    _log.AddRange(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Chair", 15, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Desk", 8, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Mouse", 25, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Keyboard", 12, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            string filePath = "inventory.json";

            var app = new InventoryApp(filePath);
            app.SeedSampleData();
            app.SaveData();

            Console.WriteLine("Data saved to file.");

            var newApp = new InventoryApp(filePath);
            newApp.LoadData();
            Console.WriteLine("Loaded data from file:");
            newApp.PrintAllItems();
        }
    }
}
