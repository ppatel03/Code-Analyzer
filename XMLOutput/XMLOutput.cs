///////////////////////////////////////////////////////////////////////
// XMLOutput.cs - XMLOutput is responsible to present the output     //
//              to the user via XML file                             //
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
 *   Display  - It provides output to the User via XML file
 */
/* Required Files:
 *   Parser.cs
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
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace CodeAnalysis
{
    /// /////////////////////////////
    // To Display Output in the XML file
    public class XMLOutput
    {
        //--------------------<displays the Function Analysis in XML format>-----------------------------
        public void displayFunctionAnalysis()
        {
            List<Elem> outputList = OutputRepository.output_;
            if (outputList.Count == 0)
            {
                Console.Write("No Data in FunctionAnalysis");
                return;
            }
            Console.Write("\n  Created XML for Function size and Complexity");
            Console.Write("\n =====================================\n");
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment comment = new XComment("Demonstration XML");
            xml.Add(comment);
            XElement root = new XElement("CODEANALYSIS");
            xml.Add(root);
            foreach (Elem e in outputList)
            {                                                                 //Addition of Child
                int size = e.end - e.begin;
                XElement childType = new XElement("Type");
                root.Add(childType);
                XElement type = new XElement("Type", e.type);
                childType.Add(type);

                XElement childName = new XElement("NAME");
                root.Add(childName);
                XElement name = new XElement("Name", e.name);
                childName.Add(name);

                XElement childComplexity = new XElement("COMPLEXITY");
                root.Add(childComplexity);
                XElement functionComplexity = new XElement("Complexity", Convert.ToString(e.functionComplexity));
                childComplexity.Add(functionComplexity);

                XElement childSize = new XElement("SIZE");
                root.Add(childSize);
                XElement sizeNew = new XElement("Size", Convert.ToString(size));
                childSize.Add(sizeNew);
                xml.Save(Directory.GetCurrentDirectory() + "\\FunctionAnalysis.xml");
            }
            Console.Write(" The Size and Complexity XML file is displayed at:\n");
            Console.Write(" ");
            Console.Write(Directory.GetCurrentDirectory());
            Console.Write("\\FunctionAnalysis.xml");
            Console.Write("\n\n");
        }
        //-------------------<displays the Summary in XML format>--------------------------
        public void displaySummary()
        {
            List<Elem> outputList = OutputRepository.output_;

            if (outputList.Count == 0)
            {
                Console.Write("No Data in Summary");
                return;
            }
            Console.Write("\n  Created XML file for Summary Display");
            Console.Write("\n =====================================\n");
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment comment = new XComment("Demonstration XML");
            xml.Add(comment);
            XElement root = new XElement("CODEANALYSIS");
            xml.Add(root);
            foreach (Elem e in outputList)
            {                                                                         //Addition of Child
                XElement childEntering = new XElement("ENTERING");
                root.Add(childEntering);
                XElement entering = new XElement("Entering", Convert.ToString(e.begin));
                childEntering.Add(entering);

                XElement childLeaving = new XElement("LEAVING");
                root.Add(childLeaving);
                XElement sizeNew = new XElement("Leaving", Convert.ToString(e.end));
                childLeaving.Add(sizeNew);

                XElement childType = new XElement("TYPE");
                root.Add(childType);
                XElement type = new XElement("Type", e.type);
                childType.Add(type);

                XElement childName = new XElement("NAME");
                root.Add(childName);
                XElement name = new XElement("Name", e.name);
                childName.Add(name);

                xml.Save(Directory.GetCurrentDirectory() + "\\Summary.xml");
            }
            Console.Write(" The Summary XML file is displayed at:\n");
            Console.Write(" ");
            Console.Write(Directory.GetCurrentDirectory());
            Console.Write("\\Summary.xml");
            Console.Write("\n\n");
        }

        //-------------<displays the Type Relationships in XML format>-----------------------------
        public void displayRelation()
        {
            List<ElemRelation> table = RelationshipRepository.relationship_;
            if (table.Count == 0)
            {
                Console.Write("No Data in Relationship");
                return;
            }
            Console.Write("\n  Created XML file for Relationship");
            Console.Write("\n =====================================\n");
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment comment = new XComment("Demonstration XML");
            xml.Add(comment);
            XElement root = new XElement("CODEANALYSIS");
            xml.Add(root);
            foreach (ElemRelation e in table)
            {                                                                //Addition of Child
                XElement from = new XElement("FROM");
                root.Add(from);
                XElement fromF = new XElement("From", Convert.ToString(e.fromClass));
                from.Add(fromF);

                XElement relation = new XElement("RelationshipType");
                root.Add(relation);
                XElement relationshipType = new XElement("Reation", Convert.ToString(e.relationType));
                relation.Add(relationshipType);

                XElement to = new XElement("TO");
                root.Add(to);
                XElement toF = new XElement("To", Convert.ToString(e.toClass));
                to.Add(toF);

                xml.Save(Directory.GetCurrentDirectory() + "\\Relationships.xml");
            }
            Console.Write(" The Relationships XML file is displayed at:\n");
            Console.Write(" ");
            Console.Write(Directory.GetCurrentDirectory());
            Console.Write("\\Relationships.xml");
            Console.Write("\n\n");
        }



        // ------------------------<TestStub for XML Output>-------------------------
#if(TEST_XMLOUTPUT)

        static void Main(string[] args)
        {

            ElemRelation e = new ElemRelation();
            e.fromClass = "Derived";
            e.toClass = "Original";
            e.relationType = "Inheritance";

            RelationshipRepository.relationship_.Add(e);

            Elem elem = new Elem();
            elem.type = "function";
            elem.name = "Derived";
            elem.begin = 1;
            elem.end = 10;
            elem.functionComplexity = 2;
            OutputRepository.output_.Add(elem);
            XMLOutput xml = new XMLOutput();
            xml.displaySummary();
            xml.displayFunctionAnalysis();
            xml.displayRelation();
        }
#endif
    }
}
