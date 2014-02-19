using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.IO;
using System.Runtime.InteropServices;
namespace dersSecimKontrol
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\drs"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\drs\\conf.ini"))
                {
                    string [] satirlar = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\drs\\conf.ini");
                    if (satirlar!=null&&Microsoft.VisualBasic.Information.IsDate( satirlar[0].Split(':')[1])&&satirlar[0].Split(':')[1]==DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        MessageBox.Show("Sistemi meşgul etme kardeşim bugün senin ders seçim günün değilmiş.Bugün git yarın gel.","Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        Application.Exit();
                    }
                    else
                    {
                        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\drs\\conf.ini");
                    }
                }

            }
            DisableClickSounds();
        }

        private void btnBasla_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKullanici.Text)||string.IsNullOrEmpty(txtSifre.Text))
            {
                MessageBox.Show("Kullanıcı adı veya şifre boş geçilemez.");
                return;
            }

           

            if (Microsoft.VisualBasic.Information.IsNumeric(txtSure.Text))
            {
                if (Convert.ToInt32(txtSure.Text) < 20)
                {
                    MessageBox.Show("20 saniyeden aşağısı kurtarmaz.");
                }
                else
                {
                    txtSure.Text = "20";
                    timer1.Interval = Convert.ToInt32(txtSure.Text) * 1000;
                    timer1.Start();
                }
            }
            else
            {
                timer1.Interval = 20000;
                timer1.Start();
            }
            MessageBox.Show("Şimdi hiçbir şeye tıklamadan arkanıza yaslanın ve sistemin açıldığına dair uyarıyı bekleyin.","Birşeye tıklamanıza gerek yok",MessageBoxButtons.OK,MessageBoxIcon.Information);
            submitted = false;
            webBrowser1.Navigate("http://orgunkayit.anadolu.edu.tr/kayit/home.seam");
        }
        int tickCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            tickCount++;
            if (!webBrowser1.IsBusy && tickCount==1)
            {
                webBrowser1.Navigate("http://orgunkayit.anadolu.edu.tr/kayit/home.seam");
                submitted = false;
            }
           
          
            if (tickCount>2&&tickCount%2==0)
            {
               mynotifyicon.ShowBalloonTip(10000, "Ders kaydı başlayacak", "Ders kaydı başlayacak galiba şuan sisteme giriş yapılamıyor gibi görünüyor.Sistem "+tickCount*Convert.ToInt32(txtSure.Text)+" saniyedir cevap vermiyor.", ToolTipIcon.Info);
            }

        }
        bool submitted = false;
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            tickCount = 0;

            if (!submitted)
            {

                HtmlDocument doc = webBrowser1.Document;
                doc.GetElementById("loginForm:username").InnerText = txtKullanici.Text;
                doc.GetElementById("loginForm:password").InnerText = txtSifre.Text;
                doc.GetElementById("loginForm:submit").InvokeMember("click");
                
                submitted = true;
                
            }
            else
            {



                if (webBrowser1.Url.LocalPath.Contains("kayit.seam"))
                {
                    mynotifyicon.ShowBalloonTip(10000, "Ders kaydı başladı", "Ders kaydı başladı koşşş", ToolTipIcon.Info);
                    this.Show();
                    this.WindowState = FormWindowState.Maximized;
                    this.Activate();
                    MessageBox.Show("Ders kaydı başladı.","Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                if (webBrowser1.Url.OriginalString.Contains("action=5"))
                {
                       mynotifyicon.ShowBalloonTip(10000, "Bir sorunumuz var", "Hocam sen hangi gün ders seçimi yapacağını bildiğinden emin misin?", ToolTipIcon.Info);
                       if (!Directory.Exists(Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData)+ "\\drs"))
                       {
                           Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\drs");

                       }
                      File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\drs\\conf.ini","NotAllowedAt:"+DateTime.Now.ToString("yyyy-MM-dd"));
                      this.Show();
                      this.WindowState = FormWindowState.Maximized;
                      this.Activate();
                      MessageBox.Show("Ders seçimizi bugün değilmiş gibi görünüyor.Lütfen tarihten emin olunuz.","Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
                      Application.Exit();
                }
            }
        }

        private void btnKucult_Click(object sender, EventArgs e)
        {

            if (FormWindowState.Minimized == this.WindowState)
            {
                mynotifyicon.Visible = true;
                mynotifyicon.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                mynotifyicon.Visible = false;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                mynotifyicon.Visible = true;
                
                mynotifyicon.ShowBalloonTip(500);
                this.Hide();
               
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                mynotifyicon.Visible = false;
            }
        }

        private void mynotifyicon_DoubleClick(object sender, EventArgs e)
        {
            
            this.Show();
            this.WindowState = FormWindowState.Maximized;
            this.Activate();
        }

        private void txtSifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBasla_Click(sender, e);
            }
        }

        const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        const int SET_FEATURE_ON_PROCESS = 0x00000002;

        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(
            int FeatureEntry,
            [MarshalAs(UnmanagedType.U4)] int dwFlags,
            bool fEnable);

        static void DisableClickSounds()
        {
            CoInternetSetFeatureEnabled(
                FEATURE_DISABLE_NAVIGATION_SOUNDS,
                SET_FEATURE_ON_PROCESS,
                true);
        }

      

       
    }


}

