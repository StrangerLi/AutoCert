using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using System;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-qingdao", "LTAI4GJzMP5NyfC3CTuWtY46", "rh6z4NdBsZXJit3b539nyiThEACIG4");
            DefaultAcsClient client = new DefaultAcsClient(profile);

            var request = new UpdateDomainRecordRequest();
            request.RecordId = "123";
            request.RR = "123";
            request.Type = "txt";
            request._Value = "ssss";

            try
            {
                var response = client.GetAcsResponse(request);
                var str = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
                Console.WriteLine();
            }
            catch (ServerException e)
            {
                Console.WriteLine(e);
            }
            catch (ClientException e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
