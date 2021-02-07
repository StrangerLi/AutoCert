using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// 查询是否存在_acme-challenge的TXT记录
        /// </summary>
        public bool IsExist(string DomainName, out IEnumerable<string> RecordIDs)
        {
            var request = new DescribeDomainRecordsRequest();
            request.DomainName = "hniot.com";
            request.RRKeyWord = "_acme-challenge";
            request.Type = "TXT";

            try
            {
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
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool AddDNS(string DomainName, string Value)
        {
            var request = new AddDomainRecordRequest();
            request.DomainName = DomainName;
            request.RR = "_acme-challenge";
            request.Type = "TXT";
            request._Value = Value;

            try
            {
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
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool UpdateDNS(string RecordID, string Value)
        {
            var request = new UpdateDomainRecordRequest();
            request.RecordId = RecordID;
            request.RR = "_acme-challenge";
            request.Type = "TXT";
            request._Value = Value;

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
    }
}
