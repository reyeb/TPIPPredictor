using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NDesk.Options;

namespace TPIP
{
    static class Program
    {
        public static CommandLineInput Input { get; set; }

        static void Main(string[] args)
        {
            //try
            //{
                Input = CommandLineInput.Create(args);

                var proteinInterfacePredictorInstance = new ProteinInterfacePredictor();
                proteinInterfacePredictorInstance.PredictInterface();
                Console.ReadKey();
            //}
            //catch (ValidationException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    Console.ReadKey();
            //}
            //catch (Exception ex)
            //{
            //    // write on file
            //    Console.WriteLine("An error occured while processing your data. Please email us the ErrorLog folder created plus all your input files.");
            //    CreateErrorLong(ex.StackTrace);
            //    Console.ReadKey();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        static void CreateErrorLong(string errorText)
        {
            //AppDomain.Current.BaseDirectory
            var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog");

            if (Directory.CreateDirectory(directoryPath).Exists == false)
                Directory.CreateDirectory(directoryPath);
            File.AppendAllText(Path.Combine(directoryPath, DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt"), errorText);
        }

        
    }
}
