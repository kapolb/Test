using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExcelBL
{
    [Serializable]
    public class Values
    {
        [XmlElement("code")]
        public string Code { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("kgPerUnit")]
        public string KgPerUnit { get; set; }

        [XmlElement("usage2010")]
        public string Usage2010 { get; set; }

        [XmlElement("usage2011")]
        public string Usage2011 { get; set; }

        [XmlElement("onStock")]
        public string OnStock { get; set; }

        [XmlArray("ordered")]
        [XmlArrayItem("order")]
        public List<string> Ordered { get; set; }
    }

    [Serializable]
    public class Formula
    {
        [XmlElement("cell")]
        public string Cell { get; set; }

        [XmlElement("value")]
        public string Value { get; set; }
    }

    [Serializable]
    public class Order
    {
        [XmlElement("orderDate")]
        public string OrderDate { get; set; }

        [XmlElement("deliveryDate")]
        public string DeliveryDate { get; set; }
    }

    [Serializable]
    [XmlRoot("template")]
    public class Template
    {
        [XmlElement("createDate")]
        public string CreateDate { get; set; }

        [XmlElement("firstRow")]
        public int FirstRow { get; set; }

        [XmlArray("orders")]
        [XmlArrayItem("order")]
        public List<Order> Orders { get; set; }

        [XmlElement("values")]
        public Values Values { get; set; }

        [XmlArray("formulas")]
        [XmlArrayItem("formula")]
        public List<Formula> Formulas { get; set; }

        [XmlElement("orderedSum")]
        public Formula OrderedSum { get; set; }
    }
}
