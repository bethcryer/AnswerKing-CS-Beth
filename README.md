# Answer King
Dotnet implementation of the Answer King Api

## The following libraries/technologies were used:
* [.NET Core (.NET is a free, cross-platform, open source developer platform)](https://dot.net)
* [LiteDb (An open source MongoDB-like database with zero configuration)](https://www.litedb.org/)
* [Swashbuckle.AspNetCore (Swagger / OpenAPI - Automatically generates Api Documentation)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* [FluentValidation.AspNetCore (A popular .NET library for building strongly-typed validation rules)](https://fluentvalidation.net/)

## Running the solution

Ensure you have the latest [.NET 7 SDK (v7.0.102)](https://dotnet.microsoft.com/download) installed.

Clone the project:

`$ git clone git@github.com:AnswerConsulting/AnswerKing-CS.git`

CD into the newly cloned repository:

`$ cd Answer.King-CS`

Now run the project:

`$ dotnet run --project src/Answer.King.Api/Answer.King.Api.csproj`

Now open your browser and navigate to `https://localhost:5001` and you should be greeted by the swagger interface describing the api


## Unit Testing

The project is accompanied by unit tests. The project uses `xUnit` for testing.

[Learn about xUnit](https://xunit.github.io/)

## Code Coverage For Unit Tests 

To generate a report for code coverage: 

Ensure you have 'Run Coverlet Report' extension added -  Extensions menu and select Manage Extensions. Then, search Run Coverlet Report.

VS Code - Click on the Tools tab and 'Run Code Coverage'

Or 

Rider - Click on Tests tab and 'Cover All Tests from Solution'

Or 

In the terminal run: 

- dotnet test --collect:"XPlat Code Coverage"

Then - reportgenerator -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

For example - reportgenerator -reports:"C:\Users\HarryStead\Documents\AnswerKing-CS\tests\Answer.King.Api.UnitTests\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

[Learn more about .Net code coverage](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows)

## Terraform

This project is currently hosted on AWS with Terraform being used to build, update and destroy the necessary resources as desired. The following instructions assume that you already have access to the AWS account and a working AWS CLI setup.

Before being able to use Terraform, you'll need a copy of the gitignore'd env_variables.tf file from member of the team. This is currently being used to store secrets before we move to a more robust solution.

Once you have that, you can preview Terraform's execution plan by running `terraform plan` and then, if you're happy with the preview, `terraform apply` to execute the plan.

Since this is a bench project without real users, we should tear down the resources when they're not needed to save unnecessary costs. Running `terraform destroy` will destroy all of the resources that Terraform is currently managing. Note: This does not included resources which have been removed from Terraform's management such as the commented out block in `backend.tf`.

## Checkov Scanning For Terraform

Checkov is used as part of the pipeline to check for common misconfigurations in our Terraform scripts.

It can be ran locally to validate changes ahead of pushing if desired, assuming you have Python installed.

---

1. From a terminal window in the root directory of this repository, run the following to set up a virtual environment:

`python -m venv .venv`

2. Access this virtual environment by one of the following:

- Git Bash: `. .venv/Scripts/activate`

- cmd.exe: `.\.venv\Scripts\activate.bat`

- Powershell: `.\.venv\Scripts\Activate.ps1`

"(.venv)" should appear at the start of your current prompt in the terminal if this has worked.

3. Run the following to install checkov to your virtual environment:

`python -m pip install checkov`

4. Once installed, run the following to scan the `terraform` directory:

`checkov -d terraform/`

5. When you're finished, the virtual environment can be left by closing the terminal or running the following:

`deactivate`

---

On subsequent runs, you can skip steps 1 and 3.
