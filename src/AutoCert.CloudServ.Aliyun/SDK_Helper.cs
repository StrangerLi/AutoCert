using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCert.CloudServ.Aliyun
{
   public static class SDK_Helper
    {
        public static DefaultAcsClient CreateClient(string accessKeyId, string accessKeySecret)
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-qingdao", accessKeyId, accessKeySecret);

            return new DefaultAcsClient(profile);
        }
    }
}
