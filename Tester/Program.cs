﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Tester.Actors;
using Tester.Messages;

namespace Tester
{
    class Program
    {
        private static ActorSystem MovieStreamingActorSystem;

        static void Main(string[] args)
        {
            const string SystemName = "MovieStreamingActorSystem";

            ConsoleLogger.LogSystemMessage("Creating MovieStreamingActorSystem");
            MovieStreamingActorSystem = ActorSystem.Create(SystemName);

            ConsoleLogger.LogSystemMessage("Creating actor supervisory hierarchy");
            MovieStreamingActorSystem.ActorOf(Props.Create<PlaybackActor>(), "Playback");
            do
            {
                ShortPause();

                Console.WriteLine();
                ConsoleLogger.LogSystemMessage("enter a command and hit enter");

                var command = Console.ReadLine().ToLowerInvariant();
                if (command.StartsWith("play"))
                {
                    int userId = int.Parse(command.Split(',')[1]);
                    string movieTitle = command.Split(',')[2];

                    var message = new PlayMovieMessage(movieTitle, userId);
                    MovieStreamingActorSystem.ActorSelection("/user/Playback/UserCoordinator").Tell(message);
                }

                if (command.StartsWith("stop"))
                {
                    int userId = int.Parse(command.Split(',')[1]);
                    var message = new StopMovieMessage(userId);

                    MovieStreamingActorSystem.ActorSelection("/user/Playback/UserCoordinator").Tell(message);
                }

                if (command.StartsWith("exit"))
                {
                    MovieStreamingActorSystem.Terminate();
                    ConsoleLogger.LogSystemMessage("Actor system shutdown. Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

            } while (true);


            //// the fact that user actor is created using the props does not mean 
            //// anything as it needs to be registered with the system to know about it.
            //Props userActorProps = Props.Create<UserActor>();

            //IActorRef actorRef = BulkProcessingSystem.ActorOf(paybackActorProps, "PlaybackActor");

            //actorRef.Tell("Akka.net rocks");
            ////Step to kill the instance of the actor
            //actorRef.Tell(PoisonPill.Instance);
            //// Attempt to send message again => message will be undelivered unless something will pick it up
            //actorRef.Tell("Akka.net rocks");

            //Console.ReadKey();
            //BulkProcessingSystem.Terminate();



        }
        public static void ShortPause()
        {
            System.Threading.Thread.Sleep(1000);
        }
    }

    /*
     * 
     *      Using poison pill
     *      //Step to kill the instance of the actor
            actorRef.Tell(PoisonPill.Instance);
            // Attempt to send message again => message will be undelivered unless something will pick it up
            actorRef.Tell(new PlayMovieMessage("Akka.net rocks", 42));
     */

    public static class ConsoleLogger
    {
        public static void LogMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void LogSystemMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }


        internal static void LogUserMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void LogTraceMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}