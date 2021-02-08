using AutoCert.CloudServ.Aliyun;
using Certes;
using Certes.Acme;
using Certes.Jws;
using Certes.Pkcs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Certes.Properties;
using Certes.Acme.Resource;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBox1.Text = "-----BEGIN EC PRIVATE KEY-----\r\nMHcCAQEEICkjuNTOa73CMj97AppMQthGjQ3gvRieean+cEdpEqM1oAoGCCqGSM49\r\nAwEHoUQDQgAEs13ieNqQR2bJGQsodwgLbupF2qvKIXwSkSJ4QoST4mN3FXS4J8Qi\r\n2Fq4TM8C3DZe8o1gqfwNDPWpChOvhxAhTg==\r\n-----END EC PRIVATE KEY-----";
            this.textBox2.Text = @$"https://acme-staging-v02.api.letsencrypt.org/acme/order/17957387/236133581";
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
            var Order = await ACME.NewOrder(new[] { "hniot.com" ,"www.hniot.com"});

            this.textBox2.Text = Order.Location.AbsoluteUri;
        }

        /// <summary>
        /// 获取要修改的DNS记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button4_Click(object sender, EventArgs e)
        {
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Order = ACME.Order(new Uri(this.textBox2.Text));

            var DNS_Txt_Records = new Dictionary<string, List<string>>();

            var authz = await Order.Authorizations();
            foreach (var item in authz)
            {
                var Challenge = await item.Dns();
                var Resource = await item.Resource();
                var DnsTxt = ACME.AccountKey.DnsTxt(Challenge.Token);

                if (!DNS_Txt_Records.ContainsKey(Resource.Identifier.Value))
                {
                    DNS_Txt_Records.Add(Resource.Identifier.Value, new List<string>());
                }

                if (!DNS_Txt_Records[Resource.Identifier.Value].Contains(DnsTxt))
                {
                    DNS_Txt_Records[Resource.Identifier.Value].Add(DnsTxt);
                }
            }

            this.textBox3.Text = JsonConvert.SerializeObject(DNS_Txt_Records, Formatting.Indented);

            foreach (var item in DNS_Txt_Records)
            {
                var host = item.Key;
                if (host.StartsWith("*."))
                {
                    host = host.Substring(2);
                }

                if (aliyun.IsExist(host, out var RecordIds))
                {
                    foreach (var RecordId in RecordIds)
                    {
                        aliyun.Delete(RecordId);
                    }
                }

                foreach (var Dns_Auth in item.Value)
                {
                    if (!aliyun.AddDNS(host, Dns_Auth))
                    {
                        throw new Exception("ROORO");
                    }
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
            var CSR = Regex.Replace(System.IO.File.ReadAllText("./csr.pem"), @"-----[^-]+-----", string.Empty).Trim().Replace(" ", "").Replace(Environment.NewLine, "");

            var ACME = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, KeyFactory.FromPem(this.textBox1.Text));
            var Order_C = ACME.Order(new Uri(this.textBox2.Text));

            var cert = await Order_C.Finalize(Convert.FromBase64String(CSR));
            if (cert.Status == Certes.Acme.Resource.OrderStatus.Valid)
            {
                MessageBox.Show("成功");
            }

            var sss = await Order_C.Download();

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

    public static class sss
    {
        public static async Task<AcmeHttpResponse<T>> Post<T>(this IAcmeHttpClient client,
IAcmeContext context,
Uri location,
object entity,
bool ensureSuccessStatusCode)
        {

            var payload = await context.Sign(entity, location);
            var response = await client.Post<T>(location, payload);
            while (response.Error?.Status == System.Net.HttpStatusCode.BadRequest &&
                response.Error.Type?.CompareTo("urn:ietf:params:acme:error:badNonce") == 0)
            {
                payload = await context.Sign(entity, location);
                response = await client.Post<T>(location, payload);
            }

            if (ensureSuccessStatusCode && response.Error != null)
            {

            }

            return response;
        }
    }
}
