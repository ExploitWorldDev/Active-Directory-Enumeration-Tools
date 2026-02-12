using System;
using System.Net.Sockets;
using System.Threading.Tasks;

class SubnetPortScanner
{
    static readonly int[] CommonPorts = new int[]
    {
        20, 21, 22, 23, 25, 53, 80, 110, 143, 443, 445, 993, 995, 3306, 3389
    };

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: SubnetPortScanner <Subnet>");
            Console.WriteLine("Example: SubnetPortScanner 192.168.1");
            return;
        }

        string subnet = args[0];
        ScanSubnet(subnet).Wait();
    }

    static async Task ScanSubnet(string subnet)
    {
        for (int i = 1; i <= 254; i++)
        {
            string ip = $"{subnet}.{i}";
            Console.WriteLine($"Scanning {ip}...");
            foreach (int port in CommonPorts)
            {
                bool isOpen = await IsPortOpenAsync(ip, port, TimeSpan.FromSeconds(2));
                if (isOpen)
                {
                    Console.WriteLine($"  Port {port} is open on {ip}");
                }
            }
        }
        Console.WriteLine("Scan complete.");
    }

    static async Task<bool> IsPortOpenAsync(string ip, int port, TimeSpan timeout)
    {
        try
        {
            using (var client = new TcpClient())
            {
                var connectTask = client.ConnectAsync(ip, port);
                var resultTask = await Task.WhenAny(connectTask, Task.Delay(timeout));
                if (resultTask == connectTask && client.Connected)
                {
                    client.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        catch
        {
            return false;
        }
    }
}
