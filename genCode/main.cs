using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace genCode
{
    public partial class main : Form
    {
        DataSet ds;
        string fileName;

        public main()
        {
            InitializeComponent();
        }

        private void btn_selectFile_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.ShowDialog();
                fileName = openFileDialog1.SafeFileName.Replace(".csv", "");
                txt_filename.Text = openFileDialog1.FileName;

                BindData(txt_filename.Text);
                label2.Text = dgv_csv.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
        }

        private void BindData(string filePath)
        {
            DataTable dt = new DataTable();
            string[] lines = System.IO.File.ReadAllLines(filePath);

            if (lines.Length > 0)
            {
                string firstLine = lines[0];
                firstLine = firstLine.Trim('\"');
                string[] headerLabels = firstLine.Split(';');
                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }

                //For Data
                for (int i = 1; i < lines.Length -1 ; i++)
                {
                    string line = lines[i];
                    line = line.Replace("\"", "");

                    string[] dataWords = line.Split(';');
                    DataRow dr = dt.NewRow();
                    int columnIndex = 0;
                    foreach (string headerWord in headerLabels)
                    {
                        try
                        {
                            dr[headerWord] = dataWords[columnIndex++];
                        }catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message);
                        }
                    }
                    dt.Rows.Add(dr);
                }

                ds = new DataSet();
                ds.Tables.Add(dt);
            }

            if (dt.Rows.Count > 0)
            {
                dgv_csv.DataSource = dt;
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if(dgv_csv.Rows.Count > 0)
                Exportpdf();
        }

        private void print_devexpress()
        {
            report_cards report = new report_cards();
            report.DataSource = ds;
            report.DataMember = "";

            frm_view fm = new frm_view();
            fm.documentViewer1.DocumentSource = report;
            fm.documentViewer1.ShowPageMargins = false;
            fm.ShowDialog();
        }

        private void Exportpdf()
        {
            report_cards report = new report_cards();
            report.DataSource = ds;
            report.DataMember = "";

            PdfExportOptions pdfExportOptions = new PdfExportOptions()
            {
                PdfACompatibility = PdfACompatibility.PdfA1b
            };

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PDF File|*.pdf";
            saveFileDialog1.FileName = fileName;
            saveFileDialog1.Title = "حفظ الملف";
            saveFileDialog1.ShowDialog();

            if(saveFileDialog1.FileName != "")
            {
                string pdfExportFile = saveFileDialog1.FileName;
                report.ExportToPdf(pdfExportFile, pdfExportOptions);
            }
        }
    }
}
