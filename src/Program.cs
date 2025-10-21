namespace HelloWorld;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("¡Hola desde .NET Core 8.0!");
        Console.WriteLine();
        Console.WriteLine("Este es un proyecto base mínimo.");
        Console.WriteLine("Puedes reemplazar este código con tu aplicación.");
        Console.WriteLine();

        if (args.Length > 0)
        {
            Console.WriteLine($"Argumentos recibidos: {string.Join(", ", args)}");
        }
        else
        {
            Console.WriteLine("💡 Tip: Ejecuta 'dotnet run --project src/HelloWorld.csproj -- arg1 arg2'");
            Console.WriteLine("   para pasar argumentos al programa.");
        }

        Console.WriteLine();
        Console.WriteLine("📚 Usa los siguientes comandos para crear nuevos proyectos:");
        Console.WriteLine("   • create-webapi <nombre>  - Crea un proyecto Web API");
        Console.WriteLine("   • create-mvc <nombre>     - Crea un proyecto MVC");
        Console.WriteLine("   • create-console <nombre> - Crea un proyecto Console");
        Console.WriteLine();
        Console.WriteLine("✨ ¡Feliz codificación!");
    }
}
