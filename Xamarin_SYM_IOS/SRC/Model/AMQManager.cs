using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.com.kc.amq
{
    public class AMQManager
    {
        String amqHost = @"tcp://210.65.88.10:61616";
        private IConnection connection;
        private ISession session;

        public const String QUEUE_ADVISORY_DESTINATION = "ActiveMQ.Advisory.Queue";
        public const String TOPIC_ADVISORY_DESTINATION = "ActiveMQ.Advisory.Topic";
        public const String TEMPQUEUE_ADVISORY_DESTINATION = "ActiveMQ.Advisory.TempQueue";
        public const String TEMPTOPIC_ADVISORY_DESTINATION = "ActiveMQ.Advisory.TempTopic";

        public const String ALLDEST_ADVISORY_DESTINATION = QUEUE_ADVISORY_DESTINATION + "," +
                                                           TOPIC_ADVISORY_DESTINATION + "," +
                                                           TEMPQUEUE_ADVISORY_DESTINATION + "," +
                                                           TEMPTOPIC_ADVISORY_DESTINATION;

        private List<IMessageProducer> producers = new List<IMessageProducer>();
        private List<IMessageConsumer> consumers = new List<IMessageConsumer>();
        private String currentDestination = "";


        private bool mEnabled = false;
        /// <summary>
        /// AMQ連線現在是否啟動
        /// </summary>
        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
        }
        public AMQManager(String amgHost)
        {
            this.amqHost = amgHost;
            //IConnectionFactory factory = new ConnectionFactory();
            Uri connecturi = new Uri(amgHost);

            Console.WriteLine("About to connect to " + connecturi);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            connection = factory.CreateConnection();


            connection.Start();
            session = connection.CreateSession();
            mEnabled = true;
        }


        public AMQManager(String userName, String password, String target)
        {
            this.amqHost = target;
            //IConnectionFactory factory = new ConnectionFactory();

            Uri connecturi = new Uri(target);

            Console.WriteLine("About to connect to " + connecturi);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);
            connection = factory.CreateConnection(userName, password);


            connection.Start();
            session = connection.CreateSession();
            mEnabled = true;
        }

        public void EnumerateQueues()
        {
            Console.WriteLine("Listing all Queues on Broker:");

            IDestination dest = session.GetTopic(QUEUE_ADVISORY_DESTINATION);

            using (IMessageConsumer consumer = session.CreateConsumer(dest))
            {
                IMessage advisory;

                while ((advisory = consumer.Receive(TimeSpan.FromMilliseconds(2000))) != null)
                {
                    ActiveMQMessage amqMsg = advisory as ActiveMQMessage;

                    if (amqMsg.DataStructure != null)
                    {
                        DestinationInfo info = amqMsg.DataStructure as DestinationInfo;
                        if (info != null)
                        {
                            Console.WriteLine("   Queue: " + info.Destination.ToString());
                        }
                    }
                }
            }
            Console.WriteLine("Listing Complete.");
        }

        public void EnumerateTopics()
        {
            Console.WriteLine("Listing all Topics on Broker:");

            IDestination dest = session.GetTopic(TOPIC_ADVISORY_DESTINATION);

            using (IMessageConsumer consumer = session.CreateConsumer(dest))
            {
                IMessage advisory;

                while ((advisory = consumer.Receive(TimeSpan.FromMilliseconds(2000))) != null)
                {
                    ActiveMQMessage amqMsg = advisory as ActiveMQMessage;

                    if (amqMsg.DataStructure != null)
                    {
                        DestinationInfo info = amqMsg.DataStructure as DestinationInfo;
                        if (info != null)
                        {
                            Console.WriteLine("   Topic: " + info.Destination.ToString());
                        }
                    }
                }
            }
            Console.WriteLine("Listing Complete.");
        }

        public void EnumerateDestinations()
        {
            Console.WriteLine("Listing all Destinations on Broker:");

            IDestination dest = session.GetTopic(ALLDEST_ADVISORY_DESTINATION);

            using (IMessageConsumer consumer = session.CreateConsumer(dest))
            {
                IMessage advisory;

                while ((advisory = consumer.Receive(TimeSpan.FromMilliseconds(2000))) != null)
                {
                    ActiveMQMessage amqMsg = advisory as ActiveMQMessage;

                    if (amqMsg.DataStructure != null)
                    {
                        DestinationInfo info = amqMsg.DataStructure as DestinationInfo;
                        if (info != null)
                        {
                            string destType = info.Destination.IsTopic ? "Topic" : "Qeue";
                            destType = info.Destination.IsTemporary ? "Temporary" + destType : destType;
                            Console.WriteLine("   " + destType + ": " + info.Destination.ToString());
                        }
                    }
                }
            }
            Console.WriteLine("Listing Complete.");
        }

        public void ShutDown()
        {
            session.Close();
            connection.Close();
            mEnabled = false;
        }


        /// <summary>
        /// 建立訊息產生者
        /// </summary>
        /// <param name="destinationStr"></param>
        /// <returns></returns>
        public IMessageProducer CreateProducer(String destinationStr)
        {
            if (destinationStr == null || session == null)
                return null;

            IDestination destination = SessionUtil.GetDestination(session, destinationStr);
            if (destination == null)
                return null;

            currentDestination = destinationStr;

            IMessageProducer producer;
            producer = session.CreateProducer(destination);
            if (producer != null)
                producers.Add(producer);
            return producer;
        }


        /// <summary>
        /// 建立訊息消費者
        /// </summary>
        /// <param name="destinationStr"></param>
        /// <returns></returns>
        public IMessageConsumer CreateConsumer(String destinationStr)
        {
            if (destinationStr == null || session == null)
                return null;

            IDestination destination = SessionUtil.GetDestination(session, destinationStr);
            if (destination == null)
                return null;

            currentDestination = destinationStr;
            IMessageConsumer consumer;
            consumer = session.CreateConsumer(destination);
            if (consumer != null)
                consumers.Add(consumer);
            return consumer;

        }

        /// <summary>
        /// 送資料
        /// </summary>
        /// <param name="producer">資料產生者</param>
        /// <param name="message">要發送的資料</param>
        /// <returns>資料發送成功與否</returns>
        public bool SendMessage(IMessageProducer producer, String message)
        {
            if (producer == null || session == null)
                return false;

            var request = session.CreateTextMessage(message);
            return SendMessage(producer, request);
        }

        /// <summary>
        /// 送資料
        /// </summary>
        /// <param name="producer">資料產生者</param>
        /// <param name="message">要發送的資料</param>
        /// <returns>資料發送成功與否</returns>
        public bool SendMessage(IMessageProducer producer, ITextMessage request)
        {
            bool ret = false;
            if (producer == null || session == null || request == null)
                return false;

            try
            {
                producer.Send(request);
                ret = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("AMQManager SendMessage Error.");
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 送字元組資料
        /// </summary>
        /// <param name="producer">資料產生者</param>
        /// <param name="message">要發送的資料</param>
        /// <returns>資料發送成功與否</returns>
        public bool SendMessage(IMessageProducer producer, byte[] message)
        {
            if (producer == null || session == null)
                return false;

            var request = session.CreateBytesMessage(message);
            return SendMessage(producer, request);
        }

        /// <summary>
        /// 送字元組資料
        /// </summary>
        /// <param name="producer">資料產生者</param>
        /// <param name="message">要發送的資料</param>
        /// <returns>資料發送成功與否</returns>
        public bool SendMessage(IMessageProducer producer, IBytesMessage request)
        {
            bool ret = false;
            if (producer == null || session == null || request == null)
                return false;

            try
            {
                producer.Send(request);
                ret = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("AMQManager SendByteMessage Error.");
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 接收資料
        /// </summary>
        /// <param name="consumer">訊息消費者</param>
        /// <param name="timeoutInMillisecs">逾時時間 millisecs</param>
        /// <returns>字串資料</returns>
        public String RecvMessage(IMessageConsumer consumer, int timeoutInMillisecs = 0)
        {
            String ret = "";
            ITextMessage result = null;
            if (timeoutInMillisecs <= 0)
                result = consumer.Receive() as ITextMessage;
            else
                result = consumer.Receive(new TimeSpan(0, 0, 0, 0, (int)timeoutInMillisecs)) as ITextMessage;
            if (result != null)
                ret = result.Text;
            return ret;
        }

        /// <summary>
        /// 接收資料
        /// </summary>
        /// <param name="consumer">資料消費者</param>
        /// <param name="timeoutInMillisecs">逾時時間</param>
        /// <returns>字元組資料</returns>
        public byte[] RecvBytesMessage(IMessageConsumer consumer, int timeoutInMillisecs = 0)
        {
            byte[] ret = null;
            IBytesMessage result = null;
            if (timeoutInMillisecs <= 0)
                result = consumer.Receive() as IBytesMessage;
            else
                result = consumer.Receive(new TimeSpan(0, 0, 0, 0, (int)timeoutInMillisecs)) as IBytesMessage;
            if (result != null)
                ret = result.Content;
            return ret;
        }

        /// <summary>
        /// 關閉AMQManager資源
        /// </summary>
        /// <returns></returns>
        public bool Dispose()
        {
            bool ret = false;

            try
            {
                foreach (var producer in producers)
                {
                    if (producer != null)
                        producer.Close();
                }
                producers.Clear();

                foreach (var consumer in consumers)
                {
                    if (consumer != null)
                        consumer.Close();
                }
                consumers.Clear();

                if (session != null)
                    session.Close();

                if (connection != null)
                    connection.Close();

                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                Console.WriteLine(ex.StackTrace);
            }
            mEnabled = false;
            return ret;
        }

        //獲取目前Session
        public ISession CurrentSession
        {
            get { return session; }
        }

        /// <summary>
        /// 刪除目標通道
        /// </summary>
        /// <returns></returns>
        public bool DeleteCurrentDestination()
        {
            if (session == null || currentDestination == null)
                return false;
            session.DeleteDestination(currentDestination);
            return true;
        }

        /// <summary>
        /// 可將int,List<int>的key,value pair組成JSon格式
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public string MyDictionaryToJson(Dictionary<int, List<int>> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }
    }
}
