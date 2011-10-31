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
        private string _statisticsPath;
        private string _usagesPath;

        public StatisticsDataFactory(string connectionString, string statisticsPath, string usagesPath)
        {
            if (!File.Exists(statisticsPath))
                throw new FileNotFoundException(statisticsPath);
            this._connectionString = connectionString;
            this._statisticsPath = statisticsPath;
            this._usagesPath = usagesPath;
            this._statistics = Serializator.Deserialize<List<StatisticInfo>>(this._statisticsPath);
        }

        public IList<StatisticInfo> AllStatistics
        {
            get
            {
                // Vraciame len aktivne statistiky
                return new List<StatisticInfo>(this._statistics.Where(it => it.Active));
            }
        }

        public bool UpdateStatisticInfo(StatisticInfo info)
        {
            if (info == null)
                return false;
            StatisticInfo infoOld = this._statistics.FirstOrDefault(st => st.Id == info.Id);
            if (infoOld == null)
            {
                infoOld = info;
                infoOld.Id = -1;
            }
            else
                infoOld.Fill(info);
            if (infoOld.Id <= 0)
            {
                infoOld.Id = this._statistics.Count + 1;
                this._statistics.Add(infoOld);
            }
            Serializator.Serialize(this._statisticsPath, new List<StatisticInfo>(this._statistics));
            return true; 
        }
        public bool DeleteStatisticInfo(decimal id)
        {
            StatisticInfo info = this._statistics.FirstOrDefault(st => st.Id == id);
            if (info != null)
            {
                info.Active = false;
                Serializator.Serialize(this._statisticsPath, new List<StatisticInfo>(this._statistics));
            }
            return true;
        }

        public Items GetStatisticsData(string name)
        {
            StatisticInfo info = this._statistics.FirstOrDefault(it => it.Name.Equals(name));
            if (info != null)
            {
                StatisticsDataBase implementation =
                    (StatisticsDataBase)Activator.CreateInstance(
                    Type.GetType(info.ClassName),
                    this._connectionString, this._usagesPath + info.Usages2010File, 
                    info.FilterForOnStockId, info.FilterForOrderedId, info.FilterForDataId);
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
                    this._connectionString, this._usagesPath + info.Usages2010File, 
                    info.FilterForOnStockId, info.FilterForOrderedId, info.FilterForDataId);
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
                using (TextWriter textWriter = new StreamWriter(path, false, Encoding.UTF8))
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
                using (TextReader textReader = new StreamReader(path, Encoding.UTF8))
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
