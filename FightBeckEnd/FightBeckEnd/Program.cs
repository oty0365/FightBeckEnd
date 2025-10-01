using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpListener listener;
    static bool isRunning = true;

    static void Main(string[] args)
    {
        int port = 7777;
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"서버 시작됨. 포트 {port}에서 대기 중...");

        Thread acceptThread = new Thread(AcceptClients);
        acceptThread.Start();

        Console.WriteLine("종료하려면 Enter를 누르세요.");
        Console.ReadLine();
        isRunning = false;
        listener.Stop();
    }

    static void AcceptClients()
    {
        while (isRunning)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("클라이언트 접속됨!");
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
            catch (SocketException)
            {
                break;
            }
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("받음: " + message);
                byte[] response = Encoding.UTF8.GetBytes("서버: " + message);
                stream.Write(response, 0, response.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("클라이언트 연결 종료: " + e.Message);
        }
        finally
        {
            client.Close();
        }
    }
}
