using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hemming
{
    public partial class form_statistic : Form
    {
        public form_statistic()
        {
            InitializeComponent();
            using (ApplicationContext db = new ApplicationContext())
            {
                for(int i=-3; i<4; i++)
                {
                    var rows = (from row in db.Statistic1 where row.Kanal == i select row).ToList();
                    string prop = $"2/15 +({0.03*i})";

                    var rowsH1 = (from rowH1 in rows where rowH1.Coder == 1 select rowH1).ToList();
                    var rowsH2 = (from rowH2 in rows where rowH2.Coder == 2 select rowH2).ToList();

                    string H1_count_A = $"{rowsH1.Count()}";
                    string H2_count_A = $"{rowsH2.Count()}";

                    var rowsH1E = (from rowH1E in rowsH1 where rowH1E.HasError == true select rowH1E).ToList();
                    var rowsH2E = (from rowH2E in rowsH2 where rowH2E.HasError == true select rowH2E).ToList();

                    string H1_count_hasE = $"{rowsH1E.Count()}";
                    string H2_count_hasE = $"{rowsH2E.Count()}";

                    var rowsH1DiscE = (from rowH1E in rowsH1 where rowH1E.DiscoveredError == true select rowH1E).ToList();
                    var rowsH2DiscE = (from rowH2E in rowsH2 where rowH2E.DiscoveredError == true select rowH2E).ToList();

                    string H1_count_DiscE = $"{rowsH1DiscE.Count()}";
                    string H2_count_DiscE = $"{rowsH2DiscE.Count()}";

                    var rowsH1FixedE = (from rowH1E in rowsH1 where rowH1E.FixedError == true select rowH1E).ToList();
                    var rowsH2FixedE = (from rowH2E in rowsH2 where rowH2E.FixedError == true select rowH2E).ToList();

                    string H1_count_FixedE = $"{rowsH1FixedE.Count()}";
                    string H2_count_FixedE = $"{rowsH2FixedE.Count()}";

                    string H1_prop_DiscE = $"{(double)rowsH1DiscE.Count()/ rowsH1E.Count()}";
                    string H2_prop_DiscE = $"{(double)rowsH2DiscE.Count() / rowsH2E.Count()}";

                    string H1_prop_NDiscE = $"{1-(double)rowsH1DiscE.Count() / rowsH1E.Count()}";
                    string H2_prop_NDiscE = $"{1-(double)rowsH2DiscE.Count() / rowsH2E.Count()}";

                    dataGridView_statistic.Rows.Add(prop, H1_count_A, H1_count_hasE, H1_count_DiscE, H1_count_FixedE, H1_prop_DiscE, H1_prop_NDiscE,
                        H2_count_A, H2_count_hasE, H2_count_DiscE, H2_count_FixedE, H2_prop_DiscE, H2_prop_NDiscE);
                }

            }
        }


    }
}
