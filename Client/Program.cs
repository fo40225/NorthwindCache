using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Process pipeServer = new Process();
            pipeServer.StartInfo.FileName = "Server.exe";

            AnonymousPipeServerStream pipeQuery = new AnonymousPipeServerStream(PipeDirection.Out,
                    HandleInheritability.Inheritable);

            AnonymousPipeServerStream pipeAnswer = new AnonymousPipeServerStream(PipeDirection.In,
                    HandleInheritability.Inheritable);

            pipeServer.StartInfo.Arguments = pipeQuery.GetClientHandleAsString() + " " + pipeAnswer.GetClientHandleAsString();
            pipeServer.StartInfo.UseShellExecute = false;
            pipeServer.Start();

            pipeQuery.DisposeLocalCopyOfClientHandle();
            pipeAnswer.DisposeLocalCopyOfClientHandle();

            StreamWriter sr = new StreamWriter(pipeQuery);
            sr.AutoFlush = true;

            BinaryReader sw = new BinaryReader(pipeAnswer);

            BinaryFormatter bf = new BinaryFormatter();
            for (; ; )
            {
                Console.WriteLine("Input CategoryID:");
                var CategoryID = Console.ReadLine();
                sr.WriteLine(CategoryID);

                var results = bf.Deserialize(pipeAnswer) as Dictionary<string, object>;

                object CategoryName;
                results.TryGetValue("CategoryName", out CategoryName);

                object Description;
                results.TryGetValue("Description", out Description);

                Console.WriteLine(CategoryName);
                Console.WriteLine(Description);
            }
        }
    }
}