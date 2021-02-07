using Certes;
using Certes.Acme;
using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Tea;
using Tea.Utils;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = CreateClient("LTAI4GJzMP5NyfC3CTuWtY46", "rh6z4NdBsZXJit3b539nyiThEACIG4");
            var RequestInfo = new AlibabaCloud.SDK.Alidns20150109.Models.DescribeRecordLogsRequest
            {
                DomainName = "hniot.com",
            };
            var dd = client.DescribeRecordLogs(RequestInfo);



            Console.WriteLine("Hello World!");
        }

        public static AlibabaCloud.SDK.Alidns20150109.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config();
            // 您的AccessKey ID
            config.AccessKeyId = accessKeyId;
            // 您的AccessKey Secret
            config.AccessKeySecret = accessKeySecret;
            // 访问的域名
            config.Endpoint = "alidns.cn-beijing.aliyuncs.com";
            return new AlibabaCloud.SDK.Alidns20150109.Client(config);
        }

    }
}
