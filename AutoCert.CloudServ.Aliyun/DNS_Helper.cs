using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCert.CloudServ.Aliyun
{
    public class DNS_Helper
    {
        /// <summary>
        /// 地域ID
        /// </summary>
        public string RegionID { get; set; }
        /// <summary>
        /// 授权KeyID
        /// </summary>
        public string AccessKeyId { get; set; }
        /// <summary>
        /// 授权秘钥ID
        /// </summary>
        public string AccessKeySecret { get; set; }

        public DNS_Helper(string accessKeyId, string accessKeySecret) : this("cn-beijing", accessKeyId, accessKeySecret)
        {
        }

        public DNS_Helper(string regionId, string accessKeyId, string accessKeySecret)
        {
            this.RegionID = regionId;
            this.AccessKeyId = accessKeyId;
            this.AccessKeySecret = accessKeySecret;
        }

        /// <summary>
        /// 根据Host拆分DomainName与RR
        /// </summary>
        /// <param name="Host"></param>
        /// <returns></returns>
        private (string DomainName, string RR) SplitDomainName(string Host)
        {
            var Host_Array = Host.Split('.');

            var RR = string.Empty;

            if (Host_Array == null && Host_Array.Length < 2)
            {
                return (string.Empty, string.Empty);
            }

            var DomainName = $@"{Host_Array[^2]}.{Host_Array[^1]}";

            if (Host_Array.Length > 2)
            {
                RR += string.Join(".", Host_Array.Take(Host_Array.Length - 2));
            }

            return (DomainName, RR);
        }

        /// <summary>
        /// 查询是否存在_acme-challenge的TXT记录
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="RecordIDs"></param>
        /// <returns></returns>
        public bool IsExist(string Host, out IEnumerable<string> RecordIDs)
        {
            try
            {
                var request = new DescribeDomainRecordsRequest();
                (request.DomainName, request.RRKeyWord) = SplitDomainName(Host);
                request.RRKeyWord = $@"_acme-challenge{(string.IsNullOrEmpty(request.RRKeyWord) ? request.RRKeyWord : $@".{request.RRKeyWord}")}";
                request.Type = "TXT";

                var response = SDK_Helper.CreateClient(this.AccessKeyId, this.AccessKeySecret).GetAcsResponse(request);
                if (response != null && response.TotalCount > 0)
                {
                    RecordIDs = response.DomainRecords.Select(P => P.RecordId);
                    return true;
                }
            }
            catch (ServerException e)
            {
                Console.WriteLine(e);
            }
            catch (ClientException e)
            {
                Console.WriteLine(e);
            }

            RecordIDs = null;
            return false;
        }

        /// <summary>
        /// 添加DNS记录
        /// </summary>
        /// <param name="DomainName"></param>
        /// <param name="RRWord"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool AddDNS(string Host, string Value)
        {
            try
            {
                var request = new AddDomainRecordRequest();
                (request.DomainName, request.RR) = SplitDomainName(Host);
                request.RR = $@"_acme-challenge{(string.IsNullOrEmpty(request.RR) ? request.RR : $@".{request.RR}")}";
                request.Type = "TXT";
                request._Value = Value;

                var response = SDK_Helper.CreateClient(this.AccessKeyId, this.AccessKeySecret).GetAcsResponse(request);
                if (response != null && !string.IsNullOrEmpty(response.RecordId) && !string.IsNullOrEmpty(response.RequestId))
                {
                    return true;
                }
            }
            catch (ServerException e)
            {
                Console.WriteLine(e);
            }
            catch (ClientException e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// 删除DNS记录
        /// </summary>
        /// <param name="RecordID"></param>
        /// <returns></returns>
        public bool Delete(string RecordID)
        {
            var request = new DeleteDomainRecordRequest();
            request.RecordId = RecordID;

            try
            {
                var response = SDK_Helper.CreateClient(this.AccessKeyId, this.AccessKeySecret).GetAcsResponse(request);
                if (response != null && response.RecordId == RecordID && !string.IsNullOrEmpty(response.RequestId))
                {
                    return true;
                }
            }
            catch (ServerException e)
            {
                Console.WriteLine(e);
            }
            catch (ClientException e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// 更新DNS记录
        /// </summary>
        /// <param name="RecordID"></param>
        /// <param name="RRWord"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool UpdateDNS(string RecordID, string Host, string Value)
        {
            try
            {
                var request = new UpdateDomainRecordRequest();
                (_, request.RR) = SplitDomainName(Host);
                request.RR = $@"_acme-challenge{(string.IsNullOrEmpty(request.RR) ? request.RR : $@".{request.RR}")}";
                request.Type = "TXT";
                request._Value = Value;

                var response = SDK_Helper.CreateClient(this.AccessKeyId, this.AccessKeySecret).GetAcsResponse(request);
                if (response != null && response.RecordId == RecordID && !string.IsNullOrEmpty(response.RequestId))
                {
                    return true;
                }
            }
            catch (ServerException e)
            {
                Console.WriteLine(e);
            }
            catch (ClientException e)
            {
                Console.WriteLine(e);
            }

            return false;
        }
    }
}
