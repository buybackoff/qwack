dotnet restore
dotnet build **/project.json
dotnet test ./test/Qwack.Dates.Tests
dotnet test ./test/Qwack.Math.Tests
dotnet test ./test/Qwack.Core.Tests