// Open GroceryItems.csv

// Parse all fields

// For each unique Department,
// Write to <DEPARTMENT>.txt
// Example: AISLE: 1, SHELF: 1, DESCRIPTION: Bread, SIZE: 10oz, DEPARTMENT: Bakery

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "GroceryItems.csv";
            string[] lines = File.ReadAllLines(path);

            List<GroceryItem> groceryItems = new List<GroceryItem>();

            foreach (string line in lines)
            {
                string[] fields = line.Split(',');

                GroceryItem groceryItem = new GroceryItem
                {
                    Aisle = fields[0],
                    Shelf = fields[1],
                    Description = fields[2],
                    Size = fields[3],
                    Department = fields[4]
                };

                groceryItems.Add(groceryItem);
            }

            var departments = groceryItems.Select(g => g.Department).Distinct();

            foreach (var department in departments)
            {
                // Clean up valid chars for filename
                string fileName = new string(department.Where(c => char.IsLetterOrDigit(c)).ToArray());

                string departmentPath = $"{fileName}.txt";

                using (StreamWriter writer = new StreamWriter(departmentPath))
                {
                    foreach (var groceryItem in groceryItems.Where(g => g.Department == department))
                    {
                        //writer.WriteLine($"AISLE: {groceryItem.Aisle}, SHELF: {groceryItem.Shelf}, DESCRIPTION: {groceryItem.Description}, SIZE: {groceryItem.Size}, DEPARTMENT: {groceryItem.Department}");
                        // Good, but please trim all values, remove linebreaks and commas
                        writer.WriteLine($"AISLE: {groceryItem.Aisle.Trim()}, SHELF: {groceryItem.Shelf.Trim()}, DESCRIPTION: {groceryItem.Description.Trim()}, SIZE: {groceryItem.Size.Trim()}, DEPARTMENT: {groceryItem.Department.Trim()}");
                    }
                }

                Console.WriteLine($"Wrote {departmentPath}");
            }
        }
    }

    class GroceryItem
    {
        public string Aisle { get; set; }
        public string Shelf { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Department { get; set; }
    }
}