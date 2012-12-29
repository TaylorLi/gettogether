using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Utility.Xml
{
    public class XPathOperator
    {
        private System.Xml.XPath.XPathDocument _XPathDoc;
        public XPathOperator(System.Xml.XPath.XPathDocument xPathDoc)
        {
            _XPathDoc = xPathDoc;
        }

        public XPathOperator(string uri)
        {
            _XPathDoc = new System.Xml.XPath.XPathDocument(uri);
        }

        public List<string> GetList(string xpath, string attrName, string namespaceUri)
        {
            if (_XPathDoc == null) return null;
            List<string> retList = new List<string>();
            System.Xml.XPath.XPathNavigator nav = _XPathDoc.CreateNavigator();
            System.Xml.XPath.XPathExpression expression = nav.Compile(xpath);
            System.Xml.XPath.XPathNodeIterator iterator = (System.Xml.XPath.XPathNodeIterator)nav.Evaluate(expression);
            while (iterator.MoveNext())
            {
                string s = string.Empty;
                if (string.IsNullOrEmpty(attrName))
                    s = iterator.Current.Value;
                else
                    s = iterator.Current.GetAttribute(attrName, namespaceUri);
                retList.Add(s);
                break;
            }
            return retList;
        }

        public string GetValue(string xpath, string attrName, string namespaceUri)
        {
            if (_XPathDoc == null) return string.Empty;

            System.Xml.XPath.XPathNodeIterator xni = _XPathDoc.CreateNavigator().Select(xpath);
            if (xni.MoveNext())
            {
                if (string.IsNullOrEmpty(attrName))
                    return xni.Current.Value;
                else
                    return xni.Current.GetAttribute(attrName, namespaceUri);
            }
            return string.Empty;
        }

        public static string GetValue(string xml, string xpath, bool isGetXml)
        {
            System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
            xd.LoadXml(xml);
            System.Xml.XmlNodeList xnl = xd.SelectNodes(xpath);
            if (xnl.Count > 0)
            {
                System.Text.StringBuilder sbValue = new System.Text.StringBuilder();
                foreach (System.Xml.XmlNode xn in xnl)
                {
                    sbValue.Append(isGetXml ? xn.InnerXml : xn.InnerText);
                    break;
                }
                return sbValue.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public static string[] GetArrayVal(string xml, string xpath, bool isGetXml)
        {
            System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
            xd.LoadXml(xml);
            System.Xml.XmlNodeList xnl = xd.SelectNodes(xpath);
            List<string> list = new List<string>();
            if (xnl.Count > 0)
            {
                foreach (System.Xml.XmlNode xn in xnl)
                {
                    list.Add(isGetXml ? xn.InnerXml : xn.InnerText);
                }
            }
            return list.ToArray();
        }
    }
}
