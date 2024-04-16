using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using HtmlAgilityPack;

namespace rg_chat_toolkit_test_harness
{

    class WebScraper
    {
        public async static Task Parse(string url)
        {
            // Get whole URL contents as string: with HttpClient class
            string htmlContent = await new System.Net.Http.HttpClient().GetStringAsync(url);

            // Load the HTML into an HtmlDocument object
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Extract the pill details
            var pillDetailsNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'ddc-pid-list')]/div[@class='ddc-card']");

            if (pillDetailsNodes != null)
            {
                foreach (var node in pillDetailsNodes)
                {
                    string imprint = node.SelectSingleNode(".//dt[text()='Imprint']/following-sibling::dd")?.InnerText.Trim();
                    string strength = node.SelectSingleNode(".//dt[text()='Strength']/following-sibling::dd")?.InnerText.Trim();
                    string color = node.SelectSingleNode(".//dt[text()='Color']/following-sibling::dd")?.InnerText.Trim();
                    string shape = node.SelectSingleNode(".//dt[text()='Shape']/following-sibling::dd")?.InnerText.Trim();
                    string drugName = node.SelectSingleNode(".//div[@class='ddc-card-content ddc-card-content-pid']/a")?.InnerText.Trim();
                    string imageUrl = node.SelectSingleNode(".//img[@data-orig-width]")?.GetAttributeValue("src", "No image available");


                    Console.WriteLine("Drug Name: " + drugName);
                    Console.WriteLine("Imprint: " + imprint);
                    Console.WriteLine("Strength: " + strength);
                    Console.WriteLine("Color: " + color);
                    Console.WriteLine("Shape: " + shape);
                    Console.WriteLine("imageUrl: " + imageUrl);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No pill details found.");
            }
        }
    }

}
