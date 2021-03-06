﻿using System;
using Autofac.Extras.DynamicProxy2;
using System.Timers;
using System.Threading;
using log4net.Config;
using Autofac;
using fody;

namespace CachingService
{
    class Program
    {
        private static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            // Set up a simple configuration that logs on the console.
            BasicConfigurator.Configure();
           

            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());

            Console.WriteLine("My cache name is" + typeof(MyDataType).ToString());
            //var myCache = new TypedObjectCache<MyDataType>(typeof(MyDataType).ToString());
            //builder.RegisterInstance(myCache).ExternallyOwned();

            /**
            builder.RegisterType<TypedObjectCache<MyDataType>>()
                .WithParameter("name", typeof(MyDataType).ToString())
                .SingleInstance();
            */


            // KingAop style
            //builder.RegisterType<MyServiceKingAopStyleImpl>().As<IMyService>(); 


            //Fody Style 
            builder.RegisterType<FodyCache>().As<ICache>(); 
            builder.RegisterType<MyFodyService>().As<IMyService>();


            //Dynamic proxystyle
            //builder.RegisterType<MyServiceDynamicProxyStyleImpl>()
            //    .As<IMyService>()
            //    .EnableInterfaceInterceptors();
            //  .InterceptedBy(typeof(CallLogger));

            builder.Register(c => new CallLogger());



            Container = builder.Build();



            using (var scope = Container.BeginLifetimeScope())
            {
                // normal and Dynamicproxystyle
                var service = scope.Resolve<IMyService>();

                // KingAop style
                //dynamic service = new MyServiceKingAopStyleImpl(scope.Resolve<TypedObjectCache<MyDataType>>());


                var data = service.Get("Funky");

                Console.WriteLine(data);
                Thread.Sleep(1000);
                data = service.Get("Funky2");
                Console.WriteLine(data);
                data = service.Get("Funky");
                Console.WriteLine(data);
                /*
                Console.WriteLine("Let's try the get All");
                var items = service.GetAll();
                Console.WriteLine(items.Count);
                foreach (var item in items)
                {
                    Console.WriteLine(item);
                }*/

            }

            //SimpleMethod(); 

            Console.ReadLine();
        }

        static void SimpleMethod()
        {
            MyCache objCache = new MyCache();
            String strUserName = objCache.Get("USER_NAME") as String;
            if (String.IsNullOrEmpty(strUserName))
            {
                objCache.Set("USER_NAME", "MyItem");
            }
            Console.WriteLine(objCache.Get("USER_NAME") as String);
        }
    }
}
