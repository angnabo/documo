using System.Collections.Generic;
using System.Linq;
using Documo.Services;
using Documo.Visitor;
using HtmlAgilityPack;

namespace Documo.Strategies
{
    public class ProcessObjectPlaceholder : IProcessPlaceholder
    {
        public bool AppliesTo(DocumentPlaceholder placeholder)
        {
            return placeholder.GetType() == typeof(DocumentObject);
        }

        public void ProcessPlaceholders(HtmlDocument doc, DocumentPlaceholder placeholder, object jsonData)
        {
                var value = GetValue((DocumentObject)placeholder, jsonData);
                
                var placeholderNodes = HtmlNodeExtractor.SelectPlaceholderNodes(doc, placeholder.GetPlaceholder());
                    
                if (placeholderNodes == null) return;
                
                foreach (var node in placeholderNodes)
                {
                    HtmlNodeProcessor.ProcessPlaceholderNode(node, value);
                }
        }

        private string GetValue(DocumentObject placeholder, object jsonData){
            var jsonType = jsonData.GetType();
            var property = jsonType.GetProperty(placeholder.ObjectName);
            var propertyValue = property.GetValue(jsonData, null);

            if (placeholder.ObjectField != null)
            {
                var propertyValueType = propertyValue?.GetType();
                var memberProperty = propertyValueType?.GetProperty(placeholder.ObjectField);
                propertyValue = memberProperty?.GetValue(propertyValue, null); 
            }

            return propertyValue?.ToString();
        }

    }
    
    
}