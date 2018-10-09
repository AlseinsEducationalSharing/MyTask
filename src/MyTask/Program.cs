using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyTask
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("test1:");
                Console.WriteLine($"result {await DoAsync1(false)}");
            }
            catch
            {
                Console.WriteLine("thrown");
            }

            try
            {
                Console.WriteLine("test2:");
                Console.WriteLine($"result {await DoAsync1(true)}");
            }
            catch
            {
                Console.WriteLine("thrown");
            }

            try
            {
                Console.WriteLine("test3:");
                Console.WriteLine($"result {await DoAsync2(false)}");
            }
            catch
            {
                Console.WriteLine("thrown");
            }

            try
            {
                Console.WriteLine("test4:");
                Console.WriteLine($"result {await DoAsync2(true)}");
            }
            catch
            {
                Console.WriteLine("thrown");
            }
        }

        private static async Task<int> DoAsync1(bool throwing)
        {
            var a = default(int);
            a = 0;
            Console.WriteLine("    a = 0;");
            try
            {
                a = 1;
                Console.WriteLine("    a = 1;");
                await Task.Delay(1000);
                if (throwing)
                {
                    throw new Exception();
                }
                a++;
                Console.WriteLine("    a++;");
                await Task.Delay(1000);
                a += 2;
                Console.WriteLine("    a += 2;");
            }
            catch
            {
                Console.WriteLine("    exception!");
                throw;
            }
            Console.WriteLine("    return a");
            return a;
        }

        private static ITask<int> DoAsync2(bool throwing)
        {
            var source = new MyTaskCompletionSource<int>();
            var step = 0;
            var a = default(int);
            var exception = default(Exception);

            void doWork()
            {
                switch (step)
                {
                    case 0:
                        a = 0;
                        Console.WriteLine("    a = 0;");
                        try
                        {
                            a = 1;
                            Console.WriteLine("    a = 1;");
                            Task.Delay(1000).GetAwaiter().OnCompleted(next);
                            break;
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                            goto case 3;
                        }

                    case 1:
                        try
                        {
                            if (throwing)
                            {
                                throw new Exception();
                            }
                            a++;
                            Console.WriteLine("    a++;");
                            Task.Delay(1000).GetAwaiter().OnCompleted(next);
                            break;
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                            goto case 3;
                        }

                    case 2:
                        try
                        {
                            a += 2;
                            Console.WriteLine("    a += 2;");
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                            goto case 3;
                        }
                        Console.WriteLine("    return a");
                        source.SetResult(a);
                        break;

                    case 3:
                        Console.WriteLine("    exception!");
                        source.SetException(exception);
                        break;
                }

            }

            void next()
            {
                step++;
                doWork();
            }

            doWork();
            return source.Task;
        }
    }
}
