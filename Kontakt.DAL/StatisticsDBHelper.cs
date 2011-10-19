using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Kontakt.DataModel;
using System.ComponentModel;

namespace Kontakt.DAL
{
    public class StatisticsConnection
    {
        public string Key { get; set; }
        public string ConnectionString { get; set; }
        public bool IsCurrent { get; set; }
        public int Year { get; set; }
    }

    public class StatisticsDBHelper
    {
        private const int STATISTICS_TYPE = 55;

        private List<StatisticsConnection> _connections = new List<StatisticsConnection>();

        //public StatisticsDBHelper(string connectionString)
        //{
        //    if (string.IsNullOrEmpty(connectionString))
        //        throw new ArgumentNullException("connectionString");

        //    this._connectionString = connectionString;
        //}

        public void AddConnection(string key, string connectionString, int year)
        {
            this.AddConnection(key, connectionString, year, false);
        }

        public void AddConnection(string key, string connectionString, int year, bool isCurrent)
        {
            if (year <= 2005 || year > DateTime.Today.Year)
                throw new ArgumentOutOfRangeException("year");
            if (isCurrent && this._connections.FirstOrDefault(conn => conn.IsCurrent) != null)
                throw new ApplicationException("Current connection already exists.");
            this._connections.Add(new StatisticsConnection() { Key = key, ConnectionString = connectionString, IsCurrent = isCurrent, Year = year });
        }

        //public void SetConnection(string connectionString)
        //{
        //    if (string.IsNullOrEmpty(connectionString))
        //        throw new ArgumentNullException("connectionString");

        //    this._connectionString = connectionString;
        //}

        public DataView GetConditionForStatistics()
        {
            StatisticsConnection conn = this.GetCurrentConnection();
            SqlConnection connection = new SqlConnection(conn.ConnectionString);
            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select * from Dotazy where RelAg = {0}", STATISTICS_TYPE), connection))
            {
                DataTable table = new DataTable("Dotazy");
                adapter.Fill(table);
                connection.Close();
                return table.DefaultView;
            }
        }


        /// <summary>
        /// Gets the statistics data.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="month">The month.</param>
        /// <returns>DataView.</returns>
        public DataView GetStatisticsData(string condition, int month)
        {
            StatisticsConnection conn = this.GetCurrentConnection();
            SqlConnection connection = new SqlConnection(conn.ConnectionString);
            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select * from qAgSKzPoh where {0} and datepart(mm, Datum) = {1} order by Datum", condition, month), connection))
            {
                DataTable table = new DataTable("qAgSKzPoh");
                adapter.Fill(table);
                connection.Close();
                return table.DefaultView;
            }
        }

        private DataView GetStatisticsData(string condition, string connectionString, int year)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string currentCondition = condition;
            if (year != DateTime.Today.Year)
                currentCondition = this.ModifyCondition(currentCondition, year);

            if (currentCondition.Contains("qAgSKzPoh.Datum = #YEAR#"))
            {
                currentCondition = currentCondition.Replace("qAgSKzPoh.Datum = #YEAR#", "YEAR(qAgSKzPoh.Datum) = 2011");
            }

            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select * from qAgSKzPoh where {0} order by Datum", currentCondition), connection))
            {
                DataTable table = new DataTable("qAgSKzPoh");
                adapter.Fill(table);
                connection.Close();
                return table.DefaultView;
            }
        }

        private string ModifyCondition(string condition, int year)
        {
            if (condition.Contains("Datum"))
            {
                // Tak toto nie je prave idealne, preplacne kazdu hodnotu 2011 pozadovanym rokom
                condition = condition.Replace(DateTime.Today.Year.ToString(), year.ToString());
                condition = condition.Replace("%výr%", "%vyr%");
                // Malo by sa to nejako vyhladavat za pomoci "Datum" a potom cekovat co je za nim a zmenit to len v CONVERT(DATETIME, '01/01/2011', 101)
                //int idx = condition.IndexOf("Datum");
            }
            return condition;
        }

        public List<ItemData> GetMainData(List<ItemData> data, string condition)
        {
            // Naplname len nazov a Id v SKz
            StatisticsConnection conn = this.GetCurrentConnection();
            DataView statAllData = this.GetFilteredGoodsData(condition);
            //Dictionary<string, ItemData> stAll = new Dictionary<string, ItemData>();
            foreach (DataRow row in statAllData.Table.Rows)
            {
                //ItemData st = null;
                string code = row["IDS"] != DBNull.Value ? row["IDS"].ToString() : null;

                if (string.IsNullOrEmpty(code))
                    continue;

                ItemData st = data.FirstOrDefault(item => item.Code == code);

                if (st != null && (string.IsNullOrEmpty(st.Name) || st.SKzId <= 0))
                {
                    st.Name = row["Nazev"] != DBNull.Value ? row["Nazev"].ToString() : null;
                    st.SKzId = row["ID"] != DBNull.Value ? (int)row["ID"] : -1;
                }
            }

            return null;
        }

        /// <summary>
        /// Tu sa nacitava nazov, hmtnost a stav zasob pre 1 polozku.
        /// </summary>
        /// <param name="item">The item.</param>
        public void GetMainItemData(ItemData item)
        {
            if (string.IsNullOrWhiteSpace(item.Code))
                throw new NullReferenceException("item.code");
            DataView goodsData = this.GetFilteredGoodsData(string.Format("IDS = '{0}'", item.Code));
            if (goodsData != null)
            {
                if (goodsData.Table.Rows.Count == 0)
                    throw new ApplicationException(string.Format("Unknown code {0}.", item.Code));
                if (goodsData.Table.Rows.Count > 1)
                    throw new ApplicationException(string.Format("More duplicate codes {0} - {1}.", item.Code, goodsData.Table.Rows.Count));

                DataRow row = goodsData.Table.Rows[0];

                item.SKzId = (int)row["ID"];
                item.Name = row["Nazev"] != DBNull.Value ? row["Nazev"].ToString() : null;
                item.KgPerUnit = row["Hmotnost"] != DBNull.Value ? (double)row["Hmotnost"] : 0;
                item.OnStock = row["StavZ"] != DBNull.Value ? (double)row["StavZ"] : 0;
            }
        }

        public void UpdateMainData(List<ItemData> data)
        {
            DataView goodsData = this.GetFilteredGoodsData(string.Format("id in ({0})", this.GetIdsStr(data)));
            if (goodsData != null)
            {
                foreach (DataRow row in goodsData.Table.Rows)
                {
                    int id = (int)row["ID"];
                    ItemData item = data.FirstOrDefault(it => it.SKzId == id);
                    if (item != null)
                    {
                        item.Name = row["Nazev"] != DBNull.Value ? row["Nazev"].ToString() : null;
                        item.KgPerUnit = row["Hmotnost"] != DBNull.Value ? (double)row["Hmotnost"] : 0;
                        item.OnStock = row["StavZ"] != DBNull.Value ? (double)row["StavZ"] : 0;
                    }
                }
            }
        }

        public void UpdateConsumption(List<ItemData> data, string condition)
        {
            StatisticsConnection conn = this.GetCurrentConnection();
            // Tutoka nacitame vydane kusy
            DataView onStockData = this.GetFilteredGoodsViewData(condition);
            if (onStockData != null)
            {
                foreach (DataRow row in onStockData.Table.Rows)
                {
                    int id = (int)row["RefSKz"];
                    ItemData item = data.FirstOrDefault(it => it.SKzId == id);
                    if (item != null)
                    {
                        double outCount = row["PohPMJ"] != DBNull.Value ? (double)row["PohPMJ"] : 0;
                        if (outCount > 0)
                            item.AddUsage(conn.Key, outCount);
                    }
                }
            }
        }

        public List<OrderedItems> UpdateOrdered(List<ItemData> data, string condition)
        {
            string idsStr = this.GetIdsStr(data);
            DataView viewData = this.GetFilteredOrders(condition + " and (OBJpol.RefSKz in (" + idsStr + "))");
            List<OrderedItems> orders = new List<OrderedItems>();
            if (viewData != null)
            {
                foreach (DataRow row in viewData.Table.Rows)
                {
                    int itemId = (int)row["RefSKz"];
                    ItemData item = data.FirstOrDefault(it => it.SKzId == itemId);
                    if (item != null)
                    {
                        double ordered = row["Mnozstvi"] != DBNull.Value ? (double)row["Mnozstvi"] : 0;
                        double delivered = row["Dodano"] != DBNull.Value ? (double)row["Dodano"] : 0;

                        // Ked je menej dodanych kusov ako objednanych
                        if (ordered - delivered > 0)
                        {
                            int id = row["ID"] != DBNull.Value ? (int)row["ID"] : 0;
                            string ordNumber = row["Cislo"] != DBNull.Value ? (string)row["Cislo"] : null;
                            item.AddOrder(id, ordNumber, ordered - delivered);
                            // Este pridame do objednavok tento zaznam, ak tam uz nie je
                            OrderedItems order = orders.FirstOrDefault(ord => ord.Id == id);
                            if (order == null)
                            {
                                order = new OrderedItems() { Id = id, Code = ordNumber};

                                if (row["Datum"] != DBNull.Value)
                                    order.OrderDate = (DateTime)row["Datum"];
                                if (row["DatDod"] != DBNull.Value)
                                    order.DeliveryDateSK = (DateTime)row["DatDod"];

                                orders.Add(order);
                            }
                        }
                    }
                }
            }
            return orders;
        }

        private DataView GetFilteredOrders(string condition)
        {
            StatisticsConnection conn = this.GetCurrentConnection();
            SqlConnection connection = new SqlConnection(conn.ConnectionString);
            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select OBJ.ID, OBJ.Cislo, OBJ.Datum, OBJ.DatDod, OBJpol.ID, OBJpol.RefSKz, OBJpol.Mnozstvi, OBJpol.Dodano from OBJ inner join OBJpol on OBJpol.RefAg = OBJ.ID where {0}", condition), connection))
            {
                DataTable table = new DataTable("OBJView");
                adapter.Fill(table);
                connection.Close();
                return table.DefaultView;
            }
        }

        private string GetIdsStr(List<ItemData> data)
        {
            StringBuilder idsStr = new StringBuilder();
            data.ForEach(item =>
            {
                if (item.SKzId > 0)
                {
                    if (idsStr.Length > 0)
                        idsStr.Append(",");
                    idsStr.Append(item.SKzId);
                }
            });

            return idsStr.ToString();
        }

        public string GetFilter(int id)
        {
            StatisticsConnection conn = this.GetCurrentConnection();
            SqlConnection connection = new SqlConnection(conn.ConnectionString);
            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select * from Dotazy where id = {0}", id), connection))
            {
                DataTable table = new DataTable("Dotazy");
                adapter.Fill(table);
                connection.Close();
                string filter = table.Rows[0]["Filter"].ToString();
                if (filter.Contains("qAgSKzPoh.Datum = #YEAR#"))
                    filter = filter.Replace("qAgSKzPoh.Datum = #YEAR#", "YEAR(qAgSKzPoh.Datum) = 2011");
                return filter;
            }
        }

        private DataView GetFilteredGoodsViewData(string condition)
        {
            StatisticsConnection conn = this.GetCurrentConnection();
            SqlConnection connection = new SqlConnection(conn.ConnectionString);
            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select * from qAgSKzPoh where {0}", condition), connection))
            {
                DataTable table = new DataTable("SKz");
                adapter.Fill(table);
                connection.Close();
                return table.DefaultView;
            }
        }

        private DataView GetFilteredGoodsData(string condition)
        {
            StatisticsConnection conn = this.GetCurrentConnection();
            SqlConnection connection = new SqlConnection(conn.ConnectionString);
            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select * from SKz where {0}", condition), connection))
            {
                DataTable table = new DataTable("SKz");
                adapter.Fill(table);
                connection.Close();
                return table.DefaultView;
            }
        }

        //public BindingList<ItemData> GetData(string condition, int month)
        //{
        //    //DataView statAllData = this.GetStatisticsData(condition, month);
        //    StatisticsConnection conn = this.GetCurrentConnection();
        //    DataView statAllData = this.GetStatisticsData(condition, conn.ConnectionString, conn.Year);
        //    Dictionary<string, ItemData> stAll = new Dictionary<string, ItemData>();
        //    Dictionary<int, string> ids = new Dictionary<int, string>();
        //    foreach (DataRow row in statAllData.Table.Rows)
        //    {
        //        ItemData st = null;
        //        string code = row["IDS"] != DBNull.Value ? row["IDS"].ToString() : null;

        //        if (string.IsNullOrEmpty(code))
        //            continue;

        //        if (!stAll.ContainsKey(code))
        //        {
        //            st = new ItemData(code);
        //            stAll.Add(st.Code, st);
        //        }
        //        else
        //            st = stAll[code];

        //        st.Name = row["Nazev"] != DBNull.Value ? row["Nazev"].ToString() : null;
        //        st.SKzId = row["RefSKz"] != DBNull.Value ? (int)row["RefSKz"] : -1;
        //        DateTime date = row["Datum"] != DBNull.Value ? (DateTime)row["Datum"] : DateTime.MinValue;
        //        double outCount = row["PohPMJ"] != DBNull.Value ? (double)row["PohPMJ"] : 0;

        //        st.AddUsage(conn.Key, outCount);

        //        if (st.SKzId > 0 && !ids.ContainsKey(st.SKzId) && !string.IsNullOrEmpty(st.Code))
        //            ids.Add(st.SKzId, st.Code);
        //    }

        //    foreach (StatisticsConnection connection in this._connections)
        //    {
        //        if (connection.IsCurrent)
        //            continue;

        //        statAllData = this.GetStatisticsData(condition, connection.ConnectionString, connection.Year);
        //        foreach (DataRow row in statAllData.Table.Rows)
        //        {
        //            ItemData st = null;
        //            string code = row["IDS"] != DBNull.Value ? row["IDS"].ToString() : null;

        //            if (string.IsNullOrEmpty(code) || !stAll.ContainsKey(code))
        //                continue;

        //            st = stAll[code];

        //            double outCount = row["PohPMJ"] != DBNull.Value ? (double)row["PohPMJ"] : 0;

        //            st.AddUsage(connection.Key, outCount);
        //        }
        //    }

        //    DataView goodsData = this.GetGoodsData(ids.Keys);
        //    if (goodsData != null)
        //    {
        //        foreach (DataRow row in goodsData.Table.Rows)
        //        {
        //            int id = (int)row["ID"];
        //            ItemData item = stAll.Values.FirstOrDefault(it => it.SKzId == id);
        //            if (item != null)
        //            {
        //                item.OnStock = row["StavZ"] != DBNull.Value ? (double)row["StavZ"] : 0;
        //                item.KgPerUnit = row["Hmotnost"] != DBNull.Value ? (double)row["Hmotnost"] : 0;
        //                //item.Ordered = row["ObjedV"] != DBNull.Value ? (double)row["ObjedV"] : 0;
        //            }
        //        }
        //    }
        //    return new BindingList<ItemData>(stAll.Values.OrderBy(i => i.SKzId).ToList());
        //}

        private StatisticsConnection GetCurrentConnection()
        {
            StatisticsConnection currentConn = this._connections.FirstOrDefault(conn => conn.IsCurrent == true);
            if (currentConn == null)
                throw new ApplicationException("No current connection.");
            return currentConn;
        }

        private DataView GetGoodsData(IEnumerable<int> ids)
        {
            if (ids == null || ids.Count() == 0)
                return null;

            StatisticsConnection conn = this.GetCurrentConnection();
            SqlConnection connection = new SqlConnection(conn.ConnectionString);
            StringBuilder condition = new StringBuilder("ID in (");
            bool first = true;
            foreach (int id in ids)
            {
                if (!first)
                    condition.Append(",");
                condition.Append(id.ToString());
                first = false;
            }
            condition.Append(")");
            using (SqlDataAdapter adapter = new SqlDataAdapter(
                string.Format("select * from SKz where {0} order by ID", condition.ToString()), connection))
            {
                DataTable table = new DataTable("SKz");
                adapter.Fill(table);
                connection.Close();
                return table.DefaultView;
            }
        }

        //public BindingList<Statistic> GetStatistics(string condition, int month)
        //{
        //    //DataView statAllData = this.GetStatisticsData(condition, month);
        //    DataView statAllData = this.GetStatisticsData(condition);
        //    List<Statistic> stAll = new List<Statistic>();
        //    foreach (DataRow row in statAllData.Table.Rows)
        //    {
        //        Statistic st = new Statistic();
        //        st.Name = row["Nazev"] != DBNull.Value ? row["Nazev"].ToString() : null;
        //        st.SKzId = row["RefSKz"] != DBNull.Value ? (int)row["RefSKz"] : -1;
        //        DateTime date = row["Datum"] != DBNull.Value ? (DateTime)row["Datum"] : DateTime.MinValue;
        //        st.PreviousUsages.Add(new Usage()
        //        {
        //            Month = date.Month,
        //            Year = date.Year,
        //            UsageKilograms = row["PohPMJ"] != DBNull.Value ? (double)row["PohPMJ"] : 0
        //        });
        //        stAll.Add(st);
        //    }
        //    return new BindingList<Statistic>(stAll.OrderBy(i => i.SKzId).ToList());
        //}

        //public DataView GetStatisticsData1(string condition, int month)
        //{
        //    SqlConnection connection = new SqlConnection(this._connectionString);
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(
        //        string.Format("select Year(Datum) as Rok, Month(Datum) as Mesiac, Nazev, Sum(PohPMJ) as Spotreba from qAgSKzPoh where {0} and datepart(mm, Datum) = {1} group by Year(Datum), Month(Datum), Nazev order by Year(Datum), Month(Datum), Nazev", condition, month), connection))
        //    {
        //        DataTable table = new DataTable("qAgSKzPoh");
        //        adapter.Fill(table);
        //        connection.Close();
        //        return table.DefaultView;
        //    }
        //}
    }
}
