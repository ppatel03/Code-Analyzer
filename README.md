Code-Analyzer
=============

This project develops a program for analysis of C# code. The analyzer, given a set of file references, finds all the types, their members, and relationships with other types. Further, the analyzer constructs two metrics for each member function: its size in lines of code, and complexity defined as the number of elements in its scope tree. The purpose of this code analysis is to find functions that may need attention because of their size and/or complexity. A secondary purpose is to help developers get a quick understanding of the structure of a set of code.
