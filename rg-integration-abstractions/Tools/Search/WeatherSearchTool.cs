using rg_chat_toolkit_cs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rg_integration_abstractions.Tools.Search;

//public class WeatherSearchTool: ToolBase
//{

//    public async Task<Message> GetToolResponse(Message toolCall)
//    {
//        throw new ApplicationException("Not implemented");
//    }

//    //    string unvalidatedArguments = toolCall.FunctionAguments;

//    //    // Deserialize the JSON in unvalidatedArguments; get the latitude and longitude
//    //    // {"latitude": 35.1495, "longitude": -90.049, "unit": "celsius"}
//    //    var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(unvalidatedArguments);
//    //    decimal latitude;
//    //    decimal longitude;


//    //                if (arguments.TryGetValue("latitude", out object latitudeObj) && latitudeObj is JsonElement latitudeElement &&
//    //                        arguments.TryGetValue("longitude", out object longitudeObj) && longitudeObj is JsonElement longitudeElement)
//    //                {
//    //                    latitude = latitudeElement.GetDecimal();
//    //                    longitude = longitudeElement.GetDecimal();
//    //                }
//    //                else
//    //                {
//    //                    // Handle missing or invalid latitude or longitude
//    //                    throw new ArgumentException("Invalid arguments. 'latitude' and 'longitude' are required and must be decimal.");
//    //                }


//    //                // Lookup data via https://api.open-meteo.com/v1/forecast?latitude=35.1495&longitude=-90.0490&current=temperature_2m
//    //                const string URL_TEMPLATE = "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&current=temperature_2m";

//    ////decimal latitude = 35.1495m;
//    ////decimal longitude = -90.0490m;
//    //string url = String.Format(URL_TEMPLATE, latitude, longitude);
//    //// Get whole URL contents as string: with HttpClient class
//    //string htmlContent = await new System.Net.Http.HttpClient().GetStringAsync(url);
//    //return new Message(role: "tool", content: htmlContent);
//}
