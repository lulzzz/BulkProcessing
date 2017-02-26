﻿using Akka.Actor;
using Common;
using System;
using BulkProcessor.Actors;
using BulkProcessor.Actors.BatchesProcessor;
using BulkProcessor.Actors.SystemMessages;
using Akka.DI;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using BulkProcessor.DI;

namespace BulkProcessor
{
    class Program
    {
        private static ActorSystem BulkProcessingActorSystem;
        const string SystemName = "BulkProcessingActorSystem";

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SystemConfig>().As<ISystemConfig>();

            builder.RegisterType<ConfigActor>();

            var container = builder.Build();


            ConsoleLogger.LogSystemMessage("Creating BulkProcessingActorSystem");
            BulkProcessingActorSystem = ActorSystem.Create(SystemName);

            IDependencyResolver resolver = new AutoFacDependencyResolver(container, BulkProcessingActorSystem);

            ConsoleLogger.LogSystemMessage("Creating actor supervisory hierarchy");
            // setup the system
            BulkProcessingActorSystem.ActorOf(Props.Create<BulkProcessorActor>(), "BulkProcessorActor");

            ////send message to start processing the data
           var batchesManager = BulkProcessingActorSystem.ActorOf(Props.Create<BatchesManagerActor>(), "BatchesManagerActor");

            var message = new StartBulkProcessingMessage();

            BulkProcessingActorSystem.Scheduler
                .Schedule(TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(30),
                    batchesManager,
                    message);



            do
            {
                ShortPause();

                Console.WriteLine();
                ConsoleLogger.LogSystemMessage("enter a command and hit enter");

                var command = Console.ReadLine().ToLowerInvariant();
                if (command.StartsWith("play"))
                {
                    //int userId = int.Parse(command.Split(',')[1]);
                    //string movieTitle = command.Split(',')[2];

                    //var message = new PlayMovieMessage(movieTitle, userId);
                    // call actor using user selector using hierarchy
                    //BulkProcessingActorSystem.ActorSelection("/user/Playback/UserCoordinator").Tell(message);
                }

                //if (command.startswith("stop"))
                //{
                //    int userid = int.parse(command.split(',')[1]);
                //    var message = new stopmoviemessage(userid);

                //    bulkprocessingactorsystem.actorselection("/user/playback/usercoordinator").tell(message);
                //}

                if (command.StartsWith("exit"))
                {
                    BulkProcessingActorSystem.Terminate();
                    ConsoleLogger.LogSystemMessage("Actor system shutdown. Press any key to exit...");
                    Environment.Exit(1);
                }

            } while (true);


            //////// the fact that user actor is created using the props does not mean 
            //////// anything as it needs to be registered with the system to know about it.
            //////Props userActorProps = Props.Create<UserActor>();

            //////IActorRef actorRef = BulkProcessingSystem.ActorOf(paybackActorProps, "PlaybackActor");

            //////actorRef.Tell("Akka.net rocks");
            ////////Step to kill the instance of the actor
            //////actorRef.Tell(PoisonPill.Instance);
            //////// Attempt to send message again => message will be undelivered unless something will pick it up
            //////actorRef.Tell("Akka.net rocks");

            //////Console.ReadKey();
            //////BulkProcessingSystem.Terminate();
        }

        public static void ShortPause()
        {
            System.Threading.Thread.Sleep(1000);
        }
    }
}
