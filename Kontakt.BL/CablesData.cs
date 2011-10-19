using System.Collections.Generic;
using Kontakt.DataModel;
using Kontakt.DAL;
using System.Linq;

namespace Kontakt.BL
{
    public class CablesData : StatisticsDataBase
    {
        public CablesData(string connectionString, int filterDataId, int filterOrderedId, int filterOnStockId)
            : base(connectionString, filterDataId, filterOrderedId, filterOnStockId)
        {
        }

        protected override IList<ItemData> GetAllItemsData()
        {
            List<ItemData> data = new List<ItemData>();
            //data.Add(new ItemData("624-L 6/6x19 FC gv", 1329, 0.1246));
            //data.Add(new ItemData("601-L 6/6x19 IWRC gv", 0, 0.1370));
            //data.Add(new ItemData("624-L 7/6x19 FC gv", 1776, 0.1700));
            //data.Add(new ItemData("601-L 7/6x19 IWRC gv", 0, 0.1860));
            //data.Add(new ItemData("624-L 8/6x19 FC gv", 516, 0.2214));
            //data.Add(new ItemData("601-L 8/6x19 IWRC gv", 13, 0.2440));
            //data.Add(new ItemData("625-L 8/6x19 IWSC gv", 0, 0.2440));
            //data.Add(new ItemData("624-L 9/6x19 FC gv", 511, 0.2800));
            //data.Add(new ItemData("625-L 9/6x19 IWSC gv", 0, 0.3080));
            //data.Add(new ItemData("601-L 9/6x19 IWRC gv", 0, 0.3080));
            //data.Add(new ItemData("625-L 10/6x19 IWSC gv", 566, 0.3810));
            //data.Add(new ItemData("624-L 12/6x19 FC gv", 670, 0.4980));
            //data.Add(new ItemData("625-L 12/6x19 IWSC gv", 0, 0.5480));
            //data.Add(new ItemData("601-L 12/6x19 IWRC gv", 0, 0.5480));
            //data.Add(new ItemData("624-L 14/6x19 FC gv", 211, 0.6780));
            //data.Add(new ItemData("601-L 16/6x37 IWRC gv", 0, 0.9740));
            //data.Add(new ItemData("624-L 16/6x37 FC gv", 1726, 0.8860));
            //data.Add(new ItemData("601-L 18/6x37 IWRC gv", 0, 1.2330));
            //data.Add(new ItemData("624-L 18/6x37 FC gv", 1224, 1.1210));
            //data.Add(new ItemData("601-L 20/6x37 IWRC gv", 0, 1.5220));
            //data.Add(new ItemData("624-L 20/6x37 FC gv", 541, 1.3840));
            //data.Add(new ItemData("624-L 22/6x37 FC gv", 121, 1.6750));
            //data.Add(new ItemData("624-L 26/6x37 FC gv", 704, 2.3390));
            //data.Add(new ItemData("601-L 26/6x37 IWRC gv", 0, 2.5730));
            //data.Add(new ItemData("624-L 28/6x37 FC gv", 73, 2.7130));
            //data.Add(new ItemData("601-L 32/6x37 IWRC gv", 0, 3.8970));
            //data.Add(new ItemData("624-L 32/6x37 FC gv", 624, 3.5430));
            data.Add(new ItemData("624-L 6/6x19 FC gv", 1329));
            data.Add(new ItemData("601-L 6/6x19 IWRC gv", 0));
            data.Add(new ItemData("624-L 7/6x19 FC gv", 1776));
            data.Add(new ItemData("601-L 7/6x19 IWRC gv", 0));
            data.Add(new ItemData("624-L 8/6x19 FC gv", 516));
            data.Add(new ItemData("601-L 8/6x19 IWRC gv", 13));
            data.Add(new ItemData("625-L 8/6x19 IWSC gv", 0));
            data.Add(new ItemData("624-L 9/6x19 FC gv", 511));
            data.Add(new ItemData("625-L 9/6x19 IWSC gv", 0));
            data.Add(new ItemData("601-L 9/6x19 IWRC gv", 0));
            data.Add(new ItemData("625-L 10/6x19 IWSC gv", 566));
            data.Add(new ItemData("624-L 12/6x19 FC gv", 670));
            data.Add(new ItemData("625-L 12/6x19 IWSC gv", 0));
            data.Add(new ItemData("601-L 12/6x19 IWRC gv", 0));
            data.Add(new ItemData("624-L 14/6x19 FC gv", 211));
            data.Add(new ItemData("601-L 16/6x37 IWRC gv", 0));
            data.Add(new ItemData("624-L 16/6x37 FC gv", 1726));
            data.Add(new ItemData("601-L 18/6x37 IWRC gv", 0));
            data.Add(new ItemData("624-L 18/6x37 FC gv", 1224));
            data.Add(new ItemData("601-L 20/6x37 IWRC gv", 0));
            data.Add(new ItemData("624-L 20/6x37 FC gv", 541));
            data.Add(new ItemData("624-L 22/6x37 FC gv", 121));
            data.Add(new ItemData("624-L 26/6x37 FC gv", 704));
            data.Add(new ItemData("601-L 26/6x37 IWRC gv", 0));
            data.Add(new ItemData("624-L 28/6x37 FC gv", 73));
            data.Add(new ItemData("601-L 32/6x37 IWRC gv", 0));
            data.Add(new ItemData("624-L 32/6x37 FC gv", 624));
            return data;
        }
    }

    public class CablesDataFiltered : CablesData
    {
        private const string CODE_601 = "601";

        public CablesDataFiltered(string connectionString, int filterDataId, int filterOrderedId, int filterOnStockId)
            : base(connectionString, filterDataId, filterOrderedId, filterOnStockId)
        {
        }

        public override Items GetStatisticData()
        {
            Items originalData = base.GetStatisticData();

            Items data = new Items() { Orders = originalData.Orders };

            List<ItemData> processed = new List<ItemData>();

            foreach (ItemData item in originalData.Rows)
            {
                if (!string.IsNullOrWhiteSpace(item.Code) && item.Code.StartsWith(CODE_601))
                {
                    processed.Add(item);
                    data.Rows.Add(item);
                    string[] parts = item.Code.Split(' ');
                    if (parts.Length > 1)
                    {
                        string common = parts[1];
                        IEnumerable<ItemData> partialData = originalData.Rows.Where(it => it.Code.Contains(common) && 
                            !it.Code.StartsWith(CODE_601));
                        foreach (ItemData partial in partialData)
                        {
                            processed.Add(partial);
                            item.OnStock += partial.OnStock;
                            //item.Ordered += partial.Ordered;
                            foreach (string year in partial.Usages.Keys)
                            {
                                if (item.Usages.ContainsKey(year))
                                    item.Usages[year] += partial.Usages[year];
                                else
                                    item.Usages.Add(year, partial.Usages[year]);
                            }
                            foreach (OrderedItem order in partial.Ordered)
                            {
                                OrderedItem old = item.Ordered.FirstOrDefault(ord=>ord.Id == order.Id);
                                if (old == null)
                                {
                                    old = new OrderedItem() { Code = order.Code, Id = order.Id };
                                    item.Ordered.Add(old);
                                }
                                old.Value += order.Value;
                            }
                        }
                    }
                }
            }
            // Este pridame tie, ktore sa doteraz nespracovali, bude to zobrazene na konci statistiky
            IEnumerable<ItemData> unprocessed = originalData.Rows.Where(he => !processed.Contains(he));
            foreach(ItemData item in unprocessed)
                data.Rows.Add(item);
            return data;
        }
    }
}
