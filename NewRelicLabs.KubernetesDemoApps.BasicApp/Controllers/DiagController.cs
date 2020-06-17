using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace NewRelicLabs.KubernetesDemoApps.BasicApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiagController : ControllerBase
    {
        readonly object o1 = new object();
        readonly object o2 = new object();
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Processor p = new Processor();


        [HttpGet]
        [Route("deadlock/")]
        public ActionResult<string> Deadlock()
        {
            logger.Info("starting deadlock");
            (new System.Threading.Thread(() =>
            {
                DeadlockFunc();
            })).Start();

            Thread.Sleep(5000);

            var threads = new Thread[300];
            for (int i = 0; i < 300; i++)
            {
                (threads[i] = new Thread(() =>
                {
                    lock (o1) { Thread.Sleep(100); }
                })).Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return "success:deadlock";
        }

        private void DeadlockFunc()
        {
            lock (o1)
            {
                (new Thread(() =>
                {
                    lock (o2) { Monitor.Enter(o1); }
                })).Start();

                Thread.Sleep(2000);
                Monitor.Enter(o2);
            }
        }

        [HttpGet]
        [Route("memspike/{seconds}")]
        public ActionResult<string> Memspike(int seconds)
        {
            logger.Info($"staring memory spike for {seconds} sec.");
            var watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                p = new Processor();
                watch.Stop();
                if (watch.ElapsedMilliseconds > seconds * 1000)
                    break;
                watch.Start();

                int it = (2000 * 1000);
                for (int i = 0; i < it; i++)
                {
                    p.ProcessTransaction(new Customer(Guid.NewGuid().ToString()));
                }

                Thread.Sleep(5000); // Sleep for 5 seconds before cleaning up

                // Cleanup
                p = null;

                // GC
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Thread.Sleep(5000); // Sleep for 5 seconds before spiking memory again
            }
            return "success:memspike";
        }

        [HttpGet]
        [Route("memleak/{kb}")]
        public ActionResult<string> Memleak(int kb)
        {
            logger.Info($"Starting memleak with {kb}kbytes");
            int it = (kb * 1000) / 100;
            for (int i = 0; i < it; i++)
            {
                p.ProcessTransaction(new Customer(Guid.NewGuid().ToString()));
            }

            return "success:memleak";
        }

        [HttpGet]
        [Route("exception")]
        public ActionResult<string> Exception()
        {
            throw new Exception("bad, bad code");
        }

        [HttpGet]
        [Route("highcpu/{milliseconds}")]
        public ActionResult<string> Highcpu(int milliseconds)
        {
            logger.Info($"starting high cpu for {milliseconds}msec");
            var watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                watch.Stop();
                if (watch.ElapsedMilliseconds > milliseconds)
                    break;
                watch.Start();
            }

            return "success:highcpu";
        }

        [HttpGet("crash")]
        public IActionResult Crash()
        {
            logger.Fatal("intentially crash!!!");
            var p = Marshal.AllocHGlobal(1);
            for (var _ = 0u; _ < 100_000_000; _++)
            {
                p = new IntPtr(p.ToInt64() + 1);
                Marshal.WriteByte(p, 0xff);
            }
            return Ok();
        }
    }

    class Customer
    {
        private readonly string id;

        public Customer(string id)
        {
            this.id = id;
        }
    }

    class CustomerCache
    {
        private readonly List<Customer> cache = new List<Customer>();

        public void AddCustomer(Customer c)
        {
            cache.Add(c);
        }
    }

    class Processor
    {
        private readonly CustomerCache cache = new CustomerCache();

        public void ProcessTransaction(Customer customer)
        {
            cache.AddCustomer(customer);
        }
    }
}
