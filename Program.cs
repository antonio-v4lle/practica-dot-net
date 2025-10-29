using System;
internal class Program
{
    private static void Main(string[] args)
    {
        // IterationChallenge1 iterationChallenge1 = new IterationChallenge1();
        // iterationChallenge1.run();

        // IterationChallenge2 iterationChallenge2 = new IterationChallenge2();
        // iterationChallenge2.run();

        // Tipo de Valor
        // int val_A = 2;
        // int val_B = val_A;
        // val_B = 5;

        // Console.WriteLine("--Value Types--");
        // Console.WriteLine($"val_A: {val_A}");
        // Console.WriteLine($"val_B: {val_B}");

        // // Tipo de Referencia
        // int[] ref_A = new int[1];
        // ref_A[0] = 2;
        // int[] ref_B = ref_A;
        // ref_B[0] = 5;

        // Console.WriteLine("--Reference Types--");
        // Console.WriteLine($"ref_A[0]: {ref_A[0]}");
        // Console.WriteLine($"ref_B[0]: {ref_B[0]}");

        string[] pallets = ["B14", "A11", "B12", "A13"];
        Console.WriteLine("");

        Array.Clear(pallets, 0, 2);
        Console.WriteLine($"Clearing 2 ... count: {pallets.Length}");
        foreach (var pallet in pallets)
        {
            Console.WriteLine($"-- {pallet}");
        }

        Console.WriteLine("");
        Array.Resize(ref pallets, 6);
        Console.WriteLine($"Resizing 6 ... count: {pallets.Length}");

        pallets[4] = "C01";
        pallets[5] = "C02";

        foreach (var pallet in pallets)
        {
            Console.WriteLine($"-- {pallet}");
        }
    }
}