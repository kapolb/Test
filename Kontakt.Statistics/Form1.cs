using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kontakt.DAL;
using System.Configuration;
using Kontakt.DataModel;
using ExcelBL;
using Kontakt.BL;

namespace Kontakt.Statistics
{
    public partial class Form1 : Form
    {
        private string _connection2010;
        private string _connection2011;
        private StatisticsDBHelper _dbHelper;
        private Items _currentData;
        StatisticsDataFactory _factory;

        public Form1()
        {
            InitializeComponent();
            this._connection2010 = ConfigurationManager.ConnectionStrings["KontaktDB2010"].ConnectionString;
            this._connection2011 = ConfigurationManager.ConnectionStrings["KontaktDB2011"].ConnectionString;
            this._factory = new StatisticsDataFactory(this._connection2011);
        }

        private StatisticsDBHelper DBHelper
        {
            get
            {
                if (this._dbHelper == null)
                {
                    this._dbHelper = new StatisticsDBHelper();
                    this._dbHelper.AddConnection("2010", this._connection2010, 2010);
                    this._dbHelper.AddConnection("2011", this._connection2011, 2011, true);
                }
                return this._dbHelper;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["KontaktDB2010"];

            //List<ConnectionStringSettings> connections = new List<ConnectionStringSettings>();
            //foreach (ConnectionStringSettings connsett in ConfigurationManager.ConnectionStrings)
            //{
            //    if (connsett.Name.StartsWith("KontaktDB"))
            //    {
            //        connections.Add(connsett);
            //    }
            //}

            //this.cbConnections.DisplayMember = "Name";
            //this.cbConnections.ValueMember = "ConnectionString";
            //this.cbConnections.DataSource = connections;

            //this.DBHelper = new StatisticsDBHelper((string)this.cbConnections.SelectedValue);

            this.cbMonth.Items.Add(1);
            this.cbMonth.Items.Add(2);
            this.cbMonth.Items.Add(3);
            this.cbMonth.Items.Add(4);
            this.cbMonth.Items.Add(5);
            this.cbMonth.Items.Add(6);
            this.cbMonth.Items.Add(7);
            this.cbMonth.Items.Add(8);
            this.cbMonth.Items.Add(9);
            this.cbMonth.Items.Add(10);
            this.cbMonth.Items.Add(11);
            this.cbMonth.Items.Add(12);
            this.cbMonth.SelectedIndex = 0;

            this.LoadDBConditions();

            this.cbMonth.SelectedIndexChanged += new EventHandler(cbMonth_SelectedIndexChanged);
            this.cbFilters2.SelectedIndexChanged += new EventHandler(cbFilters2_SelectedIndexChanged);
            this.dgvResult2.SelectionChanged += new EventHandler(dgvResult2_SelectionChanged);
            //this.cbConnections.SelectedIndexChanged += new EventHandler(cbConnections_SelectedIndexChanged);
            this.cbStatistics.SelectedIndexChanged += new EventHandler(cbStatistics_SelectedIndexChanged);
            this.LoadStatistics();

            this.ReloadData();
        }

        void cbStatistics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatistics.SelectedValue != null && this.cbStatistics.SelectedValue is StatisticInfo)
            {
                Items data = this._factory.GetStatisticsData(
                    (this.cbStatistics.SelectedValue as StatisticInfo).Name);
                _currentData = data;
                this.dgvResult2.DataSource = new BindingList<ItemData>(data.Rows);
            }
        }

        void cbFilters2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ReloadData();
        }

        void cbConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadDBConditions();
        }

        private void cbFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ReloadData();
        }

        private void LoadStatistics()
        {
            this.cbStatistics.DisplayMember = "Name";
            this.cbStatistics.DataSource = this._factory.AllStatistics;
        }

        private void LoadDBConditions()
        {
            try
            {
                //this.DBHelper.SetConnection(this._connection2010);
                //cbFilters.DisplayMember = "Popis";
                //cbFilters.DataSource = this.DBHelper.GetConditionForStatistics();
                //this.DBHelper.SetConnection(this._connection2011);
                cbFilters2.DisplayMember = "Popis";
                cbFilters2.DataSource = this.DBHelper.GetConditionForStatistics();
            }
            catch
            {
            }
        }

        private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ReloadData();
        }

        private void ReloadData()
        {
            if (this.cbMonth.SelectedItem != null && this.cbMonth.SelectedItem is int)
            {
                if (this.cbFilters2.SelectedItem != null && this.cbFilters2.SelectedItem is DataRowView)
                {
                    try
                    {
                        this.dgvResult2.DataSource = null;
                        DataRowView row = (DataRowView)this.cbFilters2.SelectedItem;
                        string filter = row.Row["Filter"].ToString();
                        if (row.Row["ID"].ToString() == "88")
                        {
                            StatisticsDataFactory factory = new StatisticsDataFactory(this._connection2011);
                            object he = factory.GetStatisticsData("Cables Filtered");
                            CablesDataFiltered cablesData = new CablesDataFiltered(this._connection2011, 88, 94, 94);
                            //_currentData = new List<ItemData>(cablesData.GetStatisticData().Rows);
                            this.dgvResult2.DataSource = new BindingList<ItemData>(cablesData.GetStatisticData().Rows);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        void dgvResult2_SelectionChanged(object sender, EventArgs e)
        {
            this.dgvDetails.DataSource = null;
            if (this.dgvResult2.SelectedRows != null && this.dgvResult2.SelectedRows.Count > 0)
            {
                ItemData st = this.dgvResult2.SelectedRows[0].DataBoundItem as ItemData;
                if (st != null)
                {
                    DataTable table = new DataTable("Usages");
                    table.Columns.Add("Year");
                    table.Columns.Add("Value");
                    foreach (string key in st.Usages.Keys)
                    {
                        table.Rows.Add(key, st.Usages[key]);
                    }
                    this.dgvDetails.DataSource = table;
                }
            }
        }

        private void btnGenerateExcel_Click(object sender, EventArgs e)
        {
            var templatesDirectory = ConfigurationManager.AppSettings["TemplatesDirectory"];
            var manager = ExcelManager.CreateManagerWithTemplate(templatesDirectory);
            manager.GenerateReport("601", this._currentData);
        }
    }
}
