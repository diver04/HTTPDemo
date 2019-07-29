using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization; 
using System.Net;
using System.IO;
//using Newtonsoft.Json;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        private static string token;
        public Form1()
        {
            InitializeComponent();
        }

        //https://10.160.0.252:9999/api/v1/login
        //http://10.0.1.9:8887/CS_server
        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> Dict = new Dictionary<string, string>();
            Dict.Add("Command", "[FSN]");
            string json = (new JavaScriptSerializer()).Serialize(Dict);

            byte[] bs = Encoding.ASCII.GetBytes(json);    //参数转化为ascii码

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://10.0.1.9:8887/CS_server");  //创建request

            req.Method = "POST";    //post方式
            req.Accept = "application/json";
            req.ContentType = "application/json";
           
            req.ContentLength = bs.Length;


            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }

            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                System.IO.StreamReader respStream = new System.IO.StreamReader(wr.GetResponseStream());
                string content = respStream.ReadToEnd();
                textBox1.Text = content;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> Dict = new Dictionary<string, string>();
            Dict.Add("username","admin");
            Dict.Add("password","IAC!123");
           
            string json = (new JavaScriptSerializer()).Serialize(Dict);
            
            byte[] bs = Encoding.ASCII.GetBytes(json);    //参数转化为ascii码
            //解决因证书无法建立安全连接问题
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://10.160.0.252:9999/api/v1/login");  //创建request

            req.Method = "POST";    //post方式  
            req.Accept = "application/json";
            req.ContentType = "application/json";
            //req.ContentLength = bs.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }

            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                System.IO.StreamReader respStream = new System.IO.StreamReader(wr.GetResponseStream());
                string s = respStream.ReadToEnd();
                JavaScriptSerializer ser = new JavaScriptSerializer();
                Dictionary<string,object> JsonDict = (Dictionary<string,object>)ser.DeserializeObject(s);
                token = (string)JsonDict["token"];
                textBox1.Text = token;
                //MessageBox.Show(content);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //解决因证书无法建立安全连接问题
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://10.160.0.252:9999/api/v1/users");  //创建request

            req.Method = "GET";
            req.Accept = "application/json";
            req.ContentType = "application/json";
            req.Headers.Add("Authorization", token);

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreanReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreanReader.ReadToEnd();

            myStreanReader.Close();
            myResponseStream.Close();
            //textBox1.Text = retString;
            MessageBox.Show(retString);
        }
    }
}
