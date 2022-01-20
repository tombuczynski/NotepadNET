using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tomproj.WPFUtils
{
    public class XMLSettings
    {
        private readonly XDocument Doc;
        private readonly XElement Root;
        private XElement SectionElement = null;

        public XMLSettings()
        {
            Root = new XElement("settings");
            Doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XComment("Application settings"), Root);
            SectionElement = Root;
        }

        public XMLSettings(XDocument xDoc)
        {
            Doc = xDoc;
            Root = Doc.Root;
            SectionElement = Root;
        }

        public void OpenSection(params string[] names)
        {
            XElement el = Root, elNew =null;

            foreach(var name in names)
            {
                elNew = el.Element(name);
                if (elNew == null)
                {
                    el.SetElementValue(name, string.Empty);
                    el = el.Element(name);
                } else
                {
                    el = elNew;
                }
            }

            SectionElement = el;
        }

        public string Content
        {
            get
            {
                string v = SectionElement?.Value;

                if (v == null)
                {
                    v = string.Empty;
                }
                return v;
            }

            set => SectionElement.SetValue(value);
        }

        public string this[string name]
        {
            get
            {
                string v = SectionElement.Attribute(name)?.Value;

                if (v == null)
                {
                    v = string.Empty;
                }
                return v;
            }

            set => SectionElement.SetAttributeValue(name, value);
        }

        public void Save(string filePath)
        {
            Doc.Save(filePath);
        }

        public static XMLSettings Load(string filePath)
        {
            try
            {
                XDocument xdoc = XDocument.Load(filePath);
                return new XMLSettings(xdoc);
            }
            catch
            {
                return new XMLSettings();
            }
        }
    }
}
