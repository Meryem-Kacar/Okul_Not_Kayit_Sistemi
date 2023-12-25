using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace WindowsFormsApp1
{
    //Data Source=Meryem\SQLEXPRESS;Initial Catalog=DbNotKayıt;Integrated Security=True"
    public partial class FrmOgretmenDetay : Form
    {
        public FrmOgretmenDetay()
        {
            InitializeComponent();
        }

        
        SqlConnection baglanti = new SqlConnection(@"Data Source=Meryem\SQLEXPRESS;Initial Catalog=DbNotKayıt;Integrated Security=True");

        private void FrmOgretmenDetay_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'dbNotKayıtDataSet.TBLDERS' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.tBLDERSTableAdapter.Fill(this.dbNotKayıtDataSet.TBLDERS);

        }

        private void btnOgrenciKaydet_Click(object sender, EventArgs e)
        {
            
                baglanti.Open();
                SqlCommand komut = new SqlCommand("INSERT INTO TBLDERS(OGRNUMARA, OGRAD, OGRSOYAD) VALUES(@p1, @p2, @p3)", baglanti);

                // MaskedTextBox ve TextBox kontrol değerlerini Text özelliğinden al
                komut.Parameters.AddWithValue("@p1", mskNumara.Text);
                komut.Parameters.AddWithValue("@p2", txtAd.Text);
                komut.Parameters.AddWithValue("@p3", txtSoyad.Text);

                // DML komutlarının işlemlerini gerçekleştirmek için yazılır.
                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Öğrenci Sisteme Eklendi");
                this.tBLDERSTableAdapter.Fill(this.dbNotKayıtDataSet.TBLDERS);

        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            mskNumara.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            txtAd.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            txtSoyad.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();

            txtSınav1.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            txtSınav2.Text = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
            txtSınav3.Text = dataGridView1.Rows[secilen].Cells[6].Value.ToString();
        }

        private void btnGüncelle_Click(object sender, EventArgs e)
        {
           
            double s1, s2, s3;
            s1 = Convert.ToDouble(txtSınav1.Text);
            s2 = Convert.ToDouble(txtSınav2.Text);
            s3 = Convert.ToDouble(txtSınav3.Text);

            double ortalama = (s1 + s2 + s3) / 3;

            lblOrtalama.Text = ortalama.ToString();

            // Durumu doğrudan SQL sorgusunda belirle
            string durum = (ortalama >= 50) ? "True" : "False";

            baglanti.Open();
            SqlCommand komut = new SqlCommand("UPDATE TBLDERS SET OGRS1=@p1, OGRS2=@p2, OGRS3=@p3, ORTALAMA=@p4, DURUM=@p5 WHERE OGRNUMARA=@p6", baglanti);
            komut.Parameters.AddWithValue("@p1", s1).SqlDbType = SqlDbType.Decimal;
            komut.Parameters.AddWithValue("@p2", s2).SqlDbType = SqlDbType.Decimal;
            komut.Parameters.AddWithValue("@p3", s3).SqlDbType = SqlDbType.Decimal;
            komut.Parameters.AddWithValue("@p4", ortalama).SqlDbType = SqlDbType.Decimal;
            komut.Parameters.AddWithValue("@p5", durum);
            komut.Parameters.AddWithValue("@p6", mskNumara.Text.Replace("-", ""));

            komut.ExecuteNonQuery();
            baglanti.Close();

            MessageBox.Show("Öğrenci Notları Güncellendi");
            this.tBLDERSTableAdapter.Fill(this.dbNotKayıtDataSet.TBLDERS);

            int gecenSayisi = this.dbNotKayıtDataSet.TBLDERS.Count(x => x.DURUM == true);
            int kalanSayisi = this.dbNotKayıtDataSet.TBLDERS.Count(x => x.DURUM == false);

            lblGecenSayısı.Text = gecenSayisi.ToString();
            lblKalanSayısı.Text = kalanSayisi.ToString();

            if (gecenSayisi + kalanSayisi > 0)
            {
                decimal toplamOrtalama = this.dbNotKayıtDataSet.TBLDERS.Sum(y => y.ORTALAMA);
                ortalama = (double)toplamOrtalama / (gecenSayisi + kalanSayisi);
                lblOrtalama.Text = ortalama.ToString();
            }
            else
            {
                lblOrtalama.Text = "N/A"; // Bölen 0 hatasını önlemek için
            }


        }
    }
}
