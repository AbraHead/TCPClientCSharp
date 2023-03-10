using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //С ipendpoint удаётся забиндить конкретный адрес под tcp клиент
        IPEndPoint ipPoint = IPEndPoint.Parse("127.0.0.1:7777");
        TcpClient client = new TcpClient(ipPoint);
        await client.ConnectAsync("127.0.0.1", 8888);

        var jsonData = new List<Dictionary<string, Dictionary<string, string>>>()
        {
            new Dictionary<string, Dictionary<string, string>>
            {
                ["Person"] = new Dictionary<string, string>
                {
                    ["first_name"] = "Tom",
                    ["last_name"] = "Testing"
                },
                ["data"] = new Dictionary<string, string>
                {
                    ["passport"] = "2455 999155",
                    ["processed"] = "false"
                }
            },
            new Dictionary<string, Dictionary<string, string>>
            {
                ["Person"] = new Dictionary<string, string>
                {
                    ["first_name"] = "Sam",
                    ["last_name"] = "Henderson"
                }
            },
            new Dictionary<string, Dictionary<string, string>>
            {
                ["Person"] = new Dictionary<string, string>
                {
                    ["first_name"] = "Emanuell",
                    ["last_name"] = "Kant"
                },
                ["data"] = new Dictionary<string, string>
                {
                    ["passport"] = "2451 244562"
                }
            }
        };
        
        //Часть отправки данных по TCP
        //Сериализуем Json, кодируем в байты, открываем сетевой поток, посылаем данные в виде байтов
        var json = JsonSerializer.Serialize(jsonData);
        byte[] requestData = Encoding.UTF8.GetBytes(json);
        NetworkStream stream = client.GetStream();
        await stream.WriteAsync(requestData);

        //Часть получения данных по TCP
        int buffersize = 512;
        byte[] buffer = new byte[buffersize];
        StringBuilder response = new StringBuilder();
        int bytes;
        do
        {
            bytes = await stream.ReadAsync(buffer);
            string strresponse = Encoding.UTF8.GetString(buffer, 0, buffersize);
            response.Append(strresponse);
        } while (bytes == buffersize);

        Console.WriteLine(response);
        stream.Close();
        client.Close();

        Console.WriteLine("Нажмите enter...");
        Console.ReadLine();
    }

    public static Dictionary<string, string> createDict (string[] key, string[] value)
    {
        Dictionary<string, string> resultDict = new Dictionary<string, string>() { };
        for (int i = 0; i < key.Length; i++)
        {
            resultDict.Add(key[i], value[i]);
        };
        return resultDict;
    }
}