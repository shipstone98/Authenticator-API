dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile="**/*.g.cs"
reportgenerator "-reports:**/coverage.cobertura.xml" -targetdir:coverage-report -filefilters:"-*.g.cs"
