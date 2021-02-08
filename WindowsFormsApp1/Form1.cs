using AutoCert.CloudServ.Aliyun;
using Certes;
using Certes.Acme;
using Certes.Pkcs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBox1.Text = "-----BEGIN EC PRIVATE KEY-----\r\nMHcCAQEEICkjuNTOa73CMj97AppMQthGjQ3gvRieean+cEdpEqM1oAoGCCqGSM49\r\nAwEHoUQDQgAEs13ieNqQR2bJGQsodwgLbupF2qvKIXwSkSJ4QoST4mN3FXS4J8Qi\r\n2Fq4TM8C3DZe8o1gqfwNDPWpChOvhxAhTg==\r\n-----END EC PRIVATE KEY-----";
            this.textBox2.Text = @$"https://acme-staging-v02.api.letsencrypt.org/acme/order/17957387/235509166";
        }

        private DNS_Helper aliyun { get; set; } = new DNS_Helper("LTAI4GJzMP5NyfC3CTuWtY46", "rh6z4NdBsZXJit3b539nyiThEACIG4");

        private async void button1_Click(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button2_Click(object sender, EventArgs e)
        {
            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Order = await ACME.NewOrder(new[] { "hniot.com" });

            this.textBox2.Text = Order.Location.AbsoluteUri;
        }

        /// <summary>
        /// 获取要修改的DNS记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button4_Click(object sender, EventArgs e)
        {
            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Order = ACME.Order(new Uri(this.textBox2.Text));
            StringBuilder Strb = new StringBuilder();

            var authz = await Order.Authorizations();
            foreach (var item in authz)
            {
                var dnsChallenge = await item.Dns();
                Strb.AppendLine(ACME.AccountKey.DnsTxt(dnsChallenge.Token));
            }

            this.textBox3.Text = Strb.ToString();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Order = ACME.Order(new Uri(this.textBox2.Text));

            var DNS_Txt_Records = new List<string>();

            var authz = await Order.Authorizations();
            foreach (var item in authz)
            {
                var dnsChallenge = await item.Dns();
                DNS_Txt_Records.Add(ACME.AccountKey.DnsTxt(dnsChallenge.Token));
            }


            if (aliyun.IsExist("hniot.com", out var RecordIds))
            {
                foreach (var item1 in RecordIds)
                {
                    aliyun.Delete(item1);
                }
            }

            foreach (var item in DNS_Txt_Records)
            {
                if (!aliyun.AddDNS("hniot.com", item))
                {
                    throw new Exception("ROORO");
                }
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Order = ACME.Order(new Uri(this.textBox2.Text));

            var authz = await Order.Authorizations();

            foreach (var item in authz)
            {
                var dnsChallenge = await item.Dns();
                var dd = await dnsChallenge.Validate();
                if (dd.Status != Certes.Acme.Resource.ChallengeStatus.Valid)
                {
                    MessageBox.Show(@$"认证失败:{dd.Status}");
                }
            }

        }

        private async void button6_Click(object sender, EventArgs e)
        {

            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Order = ACME.Order(new Uri(this.textBox2.Text));

            var bytes = System.IO.File.ReadAllBytes("./csr.pem");

            var cert = await Order.Finalize(bytes);
            if (cert.Status == Certes.Acme.Resource.OrderStatus.Valid)
            {
                MessageBox.Show("成功");
            }
            var sss = await Order.Download();

            this.textBox5.Text = sss?.ToPem();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button7_Click(object sender, EventArgs e)
        {
            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Orderctx = ACME.Order(new Uri(this.textBox2.Text));

            var order = await Orderctx.Resource();

            if (order.Status == Certes.Acme.Resource.OrderStatus.Valid)
            {
                MessageBox.Show("成功");
            }
            else
            {
                MessageBox.Show(@$"失败：{order.Status}");
            }
        }
    }
}
