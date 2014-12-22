///////////////////////////////////////////////////////////////////////
// Executive.cs - Executive is responsible for the entire application//
//                It controls the entire flow                        //
// ver 1.0                                                           //
// Language:    C#, 2013, .Net Framework 4.5                         //
// Platform:    Lenovo Y40, Win8.1                                   //
// Application: Code Analyzer for CSE681, Project #2, Fall 2014      //
// Author:      Dhaval N Dholakiya, Syracuse University              //
//              (315) 447-7644, ddholaki@syr.edu                     //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *   CommandLineParsing  - It the package provides output to the User
 */
/* Required Files:
 *   IRulesAndActions.cs, RulesAndActions.cs, Parser.cs, Semi.cs, Toker.cs, CommandLineParse.cs, FileMgr.cs, Display.cs, XMLOutput.cs
 *   
 *   
 * Maintenance History:
 * --------------------
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeAnalysis
{
    /// /////////////////////////////////////
    //Controls the flow of the Code Analyzer
    public class Executive
    {
        //-------------------<prcocesses the files>-------------------------
        public static string[] processFiles(string path, List<string> patterns, List<string> options)
        {

            FileMgr fileManager = new FileMgr();
            if (options.Contains("/S") || options.Contains("/s"))
            {
                fileManager.subDirectoriesCheck(options);

            }

            string[] files = fileManager.getFiles(path, patterns);

            return files;
        }

        //-------------<the parsing is called from this method>-----------------------------------------------
        //-------------<it also calls the display function and XML functions>---------------------------------
        public static void processAnalysis(string[] files, string path, List<string> patterns, List<string> options)
        {
            XMLOutput xml = new XMLOutput();

            Analyzer analyze = new Analyzer();
            Display display = new Display();
            if (options.Contains("/R") || options.Contains("/r"))
            {
                analyze.doAnalysis(files);
                analyze.doAnalysisRelationship(files);
                display.display(files, path, patterns, options);
                display.displaySummary(files);
                display.displayRelation(files);
            }
            else
            {
                analyze.doAnalysis(files);
                display.display(files, path, patterns, options);
                display.displaySummary(files);
                display.displayFunctionComplexity(files);
            }
            if (options.Contains("/X") || options.Contains("/x"))
            {
                xml.displaySummary();

                if (options.Contains("/R") || options.Contains("/r"))
                {
                    xml.displayRelation();
                }
                else
                {
                    xml.displayFunctionAnalysis();
                }
            }

        }


        //----------------<Starting Point of the Code Analyzer>----------------------
        public static void Main(string[] args)
        {
            if (args.Length == 0)                      //Validation of input parameters
            {
                Console.WriteLine("Please Enter Path Pattern and Options");
                return;
            }
            string path = null;
            List<string> patterns = new List<string>();
            List<string> options;

            CommandLineParsing passArguments = new CommandLineParsing();
            path = passArguments.processArgumentsPath(args);
            if (!Directory.Exists(path))                      //check if path exists
            {
                Console.WriteLine("Please Enter Correct Path");
                return;
            }
            patterns = passArguments.processArgumentsPattern(args);
            options = passArguments.processArgumentsOptions(args);

            string[] files = Executive.processFiles(path, patterns, options);
            if (files.Length == 0)
            {
                Console.WriteLine("Files not found");
                return;
            }
            Executive.processAnalysis(files, path, patterns, options);
            Console.ReadLine();
        }
    }
}
