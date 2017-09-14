# Problem Overview
I am building a custom USQL extractor for parsing WAV files. This level of complexity requires specific third party libraries and I am having trouble referencing them.

Below is a simplified example of the problem. There are two projects (USQL Project and a USQL Class Library). The class library has third party dependencies that are included when registering the assembly. 
It is also referenced in the core U-SQL project, and U-SQL file itself. 
When the U-SQL script is ran it throws an error because the class library is unable to find the managed third party dependency. 

I come from a java background and admitidly my understanding of C# and U-SQL is limited. But from the research iv'e done, the nested assembly should be included automatically using these procedures.

[This is the most useful resource I was able to find](https://blogs.msdn.microsoft.com/azuredatalake/2016/08/26/how-to-register-u-sql-assemblies-in-your-u-sql-catalog/)

# Project Setup Instructions
1. Install visual 2015+ and the data lake/data lake analytics sdk.
2. Clone this project, and open it in visual studio.
3. Copy the *letters.csv* file, to the local mock datalake location (**C:\Users\\[user]\AppData\Local\USQLDataRoot**).
4. Open **Capitalize.usql**, check that the run configurations (to the right of the submit button) show **Local-master-dbo**.
5. Click Submit to run the usql project, upon success a csv document called **capLetters.csv** will be created in the same directory as the **letters.csv** document from step 3.

# Issue Replication Instructions
Now that we have a working usql project, usql class library and functional custom extractor, we are going to replicate the issue. 
The objective is to add an assembly that can be referenced by the class library classes.
1. In the **ExampleClassLibrary** project right click references and select **Manage NuGet packages**
2. Under the browse tab search for and install a 3rd party library, (**DotNetZip** is the one I used)
3. Next open **UpperCaseExtractor.cs** and add a reference to the assembly in the Extract method. (If you used DotNetZip uncomment lines 9 and 31.)
4. Register the ExampleClassLibrary by first clicking the **ExampleClassLibrary** project and selecting register assembly.
5. Then Check **Replace if assembly already exists**.
6. Expand **Managed Dependencies** and check the third party library you included.
    
   ![Assembly Registration](https://i.imgur.com/x8xG6Pe.png)
5. Reference/Update the registered assembly in the **NestedAssemblyCheck** project by right clicking **References -> Add Reference  -> ExampleClassLibrary -> OK**.
6. Now open **Capitalize.usql** with the same configurations as before and click submit.
7. Notice the job fails this time and an error is thrown, the key takeaway being `Could not load file or assembly 'DotNetZip'`:
```Start : 9/14/2017 2:14:07 PM
Initialize : 9/14/2017 2:14:07 PM
GraphParse : 9/14/2017 2:14:07 PM
Run : 9/14/2017 2:14:07 PM
Start 'Root' : 9/14/2017 2:14:07 PM
End 'Root(Success)' : 9/14/2017 2:14:07 PM
Start '1_SV1_Extract' : 9/14/2017 2:14:07 PM
End '1_SV1_Extract(Error)' : 9/14/2017 2:14:07 PM
Completed with 'Error' : 9/14/2017 2:14:07 PM
Execution failed with error '1_SV1_Extract Error : '{"diagnosticCode":195887146,"severity":"Error","component":"RUNTIME","source":"User","errorId":"E_RUNTIME_USER_UNHANDLED_EXCEPTION_FROM_USER_CODE","message":"An unhandled exception from user code has been reported when invoking the method 'Extract' on the user type 'ExampleClassLibrary.UpperCaseExtractor'","description":"Unhandled exception from user code: \"Could not load file or assembly 'DotNetZip, Version=1.10.1.0, Culture=neutral, PublicKeyToken=6583c7c814667745' or one of its dependencies. The system cannot find the file specified.\"\nThe details includes more information including any inner exceptions and the stack trace where the exception was raised.","resolution":"Make sure the bug in the user code is fixed.","helpLink":"","details":"==== Caught exception System.IO.FileNotFoundException\n\n   at ExampleClassLibrary.UpperCaseExtractor.<Extract>d__4.MoveNext()\r\n   at ScopeEngine.SqlIpExtractor<ScopeEngine::CosmosInput,Extract_0_Data0>.GetNextRow(SqlIpExtractor<ScopeEngine::CosmosInput\\,Extract_0_Data0>* , Extract_0_Data0* output) in c:\\users\\colto\\git\\usql-_classlibrary_issue\\nestedassemblycheck\\bin\\debug\\4f2d8a4d8ae7fd9\\capitalize_4dc3be70d47aca36\\sqlmanaged.h:line 1903","internalDiagnostics":""}
'
'
Execution failed !```
   

