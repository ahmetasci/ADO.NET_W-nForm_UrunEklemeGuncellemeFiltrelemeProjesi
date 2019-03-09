using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace UrunEklemeGuncelleme
{
    /*
    Listele butonuna tıkladığımızda datagridview içinde ürünler listelenecek
    DatagridViewdaki bir kayıta tıklandığında kayıttaki veriler Güncelle groupBoxının içindeki textboxlara yazılacak
    Ekle butonuna tıklandığında Ekle groupBoxının içindeki textboxlardaki verileri datagridviewa ekleyecek.
    Güncelle butonuna tıkladındığında datagridviewdaki seçili kaydı Güncelle groupboxının içindeki textboxlardaki verilere göre güncelleyecek
    Sil butonuna tıklandığında seçili kaydı silecek
    Ara butonuna tıklandığında ProductName'e göre filtreleme yapıp datagridviewda gösterecek
    */
    public partial class Form1 : Form
    {
        //CONNECTED MİMARİ
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection(Tools.ConnectionString);
        SqlCommand cmd;
       
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void UrunDoldur()
        {
            if (con.State==ConnectionState.Closed)
            {//Sql NorthWind databaseinden ürünleri doldurduk
                con.Open();
                cmd = new SqlCommand("SELECT ProductID,ProductName,UnitPrice,UnitsInStock FROM Products ", con);
                SqlDataReader rdr = cmd.ExecuteReader();//Bütün kayıtları döndürür.
             
                while (rdr.Read())
                {
                    ListViewItem kayit = new ListViewItem();
                    kayit.Text = rdr["ProductID"].ToString();
                    kayit.SubItems.Add(rdr["ProductName"].ToString());
                    kayit.SubItems.Add(rdr["UnitPrice"].ToString());
                    kayit.SubItems.Add(rdr["UnitsInStock"].ToString());

                    listView1.Items.Add(kayit);

                }
            }
            con.Close();
        }
        private void btn_Ekle_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand("INSERT INTO Products(ProductName,UnitPrice,UnitsInStock) VALUES (@ad,@fiyat,@stok)",con);

            cmd.Parameters.AddWithValue("@id", txtid_Ekle.Text);
            cmd.Parameters.AddWithValue("@ad", txtAd_Ekle.Text);
            cmd.Parameters.AddWithValue("@fiyat", txtFiyat_Ekle.Text);
            cmd.Parameters.AddWithValue("@stok", txtStok_Ekle.Text);

            int say = cmd.ExecuteNonQuery();//Etkilenen kayıt sayısını döndürür.

            con.Close();
            listView1.Items.Clear();
            UrunDoldur();

        }

        private void btn_Guncelle_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand("UPDATE Products SET ProductName=@ad,UnitPrice=@fiyat,UnitsInStock=@stok Where ProductID=@id", con);
            cmd.Parameters.AddWithValue("@id", txtid_Ekle.Text);
            cmd.Parameters.AddWithValue("@ad", txtAd_Ekle.Text);
            cmd.Parameters.AddWithValue("@fiyat", txtFiyat_Ekle.Text);
            cmd.Parameters.AddWithValue("@stok", txtStok_Ekle.Text);
           

            int donenDeger1 = cmd.ExecuteNonQuery();//Etkilenen kayıt sayısını döndürür.


            MessageBox.Show(donenDeger1 != 0 ? "başarılı" : "başarısız");  //Ternary Operator(if-else)

            con.Close();

            UrunDoldur();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_Listele_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();//Eğer öncesinde temizlemezsek aynı ürünlerin üzerine yazıyor.
            UrunDoldur();
        }

        private void btn_Sil_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand("DELETE FROM Products WHERE ProductID=@id ", con);
            //seçili kaydı productID'ye göre siliyor.
            //Eğer isme göre sildirseydik ismi aynı olan bütün ürünlerin kayıtlarını silerdi. Fakat ID'ye göre sildirdiğimiz için tek kayıt silecek

            cmd.Parameters.AddWithValue("@id", txtid_Güncelle.Text);//verilen parametrenin ne olduğunu tanımladık


            int donenDeger = cmd.ExecuteNonQuery();//Etkilenen kayıt sayısını verir.
            MessageBox.Show (donenDeger!=0 ? "başarılı":"başarısız");// Ternary Operator kullanımı. 
            // eğer etkilenen kayıt varsa Başarılı yazısı gösterilecek. Yoksa tam tersi
            

            

            con.Close();

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count>0)
            {
                //Burada DatagridViewda bir satırı seçtiğimizde istediğimiz textboxlarda göstermesi için kodlar yazdık
                ListViewItem item = listView1.SelectedItems[0];
                txtid_Güncelle.Text = item.SubItems[0].Text;
                txtAd_Guncelle.Text = item.SubItems[1].Text;
                txtFiyat_Guncelle.Text = item.SubItems[2].Text;
                txtStok_Guncelle.Text = item.SubItems[3].Text;
                
            }
            else
            {
                txtid_Güncelle.Text = string.Empty;
                txtAd_Guncelle.Text = string.Empty;
                txtFiyat_Guncelle.Text = string.Empty;
                txtStok_Guncelle.Text = string.Empty;

            }
        }

       
        private void btn_Ara_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            cmd = new SqlCommand("SELECT ProductName , UnitPrice , UnitsInStock FROM Products WHERE ProductName like '%" + txt_Ara.Text + "%'", con);
            //joker karakter(WildCard) ile filtreleme yaptık. Eğer textboxın textindeki ifade ProductNamein herhangi bir yerinde geçiyorsa filtreleyecek
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            SqlDataReader rdr = cmd.ExecuteReader();//Bütün kayıtları döndürür.
            while (rdr.Read())
            {
                ListViewItem item = new ListViewItem(rdr["ProductName"].ToString());
                item.SubItems.Add(rdr["UnitPrice"].ToString());
                item.SubItems.Add(rdr["UnitsInStock"].ToString());
                listView1.Items.Add(item);
            }
            con.Close();
        }
    }
}
