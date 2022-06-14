# Application Approval

Code Sample For Lower Prompt

# Installation

The executables are available [here](https://github.com/PapaJohnsGarlicSauce/ApplicationApproval/releases/tag/v0.1-alpha).  **Note**: I have only tested the Windows versions but included the published MacOS app for completion purposes.

# Usage
After installing, run application from the command line using the `--file` option and path to the text file containing the loan application(s).

```bash
path-to-application.exe --file some/path.to/input.txt
```

Example:

```bash
C:/ApplicationApproval.exe --file C:/TestFiles/file.txt
```

The output will list a high-level summary of the number of applications received, approved, and the calculated approval rate, followed by specific approval statuses, the DTI, and credit score per application.

```bash
C:/ApplicationApproval.exe --file C:/TestFiles/file.txt
Summary: 1 application approved, 2 applications received, 50% approval rate
A1: approved, DTI: 0.308, Credit Score: 720
A2: denied, DTI: 0.545, Credit Score: 621
```

If any errors or exceptions occur during the process (e.g. no application Ids were provided, loan information was not specified, etc.), the error messages will be printed to the console and the process will terminate.

```bash
C:/ApplicationApproval.exe --file C:/TestFiles/Errorfile.txt
An error has occurred: Application IDs not found. (Parameter 'applicationIds')
```

# Testing
To run the tests associated with this project, navigate to the `ApplicationApprovalTest` folder from the root directory.  You can choose to open `ApplicationApprovalTest.csproj` in Visual Studio (or the IDE of your choice, but I highly recommend VS for this) and run the tests through Visual Studio's built-in test runner by right-clicking the test project and selecting "Run Tests."

![Run Tests](https://github.com/PapaJohnsGarlicSauce/ApplicationApproval/blob/main/RunTests.png)

If you prefer to run the tests through the command line, this solution targets .NET Core, so you should be able to do the following:

```bash
dotnet test path-to-test-dll/nameoftest.dll
```

Example output:

```bash
dotnet test ./nameoftest.dll
Microsoft (R) Test Execution Command Line Tool Version 17.1.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    27, Skipped:     0, Total:    27, Duration: 84 ms - ApplicationApprovalTest.dll (net6.0)
```


# Basic Overview On Approach

This ended up taking me longer than I originally anticipated due to some structural changes I implemented and because setting up the environment for writing tests (not to mention just writing tests) always takes longer than you think it will :weary:  I'd be more than happy to go into further detail during a follow-up interview, but the overall structure is a pretty simple Reader/Parser application with basic dependency injection of services via the constructor.

1. `Program.cs` - service registration and dependency injection
2. `ApplicationApprovalService.cs` - where the actual job is executed and output is generated. It has two interface dependencies: 
    * `IReader`, implemented as `LoanApplicationsReader` here.  This is where, using `StreamReader`, the file is read and parsed into the appropriate data models.
    * `IRulesEngine`, implemented as `RulesEngine` here.  This is a pretty standard implementation of the [Rules Engine](https://tenmilesquare.com/resources/software-development/basic-rules-engine-design-pattern/) pattern where you can dynamically add/edit/test business rules independently of one another.  Currently, there are just two business rules - DtiRatio < some value and CreditScore > some value.  You can see these rules in the `RulesEngine/Rules` directory.

As far as testing goes, _ideally_ there would be a lot more tests than I wrote - it just got to the point of diminishing returns.  If this were being deployed to production, I definitely would've made the unit tests more comprehensive and included end-to-end/integration tests (probably with some dummy file directory data being sent to `ApplicationApprovalService.Execute()`.  You'd probably also want to include tests for the RulesEngine itself - here, I mainly tested the rules themselves because that was were the main logic was housed - as well as tests on the Loan Application Reader.  I used some testing libraries I am pretty comfortable with (NSubstitute, Autofixture, etc.), but ideally, these test would have more robnust customizations to ensure we are generating appriopriate mocks and models to test.  I had to do some wonky stuff with Autofixture to generate test applications forthe `DtiRatioRuleTest` class that I should've just made customizations for, but that would take a considerable amount of time more.

If I had to do it again, I probably would get rid of `RegexExtensions.GetDeserializedObject<T>` and just put all of the logic to generate each model into the static `Parse` method per class.  At first I thought it was kind of fun to use named Regex groupings and reflection to generate any object with property values equal to the name regex groups, but on further inspection, I feel like there's too much unpredictability with this method.  It would mean repeating some code in each `Parse` but it would also let me update those values independently.   I would go back and update it now, but I feel like I've beaten this solution to death 😅.

# Closing

Thank you for giving me the opportunity to complete this prompt.  Good luck with running this application and feel free to reach out if you have any questions or concerns.  Thank you so much. 🙂
