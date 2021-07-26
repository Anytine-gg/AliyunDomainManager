using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text;
using System.Web.Management;
using System.Windows.Forms;

namespace DomainManager
{
    internal class Domain
    {
        private string accessKeyId;
        private string secret;
        private IClientProfile profile;
        private DefaultAcsClient client;

        public Domain(string accessKeyId, string secret)
        {
            this.accessKeyId = accessKeyId;
            this.secret = secret;
            profile = DefaultProfile.GetProfile
                ("cn-hangzhou", this.accessKeyId, this.secret);
            client = new DefaultAcsClient(profile);
        }

        public JObject GetInfo()
        {
            var request = new DescribeDomainsRequest();
            try
            {
                var response = client.GetAcsResponse(request);

                return JSON_decode(System.Text.Encoding.Default.GetString(response.HttpResponse.Content));
            }
            catch (Exception)
            {
                MessageBox.Show("获取域名列表错误！请检查AK时候正确！");
                return null;
            }
        }

        public JObject GetInfo(string DomainName)
        {
            var request = new DescribeDomainRecordsRequest();
            request.DomainName = DomainName;
            try
            {
                var response = client.GetAcsResponse(request);

                return JSON_decode(System.Text.Encoding.Default.GetString(response.HttpResponse.Content));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ERROR");
                return null;
            }
        }

        public int UPDATE_Domain(string RecordId, string RR, string Type, string Value, int TTL)
        {
            var request = new UpdateDomainRecordRequest();
            request.RecordId = RecordId;
            request.RR = RR;
            request.Type = Type;
            request._Value = Value;
            request.TTL = TTL;
            try
            {
                var response = client.GetAcsResponse(request);
                MessageBox.Show(System.Text.Encoding.Default.GetString(response.HttpResponse.Content), "成功更改解析记录！");
                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ERROR");
                return 0;
            }
        }

        public string DDNS_Domain(string RecordId, string RR, string Type, string Value, int TTL)
        {
            {
                var request = new UpdateDomainRecordRequest();
                request.RecordId = RecordId;
                request.RR = RR;
                request.Type = Type;
                request._Value = Value;
                request.TTL = TTL;
                try
                {
                    var response = client.GetAcsResponse(request);
                    return System.Text.Encoding.Default.GetString(response.HttpResponse.Content);

                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        public int AddNewRecord(string Domain, string RR, string Type, string Value, int TTL)
        {
            var request = new AddDomainRecordRequest();
            request.DomainName = Domain;
            request.RR = RR;
            request.Type = Type;
            request._Value = Value;
            request.TTL = TTL;
            try
            {
                var response = client.GetAcsResponse(request);
                MessageBox.Show(System.Text.Encoding.Default.GetString(response.HttpResponse.Content), "成功添加记录！");
                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ERROR");
            }

            return 0;
        }

        public static JObject JSON_decode(string json_str)
        {
            var json = JsonConvert.DeserializeObject(json_str) as JObject;

            return json;
        }

        public string[] Getip(string URL)
        {
            // http://members.3322.org/dyndns/getip
            try
            {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = MyWebClient.DownloadData(URL);
                string pageHtml = Encoding.UTF8.GetString(pageData);

                StringBuilder sb = new StringBuilder();
                string[] parts = pageHtml.Split(new char[] { ' ', '\n', '\t', '\r', '\f', '\v', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                int size = parts.Length;
                for (int i = 0; i < size; i++)
                {
                    sb.AppendFormat("{0}", parts[i]);
                }

                string[] rel = new string[2];
                rel[0] = "1";
                rel[1] = sb.ToString();
                return rel;

            }
            catch (WebException e)
            {
                string[] rel = new string[2];
                rel[0] = "0";
                rel[1] = e.Message;
                return rel;
            }
        }

        public int DelSingleDomain(string RecordId)
        {
            var request = new DeleteDomainRecordRequest();
            request.RecordId = RecordId;
            try
            {
                var response = client.GetAcsResponse(request);
                MessageBox.Show(System.Text.Encoding.Default.GetString(response.HttpResponse.Content), "删除成功！");
                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "删除失败！");
            }

            return 0;
        }
    }
}