using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kontakt.DataModel;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Kontakt.BL
{
    public class StatisticsDataFactory
    {
        private List<StatisticInfo> _statistics = new List<StatisticInfo>();
        private string _connectionString;

        public StatisticsDataFactory(string connectionString)
        {
            this._connectionString = connectionString;
            this._statistics.Add(new StatisticInfo()
            {
                Id = 1,
                Name = "Káble",
                ClassName = "Kontakt.BL.CablesData",
                FilterForDataId = 118,
                FilterForData = "x spotreba 601-/624-/625- vsetke lana IWRC/FC/IWSC",
                FilterForOnStockId = 94,
                FilterForOnStock = "x stav skladu 601-/624-/625- lana komplet - FC/IWRC/IWSC",
                FilterForOrderedId = 135,
                FilterForOrdered = "objednane lana pre nasu vyrobu 601-/624-/625- IWRC/FC/IWSC",
                TemplateName = "601"
            });
            this._statistics.Add(new StatisticInfo() { 
                Id = 2, 
                Name = "Káble filtrované", 
                ClassName = "Kontakt.BL.CablesDataFiltered",
                FilterForDataId = 118,
                FilterForData = "x spotreba 601-/624-/625- vsetke lana IWRC/FC/IWSC",
                FilterForOnStockId = 94,
                FilterForOnStock = "x stav skladu 601-/624-/625- lana komplet - FC/IWRC/IWSC",
                FilterForOrderedId = 135,
                FilterForOrdered = "objednane lana pre nasu vyrobu 601-/624-/625- IWRC/FC/IWSC",
                TemplateName = "601"
            });
            // 88 - x statistika ocelove lano FC/IWRC/IWSC - RelAg: 55
            // 94 - x stav skladu - lana - iba nase FC/IWRC/IWSC - RelAg: 20
        }

        public StatisticsDataFactory(string connectionString, string statisticsPath)
        {
            if (!File.Exists(statisticsPath))
                throw new FileNotFoundException(statisticsPath);
            this._connectionString = connectionString;
            this._statistics = Serializator.Deserialize<List<StatisticInfo>>(statisticsPath);
        }
        //public void Serialize()
        //{
        //    Serializator.Serialize(@"C:\Statistics.xml", this._statistics);

        //    List<StatisticInfo> statistics = Serializator.Deserialize<List<StatisticInfo>>(@"C:\Statistics.xml");
        //}

        public IList<StatisticInfo> AllStatistics
        {
            get
            {
                return this._statistics;
            }
        }

        public Items GetStatisticsData(string name)
        {
            StatisticInfo info = this._statistics.FirstOrDefault(it => it.Name.Equals(name));
            if (info != null)
            {
                StatisticsDataBase implementation =
                    (StatisticsDataBase)Activator.CreateInstance(
                    Type.GetType(info.ClassName),
                    this._connectionString, info.FilterForOnStockId, info.FilterForOrderedId, info.FilterForDataId);
                return implementation.GetStatisticData();
            }
            return null;
        }
        public Items GetStatisticsData(int id)
        {
            StatisticInfo info = this._statistics.FirstOrDefault(it => it.Id.Equals(id));
            if (info != null)
            {
                StatisticsDataBase implementation =
                    (StatisticsDataBase)Activator.CreateInstance(
                    Type.GetType(info.ClassName),
                    this._connectionString, info.FilterForOnStockId, info.FilterForOrderedId, info.FilterForDataId);
                return implementation.GetStatisticData();
            }
            return null;
        }
    }

    public static class Serializator
    {
        public static void Serialize<T>(string path, T settings)
            where T : class
        {
            try
            {
                using (TextWriter textWriter = new StreamWriter(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(textWriter, settings);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Serialization failed!", ex);
            }
        }

        public static T Deserialize<T>(string path)
            where T : class
        {
            try
            {
                using (TextReader textReader = new StreamReader(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Deserialization failed!", ex);
            }
        }
    }
}
