using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Antlr4.Runtime;
using Documo.Services;
using Documo.Strategies;
using Documo.Visitor;
using HtmlAgilityPack;

namespace Documo.Renderer
{
    public class HtmlRenderer
    {
        private List<IProcessPlaceholder> _placeholderStrategies = new List<IProcessPlaceholder>();
        public HtmlRenderer()
        {
            _placeholderStrategies.Add(new ProcessObjectPlaceholder());
            _placeholderStrategies.Add(new ProcessRepeatingSectionPlaceholders());
        }

        public async Task<IDocument> openDocument(string path)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var file = File.ReadAllText(path);
            return await context.OpenAsync(req => req.Content(file));
        }

        public async void Render(object jsonData)
        {
            try
            {
                
                var doc = await openDocument("/home/angelica/RiderProjects/Documo/Documo/NewFile1.html");
                

                //var placeholders = HtmlNodeExtractor.ExtractNodeOuterHtml(doc, "//p[@class='placeholder']");
                var placeholders = doc.All.Where(x => x.LocalName == "p" && x.ClassList.Contains("placeholder")).Select(x => x.TextContent);
                
                
                var input = string.Join("", placeholders);
                var antlrService = new AntlrService();
                var parsedPlaceholders = antlrService.Parse(input);
                
                foreach (var placeholder in parsedPlaceholders)
                {  
                    var placeholderNodes = doc.All.Where(x => x.LocalName == "p" 
                                                              && x.ClassList.Contains("placeholder") 
                                                              && x.TextContent == placeholder.GetPlaceholder());
                    if (!placeholderNodes.Any()) continue;
                    
                    var strategy = _placeholderStrategies.SingleOrDefault(x => x.AppliesTo(placeholder));
                    strategy?.ProcessPlaceholders(doc, placeholder, jsonData);
                    
                }

                File.WriteAllText("/home/angelica/RiderProjects/Documo/Documo/OutputHtml.html", doc.DocumentElement.OuterHtml);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }
    }
}