using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free.nn
{
    class NNFreeAPI
    {
        private SocketClient client;
        private int PORT;
        private string HOST = null;
        private string result = null;
        private bool IsConnected = false;
        private string p1;
        private int p2;

        public NNFreeAPI(string host,int port)
        {
            this.HOST = host;
            this.PORT = port;
            client = new SocketClient();
        }

        public int Connect()
        {
            this.result = this.client.Connect(this.HOST, this.PORT);

            if (!this.result.Contains("Success"))
            {
                this.IsConnected = false;
                return -1;
            }
            else
            {
                this.IsConnected = true;
                return 0;
            }
        }

        public void Close()
        {
            this.client.Close();
        }

        public int App_ID()
        {
            if(this.IsConnected)
            {
                this.client.Send("<get_app_id>");
                this.result = this.client.Receive();
                if(this.result.Contains("app_id="))
                {
                    return Int32.Parse(this.result.Split(new Char[] { '=','>' })[1]);
                }
                else
                {
                    return -2;
                }
            }
            else
            {
                return -1;
            }
        }

        public int CheckUser(string id)
        {
            this.client.Send("<check_user/"+id+">");
            this.result = this.client.Receive();
            
            if (this.result.Contains("<old_user>"))
            {
                return 0;
            }
            else
            {
                if(this.result.Contains("<new_user>"))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

        }
    }
}
