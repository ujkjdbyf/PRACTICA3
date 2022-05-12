using System;
using System.Collections.Concurrent;
using System.Threading;

namespace OS_Practice3
{
    internal static class Program
    {
        private const int Producers = 3;
        private const int ProducersTimeout = 300;

        private const int Consumers = 2;
        private const int ConsumersTimeout = 500;

        private const int Capacity = 200;

        private static bool _stopProducers;
        private static bool _pauseProducers;

        private static readonly BlockingCollection<int> Queue = new BlockingCollection<int>(Capacity);

        private static void Main()
        {
            Thread[] prodThreads = new Thread[Producers];
            Thread[] consThreads = new Thread[Consumers];

            for (int i = 0; i < Producers; i++)
            {
                prodThreads[i] = new Thread(Produce) {Name = $"Производитель {i + 1}"};
                prodThreads[i].Start();
            }

            for (int i = 0; i < Consumers; i++)
            {
                consThreads[i] = new Thread(Consume) {Name = $"Потребитель {i + 1}"};
                consThreads[i].Start();
            }

            do
            {
                while (!Console.KeyAvailable)
                {
                    if (Queue.Count >= 100)
                        _pauseProducers = true;
                    if (Queue.Count <= 80)
                        _pauseProducers = false;
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);

            _stopProducers = true;
            Console.WriteLine("Производители остановлены пользователем");
        }

        private static void Produce()
        {
            Random rnd = new Random();

            while (true)
            {
                if (!_pauseProducers)
                {
                    Queue.Add(rnd.Next(1, 100));
                    Console.WriteLine($"{Queue.Count}: элемент добавил {Thread.CurrentThread.Name}");
                    Thread.Sleep(ProducersTimeout);
                }

                if (_stopProducers)
                    return;
            }
        }

        private static void Consume()
        {
            while (true)
            {
                Queue.Take();
                Console.WriteLine($"{Queue.Count}: элемент взял {Thread.CurrentThread.Name}");
                Thread.Sleep(ConsumersTimeout);

                if (Queue.Count <= 0)
                    return;
            }
        }
    }
}