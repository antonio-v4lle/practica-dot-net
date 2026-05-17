using System.Reflection;
using MermaidClassDiagramGenerator;
using UltraPlatform.Worker.Services;
using UltraPlatform.Worker.Interfaces;
using UltraPlatform.Worker.Specifications;
using UltraPlatform.Worker.Models;

var generator = new DiagramGenerator(
    outputFilePath: "diagram.md",
    assembliesToScan: new List<Assembly>
    {
        typeof(DataService).Assembly
    },
    domainTypes: new List<Type>
    {
        typeof(DataService),
        typeof(DataFilteredService),
        typeof(IDataService),
        typeof(IDataServiceFactory),
        typeof(IDataValidationHelper),
        typeof(IUtilityService),
        typeof(ISpecification<>),
        typeof(DataRecord)
    },
    generateWithoutProperties: false
);

generator.Generate();
Console.WriteLine("diagram.md generado en: " + Path.GetFullPath("diagram.md"));