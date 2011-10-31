using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kontakt.DAL;
using Kontakt.DataModel;
using System.Threading.Tasks;

namespace Kontakt.BL
{
    public abstract class StatisticsDataBase
    {
        public StatisticsDataBase(string connectionString, string usages2010Path, int filterDataId, int filterOrderedId, int filterConsumptionId)
        {
            this.DBHelper = new StatisticsDBHelper();
            this.DBHelper.AddConnection("2011", connectionString, 2011, true);

            this.ConnectionString = connectionString;
            this.Usages2010Path = usages2010Path;
            this.FilterDataId = filterDataId;
            this.FilterOrderedId = filterOrderedId;
            this.FilterConsumptionId = filterConsumptionId;
        }

        protected StatisticsDBHelper DBHelper { get; private set; }
        protected string ConnectionString { get; private set; }
        protected string Usages2010Path { get; private set; }
        protected int FilterDataId { get; private set; }
        protected int FilterOrderedId { get; private set; }
        protected int FilterConsumptionId { get; private set; }

        protected virtual IList<ItemData> GetAllItemsData()
        {
            List<UsagePrevious> usages = Serializator.Deserialize<List<UsagePrevious>>(this.Usages2010Path + ".xml");
            List<ItemData> data = new List<ItemData>();
            usages.ForEach(usage =>data.Add(new ItemData(usage.Code, usage.Usage)));
            return data;
        }

        public virtual Items GetStatisticData()
        {
            Items allData = new Items();

            List<ItemData> data = new List<ItemData>(this.GetAllItemsData());

            allData.Rows = data;

            List<ItemData> currentData = this.DBHelper.GetMainData(data, this.DBHelper.GetFilter(FilterDataId));

            // Tutoka nacitame polozky, ktore neboli zahrnute v metode DBHelper.GetMainData
            IEnumerable<ItemData> unprocessed = data.Where(d => string.IsNullOrEmpty(d.Name) || d.SKzId <= 0);

            // Skusime nacitavat nespracovane data vo viacerych threadoch
            // Toto mozno bude treba optimalizovat na nacitanie len urcitych nazvov naraz, teraz to ide po jednom
            Parallel.ForEach(unprocessed, item => this.DBHelper.GetMainItemData(item));

            // Nacitanie hmotnosti a skladovych zasob
            this.DBHelper.UpdateMainData(data);

            // Nacitanie spotreby pre aktualny rok
            this.DBHelper.UpdateConsumption(data, this.DBHelper.GetFilter(this.FilterConsumptionId));

            // A nakoniec nacitanie objednavok
            allData.Orders = this.DBHelper.UpdateOrdered(data, this.DBHelper.GetFilter(this.FilterOrderedId));

            return allData;
        }
    }
}
