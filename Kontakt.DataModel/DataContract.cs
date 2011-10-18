using System.Collections.Generic;
using System;
using System.Linq;

namespace Kontakt.DataModel
{
    public class ItemData
    {
        protected ItemData()
        {
            this.Usages = new Dictionary<string, double>();
        }

        public ItemData(string code)
            : this()
        {
            this.Code = code;
            this.Ordered = new List<OrderedItem>();
        }

        public ItemData(string code, double usage2010)
            : this(code)
        {
            this.AddUsage("2010", usage2010);
        }

        public ItemData(string code, double usage2010, double kgPerUnit)
            : this(code, usage2010)
        {
            this.KgPerUnit = kgPerUnit;
        }

        public int SKzId { get; set; }
        public string Code { get; private set; }
        public string Name { get; set; }
        public double KgPerUnit { get; set; }
        public Dictionary<string, double> Usages { get; private set; }
        public double OnStock { get; set; }
        public List<OrderedItem> Ordered { get; private set; }

        public void AddUsage(string key, double value)
        {
            if (!this.Usages.ContainsKey(key))
                this.Usages.Add(key, 0);
            this.Usages[key] += value;
        }

        public void AddOrder(int id, string code, double value)
        {
            OrderedItem item = this.Ordered.FirstOrDefault(it => it.Id == id);
            if (item == null)
            {
                item = new OrderedItem() { Id = id, Code = code, Value = value };
                this.Ordered.Add(item);
            }
        }
    }

    public class Items
    {
        public Items()
        {
            this.Rows = new List<ItemData>();
            this.Orders = new List<OrderedItems>();
        }

        public List<ItemData> Rows { get; set; }
        public List<OrderedItems> Orders { get; set; }
    }

    public class OrderedItems
    {
        public OrderedItems()
        {
            //this.Items = new List<OrderedItem>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDateSK { get; set; }
        //public List<OrderedItem> Items { get; private set; }
    }

    public class OrderedItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double Value { get; set; }
    }
}
