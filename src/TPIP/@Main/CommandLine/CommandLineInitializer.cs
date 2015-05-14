using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.IO;

namespace TPIP
{
    /// <summary>
    /// This class is responsible for keeping input arguments of the program
    /// </summary>
    public class CommandLineInput
    {
        /// <summary>
        /// 
        /// </summary>
        CommandLineInput() { }

        public string InputFileAddress { get; private set; }
        public string OutPutDirectory { get; private set; }
        public string PdbCodes_tobe_removedAddress { get; private set; }
        public InputFileEntryType EntryType { get; private set; }

        public static CommandLineInput Create(string[] args)
        {
            var result = new CommandLineInput();
           
            //result.InitializeData(args);
            result.ForTest();
            return result;
        }

        void ForTest()
        {
            EntryType = InputFileEntryType.ProteinID;
            InputFileAddress = @"D:\Reyhaneh\Works\BioInformatics-Kingston\Research\Investments\Apps\Interface Prediction\TPIPPredictor-Version2\inFile.txt";
            OutPutDirectory = @"D:\Reyhaneh\Works\BioInformatics-Kingston\Research\Investments\Apps\Interface Prediction\TPIPPredictor-Version2";
            //var pdbCodes_tobe_removedAddress = @"D:\Reyhaneh\Works\BioInformatics-Kingston\Research\Investments\Apps\TPIPPredictor\a.txt";
        }
        void InitializeData(string[] args)
        {
            EntryType = 0;

            var show_help = false;
            if (args.Length < 1)
            {
                show_help = true;
            }

            var optionSet = new OptionSet() {
                { "m|FormatofInputData=", "The {format} of your input data: 0 if data is in FASTA format, 1 if protein pdbcode and chain are queried.", (int v) => EntryType = (InputFileEntryType)v },
                { "i|inputFile=", "The {input address} of your input file.", (string v) => InputFileAddress = v },
                { "o|OutPutDirectory=", "The {output directory} for results.", (string v) => OutPutDirectory = v },
                { "r|pdbCodes_tobe_removed", "The address of the file containing pdb codes to be removed.", (string v) => PdbCodes_tobe_removedAddress = v },
                { "h|help",  "To see help.", v => show_help = v != null }
            };
            try
            {
                optionSet.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `TPIP.exe --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(optionSet);
                return;
            }
            if (args.Length < 1)
            {
                ShowHelp(optionSet);
                return;
            }
            else
            {
                if (File.Exists(InputFileAddress) == false)
                {
                    throw new ValidationException("The input file you provided does not exist. Please provide a valid input file.");
                }
                if (Directory.Exists(OutPutDirectory) == false)
                {
                    throw new ValidationException("The output directory you provided does not exist.Please provide a valid output directory.");
                }
                if (EntryType != InputFileEntryType.ProteinID && EntryType != InputFileEntryType.ProteinSequence)
                {
                    throw new ValidationException("The format selected for input data does not exist. If the proteins in the input file are in FASTA format provide format should be 0 and if the protein pdbcode and chain are used the fomat should be 1.");
                }
            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

    }

    public enum InputFileEntryType
    {
        ProteinID = 0,
        ProteinSequence = 1
    }

}
