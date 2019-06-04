// using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace PruebaHubConnect
{
    class Program
    {
        static void Main(string[] args)
        {
            // https://stackoverflow.com/questions/11140164/signalr-console-app-example
            // https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-2.2
            Console.WriteLine("Empezando la aplicación");
            HubConnection connection;
            connection = new HubConnectionBuilder()
                .WithUrl("https://smarthouse48.azurewebsites.net/action-hub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(1, 2) * 1000);
                await connection.StartAsync();
            };

            Console.WriteLine("Empezamos la conexión");
            connection.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Nos conectamos al grupo por email");
            connection.InvokeAsync("JoinDashboardGroup", "lmartinez.bno@gmail.com");
            Console.WriteLine("Nos hemos conectado al grupo");

            //===============================Conexión de arduino=====================================================
            System.IO.Ports.SerialPort arduino;
            arduino = new System.IO.Ports.SerialPort();
            arduino.PortName = "COM13";
            arduino.BaudRate = 9600;
            arduino.Open();

            // connection.

            string name = "";
            Console.WriteLine("Empezamos a escuchar cabmbios de los diferentes dispositivos");
            connection.On<Device>("DeviceIsOnChange", (data) =>
            {
                Console.WriteLine(data.Id);
                Console.WriteLine(data.IsOn);
                Console.WriteLine(data.Name);
                Console.WriteLine(data.Plug);

                name = data.Name.ToLower();
                if(name.Contains("alarma"))
                {
                    Console.WriteLine("Contiene alarma");
                    if(data.IsOn)
                    {
                        arduino.Write("alarma ki on");
                    } else
                    {
                        arduino.Write("alarma ki off");
                    }
                } else if(name.Contains("enchufe"))
                {
                    Console.WriteLine("Contiene enchufe");
                    if(data.IsOn)
                    {
                        arduino.Write($"rele {data.Plug} on");
                    } else
                    {
                        arduino.Write($"rele {data.Plug} off");
                    }
                } else if(name.Contains("infrarrojo"))
                {
                    if(data.IsOn)
                    {
                        arduino.Write("infrarred one on");
                    } else
                    {
                        arduino.Write("infrarred one off");
                    }
                }

            });

            Console.ReadLine();
        }
    }
}